using DuckovESPv3.Core.Configuration;
using DuckovESPv3.Core.Systems.ESP.Detection;
using DuckovESPv3.Core.Systems.ESP.Services;
using DuckovESPv3.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using UnityEngine;
using ILogger = DuckovESPv3.Infrastructure.Logging.ILogger;

namespace DuckovESPv3.Core.Systems.MeleeAutoAttack
{
    public class MeleeAutoAttackSystem
    {
        private readonly MeleeAutoAttackConfig _config;
        private readonly ILogger _logger;
        private readonly EnemyCollectionService? _enemyCollectionService;

        // 缓存
        private CharacterMainControl? _player;
        private Camera? _mainCamera;
        private readonly Dictionary<int, CharacterMainControl> _enemyCache = new Dictionary<int, CharacterMainControl>();
        private readonly HashSet<int> _deadEnemies = new HashSet<int>();

        // 攻击状态
        private CharacterMainControl? _currentTarget;
        private float _lastAttackTime;
        private bool _isAttacking;
        
        // 伤害检测
        private float _lastTargetHealth;
        private int _consecutiveNoDamageAttacks;
        private const int MAX_NO_DAMAGE_ATTACKS = 2; // 两次攻击无伤害则强制击杀
        private float _nextDamageCheckTime; // 下次检查伤害的时间
        private bool _waitingForDamage; // 是否正在等待伤害判定
        
        // 传送后延迟
        private float _nextAttackAllowedTime; // 传送后允许攻击的时间
        private const float TELEPORT_ATTACK_DELAY = 0.15f; // 传送后延迟0.15秒再攻击

        public MeleeAutoAttackSystem(MeleeAutoAttackConfig config, ILogger logger, EnemyCollectionService? enemyCollectionService = null)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _enemyCollectionService = enemyCollectionService;
        }

        public void Initialize()
        {
            _logger.Info("[MeleeAutoAttack] Initializing...");

            _player = CharacterMainControl.Main;
            if (_player == null)
            {
                _logger.Warning("[MeleeAutoAttack] Player not found");
                return;
            }

            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                _logger.Warning("[MeleeAutoAttack] Main camera not found");
                return;
            }

            // 订阅事件以缓存敌人
            CharacterSpawnHook.OnCharacterSpawned += OnCharacterSpawned;
            Health.OnDead += OnCharacterDead;
            
            // 订阅ESP的敌人事件（优先使用ESP的数据）
            if (_enemyCollectionService != null)
            {
                _enemyCollectionService.OnEnemyAdded += OnESPEnemyAdded;
                _enemyCollectionService.OnEnemyRemoved += OnESPEnemyRemoved;
                _logger.Info("[MeleeAutoAttack] Subscribed to ESP enemy events");
            }

            // 初始化缓存
            _enemyCache.Clear();
            _deadEnemies.Clear();
            
            // 如果有ESP服务，从ESP获取已发现的敌人
            if (_enemyCollectionService != null)
            {
                var espEnemies = _enemyCollectionService.GetAllEnemies();
                foreach (var enemyData in espEnemies)
                {
                    if (enemyData.Character != null)
                    {
                        _enemyCache[enemyData.Character.GetInstanceID()] = enemyData.Character;
                    }
                }
                _logger.Info($"[MeleeAutoAttack] Loaded {_enemyCache.Count} enemies from ESP");
            }
            else
            {
                // 回退：扫描场景中的所有角色
                var allCharacters = UnityEngine.Object.FindObjectsOfType<CharacterMainControl>();
                foreach (var character in allCharacters)
                {
                    if (character != _player && character.GetComponent<Health>() != null)
                    {
                        _enemyCache[character.GetInstanceID()] = character;
                    }
                }
                _logger.Info($"[MeleeAutoAttack] Initialized with {_enemyCache.Count} enemies cached (no ESP)");
            }
        }

        public void Cleanup()
        {
            _logger.Info("[MeleeAutoAttack] Cleaning up...");

            CharacterSpawnHook.OnCharacterSpawned -= OnCharacterSpawned;
            Health.OnDead -= OnCharacterDead;
            
            // 取消订阅ESP事件
            if (_enemyCollectionService != null)
            {
                _enemyCollectionService.OnEnemyAdded -= OnESPEnemyAdded;
                _enemyCollectionService.OnEnemyRemoved -= OnESPEnemyRemoved;
            }

            _enemyCache.Clear();
            _deadEnemies.Clear();
            _currentTarget = null;
            _isAttacking = false;
        }
        
        private void OnESPEnemyAdded(ESP.Models.EnemyData enemyData)
        {
            if (enemyData.Character != null)
            {
                int id = enemyData.Character.GetInstanceID();
                if (!_enemyCache.ContainsKey(id))
                {
                    _enemyCache[id] = enemyData.Character;
                    _deadEnemies.Remove(id);
                    _logger.Debug($"[MeleeAutoAttack] Enemy added from ESP: {enemyData.Name}");
                }
            }
        }
        
        private void OnESPEnemyRemoved(CharacterMainControl character)
        {
            if (character != null)
            {
                int id = character.GetInstanceID();
                _enemyCache.Remove(id);
                _deadEnemies.Add(id);
                
                if (_currentTarget == character)
                {
                    _logger.Info("[MeleeAutoAttack] ESP removed current target, stopping attack");
                    _currentTarget = null;
                    _isAttacking = false;
                }
                
                _logger.Debug($"[MeleeAutoAttack] Enemy removed from ESP: {character.name}");
            }
        }

        private void OnCharacterSpawned(CharacterMainControl character)
        {
            if (character != null && character != _player && character.GetComponent<Health>() != null)
            {
                int id = character.GetInstanceID();
                if (!_enemyCache.ContainsKey(id))
                {
                    _enemyCache[id] = character;
                    _deadEnemies.Remove(id);
                }
            }
        }

        private void OnCharacterDead(Health health, DamageInfo damageInfo)
        {
            if (health != null)
            {
                var character = health.GetComponent<CharacterMainControl>();
                if (character != null)
                {
                    int id = character.GetInstanceID();
                    _enemyCache.Remove(id);
                    _deadEnemies.Add(id);

                    if (_currentTarget == character)
                    {
                        _logger.Info("[MeleeAutoAttack] Current target died, stopping attack");
                        _currentTarget = null;
                        _isAttacking = false;
                    }
                }
            }
        }

        public void Update()
        {
            if (!_config.Enable)
            {
                if (_isAttacking)
                {
                    _isAttacking = false;
                    _currentTarget = null;
                }
                return;
            }

            // 在基地时禁用传送功能
            if (LevelManager.Instance != null && LevelManager.Instance.IsBaseLevel)
            {
                if (_isAttacking)
                {
                    _isAttacking = false;
                    _currentTarget = null;
                }
                return;
            }

            if (_player == null)
            {
                _player = CharacterMainControl.Main;
                if (_player == null) return;
            }

            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null) return;
            }

            // 检查是否持刀
            var meleeWeapon = _player.GetMeleeWeapon();
            if (meleeWeapon == null)
            {
                // 持枪模式，停止刀攻击
                if (_isAttacking)
                {
                    _isAttacking = false;
                    _currentTarget = null;
                }
                return;
            }

            // 持刀模式
            if (_isAttacking && _currentTarget != null)
            {
                // 继续攻击当前目标
                ContinueAttack(meleeWeapon);
            }
            else
            {
                // 寻找新目标
                FindAndEngageTarget(meleeWeapon);
            }
        }

        private void FindAndEngageTarget(ItemAgent_MeleeWeapon meleeWeapon)
        {
            if (_player == null) return;
            
            CharacterMainControl? bestTarget = null;
            float closestDistance = float.MaxValue;

            foreach (var kvp in _enemyCache)
            {
                var enemy = kvp.Value;
                if (enemy == null || _deadEnemies.Contains(kvp.Key))
                {
                    continue;
                }

                // ===== 关键修复：检查是否为真正的敌人（排除宠物/队友） =====
                if (!IsEnemy(enemy))
                {
                    _logger.Debug($"[MeleeAutoAttack] 跳过队友/宠物: {enemy.name}");
                    continue;
                }

                // 检查敌人是否存活
                var health = enemy.GetComponent<Health>();
                if (health == null || health.IsDead)
                {
                    _deadEnemies.Add(kvp.Key);
                    continue;
                }

                // 检查条件1: 敌人在屏幕内
                bool isOnScreen = IsEnemyOnScreen(enemy);

                // 检查条件2: 敌人正在瞄准玩家
                bool isAimingAtPlayer = false;
                if (_config.RequireEnemyAiming)
                {
                    isAimingAtPlayer = IsEnemyAimingAtPlayer(enemy);
                }

                // 必须满足至少一个条件
                bool shouldAttack = isOnScreen || (_config.RequireEnemyAiming && isAimingAtPlayer);
                if (!shouldAttack)
                {
                    continue;
                }

                // 选择最近的目标
                float distance = Vector3.Distance(_player.transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestTarget = enemy;
                }
            }

            if (bestTarget != null)
            {
                StartAttack(bestTarget, meleeWeapon);
            }
        }

        private void StartAttack(CharacterMainControl target, ItemAgent_MeleeWeapon meleeWeapon)
        {
            if (_player == null) return;
            
            _currentTarget = target;
            _isAttacking = true;
            _consecutiveNoDamageAttacks = 0; // 重置无伤害计数
            
            // 记录初始血量
            var health = target.GetComponent<Health>();
            if (health != null)
            {
                _lastTargetHealth = health.CurrentHealth;
            }

            // 使用武器的攻击距离，而不是配置中的固定值
            float attackRange = meleeWeapon.AttackRange;
            float teleportDistance = attackRange * 0.8f; // 传送到攻击范围的80%位置，确保在攻击范围内

            // 计算敌人背后的位置并传送
            TeleportBehindTarget(target, teleportDistance);

            _logger.Info($"[MeleeAutoAttack] Teleported behind enemy, waiting {TELEPORT_ATTACK_DELAY}s before attack");

            // ===== 传送后延迟再攻击（等待位置和朝向生效） =====
            _nextAttackAllowedTime = Time.time + TELEPORT_ATTACK_DELAY;
            _lastAttackTime = Time.time - 999f; // 重置攻击时间，确保可以立即攻击
        }

        private void ContinueAttack(ItemAgent_MeleeWeapon meleeWeapon)
        {
            if (_player == null || _currentTarget == null)
            {
                _isAttacking = false;
                return;
            }

            // ===== 持续锁定玩家朝向到目标 =====
            // 在攻击期间，每帧强制设置朝向，防止被鼠标位置覆盖
            Vector3 targetPosition = _currentTarget.transform.position;
            Vector3 playerPosition = _player.transform.position;
            Vector3 directionToTarget = (targetPosition - playerPosition).normalized;
            directionToTarget.y = 0; // 只水平旋转

            if (directionToTarget.sqrMagnitude > 0.01f && _player.movementControl != null)
            {
                // 持续设置朝向到目标
                _player.movementControl.SetAimDirection(directionToTarget);
            }

            // 检查目标是否还活着
            var health = _currentTarget.GetComponent<Health>();
            if (health == null || health.IsDead)
            {
                _logger.Info("[MeleeAutoAttack] Target died, stopping attack");
                _isAttacking = false;
                _currentTarget = null;
                return;
            }

            // ===== 检测上次攻击的伤害（在DealDamageTime之后） =====
            if (_waitingForDamage && Time.time >= _nextDamageCheckTime)
            {
                _waitingForDamage = false;
                
                float currentHealth = health.CurrentHealth;
                if (Mathf.Approximately(_lastTargetHealth, currentHealth))
                {
                    // 血量没有变化，无伤害攻击
                    _consecutiveNoDamageAttacks++;
                    _logger.Warning($"[MeleeAutoAttack] No damage dealt! Consecutive: {_consecutiveNoDamageAttacks}/{MAX_NO_DAMAGE_ATTACKS}");
                    
                    // 如果连续两次无伤害，强制击杀
                    if (_consecutiveNoDamageAttacks >= MAX_NO_DAMAGE_ATTACKS)
                    {
                        _logger.Warning("[MeleeAutoAttack] Two attacks dealt no damage, force killing enemy!");
                        ForceKillTarget(health);
                        _isAttacking = false;
                        _currentTarget = null;
                        return;
                    }
                }
                else
                {
                    // 有伤害，重置计数
                    _consecutiveNoDamageAttacks = 0;
                    _logger.Debug($"[MeleeAutoAttack] Damage dealt: {_lastTargetHealth - currentHealth}");
                }
                
                _lastTargetHealth = currentHealth;
            }

            // 使用武器的攻击速度计算攻击间隔
            // AttackSpeed 是速度倍数，值越大攻击越快
            // 基础攻击间隔假设为 1 秒，然后除以攻击速度
            float baseAttackInterval = 1f / Mathf.Max(meleeWeapon.AttackSpeed, 0.1f); // 避免除以0
            float actualDelay = Mathf.Max(baseAttackInterval, 0.01f); // 最小延迟 0.01 秒

            // ===== 检查是否在传送后延迟期间 =====
            if (Time.time < _nextAttackAllowedTime)
            {
                // 还在等待传送生效，不执行攻击
                return;
            }

            // 执行攻击（考虑攻击延迟）
            if (Time.time - _lastAttackTime >= actualDelay)
            {
                // ===== 只在攻击时传送一次（而不是每帧都传送） =====
                float attackRange = meleeWeapon.AttackRange;
                float teleportDistance = attackRange * 0.8f;
                TeleportBehindTarget(_currentTarget, teleportDistance);
                
                // 传送后延迟
                _nextAttackAllowedTime = Time.time + TELEPORT_ATTACK_DELAY;
                
                PerformAttack(meleeWeapon);
                
                // 设置下次伤害检测时间
                _nextDamageCheckTime = Time.time + meleeWeapon.DealDamageTime + 0.1f;
                _waitingForDamage = true;
            }
        }

        private void PerformAttack(ItemAgent_MeleeWeapon meleeWeapon)
        {
            if (meleeWeapon == null || _player == null) return;

            // 调用攻击方法
            bool attacked = _player.Attack();
            if (attacked)
            {
                _lastAttackTime = Time.time;
            }
        }

        private bool IsEnemyOnScreen(CharacterMainControl enemy)
        {
            if (_mainCamera == null || enemy == null)
                return false;

            Vector3 screenPoint = _mainCamera.WorldToViewportPoint(enemy.transform.position);
            return screenPoint.x > 0 && screenPoint.x < 1 &&
                   screenPoint.y > 0 && screenPoint.y < 1 &&
                   screenPoint.z > 0;
        }

        private bool IsEnemyAimingAtPlayer(CharacterMainControl enemy)
        {
            if (enemy == null || _player == null)
                return false;

            // 获取敌人的AI控制器
            var aiController = enemy.GetComponent<AICharacterController>();
            if (aiController == null)
                return false;

            // 检查敌人的aimTarget是否是玩家
            if (aiController.aimTarget == null)
                return false;

            // 检查aimTarget是否指向玩家的mainDamageReceiver
            return aiController.aimTarget.gameObject == _player.mainDamageReceiver.gameObject;
        }

        /// <summary>
        /// 传送到目标身后并面向目标
        /// </summary>
        private void TeleportBehindTarget(CharacterMainControl target, float distance)
        {
            if (_player == null || target == null)
                return;

            // 计算敌人背后的位置
            Vector3 targetPosition = target.transform.position;
            Vector3 targetForward = target.transform.forward;
            Vector3 behindPosition = targetPosition - targetForward * distance;

            // 传送到敌人背后
            _player.SetPosition(behindPosition);

            // 旋转玩家面向敌人（只旋转Y轴，保持水平）
            Vector3 directionToTarget = (targetPosition - behindPosition).normalized;
            directionToTarget.y = 0; // 只水平旋转

            if (directionToTarget.sqrMagnitude > 0.01f)
            {
                // 同时设置 transform.rotation 和 Movement 的目标朝向
                // 这样可以防止 UpdateAiming() 在下一帧重置朝向
                _player.transform.rotation = Quaternion.LookRotation(directionToTarget);
                
                // 使用 Movement.SetAimDirection() 来设置目标朝向
                // 这会锁定玩家朝向，防止被鼠标位置覆盖
                var movement = _player.movementControl;
                if (movement != null)
                {
                    movement.SetAimDirection(directionToTarget);
                    // 立即应用旋转，不等待插值
                    movement.ForceSetAimDirectionToAimPoint();
                }
            }
        }

        /// <summary>
        /// 强制击杀目标（当正常攻击无效时）
        /// </summary>
        private void ForceKillTarget(Health health)
        {
            if (health == null || _player == null)
            {
                _logger.Error("[MeleeAutoAttack] ForceKillTarget: health or player is null");
                return;
            }

            _logger.Info($"[MeleeAutoAttack] Attempting force kill on {health.gameObject.name}");

            try
            {
                // 尝试多种方法获取DamageReceiver
                DamageReceiver damageReceiver = health.GetComponent<DamageReceiver>();
                
                if (damageReceiver == null)
                {
                    // 尝试从父对象获取
                    damageReceiver = health.GetComponentInParent<DamageReceiver>();
                }
                
                if (damageReceiver == null)
                {
                    // 尝试从子对象获取
                    damageReceiver = health.GetComponentInChildren<DamageReceiver>();
                }
                
                if (damageReceiver != null)
                {
                    // 方法1：造成巨额伤害（模拟正常伤害流程）
                    DamageInfo damageInfo = new DamageInfo(_player)
                    {
                        damageValue = 99999f,
                        ignoreArmor = true,
                        ignoreDifficulty = true,
                        damagePoint = damageReceiver.transform.position,
                        damageNormal = (_player.transform.position - damageReceiver.transform.position).normalized
                    };
                    
                    bool hurtResult = damageReceiver.Hurt(damageInfo);
                    _logger.Info($"[MeleeAutoAttack] Force killed using Hurt(99999), result: {hurtResult}");
                    return;
                }
                else
                {
                    _logger.Warning($"[MeleeAutoAttack] No DamageReceiver found on {health.gameObject.name} or its parents/children");
                }
            }
            catch (System.Exception ex)
            {
                _logger.Error($"[MeleeAutoAttack] Hurt method failed: {ex.Message}");
            }
            
            // 方法2（备用）：直接设置血量为0
            // 方法2（备用）：直接设置血量为0
            try
            {
                _logger.Info("[MeleeAutoAttack] Trying backup method: SetHealth(0)");
                health.SetHealth(0f);
                _logger.Info("[MeleeAutoAttack] SetHealth(0) executed");
            }
            catch (System.Exception ex2)
            {
                _logger.Error($"[MeleeAutoAttack] All kill methods failed: {ex2.Message}");
            }
        }

        /// <summary>
        /// 判断是否为真正的敌人（排除宠物、队友）
        /// </summary>
        private bool IsEnemy(CharacterMainControl character)
        {
            if (character == null || _player == null)
                return false;

            // 特殊处理：中立单位（Teams.middle）
            // 默认不攻击中立单位，除非它已经被激怒（noticed = true）
            if (character.Team == Teams.middle)
            {
                var aiController = character.GetComponent<AICharacterController>();
                if (aiController != null)
                {
                    // 只有当中立单位已经注意到玩家（被攻击过）时才反击
                    return aiController.noticed;
                }
                // 没有AI控制器的中立单位，默认不攻击
                return false;
            }

            // 使用游戏内置的队伍判断逻辑（处理所有队伍关系）
            return Team.IsEnemy(_player.Team, character.Team);
        }
    }
}

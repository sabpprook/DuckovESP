using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DuckovESPv3.Core.Configuration;
using DuckovESPv3.Core.Systems.ESP.Detection;
using ItemStatsSystem;
using UnityEngine;
using ILogger = DuckovESPv3.Infrastructure.Logging.ILogger;

namespace DuckovESPv3.Core.Systems.Aimbot
{
    /// <summary>
    /// 自动瞄准系统 - 基于 Mod_Aimbot 的优秀实现重构，适配 V3 架构
    /// 核心思路：在射击后重定向子弹，而不是控制视角
    /// </summary>
    public class AimbotSystem
    {
        private readonly AimbotSystemConfig _config;
        private readonly ILogger _logger;
        private Camera _mainCamera;
        private CharacterMainControl _player;
        private ItemAgent_Gun _trackedGun;
        
        // 反射字段定义（参考 Mod_Aimbot）
        private static readonly FieldInfo GunProjectileField = 
            typeof(ItemAgent_Gun).GetField("projInst", BindingFlags.Instance | BindingFlags.NonPublic);
        
        private static readonly FieldInfo ProjectileTraveledDistanceField = 
            typeof(Projectile).GetField("traveledDistance", BindingFlags.Instance | BindingFlags.NonPublic);
        
        // 关键：Projectile 的私有字段 direction 和 velocity（参考 Mod_Aimbot）
        private static readonly FieldInfo ProjectileDirectionField = 
            typeof(Projectile).GetField("direction", BindingFlags.Instance | BindingFlags.NonPublic);
        
        private static readonly FieldInfo ProjectileVelocityField = 
            typeof(Projectile).GetField("velocity", BindingFlags.Instance | BindingFlags.NonPublic);
        
        // 子弹重定向追踪（改为追踪 Projectile 实例而非 ID）
        private readonly HashSet<Projectile> _retargetedProjectiles = new HashSet<Projectile>();
        
        // 自动扳机状态跟踪
        private float _triggerDelayTimer = 0f;
        private bool _targetInSight = false;
        private bool _lastTriggerState = false;
        private float _continuousFireTimer = 0f;
        private const float FIRE_RATE_INTERVAL = 0.1f; // 最小射击间隔（秒）
        private AutoAimCandidate _lastBestTarget; // 缓存最佳目标（供自动扳机使用）
        
        // 障碍物层遮罩（用于LineOfSight检测）
        private static int ObstacleLayerMaskValue = -1;
        
        // ===== 性能优化：事件驱动缓存系统 =====
        private readonly List<DamageReceiver> _damageReceiverCache = new List<DamageReceiver>();
        private bool _isInitialized = false;
        
        // 组件缓存（避免重复 GetComponent 调用）
        private readonly Dictionary<DamageReceiver, Collider> _colliderCache = new Dictionary<DamageReceiver, Collider>();
        private readonly Dictionary<DamageReceiver, HeadCollider> _headColliderCache = new Dictionary<DamageReceiver, HeadCollider>();
        
        // ===== 无后座力相关字段（参考 Mod_Aimbot）=====
        private static readonly int RecoilControlStatHash = "RecoilControl".GetHashCode();
        private static readonly int RecoilHorizontalStatHash = "RecoilHorizontal".GetHashCode();
        private static readonly int RecoilVerticalStatHash = "RecoilVertical".GetHashCode();
        private float? _originalRecoilControl = null;
        private float? _originalRecoilHorizontal = null;
        private float? _originalRecoilVertical = null;
        
        public AimbotSystem(AimbotSystemConfig config, ILogger logger)
        {
            _config = config;
            _logger = logger;
            
            // 初始化障碍物层遮罩
            InitializeObstacleLayerMask();
        }
        
        /// <summary>
        /// 初始化障碍物层遮罩
        /// </summary>
        private void InitializeObstacleLayerMask()
        {
            if (ObstacleLayerMaskValue != -1)
                return;
            
            try
            {
                int wallLayer = LayerMask.NameToLayer("Wall");
                int groundLayer = LayerMask.NameToLayer("Ground");
                int halfObstacleLayer = LayerMask.NameToLayer("HalfObsticle");
                
                ObstacleLayerMaskValue = 0;
                if (wallLayer >= 0)
                {
                    ObstacleLayerMaskValue |= (1 << wallLayer);
                    _logger.Debug($"添加 Wall 层 (Layer {wallLayer})");
                }
                if (groundLayer >= 0)
                {
                    ObstacleLayerMaskValue |= (1 << groundLayer);
                    _logger.Debug($"添加 Ground 层 (Layer {groundLayer})");
                }
                if (halfObstacleLayer >= 0)
                {
                    ObstacleLayerMaskValue |= (1 << halfObstacleLayer);
                    _logger.Debug($"添加 HalfObsticle 层 (Layer {halfObstacleLayer})");
                }
                
                if (ObstacleLayerMaskValue == 0)
                {
                    _logger.Warning("⚠️ 没有找到任何障碍物层！使用默认层遮罩");
                    ObstacleLayerMaskValue = ~(1 << LayerMask.NameToLayer("Ignore Raycast"));
                }
                else
                {
                    _logger.Info($"障碍物层遮罩初始化完成: 0x{ObstacleLayerMaskValue:X}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"初始化障碍物层失败: {ex.Message}");
                ObstacleLayerMaskValue = ~(1 << LayerMask.NameToLayer("Ignore Raycast"));
            }
        }
        
        /// <summary>
        /// 初始化自瞄系统（关卡加载时调用一次）
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
            {
                _logger.Warning("已初始化，跳过");
                return;
            }
            
            try
            {
                // 订阅 Health.OnDead 事件（目标死亡时移除）
                Health.OnDead += OnTargetDeath;
                
                // 订阅角色创建事件（Harmony Hook，零性能消耗）
                CharacterSpawnHook.OnCharacterSpawned += OnCharacterSpawned;
                
                // 关卡加载时扫描一次现有目标
                DiscoverExistingTargets();
                
                _isInitialized = true;
                _logger.Info($"✓ 初始化完成，发现 {_damageReceiverCache.Count} 个潜在目标");
            }
            catch (Exception ex)
            {
                _logger.Error($"初始化失败: {ex.Message}\n{ex.StackTrace}");
            }
        }
        
        /// <summary>
        /// 关卡加载时发现现有目标（仅调用一次 FindObjectsOfType）
        /// </summary>
        private void DiscoverExistingTargets()
        {
            try
            {
                var allReceivers = UnityEngine.Object.FindObjectsOfType<DamageReceiver>();
                
                _damageReceiverCache.Clear();
                _damageReceiverCache.AddRange(allReceivers.Where(r => r != null));
                
                _logger.Debug($"发现 {_damageReceiverCache.Count} 个 DamageReceiver");
            }
            catch (Exception ex)
            {
                _logger.Error($"发现现有目标失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 目标死亡事件处理（移除死亡的目标）
        /// </summary>
        private void OnTargetDeath(Health health, DamageInfo damageInfo)
        {
            try
            {
                if (health == null) return;
                
                var receiver = health.GetComponent<DamageReceiver>();
                if (receiver == null) return;
                
                _damageReceiverCache.Remove(receiver);
                _colliderCache.Remove(receiver);
                _headColliderCache.Remove(receiver);
            }
            catch (Exception ex)
            {
                _logger.Error($"处理目标死亡事件失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 角色创建事件处理（通过 Harmony Hook 触发，零性能消耗）
        /// </summary>
        private void OnCharacterSpawned(CharacterMainControl character)
        {
            try
            {
                if (character == null) return;
                
                var receivers = character.GetComponentsInChildren<DamageReceiver>();
                if (receivers == null || receivers.Length == 0) return;
                
                foreach (var receiver in receivers)
                {
                    if (receiver != null && !_damageReceiverCache.Contains(receiver))
                    {
                        _damageReceiverCache.Add(receiver);
                    }
                }
                
                _logger.Debug($"✓ Harmony Hook 检测到新目标: {character.name}，添加了 {receivers.Length} 个 DamageReceiver");
            }
            catch (Exception ex)
            {
                _logger.Error($"处理角色创建事件失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 清理资源（关卡卸载时调用）
        /// </summary>
        public void Cleanup()
        {
            try
            {
                // 取消订阅事件
                Health.OnDead -= OnTargetDeath;
                CharacterSpawnHook.OnCharacterSpawned -= OnCharacterSpawned;
                
                // 清空缓存
                _damageReceiverCache.Clear();
                _colliderCache.Clear();
                _headColliderCache.Clear();
                _retargetedProjectiles.Clear();
                
                _isInitialized = false;
                _logger.Info("✓ 清理完成");
            }
            catch (Exception ex)
            {
                _logger.Error($"清理失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 更新自动瞄准系统
        /// </summary>
        public void Update()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null) return;
            }
            
            if (_player == null)
            {
                _player = CharacterMainControl.Main;
                if (_player == null) return;
            }
            
            // 检查玩家是否存活
            if (_player.Health == null || _player.Health.CurrentHealth <= 0)
                return;
            
            // 追踪当前持有的枪械
            UpdateTrackedGun();
            
            // 自动应用无后座力
            if (_config.EnableNoRecoil)
            {
                ApplyNoRecoil();
            }
            
            // 查找最佳目标（自动瞄准和自动扳机共享）
            _lastBestTarget = (_config.EnableAimbot || _config.EnableTriggerBot) ? FindBestTarget() : default(AutoAimCandidate);
            
            // 自动扳机（必须在自动瞄准启用时才工作）
            if (_config.EnableTriggerBot && _config.EnableAimbot)
            {
                PerformTriggerBot();
            }
            else
            {
                // 禁用时确保释放扳机
                if (_lastTriggerState)
                {
                    ReleaseTrigger();
                }
                
                _targetInSight = false;
                _triggerDelayTimer = 0f;
            }
            
            // 清理已销毁的子弹
            CleanupRetargetedProjectiles();
        }
        
        /// <summary>
        /// 更新追踪的枪械
        /// </summary>
        private void UpdateTrackedGun()
        {
            ItemAgent_Gun currentGun = _player.agentHolder?.CurrentHoldGun;
            
            if (currentGun != _trackedGun)
            {
                // 取消订阅旧枪的事件
                if (_trackedGun != null)
                {
                    _trackedGun.OnShootEvent -= OnGunShoot;
                }
                
                _trackedGun = currentGun;
                
                // 订阅新枪的射击事件
                if (_trackedGun != null)
                {
                    _trackedGun.OnShootEvent += OnGunShoot;
                }
            }
        }
        
        /// <summary>
        /// 应用无后座力（角色和武器）- 基于 Mod_Aimbot
        /// </summary>
        private void ApplyNoRecoil()
        {
            if (_player == null || _player.CharacterItem == null)
                return;

            try
            {
                // 修改角色后座力控制
                var stat = _player.CharacterItem.GetStat(RecoilControlStatHash);
                if (stat != null)
                {
                    if (_originalRecoilControl == null)
                        _originalRecoilControl = stat.BaseValue;
                    stat.BaseValue = 9999f;
                }

                // 修改武器后座力
                ItemAgent_Gun gun = _player.agentHolder?.CurrentHoldGun;
                if (gun != null && gun.Item != null)
                {
                    SetItemStat(gun.Item, RecoilHorizontalStatHash, ref _originalRecoilHorizontal);
                    SetItemStat(gun.Item, RecoilVerticalStatHash, ref _originalRecoilVertical);
                }
            }
            catch (Exception ex)
            {
                _logger.Warning($"应用无后座力时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 设置武器 Stat 属性
        /// </summary>
        private void SetItemStat(Item item, int statHash, ref float? originalValue)
        {
            try
            {
                var stat = item.GetStat(statHash);
                if (stat != null)
                {
                    if (originalValue == null)
                        originalValue = stat.BaseValue;
                    stat.BaseValue = 0f;
                }
            }
            catch (Exception ex)
            {
                _logger.Warning($"设置 Item Stat 时出错: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 尝试通过反射获取刚发射的子弹实例（快速可靠）
        /// </summary>
        private Projectile TryGetImmediateProjectile(ItemAgent_Gun gun)
        {
            if (gun == null || GunProjectileField == null)
                return null;
            
            try
            {
                Projectile proj = GunProjectileField.GetValue(gun) as Projectile;
                
                if (proj == null || proj.gameObject == null)
                    return null;
                
                // 验证子弹是否是玩家发射的
                if (proj.context.fromCharacter != _player)
                    return null;
                
                // 使用反射检查子弹的飞行距离（只重定向刚发射的子弹）
                if (ProjectileTraveledDistanceField != null)
                {
                    object distanceValue = ProjectileTraveledDistanceField.GetValue(proj);
                    if (distanceValue is float traveledDistance)
                    {
                        if (traveledDistance > 0.06f)
                            return null;
                    }
                }
                
                // 检查是否已经重定向过（避免重复处理）
                if (_retargetedProjectiles.Contains(proj))
                    return null;
                
                return proj;
            }
            catch (Exception ex)
            {
                _logger.Error($"反射获取子弹失败: {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// 枪械射击事件回调 - 核心：在这里重定向子弹！
        /// </summary>
        private void OnGunShoot()
        {
            if (!_config.EnableAimbot)
                return;
            
            if (_trackedGun == null)
                return;
            
            // 清理已失效的子弹记录
            CleanupRetargetedProjectiles();
            
            // 查找最佳目标
            AutoAimCandidate candidate = FindBestTarget();
            if (candidate.Receiver == null)
                return;
            
            // 获取枪口位置
            Vector3 muzzlePosition = _trackedGun.muzzle != null 
                ? _trackedGun.muzzle.position 
                : _trackedGun.transform.position;
            
            // TriggerBot 模式下，自动使用子弹传送（绕过墙体检测问题）
            bool useTeleportMode = _config.EnableTriggerBot;
            
            if (useTeleportMode)
            {
                TeleportProjectilesToTarget(candidate.Receiver, candidate.AimPoint);
            }
            else
            {
                RetargetProjectilesToTarget(muzzlePosition, candidate.AimPoint);
            }
        }
        
        /// <summary>
        /// 重定向所有未处理的子弹到目标
        /// </summary>
        private void RetargetProjectilesToTarget(Vector3 muzzlePosition, Vector3 targetPoint)
        {
            try
            {
                Vector3 direction = (targetPoint - muzzlePosition).normalized;
                int retargetedCount = 0;
                int requiredCount = Mathf.Max(1, _trackedGun != null ? _trackedGun.ShotCount : 1);
                
                // 方法1: 优先使用反射直接获取刚发射的子弹
                if (_trackedGun != null)
                {
                    Projectile immediateProj = TryGetImmediateProjectile(_trackedGun);
                    if (immediateProj != null)
                    {
                        if (RetargetProjectile(immediateProj, direction, targetPoint))
                        {
                            _retargetedProjectiles.Add(immediateProj);
                            retargetedCount++;
                        }
                    }
                }
                
                // 方法2: 如果反射失败或散弹枪需要处理多个子弹,使用扫描作为后备
                if (retargetedCount < requiredCount)
                {
                    Projectile[] allProjectiles = UnityEngine.Object.FindObjectsOfType<Projectile>();
                    
                    if (allProjectiles != null && allProjectiles.Length > 0)
                    {
                        foreach (Projectile projectile in allProjectiles
                            .Where(p => p != null && p.context.fromCharacter == _player)
                            .OrderBy(p => GetProjectileTraveledDistance(p)))
                        {
                            if (_retargetedProjectiles.Contains(projectile))
                                continue;
                            
                            float traveledDistance = GetProjectileTraveledDistance(projectile);
                            if (traveledDistance > 0.06f)
                                continue;
                            
                            if (RetargetProjectile(projectile, direction, targetPoint))
                            {
                                _retargetedProjectiles.Add(projectile);
                                retargetedCount++;
                                
                                if (retargetedCount >= requiredCount)
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"重定向子弹时出错: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 重定向单个子弹（完全照搬 Mod_Aimbot 的实现）
        /// </summary>
        private bool RetargetProjectile(Projectile projectile, Vector3 direction, Vector3 targetPoint)
        {
            if (projectile == null || direction.sqrMagnitude < 0.0001f)
                return false;
            
            direction.Normalize();
            
            try
            {
                projectile.context.direction = direction;
                projectile.context.firstFrameCheckStartPoint = projectile.transform.position - direction * 0.1f;
                
                float distanceToTarget = Vector3.Distance(projectile.transform.position, targetPoint) + 2f;
                if (projectile.context.distance < distanceToTarget)
                {
                    projectile.context.distance = distanceToTarget;
                }
                
                if (_config.AimbotAimAtHead)
                {
                    projectile.context.critRate = 1f;
                    projectile.context.ignoreHalfObsticle = true;
                }
                
                // 【关键】使用反射设置私有字段
                if (ProjectileDirectionField != null)
                {
                    ProjectileDirectionField.SetValue(projectile, direction);
                }
                
                if (ProjectileVelocityField != null)
                {
                    Vector3 velocity = direction * projectile.context.speed;
                    ProjectileVelocityField.SetValue(projectile, velocity);
                }
                
                projectile.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"重定向单个子弹失败: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// 传送模式：将子弹直接传送到目标附近（绕过墙体检测）
        /// </summary>
        private void TeleportProjectilesToTarget(DamageReceiver target, Vector3 targetPoint)
        {
            try
            {
                int teleportedCount = 0;
                int requiredCount = Mathf.Max(1, _trackedGun != null ? _trackedGun.ShotCount : 1);
                
                if (_trackedGun != null)
                {
                    Projectile immediateProj = TryGetImmediateProjectile(_trackedGun);
                    if (immediateProj != null)
                    {
                        if (TeleportSingleProjectile(immediateProj, target, targetPoint))
                        {
                            _retargetedProjectiles.Add(immediateProj);
                            teleportedCount++;
                        }
                    }
                }
                
                if (teleportedCount < requiredCount)
                {
                    Projectile[] allProjectiles = UnityEngine.Object.FindObjectsOfType<Projectile>();
                    foreach (Projectile projectile in allProjectiles)
                    {
                        if (projectile == null || _retargetedProjectiles.Contains(projectile))
                            continue;
                        
                        if (projectile.context.fromCharacter == _player)
                        {
                            float traveledDistance = GetProjectileTraveledDistance(projectile);
                            if (traveledDistance > 0.2f) 
                                continue;
                            
                            if (TeleportSingleProjectile(projectile, target, targetPoint))
                            {
                                _retargetedProjectiles.Add(projectile);
                                teleportedCount++;
                                
                                if (teleportedCount >= requiredCount)
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"传送子弹时出错: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 传送单个子弹到目标附近
        /// </summary>
        private bool TeleportSingleProjectile(Projectile projectile, DamageReceiver target, Vector3 targetPoint)
        {
            if (projectile == null || target == null)
                return false;
            
            try
            {
                Vector3 toTarget = (targetPoint - projectile.transform.position).normalized;
                float randomOffset = UnityEngine.Random.Range(1f, 2.5f);
                Vector3 teleportPosition = targetPoint - toTarget * randomOffset;
                
                teleportPosition += new Vector3(
                    UnityEngine.Random.Range(-0.3f, 0.3f),
                    UnityEngine.Random.Range(-0.2f, 0.2f),
                    UnityEngine.Random.Range(-0.3f, 0.3f)
                );
                
                projectile.transform.position = teleportPosition;
                
                Vector3 direction = (targetPoint - teleportPosition).normalized;
                projectile.context.direction = direction;
                projectile.context.firstFrameCheckStartPoint = teleportPosition - direction * 0.1f;
                
                float distanceToTarget = Vector3.Distance(teleportPosition, targetPoint) + 1f;
                projectile.context.distance = distanceToTarget;
                
                if (_config.AimbotAimAtHead)
                {
                    projectile.context.critRate = 1f;
                    projectile.context.ignoreHalfObsticle = true;
                }
                
                if (ProjectileDirectionField != null)
                {
                    ProjectileDirectionField.SetValue(projectile, direction);
                }
                
                if (ProjectileVelocityField != null)
                {
                    Vector3 velocity = direction * projectile.context.speed;
                    ProjectileVelocityField.SetValue(projectile, velocity);
                }
                
                projectile.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                
                if (ProjectileTraveledDistanceField != null)
                {
                    ProjectileTraveledDistanceField.SetValue(projectile, 0f);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"传送单个子弹失败: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// 清理已失效的子弹记录
        /// </summary>
        private void CleanupRetargetedProjectiles()
        {
            if (_retargetedProjectiles.Count == 0)
                return;
            
            try
            {
                HashSet<Projectile> activeProjectiles = new HashSet<Projectile>();
                Projectile[] allProjectiles = UnityEngine.Object.FindObjectsOfType<Projectile>();
                if (allProjectiles != null)
                {
                    foreach (Projectile proj in allProjectiles)
                    {
                        if (proj != null)
                            activeProjectiles.Add(proj);
                    }
                }
                
                _retargetedProjectiles.RemoveWhere(p => p == null || !activeProjectiles.Contains(p));
            }
            catch (Exception ex)
            {
                _logger.Error($"清理子弹记录失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 获取子弹已飞行的距离
        /// </summary>
        private static float GetProjectileTraveledDistance(Projectile projectile)
        {
            try
            {
                var projectileType = projectile.GetType();
                var field = projectileType.GetField("_traveledDistance", 
                    BindingFlags.NonPublic | BindingFlags.Instance);
                
                if (field != null)
                {
                    return (float)field.GetValue(projectile);
                }
            }
            catch { }
            
            return 0f;
        }
        
        /// <summary>
        /// 查找最佳目标（使用屏幕半径圆形检测）
        /// </summary>
        private AutoAimCandidate FindBestTarget()
        {
            try
            {
                if (!_isInitialized)
                {
                    Initialize();
                }
                
                if (_damageReceiverCache == null || _damageReceiverCache.Count == 0)
                {
                    return default;
                }
                
                Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                AutoAimCandidate bestCandidate = default;
                float bestAngle = float.MaxValue;
                
                Vector3 cameraOrigin = _mainCamera.transform.position;
                Vector3 cameraForward = _mainCamera.transform.forward;
                
                Vector3 muzzlePosition = _trackedGun != null && _trackedGun.muzzle != null
                    ? _trackedGun.muzzle.position
                    : _player.transform.position;
                
                Vector3 gunForward = _trackedGun != null && _trackedGun.muzzle != null
                    ? _trackedGun.muzzle.forward
                    : _trackedGun != null ? _trackedGun.transform.forward : _player.transform.forward;
                
                float maxRange = _trackedGun != null ? _trackedGun.BulletDistance : _config.AimbotMaxDistance;
                
                foreach (DamageReceiver receiver in _damageReceiverCache.ToList())
                {
                    Vector3 aimPoint;
                    if (!IsCandidateValid(receiver, out aimPoint))
                        continue;
                    
                    float distance = Vector3.Distance(muzzlePosition, aimPoint);
                    if (distance > maxRange)
                        continue;
                    
                    Vector3 directionToTarget = aimPoint - cameraOrigin;
                    float rayDistance = Vector3.Dot(directionToTarget, cameraForward);
                    
                    if (rayDistance <= 0)
                        continue;
                    
                    Vector3 screenPoint = _mainCamera.WorldToScreenPoint(aimPoint);
                    
                    if (screenPoint.z <= 0)
                        continue;
                    
                    Vector2 screenPos = new Vector2(screenPoint.x, screenPoint.y);
                    float screenDistance = Vector2.Distance(screenCenter, screenPos);
                    
                    EvaluateCandidate(receiver, aimPoint, muzzlePosition, gunForward, 
                                     screenDistance, rayDistance, ref bestCandidate, ref bestAngle);
                }
                
                return bestCandidate;
            }
            catch (Exception ex)
            {
                _logger.Error($"查找目标时出错 - {ex.Message}");
                return default;
            }
        }
        
        /// <summary>
        /// 评估候选目标（基于枪口指向角度）
        /// </summary>
        private void EvaluateCandidate(DamageReceiver receiver, Vector3 aimPoint, Vector3 muzzlePosition,
                                       Vector3 gunForward, float screenDistance, float rayDistance,
                                       ref AutoAimCandidate candidate, ref float bestAngle)
        {
            Vector3 toTarget = aimPoint - muzzlePosition;
            if (toTarget.sqrMagnitude < 0.04f)
                return;
            
            toTarget.Normalize();
            float angle = Vector3.Angle(gunForward, toTarget);
            
            float obstructionDistance;
            bool hasLineOfSight = HasLineOfSight(muzzlePosition, aimPoint, receiver, out obstructionDistance);
            
            if (!hasLineOfSight && !_config.AimbotIgnoreWalls)
                return;
            
            float angleScore = angle;
            
            if (!hasLineOfSight)
            {
                float obstaclePenalty = 15f + Mathf.Min(10f, obstructionDistance * 2f);
                angleScore += obstaclePenalty;
            }
            
            float distance = Vector3.Distance(muzzlePosition, aimPoint);
            if (distance > 50f)
            {
                angleScore += (distance - 50f) * 0.1f;
            }
            
            if (angleScore < bestAngle)
            {
                candidate = new AutoAimCandidate
                {
                    Receiver = receiver,
                    AimPoint = aimPoint,
                    ScreenDistance = screenDistance,
                    RayDistance = rayDistance,
                    RequiresPenetration = !hasLineOfSight,
                    Angle = angle
                };
                bestAngle = angleScore;
            }
        }
        
        /// <summary>
        /// 检查视线是否通畅
        /// </summary>
        private bool HasLineOfSight(Vector3 origin, Vector3 target, DamageReceiver receiver, out float obstructionDistance)
        {
            obstructionDistance = 0f;
            
            Vector3 direction = target - origin;
            float distance = direction.magnitude;
            
            if (distance <= 0.2f)
                return true;
            
            direction.Normalize();
            
            RaycastHit hit;
            bool hitSomething = Physics.Raycast(origin, direction, out hit, distance - 0.2f, ObstacleLayerMaskValue, QueryTriggerInteraction.Ignore);
            
            if (!hitSomething)
            {
                return true;
            }
            
            obstructionDistance = hit.distance;
            
            DamageReceiver hitReceiver = hit.collider.GetComponentInParent<DamageReceiver>();
            if (hitReceiver != null && hitReceiver == receiver)
            {
                return true;
            }
            
            obstructionDistance = hit.distance;
            return false;
        }
        
        /// <summary>
        /// 检查候选目标是否有效
        /// </summary>
        private bool IsCandidateValid(DamageReceiver receiver, out Vector3 aimPoint)
        {
            aimPoint = Vector3.zero;
            
            if (receiver == null || !receiver.enabled)
                return false;
            
            if (receiver.gameObject == null || !receiver.gameObject.activeInHierarchy)
                return false;
            
            if (receiver.IsDead)
                return false;
            
            if (receiver.IsMainCharacter)
                return false;
            
            if (!ReceiverHasHealth(receiver))
                return false;
            
            // ===== 队伍和中立单位判断逻辑 =====
            bool isEnemy = Team.IsEnemy(_player.Team, receiver.Team);
            
            if (_config.AimbotIgnoreTeamCheck)
            {
                if (receiver.IsMainCharacter)
                    return false;
            }
            else
            {
                // 特殊处理：中立单位（Teams.middle）
                // 默认不瞄准中立单位，除非它已经被激怒（noticed = true）
                if (receiver.Team == Teams.middle)
                {
                    // 尝试从 health 获取 CharacterMainControl
                    CharacterMainControl? character = null;
                    if (receiver.health != null)
                    {
                        character = receiver.health.TryGetCharacter();
                    }
                    
                    if (character != null)
                    {
                        var aiController = character.GetComponent<AICharacterController>();
                        if (aiController != null)
                        {
                            // 只有当中立单位已经注意到玩家（被攻击过）时才瞄准
                            if (!aiController.noticed)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            // 没有AI控制器的中立单位，默认不瞄准
                            return false;
                        }
                    }
                    else
                    {
                        // 无法获取角色信息的中立单位，默认不瞄准
                        return false;
                    }
                }
                else if (!isEnemy)
                {
                    // 非中立单位，使用标准敌对判断
                    return false;
                }
            }
            
            if (receiver.Team == Teams.all)
                return false;
            
            aimPoint = GetReceiverAimPoint(receiver);
            return true;
        }
        
        /// <summary>
        /// 检查 DamageReceiver 是否有生命值
        /// </summary>
        private static bool ReceiverHasHealth(DamageReceiver receiver)
        {
            if (receiver.useSimpleHealth)
            {
                return receiver.simpleHealth != null;
            }
            return receiver.health != null;
        }
        
        /// <summary>
        /// 获取 DamageReceiver 的瞄准点
        /// </summary>
        private Vector3 GetReceiverAimPoint(DamageReceiver receiver)
        {
            try
            {
                Collider collider = receiver.GetComponent<Collider>();
                if (collider != null)
                {
                    return collider.bounds.center;
                }
                
                if (_config.AimbotAimAtHead)
                {
                    Transform headTransform = TryGetHeadTransform(receiver);
                    if (headTransform != null)
                    {
                        Vector3 randomOffset = new Vector3(
                            UnityEngine.Random.Range(-0.03f, 0.03f),
                            UnityEngine.Random.Range(0.02f, 0.06f),
                            UnityEngine.Random.Range(-0.03f, 0.03f)
                        );
                        return headTransform.position + randomOffset;
                    }
                }
                
                return receiver.transform.position + Vector3.up * 0.5f;
            }
            catch
            {
                return receiver.transform.position + Vector3.up * 0.5f;
            }
        }
        
        /// <summary>
        /// 尝试获取头部Transform
        /// </summary>
        private Transform TryGetHeadTransform(DamageReceiver receiver)
        {
            try
            {
                if (!_headColliderCache.TryGetValue(receiver, out HeadCollider headCollider))
                {
                    headCollider = receiver.GetComponentInChildren<HeadCollider>();
                    _headColliderCache[receiver] = headCollider;
                }
                
                if (headCollider != null)
                {
                    return headCollider.transform;
                }
                
                if (receiver.health != null)
                {
                    CharacterMainControl character = receiver.health.TryGetCharacter();
                    if (character != null && character.characterModel != null)
                    {
                        if (character.characterModel.HelmatSocket != null)
                        {
                            return character.characterModel.HelmatSocket;
                        }
                        
                        if (character.characterModel.FaceMaskSocket != null)
                        {
                            return character.characterModel.FaceMaskSocket;
                        }
                        
                        return character.transform;
                    }
                }
                
                if (receiver.simpleHealth != null)
                {
                    return receiver.simpleHealth.transform;
                }
            }
            catch { }
            
            return null;
        }
        
        /// <summary>
        /// 执行自动扳机
        /// </summary>
        private void PerformTriggerBot()
        {
            if (IsPlayerInBase())
            {
                if (_lastTriggerState)
                {
                    ReleaseTrigger();
                }
                _targetInSight = false;
                _triggerDelayTimer = 0f;
                return;
            }
            
            if (_config.TriggerBotOnlyADS)
            {
                bool isAiming = Input.GetMouseButton(1);
                if (!isAiming)
                {
                    if (_lastTriggerState)
                    {
                        ReleaseTrigger();
                    }
                    _targetInSight = false;
                    _triggerDelayTimer = 0f;
                    return;
                }
            }
            
            bool currentTargetInSight = (_lastBestTarget.Receiver != null);
            
            if (currentTargetInSight && _lastBestTarget.RequiresPenetration && !_config.AimbotIgnoreWalls)
            {
                currentTargetInSight = false;
            }
            
            if (currentTargetInSight && _lastBestTarget.RequiresPenetration)
            {
                currentTargetInSight = false;
            }
            
            if (currentTargetInSight && !_targetInSight)
            {
                _targetInSight = true;
                _triggerDelayTimer = _config.TriggerBotDelay;
            }
            else if (!currentTargetInSight && _targetInSight)
            {
                _targetInSight = false;
                _triggerDelayTimer = 0f;
            }
            
            if (_triggerDelayTimer > 0)
            {
                _triggerDelayTimer -= Time.deltaTime;
            }
            
            if (_targetInSight && _triggerDelayTimer <= 0)
            {
                TrySingleShot();
            }
            else
            {
                if (_lastTriggerState)
                {
                    ReleaseTrigger();
                }
            }
        }
        
        /// <summary>
        /// 单发射击
        /// </summary>
        private void TrySingleShot()
        {
            if (_trackedGun == null) return;
            
            _continuousFireTimer += Time.deltaTime;
            if (_continuousFireTimer < FIRE_RATE_INTERVAL)
            {
                if (_lastTriggerState)
                {
                    ReleaseTrigger();
                }
                return;
            }
            
            _continuousFireTimer = 0f;
            
            try
            {
                if (!_lastTriggerState)
                {
                    _trackedGun.SetTrigger(true, true, false);
                    _lastTriggerState = true;
                }
            }
            catch (Exception)
            {
            }
        }
        
        /// <summary>
        /// 释放扳机
        /// </summary>
        private void ReleaseTrigger()
        {
            if (_trackedGun == null)
                return;
            
            try
            {
                bool justReleased = _lastTriggerState;
                _trackedGun.SetTrigger(false, false, justReleased);
                _lastTriggerState = false;
                _continuousFireTimer = 0f;
            }
            catch (Exception)
            {
            }
        }
        
        /// <summary>
        /// 检测玩家是否在基地内
        /// </summary>
        private bool IsPlayerInBase()
        {
            try
            {
                string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                
                if (sceneName == "Base_SceneV2")
                {
                    return true;
                }
                
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        /// <summary>
        /// 候选目标信息
        /// </summary>
        private struct AutoAimCandidate
        {
            public DamageReceiver Receiver;
            public Vector3 AimPoint;
            public float ScreenDistance;
            public float RayDistance;
            public bool RequiresPenetration;
            public float Angle;
        }
    }
}

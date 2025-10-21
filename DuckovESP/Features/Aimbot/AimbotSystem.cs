using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ItemStatsSystem;
using UnityEngine;

namespace DuckovESP
{
    /// <summary>
    /// 自动瞄准系统 - 基于 Mod_Aimbot 的优秀实现重构
    /// 核心思路：在射击后重定向子弹，而不是控制视角
    /// </summary>
    public class AimbotSystem
    {
        private ESPConfig _config;
        private Camera _mainCamera;
        private CharacterMainControl _player;
        private ItemAgent_Gun _trackedGun;
        private bool _gunMethodsListed = false; // 防止重复打印

        /// <summary>
        /// 列出枪械对象的所有方法（用于调试）
        /// </summary>
        private void ListGunMethods()
        {
            if (_trackedGun == null || _gunMethodsListed) return;
            
            Debug.Log("=== ItemAgent_Gun 可用方法列表 ===");
            var methods = _trackedGun.GetType().GetMethods(
                System.Reflection.BindingFlags.Public | 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            foreach (var method in methods)
            {
                // 过滤掉基类方法
                if (method.DeclaringType == typeof(object)) continue;
                
                var parameters = method.GetParameters();
                var paramString = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
                Debug.Log($"  {method.ReturnType.Name} {method.Name}({paramString})");
            }
            Debug.Log("================================");
            
            _gunMethodsListed = true;
        }
        private CheatSystem _cheatSystem; // 作弊系统引用
        
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
        private bool _triggerBotLoggedOnce = false; // 防止重复日志
        private AutoAimCandidate _lastBestTarget; // 缓存最佳目标（供自动扳机使用）
        
        // 障碍物层遮罩（用于LineOfSight检测）
        private static int ObstacleLayerMaskValue = -1;
        
        // ===== 性能优化：缓存系统 =====
        private DamageReceiver[] _damageReceiverCache = new DamageReceiver[0];
        private float _lastReceiverCacheUpdate = 0f;
        private const float RECEIVER_CACHE_INTERVAL = 1.5f; // 1.5秒更新一次目标列表
        
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
        
        public AimbotSystem(ESPConfig config)
        {
            _config = config;
            
            // 初始化障碍物层遮罩
            if (ObstacleLayerMaskValue == -1)
            {
                try
                {
                    int wallLayer = LayerMask.NameToLayer("Wall");
                    int groundLayer = LayerMask.NameToLayer("Ground");
                    int halfObstacleLayer = LayerMask.NameToLayer("HalfObsticle");
                    
                    ObstacleLayerMaskValue = 0;
                    if (wallLayer >= 0)
                    {
                        ObstacleLayerMaskValue |= (1 << wallLayer);
                        Debug.Log($"[Aimbot] 添加 Wall 层 (Layer {wallLayer})");
                    }
                    if (groundLayer >= 0)
                    {
                        ObstacleLayerMaskValue |= (1 << groundLayer);
                        Debug.Log($"[Aimbot] 添加 Ground 层 (Layer {groundLayer})");
                    }
                    if (halfObstacleLayer >= 0)
                    {
                        ObstacleLayerMaskValue |= (1 << halfObstacleLayer);
                        Debug.Log($"[Aimbot] 添加 HalfObsticle 层 (Layer {halfObstacleLayer})");
                    }
                    
                    if (ObstacleLayerMaskValue == 0)
                    {
                        Debug.LogWarning("[Aimbot] ⚠️ 没有找到任何障碍物层！使用默认层遮罩");
                        // 使用默认层遮罩（除了 IgnoreRaycast 之外的所有层）
                        ObstacleLayerMaskValue = ~(1 << LayerMask.NameToLayer("Ignore Raycast"));
                    }
                    else
                    {
                        Debug.Log($"[Aimbot] 障碍物层遮罩初始化完成: 0x{ObstacleLayerMaskValue:X}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[Aimbot] 初始化障碍物层失败: {ex.Message}");
                    // 使用默认层遮罩
                    ObstacleLayerMaskValue = ~(1 << LayerMask.NameToLayer("Ignore Raycast"));
                }
            }
        }
        
        /// <summary>
        /// 设置作弊系统引用
        /// </summary>
        public void SetCheatSystem(CheatSystem cheatSystem)
        {
            _cheatSystem = cheatSystem;
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
                // 检查是否有武器
                if (_trackedGun == null && !_triggerBotLoggedOnce)
                {
                    Debug.Log("[TriggerBot] 警告: 没有持有武器");
                }
                
                PerformTriggerBot();
            }
            else
            {
                // 禁用时确保释放扳机
                if (_lastTriggerState)
                {
                    ReleaseTrigger();
                }
                
                // 重置状态
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
                    
                    // 列出枪械方法用于调试
                    ListGunMethods();
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
                Debug.LogWarning($"[NoRecoil] 应用无后座力时出错: {ex.Message}");
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
                Debug.LogWarning($"[NoRecoil] 设置 Item Stat 时出错: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 尝试通过反射获取刚发射的子弹实例（快速可靠）
        /// </summary>
        private Projectile TryGetImmediateProjectile(ItemAgent_Gun gun)
        {
            if (gun == null)
                return null;
            
            if (GunProjectileField == null)
                return null;
            
            try
            {
                // 使用反射获取私有字段 projInst
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
                        // 只处理飞行距离小于 0.06 米的子弹（参考 Mod_Aimbot）
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
                Debug.LogError($"[AimbotSystem] 反射获取子弹失败: {ex.Message}");
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
            
            // 清理已失效的子弹记录（参考 Mod_Aimbot 的 CleanupRetargetedProjectiles）
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
                // 传送模式：子弹直接生成在目标附近（绕过墙体）
                TeleportProjectilesToTarget(candidate.Receiver, candidate.AimPoint);
            }
            else
            {
                // 正常模式：重定向子弹轨迹
                RetargetProjectilesToTarget(muzzlePosition, candidate.AimPoint);
            }
        }
        
        /// <summary>
        /// 重定向所有未处理的子弹到目标（优先使用反射，扫描作为后备）
        /// </summary>
        private void RetargetProjectilesToTarget(Vector3 muzzlePosition, Vector3 targetPoint)
        {
            try
            {
                Vector3 direction = (targetPoint - muzzlePosition).normalized;
                int retargetedCount = 0;
                int requiredCount = Mathf.Max(1, _trackedGun != null ? _trackedGun.ShotCount : 1);
                
                // 方法1: 优先使用反射直接获取刚发射的子弹（快速可靠）
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
                        int playerProjectiles = 0;
                        int retargetedAlready = 0;
                        int tooFar = 0;
                        
                        // 只重定向玩家刚发射的子弹
                        foreach (Projectile projectile in allProjectiles
                            .Where(p => p != null && p.context.fromCharacter == _player)
                            .OrderBy(p => GetProjectileTraveledDistance(p)))
                        {
                            playerProjectiles++;
                            
                            // 检查是否已经处理过
                            if (_retargetedProjectiles.Contains(projectile))
                            {
                                retargetedAlready++;
                                continue;
                            }
                            
                            // 只重定向刚发射的子弹（飞行距离 < 0.06米，参考 Mod_Aimbot）
                            float traveledDistance = GetProjectileTraveledDistance(projectile);
                            if (traveledDistance > 0.06f)
                            {
                                tooFar++;
                                continue;
                            }
                            
                            // 重定向子弹
                            if (RetargetProjectile(projectile, direction, targetPoint))
                            {
                                _retargetedProjectiles.Add(projectile);
                                retargetedCount++;
                                
                                // 限制处理数量
                                if (retargetedCount >= requiredCount)
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Aimbot] 重定向子弹时出错: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 重定向单个子弹（完全照搬 Mod_Aimbot 的实现）
        /// </summary>
        private bool RetargetProjectile(Projectile projectile, Vector3 direction, Vector3 targetPoint)
        {
            if (projectile == null)
                return false;
            
            if (direction.sqrMagnitude < 0.0001f)
                return false;
            
            direction.Normalize();
            
            try
            {
                // 1. 设置 context.direction
                projectile.context.direction = direction;
                
                // 2. 设置 firstFrameCheckStartPoint
                projectile.context.firstFrameCheckStartPoint = projectile.transform.position - direction * 0.1f;
                
                // 3. 确保射程足够
                float distanceToTarget = Vector3.Distance(projectile.transform.position, targetPoint) + 2f;
                if (projectile.context.distance < distanceToTarget)
                {
                    projectile.context.distance = distanceToTarget;
                }
                
                // 4. 如果瞄准头部，设置暴击
                if (_config.AimbotAimAtHead)
                {
                    projectile.context.critRate = 1f;
                    projectile.context.ignoreHalfObsticle = true;
                }
                
                // 5. 【关键】使用反射设置私有字段 direction（参考 Mod_Aimbot）
                if (ProjectileDirectionField != null)
                {
                    ProjectileDirectionField.SetValue(projectile, direction);
                }
                
                // 6. 【关键】使用反射设置私有字段 velocity（参考 Mod_Aimbot）
                if (ProjectileVelocityField != null)
                {
                    Vector3 velocity = direction * projectile.context.speed;
                    ProjectileVelocityField.SetValue(projectile, velocity);
                }
                
                // 7. 设置旋转
                projectile.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Aimbot] 重定向单个子弹失败: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }
        
        /// <summary>
        /// 传送模式：将子弹直接传送到目标附近（绕过墙体检测）
        /// TriggerBot 专用，用于解决墙体检测问题
        /// </summary>
        private void TeleportProjectilesToTarget(DamageReceiver target, Vector3 targetPoint)
        {
            try
            {
                int teleportedCount = 0;
                int requiredCount = Mathf.Max(1, _trackedGun != null ? _trackedGun.ShotCount : 1);
                
                // 方法1: 优先使用反射直接获取刚发射的子弹
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
                
                // 方法2: 如果反射失败，扫描场景中的子弹（后备方案）
                if (teleportedCount < requiredCount)
                {
                    Projectile[] allProjectiles = UnityEngine.Object.FindObjectsOfType<Projectile>();
                    foreach (Projectile projectile in allProjectiles)
                    {
                        if (projectile == null || _retargetedProjectiles.Contains(projectile))
                            continue;
                        
                        // 检查是否是玩家的子弹（使用 fromCharacter 字段）
                        if (projectile.context.fromCharacter == _player)
                        {
                            // 检查 traveledDistance（只传送刚发射的子弹）
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
                
                if (teleportedCount > 0)
                {
                    Debug.Log($"[Aimbot] TriggerBot传送模式: 已传送 {teleportedCount} 发子弹到目标附近");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Aimbot] 传送子弹时出错: {ex.Message}");
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
                // 计算传送位置：目标前方1-2米，随机偏移避免过于明显
                Vector3 toTarget = (targetPoint - projectile.transform.position).normalized;
                float randomOffset = UnityEngine.Random.Range(1f, 2.5f);
                Vector3 teleportPosition = targetPoint - toTarget * randomOffset;
                
                // 添加随机偏移（让子弹看起来更自然）
                teleportPosition += new Vector3(
                    UnityEngine.Random.Range(-0.3f, 0.3f),
                    UnityEngine.Random.Range(-0.2f, 0.2f),
                    UnityEngine.Random.Range(-0.3f, 0.3f)
                );
                
                // 传送子弹位置
                projectile.transform.position = teleportPosition;
                
                // 设置子弹方向（指向目标）
                Vector3 direction = (targetPoint - teleportPosition).normalized;
                
                // 1. 设置 context.direction
                projectile.context.direction = direction;
                
                // 2. 设置 firstFrameCheckStartPoint
                projectile.context.firstFrameCheckStartPoint = teleportPosition - direction * 0.1f;
                
                // 3. 确保射程足够（只需要打到目标的距离）
                float distanceToTarget = Vector3.Distance(teleportPosition, targetPoint) + 1f;
                projectile.context.distance = distanceToTarget;
                
                // 4. 如果瞄准头部，设置暴击
                if (_config.AimbotAimAtHead)
                {
                    projectile.context.critRate = 1f;
                    projectile.context.ignoreHalfObsticle = true;
                }
                
                // 5. 使用反射设置私有字段 direction
                if (ProjectileDirectionField != null)
                {
                    ProjectileDirectionField.SetValue(projectile, direction);
                }
                
                // 6. 使用反射设置私有字段 velocity
                if (ProjectileVelocityField != null)
                {
                    Vector3 velocity = direction * projectile.context.speed;
                    ProjectileVelocityField.SetValue(projectile, velocity);
                }
                
                // 7. 设置旋转
                projectile.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                
                // 8. 重置已飞行距离（让子弹从新位置开始）
                if (ProjectileTraveledDistanceField != null)
                {
                    ProjectileTraveledDistanceField.SetValue(projectile, 0f);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Aimbot] 传送单个子弹失败: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// 清理已失效的子弹记录（参考 Mod_Aimbot）
        /// </summary>
        private void CleanupRetargetedProjectiles()
        {
            if (_retargetedProjectiles.Count == 0)
                return;
            
            try
            {
                // 获取当前场景中所有子弹的实例ID
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
                
                // 移除已销毁的子弹
                _retargetedProjectiles.RemoveWhere(p => p == null || !activeProjectiles.Contains(p));
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Aimbot] 清理子弹记录失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 获取子弹已飞行的距离
        /// </summary>
        private static float GetProjectileTraveledDistance(Projectile projectile)
        {
            try
            {
                // 尝试通过反射获取私有字段 _traveledDistance
                var projectileType = projectile.GetType();
                var field = projectileType.GetField("_traveledDistance", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);
                
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
        /// 性能优化：使用缓存的 DamageReceiver 列表，减少 FindObjectsOfType 调用
        /// </summary>
        private AutoAimCandidate FindBestTarget()
        {
            try
            {
                // 性能优化：使用缓存的接收器列表
                float currentTime = Time.time;
                if (currentTime - _lastReceiverCacheUpdate > RECEIVER_CACHE_INTERVAL)
                {
                    _damageReceiverCache = UnityEngine.Object.FindObjectsOfType<DamageReceiver>();
                    _lastReceiverCacheUpdate = currentTime;
                    
                    // 清理已销毁对象的缓存
                    ClearInvalidCaches();
                }
                
                DamageReceiver[] allReceivers = _damageReceiverCache;
                
                if (allReceivers == null || allReceivers.Length == 0)
                {
                    return default;
                }
                
                // 屏幕中心点
                Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                
                // 最佳候选目标
                AutoAimCandidate bestCandidate = default;
                float bestScore = float.MaxValue;
                
                // 摄像机原点和方向
                Vector3 cameraOrigin = _mainCamera.transform.position;
                Vector3 cameraForward = _mainCamera.transform.forward;
                
                // 枪口位置
                Vector3 muzzlePosition = _trackedGun != null && _trackedGun.muzzle != null
                    ? _trackedGun.muzzle.position
                    : _player.transform.position;
                
                // 最大射程
                float maxRange = _trackedGun != null ? _trackedGun.BulletDistance : _config.AimbotMaxDistance;
                
                int validCount = 0;
                int distanceFailCount = 0;
                int screenFailCount = 0;
                
                foreach (DamageReceiver receiver in allReceivers)
                {
                    // 基础有效性检查
                    Vector3 aimPoint;
                    if (!IsCandidateValid(receiver, out aimPoint))
                        continue;
                    
                    validCount++;
                    
                    // 距离检查
                    float distance = Vector3.Distance(muzzlePosition, aimPoint);
                    if (distance > maxRange)
                    {
                        distanceFailCount++;
                        continue;
                    }
                    
                    // 计算从摄像机到目标的方向
                    Vector3 directionToTarget = aimPoint - cameraOrigin;
                    float rayDistance = Vector3.Dot(directionToTarget, cameraForward);
                    
                    // 目标在摄像机后面
                    if (rayDistance <= 0)
                    {
                        screenFailCount++;
                        continue;
                    }
                    
                    // 转换为屏幕坐标
                    Vector3 screenPoint = _mainCamera.WorldToScreenPoint(aimPoint);
                    
                    // 屏幕外或摄像机后面
                    if (screenPoint.z <= 0)
                    {
                        screenFailCount++;
                        continue;
                    }
                    
                    // 计算屏幕距离（仅用于优先级排序，不做限制）
                    Vector2 screenPos = new Vector2(screenPoint.x, screenPoint.y);
                    float screenDistance = Vector2.Distance(screenCenter, screenPos);
                    
                    // 移除 FOV 限制 - 游戏只在靠近时刷怪，不需要限制屏幕范围
                    // if (screenDistance > _config.AimbotFOV) continue;
                    
                    // 评估候选目标
                    EvaluateCandidate(receiver, aimPoint, muzzlePosition, cameraForward, 
                                     screenDistance, rayDistance, ref bestCandidate, ref bestScore);
                }
                
                
                return bestCandidate;
            }
            catch (Exception ex)
            {
                Debug.LogError($"DuckovESP Aimbot: 查找目标时出错 - {ex.Message}");
                return default;
            }
        }
        
        /// <summary>
        /// 评估候选目标
        /// </summary>
        private void EvaluateCandidate(DamageReceiver receiver, Vector3 aimPoint, Vector3 muzzlePosition,
                                       Vector3 cameraForward, float screenDistance, float rayDistance,
                                       ref AutoAimCandidate candidate, ref float bestScore)
        {
            // 计算从枪口到目标的方向
            Vector3 toTarget = aimPoint - muzzlePosition;
            if (toTarget.sqrMagnitude < 0.04f) // 太近了
                return;
            
            // 视野遮挡检查
            float obstructionDistance;
            bool hasLineOfSight = HasLineOfSight(muzzlePosition, aimPoint, receiver, out obstructionDistance);
            
            // 如果没有视线且不允许穿墙，跳过
            if (!hasLineOfSight && !_config.AimbotIgnoreWalls)
                return;
            
            // 计算角度
            Vector3 gunForward = _trackedGun != null && _trackedGun.muzzle != null
                ? _trackedGun.muzzle.forward
                : _trackedGun.transform.forward;
            float angle = Vector3.Angle(gunForward, toTarget);
            
            // 计算综合得分（越小越好）
            // 主要因素：屏幕距离
            // 次要因素：角度偏差、实际距离
            float score = screenDistance + (angle * 2.25f) + (Mathf.Max(0f, rayDistance) * 0.1f);
            
            // 如果被遮挡，增加惩罚
            if (!hasLineOfSight)
            {
                float obstaclePenalty = 35f + Mathf.Max(0f, obstructionDistance) * 1.75f;
                score += obstaclePenalty;
            }
            
            // 更新最佳候选
            if (score < bestScore)
            {
                candidate = new AutoAimCandidate
                {
                    Receiver = receiver,
                    AimPoint = aimPoint,
                    ScreenDistance = screenDistance,
                    RayDistance = rayDistance,
                    RequiresPenetration = !hasLineOfSight
                };
                bestScore = score;
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
            
            // 射线检测
            RaycastHit hit;
            bool hitSomething = Physics.Raycast(origin, direction, out hit, distance - 0.2f, ObstacleLayerMaskValue, QueryTriggerInteraction.Ignore);
            
            if (!hitSomething)
            {
                return true; // 没有障碍物
            }
            
            // 记录碰撞信息（用于调试）
            obstructionDistance = hit.distance;
            
            // 检查碰撞物是否就是目标本身
            DamageReceiver hitReceiver = hit.collider.GetComponentInParent<DamageReceiver>();
            if (hitReceiver != null && hitReceiver == receiver)
            {
                return true; // 直接命中目标
            }
            
            // 被障碍物遮挡
            // Debug.Log($"[Aimbot] 目标被遮挡: {hit.collider.name} (Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}, 距离: {hit.distance:F2}m)");
            
            obstructionDistance = hit.distance;
            return false; // 被障碍物遮挡
        }
        
        /// <summary>
        /// 检查候选目标是否有效
        /// </summary>
        private bool IsCandidateValid(DamageReceiver receiver, out Vector3 aimPoint)
        {
            aimPoint = Vector3.zero;
            
            if (receiver == null || !receiver.enabled)
            {
                return false;
            }
            
            if (receiver.gameObject == null || !receiver.gameObject.activeInHierarchy)
            {
                return false;
            }
            
            // 已死亡
            if (receiver.IsDead)
            {
                return false;
            }
            
            // 是主角自己
            if (receiver.IsMainCharacter)
            {
                return false;
            }
            
            // 没有生命值
            if (!ReceiverHasHealth(receiver))
            {
                return false;
            }
            
            // 队伍检查
            bool isEnemy = Team.IsEnemy(_player.Team, receiver.Team);
            
            // 测试模式：忽略队伍检查（用于测试墙体检测）
            if (_config.AimbotIgnoreTeamCheck)
            {
                // 跳过队伍检查，但不攻击自己
                if (receiver.IsMainCharacter)
                {
                    return false;
                }
                // 继续执行后续检查
            }
            else if (!isEnemy)
            {
                // 记录第一个非敌人的情况用于调试
                if (UnityEngine.Random.value < 0.01f) // 1%概率记录，避免刷屏
                {
                    Debug.Log($"[Aimbot] 目标 {receiver.name} 队伍检查失败: 玩家队伍={_player.Team}, 目标队伍={receiver.Team}");
                }
                return false;
            }
            
            // 中立单位
            if (receiver.Team == Teams.all)
            {
                return false;
            }
            
            // 获取瞄准点
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
        /// 检测目标是否为无头 Boss（如暴走街机）
        /// 这些 Boss 没有头部或头部位置异常，需要强制瞄准身体
        /// </summary>
        private bool IsHeadlessBoss(DamageReceiver receiver)
        {
            if (receiver == null || receiver.name == null)
                return false;

            string name = receiver.name.ToLower();
            
            // 已知的无头 Boss 列表
            string[] headlessBosses = 
            {
                "暴走街机",      // 中文名
                "arcade",        // 可能的英文名
                "boss_arcade",   // 带前缀的名称
                "狂暴街机",      // 别名
                "疯狂街机"       // 别名
            };

            foreach (string bossName in headlessBosses)
            {
                if (name.Contains(bossName.ToLower()))
                {
                    // 首次检测到无头 Boss 时记录日志
                    if (UnityEngine.Random.value < 0.01f)
                    {
                        Debug.Log($"[Aimbot] 检测到无头 Boss: {receiver.name}，切换为身体瞄准模式");
                    }
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// 获取 DamageReceiver 的瞄准点
        /// 性能优化：缓存 Collider 和 HeadCollider
        /// 特殊处理：对于没有头部的 Boss（如暴走街机），自动改为身体瞄准
        /// </summary>
        private Vector3 GetReceiverAimPoint(DamageReceiver receiver)
        {
            try
            {
                // 步骤1：首先使用 Collider.bounds.center（最可靠的方法 - 参考 Mod_Aimbot）
                Collider collider = receiver.GetComponent<Collider>();
                if (collider != null)
                {
                    return collider.bounds.center;
                }
                
                // 步骤2：如果启用了瞄准头部，尝试找头部
                if (_config.AimbotAimAtHead)
                {
                    Transform headTransform = TryGetHeadTransform(receiver);
                    if (headTransform != null)
                    {
                        // 添加小随机偏移（模拟真实瞄准）
                        Vector3 randomOffset = new Vector3(
                            UnityEngine.Random.Range(-0.03f, 0.03f),
                            UnityEngine.Random.Range(0.02f, 0.06f),
                            UnityEngine.Random.Range(-0.03f, 0.03f)
                        );
                        return headTransform.position + randomOffset;
                    }
                }
                
                // 步骤3：默认位置（身体中心向上 0.5 米）- Mod_Aimbot 的标准做法
                return receiver.transform.position + Vector3.up * 0.5f;
            }
            catch
            {
                return receiver.transform.position + Vector3.up * 0.5f;
            }
        }
        
        /// <summary>
        /// 尝试获取头部Transform
        /// 性能优化：缓存 HeadCollider
        /// </summary>
        private Transform TryGetHeadTransform(DamageReceiver receiver)
        {
            try
            {
                // 性能优化：使用缓存的 HeadCollider
                if (!_headColliderCache.TryGetValue(receiver, out HeadCollider headCollider))
                {
                    headCollider = receiver.GetComponentInChildren<HeadCollider>();
                    _headColliderCache[receiver] = headCollider; // 即使为 null 也缓存，避免重复查找
                }
                
                if (headCollider != null)
                {
                    return headCollider.transform;
                }
                
                // 尝试通过 Health 获取角色
                if (receiver.health != null)
                {
                    CharacterMainControl character = receiver.health.TryGetCharacter();
                    if (character != null && character.characterModel != null)
                    {
                        // 尝试获取头盔插槽
                        if (character.characterModel.HelmatSocket != null)
                        {
                            return character.characterModel.HelmatSocket;
                        }
                        
                        // 尝试获取面罩插槽
                        if (character.characterModel.FaceMaskSocket != null)
                        {
                            return character.characterModel.FaceMaskSocket;
                        }
                        
                        return character.transform;
                    }
                }
                
                // 尝试通过 SimpleHealth
                if (receiver.simpleHealth != null)
                {
                    return receiver.simpleHealth.transform;
                }
            }
            catch { }
            
            return null;
        }
        
        /// <summary>
        /// 执行自动扳机（使用自动瞄准的目标检测）
        /// </summary>
        private void PerformTriggerBot()
        {
            // 首次启动时记录
            if (!_triggerBotLoggedOnce)
            {
                Debug.Log($"[TriggerBot] 已启动 - 仅瞄准触发: {_config.TriggerBotOnlyADS}, 延迟: {_config.TriggerBotDelay}s");
                Debug.Log($"[TriggerBot] 使用自动瞄准的目标检测系统");
                Debug.Log($"[TriggerBot] 基地保护：在基地内禁用自动扳机");
                _triggerBotLoggedOnce = true;
            }
            
            // 基地检测：在基地内禁用自动扳机
            if (IsPlayerInBase())
            {
                // 在基地内，确保扳机释放
                if (_lastTriggerState)
                {
                    ReleaseTrigger();
                }
                _targetInSight = false;
                _triggerDelayTimer = 0f;
                return;
            }
            
            // 检查是否仅在瞄准时触发（检测右键是否按下）
            if (_config.TriggerBotOnlyADS)
            {
                bool isAiming = Input.GetMouseButton(1); // 右键 = 瞄准
                if (!isAiming)
                {
                    // 不在瞄准状态，确保扳机释放
                    if (_lastTriggerState)
                    {
                        ReleaseTrigger();
                    }
                    _targetInSight = false;
                    _triggerDelayTimer = 0f;
                    return;
                }
            }
            
            // 使用自动瞄准找到的最佳目标
            bool currentTargetInSight = (_lastBestTarget.Receiver != null);
            
            // 详细调试：输出目标状态
            if (UnityEngine.Random.value < 0.05f) // 5% 概率输出，避免刷屏
            {
                if (_lastBestTarget.Receiver != null)
                {
                    Debug.Log($"[TriggerBot] 当前目标: {_lastBestTarget.Receiver.name}, RequiresPenetration={_lastBestTarget.RequiresPenetration}, IgnoreWalls={_config.AimbotIgnoreWalls}");
                }
                else
                {
                    Debug.Log($"[TriggerBot] 无目标 (_lastBestTarget.Receiver == null)");
                }
            }
            
            // TriggerBot 遵守 AimbotIgnoreWalls 设置
            // 如果目标被墙遮挡且不允许穿墙，则不触发
            if (currentTargetInSight && _lastBestTarget.RequiresPenetration && !_config.AimbotIgnoreWalls)
            {
                Debug.Log($"[TriggerBot] 目标被墙遮挡，不触发 (RequiresPenetration={_lastBestTarget.RequiresPenetration}, IgnoreWalls={_config.AimbotIgnoreWalls})");
                currentTargetInSight = false;
            }
            
            // 额外检查：即使配置允许穿墙，TriggerBot 默认也不穿墙射击（可选）
            // 如果你想让 TriggerBot 也能穿墙，注释掉下面这段
            if (currentTargetInSight && _lastBestTarget.RequiresPenetration)
            {
                Debug.Log($"[TriggerBot] 目标被墙遮挡，TriggerBot 不穿墙射击");
                currentTargetInSight = false;
            }
            
            // 调试输出
            if (currentTargetInSight && !_targetInSight)
            {
                string wallStatus = _lastBestTarget.RequiresPenetration ? " [穿墙]" : " [无遮挡]";
                Debug.Log($"[TriggerBot] 检测到目标: {_lastBestTarget.Receiver.name}, 距离: {_lastBestTarget.RayDistance:F1}m{wallStatus}");
            }
            
            // 检测到新目标
            if (currentTargetInSight && !_targetInSight)
            {
                _targetInSight = true;
                _triggerDelayTimer = _config.TriggerBotDelay;
            }
            // 目标消失
            else if (!currentTargetInSight && _targetInSight)
            {
                _targetInSight = false;
                _triggerDelayTimer = 0f;
                Debug.Log("[TriggerBot] 目标消失");
            }
            
            // 更新延迟计时器
            if (_triggerDelayTimer > 0)
            {
                _triggerDelayTimer -= Time.deltaTime;
            }
            
            // 延迟结束，执行射击（改为单发而非持续按住）
            if (_targetInSight && _triggerDelayTimer <= 0)
            {
                // 单发射击模式：每间隔触发一次
                TrySingleShot();
            }
            else
            {
                // 没有目标或在延迟中时，确保扳机释放
                if (_lastTriggerState)
                {
                    ReleaseTrigger();
                }
            }
        }
        
        /// <summary>
        /// 获取准星下的目标（已弃用，自动扳机现在使用自动瞄准的目标检测）
        /// </summary>
        private DamageReceiver GetTargetUnderCrosshair()
        {
            try
            {
                if (_mainCamera == null || _trackedGun == null)
                    return null;
                
                // 从屏幕中心发射射线
                Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                RaycastHit hit;
                
                float maxRange = _trackedGun != null ? _trackedGun.BulletDistance : _config.AimbotMaxDistance;
                
                if (Physics.Raycast(ray, out hit, maxRange))
                {
                    // 尝试获取 DamageReceiver 组件
                    DamageReceiver receiver = hit.collider.GetComponentInParent<DamageReceiver>();
                    
                    if (receiver != null)
                    {
                        Vector3 aimPoint;
                        // 检查是否是有效目标
                        if (IsCandidateValid(receiver, out aimPoint))
                        {
                            return receiver;
                        }
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TriggerBot] 检测目标时出错 - {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// 尝试射击
        /// </summary>
        private void TryShoot()
        {
            try
            {
                if (_trackedGun == null)
                {
                    Debug.Log("DuckovESP TriggerBot: 没有持有武器");
                    return;
                }
                
                // 使用游戏的 SetTrigger 方法来模拟射击
                // SetTrigger(bool trigger, bool triggerThisFrame, bool releaseThisFrame)
                _trackedGun.SetTrigger(true, true, false);
                
                Debug.Log($"DuckovESP TriggerBot: 触发射击");
                
            }
            catch (Exception ex)
            {
                Debug.LogError($"DuckovESP TriggerBot: 射击时出错 - {ex.Message}\n{ex.StackTrace}");
            }
        }
        
        /// <summary>
        /// 单发射击（避免持续占用扳机）
        /// </summary>
        private void TrySingleShot()
        {
            if (_trackedGun == null) return;
            
            // 使用射击间隔控制（避免射速过快）
            _continuousFireTimer += Time.deltaTime;
            if (_continuousFireTimer < FIRE_RATE_INTERVAL)
            {
                // 还在冷却中，释放扳机等待
                if (_lastTriggerState)
                {
                    ReleaseTrigger();
                }
                return;
            }
            
            // 重置计时器
            _continuousFireTimer = 0f;
            
            try
            {
                // 单次触发：按下扳机
                if (!_lastTriggerState)
                {
                    _trackedGun.SetTrigger(true, true, false);  // 按下扳机，justPressed=true
                    _lastTriggerState = true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TriggerBot] 射击失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 尝试直接射击（多种方法）- 已弃用，使用 TrySingleShot 代替
        /// </summary>
        private void TryDirectShoot()
        {
            if (_trackedGun == null) return;
            
            // 方法1: 尝试直接调用 Shoot 方法（如果存在）
            try
            {
                var shootMethod = _trackedGun.GetType().GetMethod("Shoot", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (shootMethod != null)
                {
                    shootMethod.Invoke(_trackedGun, null);
                    Debug.Log("[TriggerBot] 使用 Shoot() 方法射击");
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[TriggerBot] Shoot 方法调用失败: {ex.Message}");
            }
            
            // 方法2: 尝试 Fire 方法
            try
            {
                var fireMethod = _trackedGun.GetType().GetMethod("Fire", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (fireMethod != null)
                {
                    fireMethod.Invoke(_trackedGun, null);
                    Debug.Log("[TriggerBot] 使用 Fire() 方法射击");
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[TriggerBot] Fire 方法调用失败: {ex.Message}");
            }
            
            // 方法3: 尝试 OnShoot 方法
            try
            {
                var onShootMethod = _trackedGun.GetType().GetMethod("OnShoot", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (onShootMethod != null)
                {
                    onShootMethod.Invoke(_trackedGun, null);
                    Debug.Log("[TriggerBot] 使用 OnShoot() 方法射击");
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[TriggerBot] OnShoot 方法调用失败: {ex.Message}");
            }
            
            // 方法4: 尝试使用 SetTrigger（持续按住）
            HoldTrigger();
        }
        
        /// <summary>
        /// 持续按住扳机（用于全自动武器）
        /// </summary>
        private void HoldTrigger()
        {
            if (_trackedGun == null)
                return;
            
            try
            {
                // 计算是否需要触发"justPressed"事件
                bool justPressed = !_lastTriggerState;
                
                // 设置扳机状态：持续按下
                _trackedGun.SetTrigger(true, justPressed, false);
                _lastTriggerState = true;
                
                if (justPressed)
                {
                    Debug.Log("[TriggerBot] 按下扳机");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TriggerBot] HoldTrigger 出错: {ex.Message}");
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
                // 计算是否需要触发"justReleased"事件
                bool justReleased = _lastTriggerState;
                
                // 设置扳机状态：释放
                _trackedGun.SetTrigger(false, false, justReleased);
                _lastTriggerState = false;
                _continuousFireTimer = 0f;
                
                if (justReleased)
                {
                    Debug.Log("[TriggerBot] 释放扳机");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TriggerBot] ReleaseTrigger 出错: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 检测玩家是否在基地内
        /// 基于场景名称判断：如果在 Shelter（庇护所）或 MainMenu（主菜单），则认为在基地
        /// </summary>
        private bool IsPlayerInBase()
        {
            try
            {
                // 检查场景名称
                string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                
                // 精确匹配基地场景名称
                if (sceneName == "Base_SceneV2")
                {
                    Debug.Log("[TriggerBot] 玩家在基地内 (Base_SceneV2)，禁用自动扳机");
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TriggerBot] 基地检测失败: {ex.Message}");
                return false; // 出错时允许触发，避免功能失效
            }
        }
        
        /// <summary>
        /// 清理已销毁对象的缓存（性能优化）
        /// </summary>
        private void ClearInvalidCaches()
        {
            // 清理 Collider 缓存
            var invalidColliders = _colliderCache.Where(kvp => kvp.Key == null || kvp.Value == null).Select(kvp => kvp.Key).ToList();
            foreach (var key in invalidColliders)
            {
                _colliderCache.Remove(key);
            }
            
            // 清理 HeadCollider 缓存
            var invalidHeadColliders = _headColliderCache.Where(kvp => kvp.Key == null).Select(kvp => kvp.Key).ToList();
            foreach (var key in invalidHeadColliders)
            {
                _headColliderCache.Remove(key);
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
        }
    }
}

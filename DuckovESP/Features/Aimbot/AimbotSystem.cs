using System;
using System.Collections.Generic;
using System.Linq;
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
        private CheatSystem _cheatSystem; // 作弊系统引用
        
        // 子弹重定向追踪
        private readonly HashSet<int> _retargetedProjectiles = new HashSet<int>();
        
        // 自动扳机延迟计时器
        private float _triggerDelayTimer = 0f;
        private bool _targetInSight = false;
        
        // 障碍物层遮罩（用于LineOfSight检测）
        private static int ObstacleLayerMaskValue = -1;
        
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
                    if (wallLayer >= 0) ObstacleLayerMaskValue |= (1 << wallLayer);
                    if (groundLayer >= 0) ObstacleLayerMaskValue |= (1 << groundLayer);
                    if (halfObstacleLayer >= 0) ObstacleLayerMaskValue |= (1 << halfObstacleLayer);
                }
                catch
                {
                    ObstacleLayerMaskValue = 0;
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
            
            // 自动扳机
            if (_config.EnableTriggerBot)
            {
                PerformTriggerBot();
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
        /// 枪械射击事件回调 - 核心：在这里重定向子弹！
        /// </summary>
        private void OnGunShoot()
        {
            if (!_config.EnableAimbot)
                return;
            
            if (_trackedGun == null)
                return;
            
            // 查找最佳目标
            AutoAimCandidate candidate = FindBestTarget();
            if (candidate.Receiver == null)
            {
                Debug.Log("DuckovESP Aimbot: 未找到有效目标");
                return;
            }
            
            Debug.Log($"DuckovESP Aimbot: 锁定目标 {candidate.Receiver.name}, 屏幕距离 {candidate.ScreenDistance:F1}px, 实际距离 {candidate.RayDistance:F1}m");
            
            // 获取枪口位置
            Vector3 muzzlePosition = _trackedGun.muzzle != null 
                ? _trackedGun.muzzle.position 
                : _trackedGun.transform.position;
            
            // 重定向子弹
            RetargetProjectilesToTarget(muzzlePosition, candidate.AimPoint);
        }
        
        /// <summary>
        /// 重定向所有未处理的子弹到目标
        /// </summary>
        private void RetargetProjectilesToTarget(Vector3 muzzlePosition, Vector3 targetPoint)
        {
            try
            {
                Projectile[] allProjectiles = UnityEngine.Object.FindObjectsOfType<Projectile>();
                if (allProjectiles == null || allProjectiles.Length == 0)
                {
                    Debug.Log("DuckovESP Aimbot: 场景中没有子弹");
                    return;
                }
                
                Vector3 direction = (targetPoint - muzzlePosition).normalized;
                int retargetedCount = 0;
                
                // 只重定向玩家刚发射的子弹
                foreach (Projectile projectile in allProjectiles.Where(p => p != null && p.context.fromCharacter == _player)
                                                                  .OrderBy(p => GetProjectileTraveledDistance(p)))
                {
                    // 检查是否已经处理过
                    int projectileId = projectile.GetInstanceID();
                    if (_retargetedProjectiles.Contains(projectileId))
                        continue;
                    
                    // 只重定向刚发射的子弹（飞行距离 < 0.06米）
                    float traveledDistance = GetProjectileTraveledDistance(projectile);
                    if (traveledDistance > 0.06f)
                        continue;
                    
                    // 重定向子弹
                    if (RetargetProjectile(projectile, direction, targetPoint))
                    {
                        _retargetedProjectiles.Add(projectileId);
                        retargetedCount++;
                        
                        // 限制处理数量（根据散弹数）
                        if (retargetedCount >= Mathf.Max(1, _trackedGun.ShotCount))
                            break;
                    }
                }
                
                if (retargetedCount > 0)
                {
                    Debug.Log($"DuckovESP Aimbot: 成功重定向 {retargetedCount} 枚子弹到目标 {targetPoint}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"DuckovESP Aimbot: 重定向子弹时出错 - {ex.Message}");
            }
        }
        
        /// <summary>
        /// 重定向单个子弹
        /// </summary>
        private bool RetargetProjectile(Projectile projectile, Vector3 direction, Vector3 targetPoint)
        {
            try
            {
                // 设置子弹方向
                projectile.context.direction = direction;
                projectile.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                
                // 调整检测起点，避免从枪口后方开始检测
                projectile.context.firstFrameCheckStartPoint = projectile.transform.position - direction * 0.1f;
                
                // 确保射程足够到达目标
                float distanceToTarget = Vector3.Distance(projectile.transform.position, targetPoint) + 2f;
                if (projectile.context.distance < distanceToTarget)
                {
                    projectile.context.distance = distanceToTarget;
                }
                
                // 强制暴击（如果配置了瞄准头部）
                if (_config.AimbotAimAtHead)
                {
                    projectile.context.critRate = 1f;
                }
                
                // 一击必杀：如果启用，则大幅提升伤害
                if (_cheatSystem != null && _cheatSystem.IsOneHitKillEnabled())
                {
                    projectile.context.damage *= 999f; // 伤害提升999倍
                    projectile.context.critRate = 1f; // 强制暴击
                    projectile.context.critDamageFactor = 10f; // 暴击伤害10倍
                }
                
                // 忽略半掩体
                projectile.context.ignoreHalfObsticle = true;
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"DuckovESP Aimbot: 重定向单个子弹失败 - {ex.Message}");
                return false;
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
        /// 清理已销毁的子弹记录
        /// </summary>
        private void CleanupRetargetedProjectiles()
        {
            if (_retargetedProjectiles.Count == 0)
                return;
            
            try
            {
                // 获取所有活跃子弹的ID
                HashSet<int> activeIds = new HashSet<int>();
                foreach (Projectile projectile in UnityEngine.Object.FindObjectsOfType<Projectile>())
                {
                    if (projectile != null)
                    {
                        activeIds.Add(projectile.GetInstanceID());
                    }
                }
                
                // 移除已销毁的子弹记录
                _retargetedProjectiles.RemoveWhere(id => !activeIds.Contains(id));
            }
            catch { }
        }
        
        /// <summary>
        /// 查找最佳目标（使用屏幕半径圆形检测）
        /// </summary>
        private AutoAimCandidate FindBestTarget()
        {
            try
            {
                // 获取所有伤害接收器（DamageReceiver）
                DamageReceiver[] allReceivers = UnityEngine.Object.FindObjectsOfType<DamageReceiver>();
                
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
                
                foreach (DamageReceiver receiver in allReceivers)
                {
                    // 基础有效性检查
                    Vector3 aimPoint;
                    if (!IsCandidateValid(receiver, out aimPoint))
                        continue;
                    
                    // 距离检查
                    float distance = Vector3.Distance(muzzlePosition, aimPoint);
                    if (distance > maxRange)
                        continue;
                    
                    // 计算从摄像机到目标的方向
                    Vector3 directionToTarget = aimPoint - cameraOrigin;
                    float rayDistance = Vector3.Dot(directionToTarget, cameraForward);
                    
                    // 目标在摄像机后面
                    if (rayDistance <= 0)
                        continue;
                    
                    // 转换为屏幕坐标
                    Vector3 screenPoint = _mainCamera.WorldToScreenPoint(aimPoint);
                    
                    // 屏幕外或摄像机后面
                    if (screenPoint.z <= 0)
                        continue;
                    
                    // 计算屏幕距离
                    Vector2 screenPos = new Vector2(screenPoint.x, screenPoint.y);
                    float screenDistance = Vector2.Distance(screenCenter, screenPos);
                    
                    // 超出屏幕半径
                    if (screenDistance > _config.AimbotFOV)
                        continue;
                    
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
            if (!Physics.Raycast(origin, direction, out hit, distance - 0.2f, ObstacleLayerMaskValue, QueryTriggerInteraction.Ignore))
            {
                return true; // 没有障碍物
            }
            
            // 检查碰撞物是否就是目标本身
            DamageReceiver hitReceiver = hit.collider.GetComponentInParent<DamageReceiver>();
            if (hitReceiver != null && hitReceiver == receiver)
            {
                return true; // 直接命中目标
            }
            
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
                return false;
            
            if (receiver.gameObject == null || !receiver.gameObject.activeInHierarchy)
                return false;
            
            // 已死亡
            if (receiver.IsDead)
                return false;
            
            // 是主角自己
            if (receiver.IsMainCharacter)
                return false;
            
            // 没有生命值
            if (!ReceiverHasHealth(receiver))
                return false;
            
            // 队伍检查
            if (!Team.IsEnemy(_player.Team, receiver.Team))
                return false;
            
            // 中立单位
            if (receiver.Team == Teams.all)
                return false;
            
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
        /// 获取 DamageReceiver 的瞄准点
        /// </summary>
        private Vector3 GetReceiverAimPoint(DamageReceiver receiver)
        {
            try
            {
                // 如果启用了瞄准头部
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
                
                // 尝试获取碰撞体中心
                Collider collider = receiver.GetComponent<Collider>();
                if (collider != null)
                {
                    return collider.bounds.center;
                }
                
                // 默认位置（身体中心）
                return receiver.transform.position + Vector3.up * 1.0f;
            }
            catch
            {
                return receiver.transform.position + Vector3.up * 1.0f;
            }
        }
        
        /// <summary>
        /// 尝试获取头部Transform
        /// </summary>
        private static Transform TryGetHeadTransform(DamageReceiver receiver)
        {
            try
            {
                // 尝试查找 HeadCollider 组件
                HeadCollider headCollider = receiver.GetComponentInChildren<HeadCollider>();
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
        /// 执行自动扳机
        /// </summary>
        private void PerformTriggerBot()
        {
            // 更新延迟计时器
            if (_triggerDelayTimer > 0)
            {
                _triggerDelayTimer -= Time.deltaTime;
            }
            
            // 检查是否仅在瞄准时触发
            if (_config.TriggerBotOnlyADS)
            {
                // TODO: 检查玩家是否在瞄准状态（ADS）
                // 这需要访问游戏的瞄准状态，暂时跳过这个检查
            }
            
            // 检查准星下是否有敌人
            DamageReceiver target = GetTargetUnderCrosshair();
            
            bool currentTargetInSight = (target != null);
            
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
            }
            
            // 延迟结束，执行射击
            if (_targetInSight && _triggerDelayTimer <= 0)
            {
                TryShoot();
            }
        }
        
        /// <summary>
        /// 获取准星下的目标
        /// </summary>
        private DamageReceiver GetTargetUnderCrosshair()
        {
            try
            {
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
                Debug.LogError($"DuckovESP TriggerBot: 检测目标时出错 - {ex.Message}");
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

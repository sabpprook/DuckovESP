using System;
using UnityEngine;

namespace DuckovESPv3.Core.Systems.ESP.Models
{
    /// <summary>
    /// 敌人数据模型
    /// </summary>
    public class EnemyData
    {
        // ========== 核心引用 ==========
        /// <summary>角色控制器</summary>
        public CharacterMainControl Character { get; set; } = null!;
        
        /// <summary>生命组件</summary>
        public Health HealthComponent { get; set; } = null!;

        // ========== 基础信息 ==========
        /// <summary>敌人名称</summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>队伍</summary>
        public Teams Team { get; set; }
        
        /// <summary>位置</summary>
        public Vector3 Position { get; set; }
        
        /// <summary>头部位置</summary>
        public Vector3 HeadPosition { get; set; }

        // ========== 生命信息 ==========
        /// <summary>当前血量</summary>
        public float CurrentHealth { get; set; }
        
        /// <summary>最大血量</summary>
        public float MaxHealth { get; set; }
        
        /// <summary>血量百分比</summary>
        public float HealthPercent => MaxHealth > 0 ? CurrentHealth / MaxHealth : 0f;
        
        /// <summary>是否存活</summary>
        public bool IsAlive { get; set; }

        // ========== 装备信息 ==========
        /// <summary>武器名称</summary>
        public string WeaponName { get; set; } = string.Empty;
        
        /// <summary>库存价值</summary>
        public long InventoryValue { get; set; }

        // ========== 位置信息 ==========
        /// <summary>距离玩家的距离</summary>
        public float DistanceToPlayer { get; set; }

        // ========== 威胁等级 ==========
        /// <summary>是否正在瞄准玩家</summary>
        public bool IsAimingAtPlayer { get; set; }
        
        /// <summary>是否为 Boss</summary>
        public bool IsBoss { get; set; }
        
        /// <summary>威胁等级（1-5）</summary>
        public int ThreatLevel { get; set; }

        // ========== 时间戳 ==========
        /// <summary>发现时间</summary>
        public DateTime DiscoveryTime { get; set; }
        
        /// <summary>最后更新时间</summary>
        public DateTime LastUpdateTime { get; set; }

        // ========== 辅助信息 ==========
        /// <summary>AI 控制器</summary>
        public AICharacterController? AIController { get; set; }
        
        /// <summary>是否有 AI</summary>
        public bool HasAI => AIController != null;

        /// <summary>
        /// 更新位置信息
        /// </summary>
        public void UpdatePosition(Vector3 playerPosition)
        {
            if (Character != null)
            {
                Position = Character.transform.position;
                HeadPosition = GetHeadPosition();
                DistanceToPlayer = Vector3.Distance(playerPosition, Position);
                LastUpdateTime = DateTime.Now;
            }
        }

        /// <summary>
        /// 获取头部位置
        /// </summary>
        private Vector3 GetHeadPosition()
        {
            try
            {
                if (Character?.characterModel?.HelmatSocket != null)
                {
                    return Character.characterModel.HelmatSocket.position;
                }
            }
            catch
            {
                // 忽略异常
            }

            return Character?.transform.position + Vector3.up * 1.8f ?? Vector3.zero;
        }
    }
}

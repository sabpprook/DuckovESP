using UnityEngine;

namespace DuckovESP
{
    /// <summary>
    /// 敌人信息数据结构
    /// </summary>
    public class EnemyInfo
    {
        public string Name { get; set; }
        public float Distance { get; set; }
        public Vector3 HeadPosition { get; set; }
        public Vector3 BodyPosition { get; set; }
        public float CurrentHealth { get; set; }
        public float MaxHealth { get; set; }
        public float HealthPercent { get; set; }
        public string WeaponName { get; set; }
        public long InventoryValue { get; set; }
        public bool IsAimingAtPlayer { get; set; }
        public bool IsAlive { get; set; }
        public Color DisplayColor { get; set; }
        public CharacterMainControl Character { get; set; }
    }
}

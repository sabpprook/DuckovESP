using DuckovESPv3.Core.Systems.ESP.Models;

namespace DuckovESPv3.Core.EventBus
{
    /// <summary>
    /// 敌人发现事件
    /// </summary>
    public class EnemyDiscoveredEvent
    {
        public EnemyData EnemyData { get; }

        public EnemyDiscoveredEvent(EnemyData enemyData)
        {
            EnemyData = enemyData;
        }
    }

    /// <summary>
    /// 敌人死亡事件
    /// </summary>
    public class EnemyDiedEvent
    {
        public CharacterMainControl Character { get; }

        public EnemyDiedEvent(CharacterMainControl character)
        {
            Character = character;
        }
    }

    /// <summary>
    /// 敌人血量变化事件
    /// </summary>
    public class EnemyHealthChangedEvent
    {
        public EnemyData EnemyData { get; }

        public EnemyHealthChangedEvent(EnemyData enemyData)
        {
            EnemyData = enemyData;
        }
    }
}

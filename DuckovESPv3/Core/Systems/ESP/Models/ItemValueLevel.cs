namespace DuckovESPv3.Core.Systems.ESP.Models
{
    /// <summary>
    /// 物品品质等级枚举
    /// 用于分类物品的价值和稀有度
    /// </summary>
    public enum ItemValueLevel
    {
        White = 0,      // 白色 - 普通
        Green = 1,      // 绿色 - 稀有
        Blue = 2,       // 蓝色 - 精良
        Purple = 3,     // 紫色 - 史诗
        Orange = 4,     // 橙色 - 传说
        LightRed = 5,   // 浅红色 - 神话
        Red = 6         // 红色 - 至尊
    }
}

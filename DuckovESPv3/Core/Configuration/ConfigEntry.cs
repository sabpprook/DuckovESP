using System;

namespace DuckovESPv3.Core.Configuration
{
    /// <summary>
    /// 配置项特性 - 标记配置字段的元数据
    /// </summary>
    /// <remarks>
    /// 用于标注可配置的属性，支持指定默认值和配置键
    /// 示例：
    ///   [ConfigEntry("ESP.MaxDistance", DefaultValue = 100f)]
    ///   public float MaxDistance { get; set; }
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigEntry : Attribute
    {
        /// <summary>
        /// 配置键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public object? DefaultValue { get; set; }

        /// <summary>
        /// 配置项描述
        /// </summary>
        public string Description { get; set; }

        public ConfigEntry(string key, object? defaultValue = null, string description = "")
        {
            Key = key;
            DefaultValue = defaultValue;
            Description = description;
        }
    }
}

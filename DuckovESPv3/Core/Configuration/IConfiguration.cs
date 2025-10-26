using System;

namespace DuckovESPv3.Core.Configuration
{
    /// <summary>
    /// 配置系统接口
    /// </summary>
    /// <remarks>
    /// 架构层级：Core/Configuration
    /// 职责：定义配置系统的契约，支持读取、保存、变更通知
    /// </remarks>
    public interface IConfiguration
    {
        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <typeparam name="T">配置值类型</typeparam>
        /// <param name="key">配置键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>配置值或默认值</returns>
        T? Get<T>(string key, T? defaultValue = default);

        /// <summary>
        /// 设置配置值
        /// </summary>
        /// <typeparam name="T">配置值类型</typeparam>
        /// <param name="key">配置键</param>
        /// <param name="value">配置值</param>
        void Set<T>(string key, T value);

        /// <summary>
        /// 检查配置键是否存在
        /// </summary>
        /// <param name="key">配置键</param>
        /// <returns>是否存在</returns>
        bool HasKey(string key);

        /// <summary>
        /// 保存配置到文件
        /// </summary>
        void Save();

        /// <summary>
        /// 从文件加载配置
        /// </summary>
        void Load();

        /// <summary>
        /// 重置为默认配置
        /// </summary>
        void ResetToDefault();

        /// <summary>
        /// 配置变更事件
        /// </summary>
        event Action<string, object> OnConfigChanged;
    }
}

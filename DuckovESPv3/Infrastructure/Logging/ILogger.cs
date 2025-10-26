using System;

namespace DuckovESPv3.Infrastructure.Logging
{
    /// <summary>
    /// 日志系统接口
    /// </summary>
    /// <remarks>
    /// 架构层级：Infrastructure/Logging
    /// 职责：定义统一的日志接口，支持不同级别的日志记录
    /// </remarks>
    public interface ILogger
    {
        /// <summary>
        /// 记录调试日志
        /// </summary>
        void Debug(string message);

        /// <summary>
        /// 记录信息日志
        /// </summary>
        void Info(string message);

        /// <summary>
        /// 记录警告日志
        /// </summary>
        void Warning(string message);

        /// <summary>
        /// 记录错误日志
        /// </summary>
        void Error(string message, Exception? exception = null);
    }
}

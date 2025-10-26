using System;
using UnityEngine;

namespace DuckovESPv3.Infrastructure.Logging
{
    /// <summary>
    /// Unity日志实现
    /// </summary>
    /// <remarks>
    /// 架构层级：Infrastructure/Logging
    /// 性能特征：直接调用 Unity Debug，性能开销最小
    /// </remarks>
    public class UnityLogger : ILogger
    {
        private readonly string _category;
        private readonly bool _debugEnabled;

        public UnityLogger(string category = "DuckovESPv3", bool debugEnabled = false)
        {
            _category = category;
            _debugEnabled = debugEnabled;
        }

        /// <summary>
        /// 记录调试日志
        /// </summary>
        public void Debug(string message)
        {
            if (!_debugEnabled)
                return;

            UnityEngine.Debug.Log($"[{_category}] {message}");
        }

        /// <summary>
        /// 记录信息日志
        /// </summary>
        public void Info(string message)
        {
            UnityEngine.Debug.Log($"[{_category}] {message}");
        }

        /// <summary>
        /// 记录警告日志
        /// </summary>
        public void Warning(string message)
        {
            UnityEngine.Debug.LogWarning($"[{_category}] {message}");
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        public void Error(string message, Exception? exception = null)
        {
            if (exception != null)
            {
                UnityEngine.Debug.LogError($"[{_category}] {message}\n{exception}");
            }
            else
            {
                UnityEngine.Debug.LogError($"[{_category}] {message}");
            }
        }
    }
}

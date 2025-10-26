using UnityEngine;
using UnityEngine.Rendering;

namespace DuckovESPv3.Core.Systems.ESP.Rendering
{
    /// <summary>
    /// ESP CommandBuffer 渲染管理器
    /// 使用CommandBuffer确保ESP在所有内容之后渲染，彻底解决遮挡问题
    /// </summary>
    public class ESPCommandBufferManager : MonoBehaviour
    {
        private static ESPCommandBufferManager? _instance;
        public static ESPCommandBufferManager? Instance => _instance;

        private CommandBuffer? _espCommandBuffer;
        private Camera? _mainCamera;
        private bool _isInitialized = false;
        private float _retryTimer = 0f;
        private const float RETRY_INTERVAL = 2f;  // 每2秒重试一次，而不是每帧

        private const string COMMAND_BUFFER_NAME = "ESP_AfterEverything";

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// 初始化CommandBuffer
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
                return;

            try
            {
                // 获取主摄像机
                _mainCamera = Camera.main;
                if (_mainCamera == null)
                {
                    // 在开始菜单等场景没有主摄像机是正常的，不输出警告
                    return;
                }

                // 创建CommandBuffer
                _espCommandBuffer = new CommandBuffer
                {
                    name = COMMAND_BUFFER_NAME
                };

                // 添加到摄像机的AfterEverything事件
                _mainCamera.AddCommandBuffer(CameraEvent.AfterEverything, _espCommandBuffer);

                _isInitialized = true;
                Debug.Log($"[ESPCommandBufferManager] CommandBuffer已添加到相机: {_mainCamera.name}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ESPCommandBufferManager] 初始化失败: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void LateUpdate()
        {
            // 如果初始化失败，降低重试频率（避免在开始菜单疯狂重试）
            if (!_isInitialized)
            {
                _retryTimer += Time.deltaTime;
                if (_retryTimer >= RETRY_INTERVAL)
                {
                    _retryTimer = 0f;
                    Initialize();
                }
            }
            // 检查摄像机是否变化
            else if (_mainCamera != Camera.main && Camera.main != null)
            {
                Debug.Log("[ESPCommandBufferManager] 检测到摄像机变化，重新初始化");
                Cleanup();
                Initialize();
            }
        }

        /// <summary>
        /// 清理CommandBuffer
        /// </summary>
        private void Cleanup()
        {
            if (_mainCamera != null && _espCommandBuffer != null)
            {
                try
                {
                    _mainCamera.RemoveCommandBuffer(CameraEvent.AfterEverything, _espCommandBuffer);
                    Debug.Log("[ESPCommandBufferManager] CommandBuffer已从相机移除");
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"[ESPCommandBufferManager] 移除CommandBuffer失败: {ex.Message}");
                }
            }

            if (_espCommandBuffer != null)
            {
                _espCommandBuffer.Release();
                _espCommandBuffer = null;
            }

            _isInitialized = false;
            _mainCamera = null;
        }

        private void OnDestroy()
        {
            Cleanup();

            if (_instance == this)
            {
                _instance = null;
            }
        }

        private void OnDisable()
        {
            Cleanup();
        }

        /// <summary>
        /// 手动触发重新初始化
        /// </summary>
        public void Reinitialize()
        {
            Cleanup();
            Initialize();
        }

        /// <summary>
        /// 获取当前状态信息
        /// </summary>
        public string GetStatusInfo()
        {
            if (!_isInitialized)
                return "未初始化";

            if (_mainCamera == null)
                return "摄像机丢失";

            if (_espCommandBuffer == null)
                return "CommandBuffer丢失";

            return $"已初始化 - 摄像机: {_mainCamera.name}";
        }
    }
}

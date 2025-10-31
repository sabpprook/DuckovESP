﻿using UnityEngine;
using DuckovESPv3.Core.Configuration;
using DuckovESPv3.Core.EventBus;
using DuckovESPv3.Core.DependencyInjection;
using DuckovESPv3.Core.Systems.ESP;
using DuckovESPv3.Core.Systems.ESP.Detection;
using DuckovESPv3.Infrastructure.Logging;
using DuckovESPv3.Infrastructure.Localization;
using DuckovESPv3.UI.Windows;
using DuckovESPv3.UI.Rendering;
using DuckovESPv3.Features.QuestMarker.Services;
using HarmonyLib;
using System.Reflection;
using ILogger = DuckovESPv3.Infrastructure.Logging.ILogger;
namespace DuckovESPv3
{
    /// <summary>
    /// Mod行为入口点
    /// </summary>
    /// <remarks>
    /// 架构层级：ModBehaviour（入口点）
    /// 职责：
    /// - 系统初始化和依赖注入
    /// - 事件订阅和分发
    /// - 生命周期管理（Awake, OnEnable, OnDisable, Update）
    /// 
    /// ⚠️ 禁止：在此类中编写业务逻辑，仅做初始化和协调
    /// </remarks>
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private ServiceContainer? _serviceContainer;
        private ConfigurationManager? _configuration;
        private IEventBus? _eventBus;
        private ILogger? _logger;
        private ConfigWindow? _configWindow;
        private ESPSystemConfig? _espConfig;

        // 数据采集器
        private ILootboxCollectionService? _lootboxCollector;
        private IWorldItemCollectionService? _worldItemCollector;
        
        // Quest标记收集服务
        private QuestMarkerCollectionService? _questMarkerService;
        
        // ESP 系统管理器
        private ESPSystemManager? _espSystemManager;
        
        // 小地图标记系统
        private Core.Systems.ESP.Minimap.MinimapMarkerSystem? _minimapMarkerSystem;
        
        // CommandBuffer管理器
        private Core.Systems.ESP.Rendering.ESPCommandBufferManager? _commandBufferManager;
        
        // Harmony 实例
        private Harmony? _harmony;

        private void Awake()
        {
            Debug.Log("[ModBehaviour] DuckovESPv3 Mod 正在初始化...");

            try
            {
                // 0. 应用 Harmony Patches（必须最先执行）
                try
                {
                    _harmony = new Harmony("com.duckov.espv3");
                    _harmony.PatchAll(Assembly.GetExecutingAssembly());
                    Debug.Log("[ModBehaviour] Harmony Patches 应用成功");
                    
                    // 应用额外的 Hook（需要日志器，所以延迟到获取 logger 之后）
                    // LootboxVisibilityHook 将在 RegisterServices 后初始化
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[ModBehaviour] Harmony Patches 应用失败: {ex}");
                }

                // 1. 初始化依赖注入容器
                _serviceContainer = new ServiceContainer();
                RegisterServices();

                // 2. 获取服务
                _eventBus = _serviceContainer.Resolve<IEventBus>();
                _logger = _serviceContainer.Resolve<ILogger>();
                _configuration = _serviceContainer.Resolve<ConfigurationManager>();
                
                // 应用箱子可见性 Hook（需要 logger）
                try
                {
                    DuckovESPv3.Core.Systems.ESP.Hooks.LootboxVisibilityHook.ApplyPatches(_harmony, _logger);
                    _logger?.Info("[ModBehaviour] ✓ 箱子可见性 Hook 已应用");
                }
                catch (System.Exception ex)
                {
                    _logger?.Error($"[ModBehaviour] 箱子可见性 Hook 应用失败: {ex}");
                }

                // 3. 初始化配置系统
                _configuration.Initialize<ESPSystemConfig>();
                _espConfig = _configuration.GetConfigInstance<ESPSystemConfig>();

                // 4. 初始化本地化系统
                // 优先级：用户配置的语言 → 游戏语言 → 系统语言 → 英文
                string? languageToUse = null;
                
                // 检查用户是否有配置语言
                if (_espConfig?.Language != null && !string.IsNullOrEmpty(_espConfig.Language))
                {
                    languageToUse = _espConfig.Language;
                    _logger?.Info($"[ModBehaviour] 使用用户配置的语言: {languageToUse}");
                }
                else
                {
                    _logger?.Info("[ModBehaviour] 未找到用户配置的语言，尝试自动检测...");
                    // 调用无参数的Initialize()进行自动检测
                    LocalizationManager.Instance.Initialize();
                    _logger?.Info($"[ModBehaviour] 自动检测的语言: {LocalizationManager.GetCurrentLanguage()}");
                }
                
                // 如果用户有配置，则使用用户配置
                if (languageToUse != null)
                {
                    LocalizationManager.Instance.Initialize(languageToUse);
                }
                
                _logger?.Info($"[ModBehaviour] 本地化系统已初始化，当前语言: {LocalizationManager.GetCurrentLanguage()}");

                // 5. 初始化 ESP 对象池（必须在数据采集器之前）
                var poolObject = new GameObject("ESPMarkerPool");
                // 不设置 Parent，让它成为根对象
                // poolObject.transform.SetParent(transform);
                DontDestroyOnLoad(poolObject);  // 场景切换时不销毁
                var pool = poolObject.AddComponent<DuckovESPv3.Core.Systems.ESP.Rendering.ESPMarkerPool>();
                _logger?.Info("[ModBehaviour] ESP对象池已创建");

                // 注意：不在Awake中创建CommandBufferManager，而是在进入游戏后创建
                // 避免在开始菜单没有Camera.main时疯狂重试

                // 6. 初始化 ESP 系统管理器
                var espManagerObject = new GameObject("ESPSystemManager");
                // 不设置 Parent，让它成为根对象
                // espManagerObject.transform.SetParent(transform);
                DontDestroyOnLoad(espManagerObject);  // 场景切换时不销毁
                _espSystemManager = espManagerObject.AddComponent<ESPSystemManager>();
                
                if (_espConfig != null && _eventBus != null && _logger != null)
                {
                    _espSystemManager.Initialize(_espConfig, _eventBus, _logger);
                    _logger.Info("[ModBehaviour] ESP系统管理器已初始化");
                }

                // 7. 初始化数据采集器（必须在 ESP 系统之后，确保事件订阅完成）
                if (_eventBus != null && _logger != null)
                {
                    _lootboxCollector = new LootboxDataCollector(_eventBus, _logger);
                    _worldItemCollector = new WorldItemDataCollector(_eventBus, _logger);
                    _logger.Info("[ModBehaviour] 数据采集器已创建，等待关卡加载...");
                    
                    // 8. 将数据采集器引用传递给 ESP 系统管理器（用于 RefreshAllMarkers）
                    if (_espSystemManager != null)
                    {
                        _espSystemManager.SetDataCollectors(_lootboxCollector, _worldItemCollector);
                        _logger.Info("[ModBehaviour] 已将数据采集器引用传递给ESP系统管理器（包含敌人采集器）");
                    }
                }

                // 9. 初始化小地图标记系统
                if (_espConfig != null && _eventBus != null)
                {
                    _minimapMarkerSystem = new Core.Systems.ESP.Minimap.MinimapMarkerSystem(_espConfig, _eventBus);
                    _logger?.Info("[ModBehaviour] 小地图标记系统已创建，等待关卡加载...");
                }

                Debug.Log("[ModBehaviour] DuckovESPv3 Mod 初始化完成");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ModBehaviour] 初始化失败: {ex}");
            }
        }

        private void OnEnable()
        {
            Debug.Log("[ModBehaviour] DuckovESPv3 已启用");
            
            // 订阅关卡开始初始化事件（清理旧数据）
            LevelManager.OnLevelBeginInitializing += OnLevelBeginInitializing;
            _logger?.Info("[ModBehaviour] 已订阅LevelManager.OnLevelBeginInitializing事件");
            
            // 订阅关卡加载完成事件（初始化采集器）
            LevelManager.OnAfterLevelInitialized += OnLevelLoaded;
            _logger?.Info("[ModBehaviour] 已订阅LevelManager.OnAfterLevelInitialized事件");
            
            // 订阅子场景加载事件
            Duckov.Scenes.MultiSceneCore.OnSubSceneLoaded += OnSubSceneLoaded;
            _logger?.Info("[ModBehaviour] 已订阅MultiSceneCore.OnSubSceneLoaded事件");
        }

        private void OnDisable()
        {
            Debug.Log("[ModBehaviour] DuckovESPv3 已禁用");
            
            // 取消订阅关卡初始化事件
            LevelManager.OnLevelBeginInitializing -= OnLevelBeginInitializing;
            _logger?.Info("[ModBehaviour] 已取消订阅LevelManager.OnLevelBeginInitializing事件");
            
            // 取消订阅关卡加载事件
            LevelManager.OnAfterLevelInitialized -= OnLevelLoaded;
            _logger?.Info("[ModBehaviour] 已取消订阅LevelManager.OnAfterLevelInitialized事件");
            
            // 取消订阅子场景加载事件
            Duckov.Scenes.MultiSceneCore.OnSubSceneLoaded -= OnSubSceneLoaded;
            _logger?.Info("[ModBehaviour] 已取消订阅MultiSceneCore.OnSubSceneLoaded事件");
            
            // 清理采集器
            _lootboxCollector?.Cleanup();
            _worldItemCollector?.Cleanup();
            
            // 清理Quest标记收集服务
            _questMarkerService?.Dispose();
            
            // 清理 ESP 系统
            _espSystemManager?.ClearAllMarkers();

            // 清理配置
            _configuration?.Save();
        }

        private void Update()
        {
            if (_configuration == null || _espConfig == null)
                return;

            // 检查菜单切换快捷键
            if (Input.GetKeyDown(_espConfig.MenuToggleKey))
            {
                _configWindow?.Toggle();
            }
            
            // 更新Quest标记服务（更新距离计算）
            _questMarkerService?.Update();
        }

        private void OnGUI()
        {
            // 延迟初始化ConfigWindow（GUI必须在OnGUI中初始化）
            if (_configWindow == null && _espConfig != null)
            {
                try
                {
                    GUIStyleManager styleManager = new GUIStyleManager();
                    _configWindow = new ConfigWindow(_espConfig, styleManager);
                    
                    // 传入ESPSystemManager引用以支持配置实时刷新
                    if (_espSystemManager != null)
                    {
                        _configWindow.SetESPSystemManager(_espSystemManager);
                    }
                    
                    // 传入小地图标记系统引用以支持配置实时刷新
                    if (_minimapMarkerSystem != null)
                    {
                        _configWindow.SetMinimapMarkerSystem(_minimapMarkerSystem);
                    }
                    
                    // 传入ConfigurationManager引用以支持配置保存
                    if (_configuration != null)
                    {
                        _configWindow.SetConfigurationManager(_configuration);
                    }
                    
                    _logger?.Info("[ModBehaviour] ConfigWindow已初始化");
                }
                catch (System.Exception ex)
                {
                    _logger?.Error($"[ModBehaviour] ConfigWindow初始化失败: {ex.Message}");
                }
            }

            _configWindow?.Draw();
        }

        /// <summary>
        /// 关卡加载完成回调（由LevelManager.OnAfterLevelInitialized触发）
        /// </summary>
        /// <summary>
        /// 关卡开始初始化回调（由LevelManager.OnLevelBeginInitializing触发）
        /// 在此阶段清理旧数据，避免采集到上一关卡的残留数据
        /// </summary>
        private void OnLevelBeginInitializing()
        {
            _logger?.Info("[ModBehaviour] 关卡开始初始化，清理旧数据...");
            
            // 先停止监听（避免清理过程中继续添加数据）
            _lootboxCollector?.Cleanup();
            _worldItemCollector?.Cleanup();
            
            // 清理 ESP 标记
            _espSystemManager?.ClearAllMarkers();
            
            // 清理小地图标记
            _minimapMarkerSystem?.Cleanup();
            
            // 重新初始化小地图标记系统（在数据采集之前订阅事件）
            if (_minimapMarkerSystem != null && !_minimapMarkerSystem.IsInitialized)
            {
                _minimapMarkerSystem.Initialize();
                _logger?.Info("[ModBehaviour] 小地图标记系统已重新初始化（关卡开始）");
            }
            
            _logger?.Info("[ModBehaviour] 旧数据清理完成，等待关卡加载...");
        }

        /// <summary>
        /// 关卡加载完成回调（由LevelManager.OnAfterLevelInitialized触发）
        /// 在此阶段初始化采集器，开始采集新关卡的数据
        /// </summary>
        private void OnLevelLoaded()
        {
            _logger?.Info("[ModBehaviour] 关卡加载完成，初始化采集器...");
            
            // 初始化CommandBuffer管理器（只在进入游戏后创建，避免开始菜单性能问题）
            if (_commandBufferManager == null)
            {
                var commandBufferObject = new GameObject("ESPCommandBufferManager");
                // 不设置 Parent，让它成为根对象
                // commandBufferObject.transform.SetParent(transform);
                DontDestroyOnLoad(commandBufferObject);  // 场景切换时不销毁
                _commandBufferManager = commandBufferObject.AddComponent<DuckovESPv3.Core.Systems.ESP.Rendering.ESPCommandBufferManager>();
                _logger?.Info("[ModBehaviour] ESP CommandBuffer管理器已创建");
            }
            
            // 重新初始化采集器（开始采集新关卡的数据）
            InitializeDataCollectors();
        }
        
        /// <summary>
        /// 子场景加载完成回调（由MultiSceneCore.OnSubSceneLoaded触发）
        /// </summary>
        private void OnSubSceneLoaded(Duckov.Scenes.MultiSceneCore core, UnityEngine.SceneManagement.Scene scene)
        {
            _logger?.Info($"[ModBehaviour] 子场景加载完成: {scene.name}，追加扫描新物品");
            
            // 不清理数据，使用 Rescan 方法追加新物品
            _lootboxCollector?.Rescan();
            
            _worldItemCollector?.Rescan();
        }

        /// <summary>
        /// 初始化数据采集器
        /// </summary>
        private void InitializeDataCollectors()
        {
            try
            {
                if (_lootboxCollector != null)
                {
                    _lootboxCollector.Initialize();
                    _logger?.Info("[ModBehaviour] Lootbox采集器已初始化");
                }

                if (_worldItemCollector != null)
                {
                    _worldItemCollector.Initialize();
                    _logger?.Info("[ModBehaviour] WorldItem采集器已初始化");
                }

                // 初始化Quest标记收集服务
                if (_eventBus != null && _logger != null)
                {
                    _questMarkerService = new QuestMarkerCollectionService(_eventBus, _logger);
                    _questMarkerService.Initialize();
                    _logger?.Info("[ModBehaviour] ✓ Quest标记收集服务已初始化（事件驱动，零反射）");
                    
                    // 【关键修复】将追踪器传递给ESPSystemManager，以便它能每帧调用Update()更新距离
                    if (_espSystemManager != null)
                    {
                        _espSystemManager.SetDataTrackers(
                            _questMarkerService.GetEvacuationPointTracker(),
                            _questMarkerService.GetQuestZoneTracker()
                        );
                        _logger?.Info("[ModBehaviour] ✓ 已将追踪器传递给ESPSystemManager");
                    }
                    
                    // 【Bug Fix #2】手动触发初始数据刷新，确保所有订阅者都能收到初始事件
                    // 因为订阅可能在Initialize()发布事件之后才完成
                    System.Threading.Tasks.Task.Delay(100).ContinueWith(_ =>
                    {
                        try
                        {
                            _questMarkerService?.RefreshAll();
                            _logger?.Info("[ModBehaviour] ✓ Quest标记初始数据已刷新");
                        }
                        catch (System.Exception ex)
                        {
                            _logger?.Error($"[ModBehaviour] Quest标记刷新失败: {ex.Message}");
                        }
                    }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (System.Exception ex)
            {
                _logger?.Error($"[ModBehaviour] 初始化采集器失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 注册所有服务到依赖注入容器
        /// </summary>
        private void RegisterServices()
        {
            if (_serviceContainer == null)
                return;

            // 日志服务
            _serviceContainer.RegisterSingleton<ILogger>(
                container => new UnityLogger("DuckovESPv3", false)
            );

            // 事件总线
            _serviceContainer.RegisterSingleton<IEventBus, EventBus>();

            // 配置管理器
            _serviceContainer.RegisterSingleton<ConfigurationManager>(
                container => new ConfigurationManager("DuckovESPv3")
            );
        }
    }
}

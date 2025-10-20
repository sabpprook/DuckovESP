using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Duckov.Modding;
using Duckov.MiniMaps;
using Duckov.Scenes;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using UnityEngine;
using UnityEngine.SceneManagement;
using DuckovESP.UI;

namespace DuckovESP
{
    /// <summary>
    /// 箱子物品透视ESP Mod
    /// 显示地图上所有箱子的位置和内容物品信息
    /// 同时在3D游戏画面中实时显示ESP标记
    /// </summary>
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        // 追踪的箱子和标记
        private readonly Dictionary<InteractableLootbox, LootboxMarkerData> _trackedLootboxes = new Dictionary<InteractableLootbox, LootboxMarkerData>();
        
        // 追踪的世界物品和标记
        private readonly Dictionary<DuckovItemAgent, GameObject> _trackedWorldItems = new Dictionary<DuckovItemAgent, GameObject>();
        
        // 3D ESP缓存数据
        private readonly List<ESPData> _espDataCache = new List<ESPData>(100);
        private float _espCacheTimer = 0f;
        private const float ESP_CACHE_INTERVAL = 0.3f; // 优化：从0.2秒改为0.3秒更新一次ESP缓存（减少 33% CPU 负载）
        
        // 扫描计时器
        private float _scanTimer = 0f;
        private const float SCAN_INTERVAL = 2f; // 优化：从1秒改为2秒扫描一次（减少50% FindObjectsOfType 调用）
        
        // 配置系统
        private ESPConfig _config;
        private ESPConfigMenu _configMenu;
        
        // 自动瞄准系统
        private AimbotSystem _aimbotSystem;
        
        // 敌人ESP系统
        private EnemyDetector _enemyDetector;
        private EnemyESPRenderer _enemyESPRenderer;
        private EnemyListWindow _enemyListWindow;
        
        // 任务物品检测器
        private QuestItemDetector _questItemDetector;
        
        // 作弊系统
        private CheatSystem _cheatSystem;
        
        // 欢迎弹窗
        private WelcomePopup _welcomePopup;
        
        // 兼容性标志
        private bool _bossLiveMapModDetected = false;
        
        // 兼容旧代码的配置属性（从_config读取）
        private bool _showLowValueItems => _config.ShowEmptyBoxes;
        private int _minQualityFilter => _config.MinQualityForMapMarkers;
        private bool _enable3DESP => _config.Enable3DESP;
        private float _maxESPDistance => _config.MaxESPDistance;
        
        // GUI样式和纹理缓存
        private GUIStyle _espTextStyle;
        private Camera _mainCamera;
        private static Texture2D _whiteTexture;
        
        /// <summary>
        /// ESP显示数据缓存
        /// </summary>
        private class ESPData
        {
            public Vector3 worldPosition;
            public List<Item> items;
            public float distance;
            public ItemValueLevel maxLevel;
        }
        
        private void Awake()
        {
            Debug.Log("=== DuckovESP Loaded ===");
            
            // 检测 BossLiveMapMod 是否存在
            DetectBossLiveMapMod();
            
            // 加载配置
            _config = ESPConfig.Load();
            _configMenu = new ESPConfigMenu(_config);
            
            // 初始化自动瞄准系统
            _aimbotSystem = new AimbotSystem(_config);
            
            // 初始化敌人ESP系统
            _enemyDetector = new EnemyDetector(_config);
            _enemyESPRenderer = new EnemyESPRenderer(_config);
            _enemyListWindow = new EnemyListWindow(_config);
            
            // 初始化任务物品检测器
            _questItemDetector = new QuestItemDetector();
            
            // 初始化作弊系统
            _cheatSystem = new CheatSystem(_config);
            
            // 初始化欢迎弹窗
            _welcomePopup = new WelcomePopup();
            
            // 将作弊系统引用传递给自动瞄准系统
            _aimbotSystem.SetCheatSystem(_cheatSystem);
            
            // 创建共享白色纹理（用于所有GUI绘制）
            if (_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
            }
            
            Debug.Log("箱子物品透视ESP已启动");
            Debug.Log($"3D ESP: {(_config.Enable3DESP ? "启用" : "禁用")}");
            Debug.Log($"小地图标记: {(_config.EnableMapMarkers ? "启用" : "禁用")}");
            Debug.Log($"敌人ESP: {(_config.EnableEnemyESP ? "启用" : "禁用")}");
            Debug.Log($"自动瞄准: {(_config.EnableAimbot ? "⚠️启用" : "禁用")}");
            Debug.Log($"自动扳机: {(_config.EnableTriggerBot ? "⚠️启用" : "禁用")}");
            Debug.Log($"按 {_config.MenuToggleKey} 打开配置菜单");
            Debug.Log("作弊功能: F7=无敌 F8=一击必杀 F9=速度 F10=负重 F11=子弹 F12=耐力");
            
            // 初始化GUI样式
            InitializeGUIStyle();
        }

        /// <summary>
        /// 初始化GUI样式
        /// </summary>
        private void InitializeGUIStyle()
        {
            _espTextStyle = new GUIStyle();
            _espTextStyle.fontSize = _config.ESPFontSize;
            _espTextStyle.fontStyle = FontStyle.Bold;
            _espTextStyle.normal.textColor = Color.white;
            _espTextStyle.alignment = TextAnchor.MiddleCenter;
        }

        private void OnEnable()
        {
            // 订阅关卡事件
            LevelManager.OnLevelInitialized += OnLevelInitialized;
            LevelManager.OnAfterLevelInitialized += OnAfterLevelInitialized;
            MultiSceneCore.OnSubSceneLoaded += OnSubSceneLoaded;
            
            Debug.Log("DuckovESP: 事件已订阅");
            
            // 检查是否首次运行，如果是则显示欢迎界面
            _welcomePopup?.CheckFirstRun();
        }

        private void OnDisable()
        {
            // 取消订阅
            LevelManager.OnLevelInitialized -= OnLevelInitialized;
            LevelManager.OnAfterLevelInitialized -= OnAfterLevelInitialized;
            MultiSceneCore.OnSubSceneLoaded -= OnSubSceneLoaded;
            
            // 清理所有标记
            ClearAllMarkers();
            
            // 清理敌人ESP渲染器资源
            _enemyESPRenderer?.Dispose();
            
            // 清理物品连线材质
            if (_itemLineMaterial != null)
            {
                UnityEngine.Object.DestroyImmediate(_itemLineMaterial);
                _itemLineMaterial = null;
            }
            
            Debug.Log("DuckovESP: 已清理并禁用");
        }

        private void OnLevelInitialized()
        {
            Debug.Log("DuckovESP: 关卡初始化，清理旧数据");
            ClearAllMarkers();
            _scanTimer = 0f;
            
            // 重置作弊系统状态
            _cheatSystem?.OnLevelUnload();
        }

        /// <summary>
        /// 检测 BossLiveMapMod 是否已加载
        /// </summary>
        private void DetectBossLiveMapMod()
        {
            try
            {
                // 尝试通过反射查找 BossLiveMapMod 的 ModBehaviour 类
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    if (assembly.GetName().Name.Contains("BossLiveMapMod") || 
                        assembly.GetName().Name.Contains("BossLiveMap"))
                    {
                        _bossLiveMapModDetected = true;
                        Debug.Log("[DuckovESP] 检测到 BossLiveMapMod，启用兼容模式");
                        break;
                    }
                    
                    // 也检查类型名
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (type.Namespace != null && type.Namespace.Contains("BossLiveMapMod"))
                        {
                            _bossLiveMapModDetected = true;
                            Debug.Log("[DuckovESP] 检测到 BossLiveMapMod，启用兼容模式");
                            return;
                        }
                    }
                }
                
                if (!_bossLiveMapModDetected)
                {
                    Debug.Log("[DuckovESP] 未检测到 BossLiveMapMod");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[DuckovESP] 检测 BossLiveMapMod 时出错: {ex.Message}");
            }
        }

        private void OnAfterLevelInitialized()
        {
            Debug.Log("DuckovESP: 关卡完全加载，开始扫描箱子");
            ScanAllLootboxes();
        }

        private void OnSubSceneLoaded(MultiSceneCore core, Scene scene)
        {
            Debug.Log($"DuckovESP: 子场景加载 - {scene.name}");
            _scanTimer = 0f; // 立即触发扫描
        }

        private void Update()
        {
            if (!LevelManager.LevelInited)
                return;

            // 更新主摄像机引用
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }

            // 检查配置菜单切换 (需要按住 Shift)
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(_config.MenuToggleKey))
            {
                _configMenu.ToggleMenu();
            }
            
            // 检查敌人列表窗口切换 (需要按住 Shift)
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(_config.EnemyListToggleKey))
            {
                _enemyListWindow?.ToggleWindow();
            }

            // 定时扫描小地图标记
            _scanTimer -= Time.unscaledDeltaTime;
            if (_scanTimer <= 0f)
            {
                _scanTimer = SCAN_INTERVAL;
                ScanAllLootboxes();
                ScanWorldItems(); // 添加世界物品扫描
            }
            
            // 定时更新3D ESP缓存（降低频率以提升性能）
            if (_enable3DESP && _mainCamera != null)
            {
                _espCacheTimer -= Time.unscaledDeltaTime;
                if (_espCacheTimer <= 0f)
                {
                    _espCacheTimer = ESP_CACHE_INTERVAL;
                    UpdateESPCache();
                }
            }

            // 更新标记位置
            UpdateMarkerPositions();
            
            // 更新敌人检测系统
            if (_config.EnableEnemyESP && CharacterMainControl.Main != null)
            {
                _enemyDetector?.Update(CharacterMainControl.Main);
            }
            
            // 更新任务物品检测器
            if (_config.HighlightQuestItems || _config.HighlightBuildingMaterials)
            {
                _questItemDetector?.Update();
            }
            
            // 更新自动瞄准系统
            _aimbotSystem?.Update();
            
            // 更新作弊系统
            _cheatSystem?.Update();
            
            // 更新GUI样式（如果配置改变了）
            if (_espTextStyle != null && _espTextStyle.fontSize != _config.ESPFontSize)
            {
                _espTextStyle.fontSize = _config.ESPFontSize;
            }
        }

        private void LateUpdate()
        {
            if (!LevelManager.LevelInited)
                return;

            // 确保标记位置同步
            UpdateMarkerPositions();
        }

        /// <summary>
        /// 更新3D ESP缓存数据（在Update中调用，不在OnGUI中）
        /// </summary>
        private void UpdateESPCache()
        {
            _espDataCache.Clear();
            
            if (_mainCamera == null)
                return;
            
            // 获取玩家位置
            Vector3 playerPos = Vector3.zero;
            CharacterMainControl player = CharacterMainControl.Main;
            if (player != null)
            {
                playerPos = player.transform.position;
            }
            
            // 1. 扫描箱子中的物品
            ScanLootboxItems(playerPos);
            
            // 2. 扫描地图上直接生成的物品（不在箱子里的ItemAgent）
            ScanWorldItems(playerPos);
        }
        
        /// <summary>
        /// 扫描箱子中的物品（用于3D ESP，独立于地图标记）
        /// </summary>
        private void ScanLootboxItems(Vector3 playerPos)
        {
            // 直接查找所有箱子，不依赖地图标记的追踪列表
            InteractableLootbox[] allBoxes = UnityEngine.Object.FindObjectsOfType<InteractableLootbox>();
            
            foreach (InteractableLootbox box in allBoxes)
            {
                if (box == null || !box.gameObject.activeInHierarchy)
                    continue;

                // 获取箱子的Inventory
                Inventory inventory = box.Inventory;
                if (inventory == null || inventory.Loading)
                    continue;

                // 检查是否为空或已掠夺
                if (box.Looted || inventory.IsEmpty())
                    continue;

                Vector3 boxPos = box.transform.position;
                
                // 计算距离
                float distance = Vector3.Distance(playerPos, boxPos);
                
                // 超出距离不显示
                if (distance > _maxESPDistance)
                    continue;

                // 转换为屏幕坐标检查是否在视野内
                Vector3 screenPos = _mainCamera.WorldToScreenPoint(boxPos);
                
                // 在摄像机后面不显示
                if (screenPos.z <= 0)
                    continue;

                // 获取物品列表
                List<Item> items = GetItemsFromInventory(inventory);
                if (items == null || items.Count == 0)
                    continue;
                
                // 应用3D ESP的品质过滤（独立于地图标记） + 钥匙特殊逻辑 + 任务/建筑材料特殊逻辑
                List<Item> filteredItems = new List<Item>();
                
                foreach (Item item in items)
                {
                    if (item == null)
                        continue;
                    
                    // 检查是否是未录入的钥匙
                    bool isUnregisteredKey = IsUnregisteredKey(item);
                    
                    // 检查是否是任务物品或建筑材料
                    bool isQuestOrBuilding = false;
                    if (_questItemDetector != null)
                    {
                        isQuestOrBuilding = (_config.HighlightQuestItems && _questItemDetector.IsQuestRequiredItem(item)) ||
                                           (_config.HighlightBuildingMaterials && _questItemDetector.IsBuildingRequiredItem(item));
                    }
                    
                    // 如果是未录入的钥匙或任务/建筑材料，直接添加（绕过品质过滤）
                    if (isUnregisteredKey || isQuestOrBuilding)
                    {
                        filteredItems.Add(item);
                        continue;
                    }
                    
                    // 【关键】应用3D ESP的品质过滤（MinQualityFilter3D）
                    if (_config.MinQualityFilter3D > 0)
                    {
                        ItemValueLevel level = ItemQualityUtil.GetItemValueLevel(item);
                        if ((int)level >= _config.MinQualityFilter3D)
                        {
                            filteredItems.Add(item);
                        }
                    }
                    else
                    {
                        // 品质过滤为0时，显示所有物品
                        filteredItems.Add(item);
                    }
                }
                
                if (filteredItems.Count == 0)
                    continue;
                
                // 计算最高品质
                ItemValueLevel maxLevel = ItemValueLevel.White;
                foreach (Item item in filteredItems)
                {
                    if (item != null)
                    {
                        ItemValueLevel level = ItemQualityUtil.GetItemValueLevel(item);
                        if (level > maxLevel)
                            maxLevel = level;
                    }
                }
                
                // 添加到缓存
                _espDataCache.Add(new ESPData
                {
                    worldPosition = boxPos,
                    items = filteredItems,
                    distance = distance,
                    maxLevel = maxLevel
                });
            }
        }
        
        /// <summary>
        /// 扫描地图上直接生成的物品（不在箱子里的ItemAgent）
        /// </summary>
        private void ScanWorldItems(Vector3 playerPos)
        {
            try
            {
                // 查找所有活跃的 ItemAgent（拾取类型）
                DuckovItemAgent[] allItemAgents = UnityEngine.Object.FindObjectsOfType<DuckovItemAgent>();
                
                foreach (DuckovItemAgent itemAgent in allItemAgents)
                {
                    if (itemAgent == null || !itemAgent.gameObject.activeInHierarchy)
                        continue;
                    
                    // 只扫描pickup类型的ItemAgent（地图上的物品）
                    if (itemAgent.AgentType != ItemAgent.AgentTypes.pickUp)
                        continue;
                    
                    // 获取关联的Item
                    Item item = itemAgent.Item;
                    if (item == null)
                        continue;
                    
                    Vector3 itemPos = itemAgent.transform.position;
                    
                    // 计算距离
                    float distance = Vector3.Distance(playerPos, itemPos);
                    
                    // 超出距离不显示
                    if (distance > _maxESPDistance)
                        continue;
                    
                    // 转换为屏幕坐标检查是否在视野内
                    Vector3 screenPos = _mainCamera.WorldToScreenPoint(itemPos);
                    
                    // 在摄像机后面不显示
                    if (screenPos.z <= 0)
                        continue;
                    
                    // 应用品质过滤
                    ItemValueLevel itemLevel = ItemQualityUtil.GetItemValueLevel(item);
                    
                    // 检查是否是未录入的钥匙（钥匙总是显示）
                    bool isUnregisteredKey = IsUnregisteredKey(item);
                    
                    // 检查是否是任务物品或建筑材料
                    bool isQuestOrBuilding = false;
                    if (_questItemDetector != null)
                    {
                        isQuestOrBuilding = (_config.HighlightQuestItems && _questItemDetector.IsQuestRequiredItem(item)) ||
                                           (_config.HighlightBuildingMaterials && _questItemDetector.IsBuildingRequiredItem(item));
                    }
                    
                    // 品质过滤：如果不是钥匙且不是任务/建筑材料且品质低于阈值，跳过
                    if (!isUnregisteredKey && !isQuestOrBuilding && (int)itemLevel < _config.MinQualityFilter3D)
                        continue;
                    
                    // 创建单个物品的列表
                    List<Item> singleItemList = new List<Item> { item };
                    
                    // 添加到缓存
                    _espDataCache.Add(new ESPData
                    {
                        worldPosition = itemPos,
                        items = singleItemList,
                        distance = distance,
                        maxLevel = itemLevel
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"DuckovESP: 扫描地图物品时出错 - {ex.Message}");
            }
        }

        /// <summary>
        /// 在屏幕上绘制3D ESP标记（使用缓存数据）
        /// </summary>
        private void OnGUI()
        {
            // 绘制欢迎弹窗（优先级最高，可能覆盖其他UI）
            _welcomePopup?.OnGUI();
            
            // 绘制配置菜单（总是检查，即使不在关卡中）
            _configMenu?.DrawMenu();
            
            // 绘制敌人列表窗口
            if (_config.EnableEnemyESP && LevelManager.LevelInited)
            {
                var enemyList = _enemyDetector?.GetEnemyInfoList();
                if (enemyList != null)
                {
                    _enemyListWindow?.DrawWindow(enemyList);
                }
            }
            
            // 绘制作弊功能状态指示器
            if (LevelManager.LevelInited && _cheatSystem != null)
            {
                DrawCheatStatusIndicator();
            }
            
            if (!_enable3DESP || !LevelManager.LevelInited || _mainCamera == null)
                return;

            // 确保GUI样式已初始化
            if (_espTextStyle == null)
            {
                InitializeGUIStyle();
            }
            
            // 绘制敌人ESP标签
            if (_config.EnableEnemyESP && CharacterMainControl.Main != null)
            {
                var enemyList = _enemyDetector?.GetEnemyInfoList();
                if (enemyList != null && enemyList.Count > 0)
                {
                    _enemyESPRenderer?.DrawESPLabels(enemyList, _mainCamera, _espTextStyle);
                }
            }

            // 获取玩家屏幕位置
            Vector2 playerScreenPos = new Vector2(Screen.width / 2, Screen.height / 2);
            CharacterMainControl player = CharacterMainControl.Main;
            if (player != null)
            {
                Vector3 playerWorldScreenPos = _mainCamera.WorldToScreenPoint(player.transform.position);
                if (playerWorldScreenPos.z > 0)
                {
                    playerScreenPos = new Vector2(playerWorldScreenPos.x, Screen.height - playerWorldScreenPos.y);
                }
            }

            // 使用缓存的ESP数据进行绘制
            foreach (ESPData espData in _espDataCache)
            {
                // 转换为屏幕坐标
                Vector3 screenPos = _mainCamera.WorldToScreenPoint(espData.worldPosition);
                
                // 在摄像机后面不显示
                if (screenPos.z <= 0)
                    continue;

                // Unity的屏幕坐标Y轴需要翻转
                screenPos.y = Screen.height - screenPos.y;

                // 绘制ESP信息
                DrawESPBox(screenPos, espData.items, espData.distance, espData.maxLevel, playerScreenPos);
            }
        }
        
        /// <summary>
        /// 在渲染时绘制GL线条（敌人连线 + 物品连线）
        /// </summary>
        private void OnRenderObject()
        {
            if (!LevelManager.LevelInited || _mainCamera == null)
                return;
            
            var player = CharacterMainControl.Main;
            if (player == null)
                return;
            
            // 绘制敌人连线
            if (_config.EnableEnemyESP && _config.EnableEnemyLines)
            {
                var enemyList = _enemyDetector?.GetEnemyInfoList();
                if (enemyList != null && enemyList.Count > 0)
                {
                    _enemyESPRenderer?.DrawLines(enemyList, player, _mainCamera);
                }
            }
            
            // 绘制物品连线（使用相同的GL渲染方式）
            if (_enable3DESP && _config.ShowConnectLine)
            {
                DrawItemLines(player);
            }
        }
        
        /// <summary>
        /// 绘制物品连线（使用GL，与敌人连线样式一致）
        /// 只绘制在小地图上有标记的物品连线
        /// </summary>
        private void DrawItemLines(CharacterMainControl player)
        {
            if (_espDataCache.Count == 0 || _enemyESPRenderer == null)
                return;
            
            Vector3 playerPos = player.transform.position + Vector3.up * 1.5f;
            
            // 使用敌人渲染器的材质
            var lineMaterial = GetOrCreateLineMaterial();
            if (lineMaterial == null)
                return;
            
            GL.PushMatrix();
            lineMaterial.SetPass(0);
            GL.LoadOrtho();
            GL.Begin(GL.LINES);
            
            foreach (var espData in _espDataCache)
            {
                // 检查这个物品位置是否有对应的小地图标记
                bool hasMapMarker = false;
                
                // 检查箱子标记
                foreach (var markerData in _trackedLootboxes.Values)
                {
                    if (markerData.lootbox != null && 
                        Vector3.Distance(markerData.lootbox.transform.position, espData.worldPosition) < 0.1f)
                    {
                        hasMapMarker = true;
                        break;
                    }
                }
                
                // 检查世界物品标记
                if (!hasMapMarker)
                {
                    foreach (var worldItem in _trackedWorldItems.Keys)
                    {
                        if (worldItem != null && 
                            Vector3.Distance(worldItem.transform.position, espData.worldPosition) < 0.1f)
                        {
                            hasMapMarker = true;
                            break;
                        }
                    }
                }
                
                // 如果没有小地图标记，跳过绘制连线
                if (!hasMapMarker)
                    continue;
                
                Vector3 playerScreen = _mainCamera.WorldToScreenPoint(playerPos);
                Vector3 itemScreen = _mainCamera.WorldToScreenPoint(espData.worldPosition);
                
                // 检查是否在屏幕前方
                if (playerScreen.z <= 0 || itemScreen.z <= 0) 
                    continue;
                
                // 转换为GL坐标 (0-1范围)
                Vector2 p1 = new Vector2(playerScreen.x / Screen.width, playerScreen.y / Screen.height);
                Vector2 p2 = new Vector2(itemScreen.x / Screen.width, itemScreen.y / Screen.height);
                
                // 获取物品颜色（与ESP标记一致）
                Color lineColor = ItemQualityUtil.GetItemValueLevelColor(espData.maxLevel);
                
                // 检查是否是任务物品或建筑材料
                if (_questItemDetector != null && espData.items.Count > 0)
                {
                    bool isQuestItem = _config.HighlightQuestItems && 
                        espData.items.Any(item => _questItemDetector.IsQuestRequiredItem(item));
                    bool isBuildingMaterial = _config.HighlightBuildingMaterials && 
                        espData.items.Any(item => _questItemDetector.IsBuildingRequiredItem(item));
                    
                    if (isQuestItem)
                        lineColor = _config.QuestItemColor;
                    else if (isBuildingMaterial)
                        lineColor = _config.BuildingMaterialColor;
                }
                
                lineColor.a = 0.6f; // 半透明
                GL.Color(lineColor);
                
                // 绘制粗线条（与敌人连线相同的宽度）
                DrawThickLineGL(p1, p2, _config.EnemyLineWidth);
            }
            
            GL.End();
            GL.PopMatrix();
        }
        
        private Material _itemLineMaterial;
        
        /// <summary>
        /// 获取或创建线条材质
        /// </summary>
        private Material GetOrCreateLineMaterial()
        {
            if (_itemLineMaterial == null)
            {
                var shader = Shader.Find("Hidden/Internal-Colored") ?? Shader.Find("Sprites/Default");
                if (shader != null)
                {
                    _itemLineMaterial = new Material(shader)
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                    _itemLineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    _itemLineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    _itemLineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                    _itemLineMaterial.SetInt("_ZWrite", 0);
                }
            }
            return _itemLineMaterial;
        }
        
        /// <summary>
        /// 绘制粗线条（GL版本）
        /// </summary>
        private void DrawThickLineGL(Vector2 p1, Vector2 p2, float width)
        {
            float offset = width / Screen.width;
            
            // 主线
            GL.Vertex3(p1.x, p1.y, 0f);
            GL.Vertex3(p2.x, p2.y, 0f);
            
            // 增加厚度（绘制多条偏移线）
            for (int i = 1; i <= 2; i++)
            {
                float o = offset * i;
                
                // 水平偏移
                GL.Vertex3(p1.x + o, p1.y, 0f);
                GL.Vertex3(p2.x + o, p2.y, 0f);
                
                GL.Vertex3(p1.x - o, p1.y, 0f);
                GL.Vertex3(p2.x - o, p2.y, 0f);
                
                // 垂直偏移
                GL.Vertex3(p1.x, p1.y + o, 0f);
                GL.Vertex3(p2.x, p2.y + o, 0f);
                
                GL.Vertex3(p1.x, p1.y - o, 0f);
                GL.Vertex3(p2.x, p2.y - o, 0f);
            }
        }

        /// <summary>
        /// 绘制作弊功能状态指示器
        /// </summary>
        private void DrawCheatStatusIndicator()
        {
            // 获取当前激活的作弊功能
            List<string> activeCheatsList = new List<string>();
            
            if (_cheatSystem.IsGodModeEnabled())
                activeCheatsList.Add("<color=#FFD700>无敌模式</color>");
            
            if (_cheatSystem.IsOneHitKillEnabled())
                activeCheatsList.Add("<color=#FF4444>一击必杀</color>");
            
            if (_cheatSystem.IsSpeedBoostEnabled())
                activeCheatsList.Add("<color=#44FF44>速度提升</color>");
            
            if (_cheatSystem.IsInfiniteWeightEnabled())
                activeCheatsList.Add("<color=#00FFFF>无限负重</color>");
            
            if (_cheatSystem.IsInfiniteAmmoEnabled())
                activeCheatsList.Add("<color=#FFA500>无限子弹</color>");
            
            if (_cheatSystem.IsInfiniteStaminaEnabled())
                activeCheatsList.Add("<color=#FF00FF>无限耐力</color>");
            
            // 如果没有激活的作弊功能，不显示
            if (activeCheatsList.Count == 0)
                return;
            
            // 创建显示文本
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("【作弊功能】");
            foreach (string cheat in activeCheatsList)
            {
                sb.AppendLine($"• {cheat}");
            }
            
            // 设置文本样式
            GUIStyle indicatorStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 14,
                alignment = TextAnchor.UpperLeft,
                normal = { background = MakeBackgroundTexture(2, 2, new Color(0f, 0f, 0f, 0.7f)) },
                padding = new RectOffset(10, 10, 10, 10),
                richText = true
            };
            
            GUIStyle textStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                alignment = TextAnchor.UpperLeft,
                normal = { textColor = Color.white },
                richText = true,
                wordWrap = false
            };
            
            // 计算内容大小
            string content = sb.ToString().TrimEnd();
            Vector2 contentSize = textStyle.CalcSize(new GUIContent(content));
            
            // 在屏幕左上角绘制（考虑padding）
            float x = 10f;
            float y = 10f;
            float width = contentSize.x + 20f;
            float height = contentSize.y + 20f;
            
            Rect boxRect = new Rect(x, y, width, height);
            Rect textRect = new Rect(x + 10f, y + 10f, contentSize.x, contentSize.y);
            
            // 绘制背景框和文本
            GUI.Box(boxRect, "", indicatorStyle);
            GUI.Label(textRect, content, textStyle);
        }
        
        /// <summary>
        /// 创建纯色背景纹理
        /// </summary>
        private Texture2D MakeBackgroundTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            
            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
        
        /// <summary>
        /// 绘制单个ESP框和信息（优化版：接收预计算的品质）
        /// </summary>
        private void DrawESPBox(Vector3 screenPos, List<Item> items, float distance, ItemValueLevel maxLevel, Vector2 playerScreenPos)
        {
            try
            {
                // 获取物品列表文本
                StringBuilder sb = new StringBuilder();
                
                // 添加距离信息（如果启用）
                if (_config.ShowDistance)
                {
                    sb.AppendLine($"[{distance:F0}m]");
                }
                
                // 添加物品信息
                int displayCount = 0;
                int maxDisplay = Mathf.Min(_config.MaxDisplayItems, items.Count);
                
                for (int i = 0; i < maxDisplay; i++)
                {
                    Item item = items[i];
                    if (item == null) continue;
                    
                    string itemName = item.DisplayName;
                    if (item.StackCount > 1)
                    {
                        itemName = $"{itemName} x{item.StackCount}";
                    }
                    
                    // 检查是否是任务物品或建筑材料
                    string requirementTag = "";
                    if (_questItemDetector != null)
                    {
                        requirementTag = _questItemDetector.GetItemRequirementType(item);
                    }
                    
                    // 检查是否是未录入的钥匙
                    bool isUnregisteredKey = IsUnregisteredKey(item);
                    
                    if (isUnregisteredKey)
                    {
                        // 未录入的钥匙用特殊标记
                        sb.AppendLine($"[🔑未录入]{requirementTag} {itemName}");
                    }
                    else
                    {
                        // 根据新的品质系统添加颜色标记
                        ItemValueLevel level = ItemQualityUtil.GetItemValueLevel(item);
                        string qualityTag = $"[{ItemQualityUtil.GetQualityName(level)}]";
                        sb.AppendLine($"{qualityTag}{requirementTag} {itemName}");
                    }
                    
                    displayCount++;
                }
                
                // 如果还有更多物品
                if (items.Count > displayCount)
                {
                    sb.AppendLine($"... (+{items.Count - displayCount}件)");
                }

                string text = sb.ToString();
                
                // 计算文本大小
                GUIContent content = new GUIContent(text);
                Vector2 textSize = _espTextStyle.CalcSize(content);
                
                // 添加一些padding
                float padding = 10f;
                Rect backgroundRect = new Rect(
                    screenPos.x - textSize.x / 2 - padding,
                    screenPos.y - textSize.y / 2 - padding,
                    textSize.x + padding * 2,
                    textSize.y + padding * 2
                );
                
                // 获取品质颜色
                Color itemColor = ItemQualityUtil.GetItemValueLevelColor(maxLevel);
                
                // 检查是否包含任务物品或建筑材料，如果是则使用特殊颜色
                bool hasQuestItem = false;
                bool hasBuildingMaterial = false;
                if (_questItemDetector != null)
                {
                    foreach (Item item in items)
                    {
                        if (_config.HighlightQuestItems && _questItemDetector.IsQuestRequiredItem(item))
                        {
                            hasQuestItem = true;
                        }
                        if (_config.HighlightBuildingMaterials && _questItemDetector.IsBuildingRequiredItem(item))
                        {
                            hasBuildingMaterial = true;
                        }
                    }
                }
                
                // 边框颜色优先级：高价值物品 > 任务物品 > 建筑材料
                Color borderColor = itemColor;
                bool useThickBorder = false;
                
                // 如果是高价值物品（紫色+），优先使用物品颜色
                if ((int)maxLevel >= (int)ItemValueLevel.Purple)
                {
                    borderColor = itemColor;
                }
                // 否则检查任务和建筑材料
                else if (hasQuestItem && hasBuildingMaterial)
                {
                    // 同时是任务物品和建筑材料，混合颜色
                    borderColor = Color.Lerp(_config.QuestItemColor, _config.BuildingMaterialColor, 0.5f);
                    useThickBorder = true;
                }
                else if (hasQuestItem)
                {
                    borderColor = _config.QuestItemColor;
                    useThickBorder = true;
                }
                else if (hasBuildingMaterial)
                {
                    borderColor = _config.BuildingMaterialColor;
                    useThickBorder = true;
                }
                
                // 绘制半透明背景
                Color bgColor = new Color(0, 0, 0, _config.BackgroundAlpha);
                DrawRectFast(backgroundRect, bgColor);
                
                // 绘制边框
                float borderThickness = _config.BorderThickness;
                // 任务/建筑物品边框加粗（但不覆盖高价值物品）
                if (useThickBorder)
                {
                    borderThickness *= 1.5f;
                }
                DrawRectOutlineFast(backgroundRect, borderColor, borderThickness);
                
                // 设置文本颜色并绘制
                _espTextStyle.normal.textColor = itemColor;
                Rect textRect = new Rect(
                    screenPos.x - textSize.x / 2,
                    screenPos.y - textSize.y / 2,
                    textSize.x,
                    textSize.y
                );
                GUI.Label(textRect, text, _espTextStyle);
                
                // 注意：连线现在在 OnRenderObject() 中使用 GL 绘制，与敌人连线统一样式
            }
            catch
            {
                // 忽略GUI绘制错误
            }
        }

        /// <summary>
        /// 绘制实心矩形
        /// </summary>
        private void ScanAllLootboxesOld_ForMapMarkers()
        {
            try
            {
                // 清理无效标记
                CleanupInvalidMarkers();

                // 查找所有箱子
                InteractableLootbox[] allLootboxes = UnityEngine.Object.FindObjectsOfType<InteractableLootbox>();
                
                int newBoxCount = 0;
                int totalItems = 0;

                foreach (InteractableLootbox box in allLootboxes)
                {
                    if (box == null || !box.gameObject.activeInHierarchy)
                        continue;

                    // 如果已经追踪过，跳过
                    if (_trackedLootboxes.ContainsKey(box))
                        continue;

                    // 获取箱子的Inventory
                    Inventory inventory = box.Inventory;
                    if (inventory == null)
                        continue;

                    // 检查是否正在加载
                    if (inventory.Loading)
                        continue;

                    // 获取物品列表
                    List<Item> items = GetItemsFromInventory(inventory);
                    
                    // 应用品质过滤（仅用于小地图标记）
                    if (_minQualityFilter > 0)
                    {
                        items = items.Where(item => item.Quality >= _minQualityFilter).ToList();
                    }

                    // 如果箱子为空或没有符合条件的物品，可以选择不显示
                    if (items.Count == 0 && !_showLowValueItems)
                        continue;

                    // 创建标记
                    GameObject markerObj = CreateMarkerForLootbox(box, items);
                    if (markerObj != null)
                    {
                        LootboxMarkerData data = new LootboxMarkerData
                        {
                            lootbox = box,
                            marker = markerObj,
                            items = items,
                            inventory = inventory
                        };

                        _trackedLootboxes[box] = data;
                        newBoxCount++;
                        totalItems += items.Count;
                    }
                }

                if (newBoxCount > 0)
                {
                    Debug.Log($"DuckovESP: 发现 {newBoxCount} 个新箱子（小地图标记），共包含 {totalItems} 个物品");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"DuckovESP: 扫描箱子时出错 - {ex.Message}");
            }
        }

        /// <summary>
        /// 扫描世界物品（露天刷新的物品）
        /// </summary>
        private void ScanWorldItems()
        {
            try
            {
                if (!LevelManager.LevelInited)
                    return;

                // 清理已销毁的世界物品标记
                List<DuckovItemAgent> toRemove = new List<DuckovItemAgent>();
                foreach (var kvp in _trackedWorldItems)
                {
                    if (kvp.Key == null || kvp.Value == null)
                    {
                        toRemove.Add(kvp.Key);
                    }
                }
                foreach (var key in toRemove)
                {
                    if (_trackedWorldItems.TryGetValue(key, out GameObject marker))
                    {
                        if (marker != null)
                        {
                            UnityEngine.Object.Destroy(marker);
                        }
                    }
                    _trackedWorldItems.Remove(key);
                }

                // 查找所有世界物品
                DuckovItemAgent[] allWorldItems = UnityEngine.Object.FindObjectsOfType<DuckovItemAgent>();
                int newItemCount = 0;

                foreach (DuckovItemAgent itemAgent in allWorldItems)
                {
                    if (itemAgent == null || !itemAgent.gameObject.activeInHierarchy)
                        continue;

                    // 如果已经追踪过，跳过
                    if (_trackedWorldItems.ContainsKey(itemAgent))
                        continue;

                    // 获取物品
                    Item item = itemAgent.Item;
                    if (item == null)
                        continue;

                    // 检查是否是任务物品或建筑材料（优先显示）
                    bool isQuestOrBuilding = false;
                    if (_questItemDetector != null)
                    {
                        isQuestOrBuilding = (_config.HighlightQuestItems && _questItemDetector.IsQuestRequiredItem(item)) ||
                                           (_config.HighlightBuildingMaterials && _questItemDetector.IsBuildingRequiredItem(item));
                    }

                    // 如果是任务物品或建筑材料，绕过品质过滤
                    if (!isQuestOrBuilding)
                    {
                        // 应用品质过滤
                        if (_minQualityFilter > 0 && item.Quality < _minQualityFilter)
                            continue;
                    }

                    // 创建小地图标记
                    GameObject markerObj = CreateWorldItemMarker(itemAgent, item);
                    if (markerObj != null)
                    {
                        _trackedWorldItems[itemAgent] = markerObj;
                        newItemCount++;
                    }
                }

                if (newItemCount > 0)
                {
                    Debug.Log($"DuckovESP: 发现 {newItemCount} 个新的世界物品（小地图标记）");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"DuckovESP: 扫描世界物品时出错 - {ex.Message}");
            }
        }

        /// <summary>
        /// 为世界物品创建小地图标记
        /// </summary>
        private GameObject CreateWorldItemMarker(DuckovItemAgent itemAgent, Item item)
        {
            try
            {
                // 获取物品价值等级和颜色
                ItemValueLevel valueLevel = ItemQualityUtil.GetItemValueLevel(item);
                Color markerColor = ItemQualityUtil.GetItemValueLevelColor(valueLevel);
                
                // 检查是否是任务物品或建筑材料，使用特殊颜色
                bool isQuestItem = false;
                bool isBuildingMaterial = false;
                if (_questItemDetector != null)
                {
                    isQuestItem = _config.HighlightQuestItems && _questItemDetector.IsQuestRequiredItem(item);
                    isBuildingMaterial = _config.HighlightBuildingMaterials && _questItemDetector.IsBuildingRequiredItem(item);
                }
                
                // 任务物品优先使用黄色，建筑材料使用青色
                if (isQuestItem)
                {
                    markerColor = _config.QuestItemColor;
                }
                else if (isBuildingMaterial)
                {
                    markerColor = _config.BuildingMaterialColor;
                }

                // 创建标记对象
                GameObject markerObj = new GameObject($"WorldItemMarker_{itemAgent.GetInstanceID()}");
                markerObj.transform.position = itemAgent.transform.position;

                // 添加地图标记组件
                SimplePointOfInterest poi = markerObj.AddComponent<SimplePointOfInterest>();
                poi.Color = markerColor;
                poi.ShadowColor = Color.black;
                poi.ShadowDistance = 0f;

                // 设置标记图标和文本（添加任务/建筑材料标签）
                Sprite icon = GetMarkerIcon();
                string itemName = item.DisplayName;
                if (item.StackCount > 1)
                {
                    itemName = $"{itemName}x{item.StackCount}";
                }
                
                // 添加任务物品或建筑材料标签
                if (isQuestItem)
                {
                    itemName = $"[任务] {itemName}";
                }
                else if (isBuildingMaterial)
                {
                    itemName = $"[建材] {itemName}";
                }
                
                poi.Setup(icon, itemName, true, null);

                // 移动到主场景
                if (MultiSceneCore.MainScene != null)
                {
                    SceneManager.MoveGameObjectToScene(markerObj, MultiSceneCore.MainScene.Value);
                }

                return markerObj;
            }
            catch (Exception ex)
            {
                Debug.LogError($"DuckovESP: 创建世界物品标记失败 - {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 绘制实心矩形（优化版：使用共享纹理）
        /// </summary>
        private void DrawRectFast(Rect rect, Color color)
        {
            GUI.color = color;
            GUI.DrawTexture(rect, _whiteTexture);
            GUI.color = Color.white;
        }

        /// <summary>
        /// 绘制矩形边框（优化版：使用共享纹理）
        /// </summary>
        private void DrawRectOutlineFast(Rect rect, Color color, float thickness)
        {
            // 上
            DrawRectFast(new Rect(rect.x, rect.y, rect.width, thickness), color);
            // 下
            DrawRectFast(new Rect(rect.x, rect.y + rect.height - thickness, rect.width, thickness), color);
            // 左
            DrawRectFast(new Rect(rect.x, rect.y, thickness, rect.height), color);
            // 右
            DrawRectFast(new Rect(rect.x + rect.width - thickness, rect.y, thickness, rect.height), color);
        }

        /// <summary>
        /// 绘制线条（优化版：使用共享纹理）
        /// </summary>
        private void DrawLineFast(Vector2 from, Vector2 to, Color color, float thickness)
        {
            Vector2 dir = (to - from).normalized;
            float distance = Vector2.Distance(from, to);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            
            Matrix4x4 matrixBackup = GUI.matrix;
            GUIUtility.RotateAroundPivot(angle, from);
            DrawRectFast(new Rect(from.x, from.y - thickness / 2, distance, thickness), color);
            GUI.matrix = matrixBackup;
        }
        
        /// <summary>
        /// 旧版绘制方法（已弃用，保留用于兼容性）
        /// </summary>
        [System.Obsolete("Use DrawRectFast instead")]
        private void DrawRect(Rect rect, Color color)
        {
            DrawRectFast(rect, color);
        }

        /// <summary>
        /// 旧版绘制边框方法（已弃用，保留用于兼容性）
        /// </summary>
        [System.Obsolete("Use DrawRectOutlineFast instead")]
        private void DrawRectOutline(Rect rect, Color color, float thickness)
        {
            DrawRectOutlineFast(rect, color, thickness);
        }

        /// <summary>
        /// 旧版绘制线条方法（已弃用，保留用于兼容性）
        /// </summary>
        [System.Obsolete("Use DrawLineFast instead")]
        private void DrawLine(Vector2 from, Vector2 to, Color color, float thickness)
        {
            DrawLineFast(from, to, color, thickness);
        }

        /// <summary>
        /// 扫描场景中所有箱子
        /// </summary>
        private void ScanAllLootboxes()
        {
            // 只在启用小地图标记时才扫描
            if (!_config.EnableMapMarkers)
            {
                // 如果禁用了地图标记，清除所有现有标记
                if (_trackedLootboxes.Count > 0)
                {
                    ClearAllMarkers();
                }
                return;
            }
            
            try
            {
                // 清理无效标记
                CleanupInvalidMarkers();

                // 查找所有箱子
                InteractableLootbox[] allLootboxes = UnityEngine.Object.FindObjectsOfType<InteractableLootbox>();
                
                int newBoxCount = 0;
                int totalItems = 0;

                foreach (InteractableLootbox box in allLootboxes)
                {
                    if (box == null || !box.gameObject.activeInHierarchy)
                        continue;

                    // 如果已经追踪过，跳过
                    if (_trackedLootboxes.ContainsKey(box))
                        continue;

                    // 【过滤人物装备】检查 InteractableLootbox 是否在人物身上
                    if (IsLootboxOnCharacter(box))
                        continue;

                    // 获取箱子的Inventory
                    Inventory inventory = box.Inventory;
                    if (inventory == null)
                        continue;

                    // 检查是否正在加载
                    if (inventory.Loading)
                        continue;

                    // 获取物品列表
                    List<Item> items = GetItemsFromInventory(inventory);
                    
                    // 应用品质过滤（使用新的ItemValueLevel系统）+ 钥匙特殊逻辑 + 任务/建筑材料特殊逻辑
                    List<Item> filteredItems = new List<Item>();
                    
                    foreach (Item item in items)
                    {
                        if (item == null)
                            continue;
                        
                        // 检查是否是未录入的钥匙
                        bool isUnregisteredKey = IsUnregisteredKey(item);
                        
                        // 检查是否是任务物品或建筑材料
                        bool isQuestOrBuilding = false;
                        if (_questItemDetector != null)
                        {
                            isQuestOrBuilding = (_config.HighlightQuestItems && _questItemDetector.IsQuestRequiredItem(item)) ||
                                               (_config.HighlightBuildingMaterials && _questItemDetector.IsBuildingRequiredItem(item));
                        }
                        
                        // 如果是未录入的钥匙或任务/建筑材料，直接添加（绕过品质过滤）
                        if (isUnregisteredKey || isQuestOrBuilding)
                        {
                            filteredItems.Add(item);
                            continue;
                        }
                        
                        // 否则应用品质过滤
                        if (_minQualityFilter > 0)
                        {
                            ItemValueLevel level = ItemQualityUtil.GetItemValueLevel(item);
                            if ((int)level >= _minQualityFilter)
                            {
                                filteredItems.Add(item);
                            }
                        }
                        else
                        {
                            // 品质过滤为0时，显示所有物品
                            filteredItems.Add(item);
                        }
                    }

                    // 如果箱子为空或没有符合条件的物品，可以选择不显示
                    if (filteredItems.Count == 0 && !_showLowValueItems)
                        continue;

                    // 创建标记
                    GameObject markerObj = CreateMarkerForLootbox(box, filteredItems);
                    if (markerObj != null)
                    {
                        LootboxMarkerData data = new LootboxMarkerData
                        {
                            lootbox = box,
                            marker = markerObj,
                            items = filteredItems,  // 使用过滤后的物品列表
                            inventory = inventory
                        };

                        _trackedLootboxes[box] = data;
                        newBoxCount++;
                        totalItems += filteredItems.Count;
                    }
                }

                if (newBoxCount > 0)
                {
                    Debug.Log($"DuckovESP: 发现 {newBoxCount} 个新箱子，共包含 {totalItems} 个物品");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"DuckovESP: 扫描箱子时出错 - {ex.Message}");
            }
        }

        /// <summary>
        /// 从Inventory中获取所有物品（排除宠物狗的物品）
        /// </summary>
        private List<Item> GetItemsFromInventory(Inventory inventory)
        {
            List<Item> items = new List<Item>();

            if (inventory == null)
                return items;

            try
            {
                // 检查是否是宠物狗的背包
                bool isPetInventory = false;
                try
                {
                    // 使用 PetProxy.PetInventory 判断是否是宠物背包
                    if (PetProxy.PetInventory != null && inventory == PetProxy.PetInventory)
                    {
                        isPetInventory = true;
                    }
                }
                catch
                {
                    // 如果PetProxy不可用，忽略
                }

                // 如果是宠物背包，不读取物品
                if (isPetInventory)
                {
                    return items;
                }

                // Inventory实现了IEnumerable<Item>
                foreach (Item item in inventory)
                {
                    if (item != null)
                    {
                        items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"DuckovESP: 读取Inventory时出错 - {ex.Message}");
            }

            return items;
        }

        /// <summary>
        /// 检查 InteractableLootbox 是否在人物身上（而不是独立的箱子）
        /// </summary>
        private bool IsLootboxOnCharacter(InteractableLootbox lootbox)
        {
            if (lootbox == null)
                return false;

            try
            {
                // 检查 Lootbox 的 GameObject 或其父节点是否有 CharacterMainControl 组件
                CharacterMainControl character = lootbox.GetComponent<CharacterMainControl>();
                if (character == null)
                {
                    character = lootbox.GetComponentInParent<CharacterMainControl>();
                }
                
                if (character != null)
                {
                    // 是人物身上的 Lootbox（如背包、装备栏）
                    return true;
                }
                
                // 检查 GameObject 名字，人物相关的通常有特定命名
                string objName = lootbox.gameObject.name.ToLower();
                if (objName.Contains("character") || 
                    objName.Contains("player") || 
                    objName.Contains("backpack") ||
                    objName.Contains("equipment") ||
                    objName.Contains("inventory_character"))
                {
                    return true;
                }
                
                // 不是人物身上的，是独立箱子
                return false;
            }
            catch
            {
                // 如果检查失败，保守起见认为不是人物装备
                return false;
            }
        }

        /// <summary>
        /// 检查Inventory是否属于人物装备（玩家、Boss、NPC等）
        /// 通过检查 Inventory 的父节点是否在 LootBoxInventoriesParent 下来判断
        /// </summary>
        private bool IsCharacterInventory(Inventory inventory)
        {
            if (inventory == null)
                return false;

            try
            {
                // 箱子的 Inventory 都在 LevelManager.LootBoxInventoriesParent 下
                // 人物装备的 Inventory 在人物 GameObject 下
                Transform lootBoxParent = LevelManager.LootBoxInventoriesParent;
                if (lootBoxParent != null && inventory.transform.IsChildOf(lootBoxParent))
                {
                    // 是箱子 Inventory，不是人物装备
                    return false;
                }
                
                // 不在 LootBoxInventoriesParent 下，可能是人物装备或其他
                // 进一步检查是否有 CharacterMainControl 组件
                CharacterMainControl character = inventory.GetComponentInParent<CharacterMainControl>();
                if (character != null)
                {
                    // 是人物装备
                    return true;
                }
                
                // 保守起见，如果不在 LootBoxInventoriesParent 下且没有明确证据是箱子，就认为可能是人物装备
                return true;
            }
            catch
            {
                // 如果检查失败，保守起见认为是人物装备（跳过）
                return true;
            }
        }

        /// <summary>
        /// 判断物品是否为未录入的钥匙
        /// </summary>
        private bool IsUnregisteredKey(Item item)
        {
            if (item == null)
                return false;

            try
            {
                // 检查物品是否有"Key"标签
                bool isKey = false;
                foreach (var tag in item.Tags)
                {
                    if (tag != null && tag.name != null && tag.name.Equals("Key", StringComparison.OrdinalIgnoreCase))
                    {
                        isKey = true;
                        break;
                    }
                }

                if (!isKey)
                    return false;

                // 检查钥匙是否已经录入
                bool isRegistered = Duckov.MasterKeys.MasterKeysManager.IsActive(item.TypeID);
                
                // 返回：是钥匙 且 未录入
                return !isRegistered;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"DuckovESP: 检查钥匙状态时出错 - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 为箱子创建地图标记
        /// </summary>
        private GameObject CreateMarkerForLootbox(InteractableLootbox box, List<Item> items)
        {
            try
            {
                string boxName = GetLootboxDisplayName(box, items);
                
                GameObject markerObj = new GameObject($"ESP_Marker_{box.GetInstanceID()}");
                markerObj.transform.position = box.transform.position;

                // 添加地图标记组件
                SimplePointOfInterest poi = markerObj.AddComponent<SimplePointOfInterest>();
                
                // 根据物品品质和任务需求设置颜色
                Color markerColor = GetMarkerColorByQuality(items);
                poi.Color = markerColor;
                poi.ShadowColor = Color.black;
                poi.ShadowDistance = 0f;

                // 设置标记图标和文本（带任务/建筑材料标记）
                Sprite icon = GetMarkerIcon();
                string markerText = GetMarkerTextWithTags(boxName, items);
                poi.Setup(icon, markerText, true, null);

                // 移动到主场景
                if (MultiSceneCore.MainScene != null)
                {
                    SceneManager.MoveGameObjectToScene(markerObj, MultiSceneCore.MainScene.Value);
                }

                return markerObj;
            }
            catch (Exception ex)
            {
                Debug.LogError($"DuckovESP: 创建标记失败 - {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 获取箱子显示名称 - 显示物品中文名称列表
        /// </summary>
        private string GetLootboxDisplayName(InteractableLootbox box, List<Item> items)
        {
            if (items.Count == 0)
                return "空箱子";

            // 获取所有物品的中文名称
            List<string> itemNames = new List<string>();
            foreach (Item item in items)
            {
                string displayName = item.DisplayName;
                
                // 如果有堆叠数量大于1，显示数量
                if (item.StackCount > 1)
                {
                    displayName = $"{displayName}x{item.StackCount}";
                }
                
                itemNames.Add(displayName);
            }

            // 如果物品太多，只显示前3个
            if (itemNames.Count > 3)
            {
                string displayText = string.Join(", ", itemNames.Take(3));
                return $"{displayText}... (+{itemNames.Count - 3}件)";
            }
            else
            {
                return string.Join(", ", itemNames);
            }
        }

        /// <summary>
        /// 根据物品品质获取标记颜色 - 使用新的ItemValueLevel系统
        /// 优先级调整：高价值物品(紫色+) > 任务物品 > 建筑材料 > 普通物品颜色
        /// 任务物品和建筑材料通过文字标记 [任务物品] [建筑材料] 体现，不覆盖高价值物品颜色
        /// </summary>
        private Color GetMarkerColorByQuality(List<Item> items)
        {
            if (items.Count == 0)
                return Color.gray;

            // 计算最高品质
            ItemValueLevel maxLevel = items.Max(item => ItemQualityUtil.GetItemValueLevel(item));
            
            // 最高优先级：高价值物品（紫色及以上）
            if ((int)maxLevel >= (int)ItemValueLevel.Purple)
            {
                return ItemQualityUtil.GetItemValueLevelColor(maxLevel);
            }
            
            // 第二优先级：任务物品（仅当没有紫色+物品时）
            if (_questItemDetector != null && _config.HighlightQuestItems)
            {
                foreach (Item item in items)
                {
                    if (_questItemDetector.IsQuestRequiredItem(item))
                    {
                        return _config.QuestItemColor; // 任务物品用黄色
                    }
                }
            }
            
            // 第三优先级：建筑材料（仅当没有高价值和任务物品时）
            if (_questItemDetector != null && _config.HighlightBuildingMaterials)
            {
                foreach (Item item in items)
                {
                    if (_questItemDetector.IsBuildingRequiredItem(item))
                    {
                        return _config.BuildingMaterialColor; // 建筑材料用青色
                    }
                }
            }

            // 最低优先级：使用普通品质颜色
            return ItemQualityUtil.GetItemValueLevelColor(maxLevel);
        }
        
        /// <summary>
        /// 获取带任务/建筑材料标记的文本
        /// </summary>
        private string GetMarkerTextWithTags(string baseName, List<Item> items)
        {
            if (_questItemDetector == null || items.Count == 0)
                return baseName;
            
            // 检查是否有任务物品或建筑材料
            bool hasQuestItem = false;
            bool hasBuildingMaterial = false;
            
            foreach (Item item in items)
            {
                if (_config.HighlightQuestItems && _questItemDetector.IsQuestRequiredItem(item))
                {
                    hasQuestItem = true;
                }
                if (_config.HighlightBuildingMaterials && _questItemDetector.IsBuildingRequiredItem(item))
                {
                    hasBuildingMaterial = true;
                }
            }
            
            // 添加标记前缀
            string prefix = "";
            if (hasQuestItem && hasBuildingMaterial)
                prefix = "[任务+建筑] ";
            else if (hasQuestItem)
                prefix = "[任务物品] ";
            else if (hasBuildingMaterial)
                prefix = "[建筑材料] ";
            
            return prefix + baseName;
        }

        /// <summary>
        /// 获取品质文本（已弃用，使用ItemQualityUtil.GetQualityName替代）
        /// </summary>
        private string GetQualityText(int quality)
        {
            switch (quality)
            {
                case 0: return "普通";
                case 1: return "常见";
                case 2: return "优秀";
                case 3: return "稀有";
                case 4: return "史诗";
                case 5: return "传说";
                default: return "未知";
            }
        }

        /// <summary>
        /// 获取物品价值估算
        /// </summary>
        private int GetItemValue(Item item)
        {
            try
            {
                // 基础价值根据品质
                int baseValue = item.Quality * 100;
                
                // 如果有堆叠，乘以数量
                baseValue *= Mathf.Max(1, item.StackCount);

                return baseValue;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取标记图标
        /// </summary>
        private Sprite GetMarkerIcon()
        {
            try
            {
                List<Sprite> icons = MapMarkerManager.Icons;
                if (icons != null && icons.Count > 0)
                {
                    // 兼容 BossLiveMapMod: 避免使用 icons[2]（它用于 Boss 标记）
                    // icons[0] - 通常是默认标记
                    // icons[1] - 可能是重要位置
                    // icons[2] - BossLiveMapMod 使用（避免冲突）
                    // icons[5] - 可能是宝箱/物品
                    // icons[7] - 其他特殊标记
                    
                    // 优先使用第6个图标（索引5）- 通常用于物品/宝箱
                    if (icons.Count >= 6 && icons[5] != null)
                    {
                        return icons[5];
                    }
                    
                    // 备选：第8个图标（索引7）
                    if (icons.Count >= 8 && icons[7] != null)
                    {
                        return icons[7];
                    }
                    
                    // 备选：第2个图标（索引1）- 但避免 icons[2]
                    if (icons.Count >= 2 && icons[1] != null)
                    {
                        return icons[1];
                    }
                    
                    // 最后备选：第1个图标
                    if (icons[0] != null)
                    {
                        return icons[0];
                    }
                }
            }
            catch { }

            return null;
        }

        /// <summary>
        /// 更新所有标记位置
        /// </summary>
        private void UpdateMarkerPositions()
        {
            List<InteractableLootbox> toRemove = null;

            foreach (KeyValuePair<InteractableLootbox, LootboxMarkerData> kv in _trackedLootboxes)
            {
                InteractableLootbox box = kv.Key;
                LootboxMarkerData data = kv.Value;

                // 检查箱子是否还有效
                if (box == null || data.marker == null || data.inventory == null)
                {
                    if (toRemove == null)
                        toRemove = new List<InteractableLootbox>();
                    toRemove.Add(box);
                    continue;
                }

                // 检查箱子是否为空（被掠夺）
                if (box.Looted || data.inventory.IsEmpty())
                {
                    if (toRemove == null)
                        toRemove = new List<InteractableLootbox>();
                    toRemove.Add(box);
                    continue;
                }

                // 重新检查物品是否仍然符合当前的品质过滤条件
                List<Item> currentItems = GetItemsFromInventory(data.inventory);
                List<Item> filteredItems = new List<Item>();
                
                foreach (Item item in currentItems)
                {
                    if (item == null)
                        continue;
                    
                    // 检查是否是未录入的钥匙
                    bool isUnregisteredKey = IsUnregisteredKey(item);
                    
                    // 检查是否是任务物品或建筑材料
                    bool isQuestOrBuilding = false;
                    if (_questItemDetector != null)
                    {
                        isQuestOrBuilding = (_config.HighlightQuestItems && _questItemDetector.IsQuestRequiredItem(item)) ||
                                           (_config.HighlightBuildingMaterials && _questItemDetector.IsBuildingRequiredItem(item));
                    }
                    
                    // 如果是未录入的钥匙或任务/建筑材料，直接添加
                    if (isUnregisteredKey || isQuestOrBuilding)
                    {
                        filteredItems.Add(item);
                        continue;
                    }
                    
                    // 否则应用品质过滤
                    if (_minQualityFilter > 0)
                    {
                        ItemValueLevel level = ItemQualityUtil.GetItemValueLevel(item);
                        if ((int)level >= _minQualityFilter)
                        {
                            filteredItems.Add(item);
                        }
                    }
                    else
                    {
                        // 品质过滤为0时，显示所有物品
                        filteredItems.Add(item);
                    }
                }
                
                // 如果没有符合条件的物品了，移除标记
                if (filteredItems.Count == 0 && !_showLowValueItems)
                {
                    if (toRemove == null)
                        toRemove = new List<InteractableLootbox>();
                    toRemove.Add(box);
                    continue;
                }
                
                // 更新缓存的物品列表
                data.items = filteredItems;

                // 更新标记位置
                data.marker.transform.position = box.transform.position;
            }

            // 清理已移除的箱子
            if (toRemove != null)
            {
                foreach (InteractableLootbox box in toRemove)
                {
                    RemoveMarker(box);
                }
            }
        }

        /// <summary>
        /// 清理无效的标记
        /// </summary>
        private void CleanupInvalidMarkers()
        {
            List<InteractableLootbox> toRemove = null;

            foreach (KeyValuePair<InteractableLootbox, LootboxMarkerData> kv in _trackedLootboxes)
            {
                if (kv.Key == null || kv.Value.marker == null)
                {
                    if (toRemove == null)
                        toRemove = new List<InteractableLootbox>();
                    toRemove.Add(kv.Key);
                }
            }

            if (toRemove != null)
            {
                foreach (InteractableLootbox box in toRemove)
                {
                    RemoveMarker(box);
                }
            }
        }

        /// <summary>
        /// 移除指定箱子的标记
        /// </summary>
        private void RemoveMarker(InteractableLootbox box)
        {
            if (box != null && _trackedLootboxes.TryGetValue(box, out LootboxMarkerData data))
            {
                if (data.marker != null)
                {
                    try
                    {
                        UnityEngine.Object.Destroy(data.marker);
                    }
                    catch { }
                }
                _trackedLootboxes.Remove(box);
            }
        }

        /// <summary>
        /// 清理所有标记（兼容 BossLiveMapMod）
        /// </summary>
        private void ClearAllMarkers()
        {
            // 清理箱子标记（只删除我们自己创建的）
            foreach (KeyValuePair<InteractableLootbox, LootboxMarkerData> kv in _trackedLootboxes)
            {
                if (kv.Value.marker != null)
                {
                    try
                    {
                        // 检查是否是我们创建的标记（名称检查）
                        if (kv.Value.marker.name.StartsWith("LootboxMarker_"))
                        {
                            UnityEngine.Object.Destroy(kv.Value.marker);
                        }
                    }
                    catch { }
                }
            }
            _trackedLootboxes.Clear();
            
            // 清理世界物品标记（只删除我们自己创建的）
            foreach (KeyValuePair<DuckovItemAgent, GameObject> kv in _trackedWorldItems)
            {
                if (kv.Value != null)
                {
                    try
                    {
                        // 检查是否是我们创建的标记（名称检查）
                        if (kv.Value.name.StartsWith("WorldItemMarker_"))
                        {
                            UnityEngine.Object.Destroy(kv.Value);
                        }
                    }
                    catch { }
                }
            }
            _trackedWorldItems.Clear();
            
            Debug.Log("DuckovESP: 已清理所有标记");
        }

        /// <summary>
        /// 箱子标记数据
        /// </summary>
        private class LootboxMarkerData
        {
            public InteractableLootbox lootbox;
            public GameObject marker;
            public List<Item> items;
            public Inventory inventory;
        }
    }
}

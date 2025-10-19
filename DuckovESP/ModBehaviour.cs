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
        
        // 3D ESP缓存数据
        private readonly List<ESPData> _espDataCache = new List<ESPData>(100);
        private float _espCacheTimer = 0f;
        private const float ESP_CACHE_INTERVAL = 0.2f; // 每0.2秒更新一次ESP缓存
        
        // 扫描计时器
        private float _scanTimer = 0f;
        private const float SCAN_INTERVAL = 1f; // 每秒扫描一次
        
        // 配置系统
        private ESPConfig _config;
        private ESPConfigMenu _configMenu;
        
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
            
            // 加载配置
            _config = ESPConfig.Load();
            _configMenu = new ESPConfigMenu(_config);
            
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
            Debug.Log($"按 {_config.MenuToggleKey} 打开配置菜单");
            
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
        }

        private void OnDisable()
        {
            // 取消订阅
            LevelManager.OnLevelInitialized -= OnLevelInitialized;
            LevelManager.OnAfterLevelInitialized -= OnAfterLevelInitialized;
            MultiSceneCore.OnSubSceneLoaded -= OnSubSceneLoaded;
            
            // 清理所有标记
            ClearAllMarkers();
            
            Debug.Log("DuckovESP: 已清理并禁用");
        }

        private void OnLevelInitialized()
        {
            Debug.Log("DuckovESP: 关卡初始化，清理旧数据");
            ClearAllMarkers();
            _scanTimer = 0f;
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

            // 检查配置菜单切换
            if (Input.GetKeyDown(_config.MenuToggleKey))
            {
                _configMenu.ToggleMenu();
            }

            // 定时扫描小地图标记
            _scanTimer -= Time.unscaledDeltaTime;
            if (_scanTimer <= 0f)
            {
                _scanTimer = SCAN_INTERVAL;
                ScanAllLootboxes();
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
            
            // 使用已追踪的箱子列表，而不是FindObjectsOfType
            foreach (var kvp in _trackedLootboxes)
            {
                InteractableLootbox box = kvp.Key;
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
                List<Item> items = kvp.Value.items;
                if (items == null || items.Count == 0)
                    continue;
                
                // 应用3D ESP的品质过滤 + 钥匙特殊逻辑
                List<Item> filteredItems = new List<Item>();
                
                foreach (Item item in items)
                {
                    if (item == null)
                        continue;
                    
                    // 检查是否是未录入的钥匙
                    bool isUnregisteredKey = IsUnregisteredKey(item);
                    
                    // 如果是未录入的钥匙，直接添加
                    if (isUnregisteredKey)
                    {
                        filteredItems.Add(item);
                        continue;
                    }
                    
                    // 否则应用品质过滤
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
        /// 在屏幕上绘制3D ESP标记（使用缓存数据）
        /// </summary>
        private void OnGUI()
        {
            // 绘制配置菜单（总是检查，即使不在关卡中）
            _configMenu?.DrawMenu();
            
            if (!_enable3DESP || !LevelManager.LevelInited || _mainCamera == null)
                return;

            // 确保GUI样式已初始化
            if (_espTextStyle == null)
            {
                InitializeGUIStyle();
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
                    
                    // 检查是否是未录入的钥匙
                    bool isUnregisteredKey = IsUnregisteredKey(item);
                    
                    if (isUnregisteredKey)
                    {
                        // 未录入的钥匙用特殊标记
                        sb.AppendLine($"[🔑未录入] {itemName}");
                    }
                    else
                    {
                        // 根据新的品质系统添加颜色标记
                        ItemValueLevel level = ItemQualityUtil.GetItemValueLevel(item);
                        string qualityTag = $"[{ItemQualityUtil.GetQualityName(level)}]";
                        sb.AppendLine($"{qualityTag} {itemName}");
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
                
                // 绘制半透明背景
                Color bgColor = new Color(0, 0, 0, _config.BackgroundAlpha);
                DrawRectFast(backgroundRect, bgColor);
                
                // 绘制边框
                DrawRectOutlineFast(backgroundRect, itemColor, _config.BorderThickness);
                
                // 设置文本颜色并绘制
                _espTextStyle.normal.textColor = itemColor;
                Rect textRect = new Rect(
                    screenPos.x - textSize.x / 2,
                    screenPos.y - textSize.y / 2,
                    textSize.x,
                    textSize.y
                );
                GUI.Label(textRect, text, _espTextStyle);
                
                // 绘制从角色到箱子的连线（如果启用）
                if (_config.ShowConnectLine)
                {
                    DrawLineFast(playerScreenPos, 
                            new Vector2(screenPos.x, screenPos.y), 
                            itemColor, 1f);
                }
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

                    // 获取箱子的Inventory
                    Inventory inventory = box.Inventory;
                    if (inventory == null)
                        continue;

                    // 检查是否正在加载
                    if (inventory.Loading)
                        continue;

                    // 获取物品列表
                    List<Item> items = GetItemsFromInventory(inventory);
                    
                    // 应用品质过滤（使用新的ItemValueLevel系统）+ 钥匙特殊逻辑
                    List<Item> filteredItems = new List<Item>();
                    
                    foreach (Item item in items)
                    {
                        if (item == null)
                            continue;
                        
                        // 检查是否是未录入的钥匙
                        bool isUnregisteredKey = IsUnregisteredKey(item);
                        
                        // 如果是未录入的钥匙，直接添加
                        if (isUnregisteredKey)
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
                
                // 根据物品品质设置颜色
                Color markerColor = GetMarkerColorByQuality(items);
                poi.Color = markerColor;
                poi.ShadowColor = Color.black;
                poi.ShadowDistance = 0f;

                // 设置标记图标和文本
                Sprite icon = GetMarkerIcon();
                poi.Setup(icon, boxName, true, null);

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
        /// </summary>
        private Color GetMarkerColorByQuality(List<Item> items)
        {
            if (items.Count == 0)
                return Color.gray;

            ItemValueLevel maxLevel = items.Max(item => ItemQualityUtil.GetItemValueLevel(item));
            return ItemQualityUtil.GetItemValueLevelColor(maxLevel);
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
                if (icons != null && icons.Count >= 4 && icons[3] != null)
                {
                    return icons[3]; // 使用第4个图标
                }
                
                if (icons != null && icons.Count >= 1 && icons[0] != null)
                {
                    return icons[0]; // 备用第1个图标
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
        /// 清理所有标记
        /// </summary>
        private void ClearAllMarkers()
        {
            foreach (KeyValuePair<InteractableLootbox, LootboxMarkerData> kv in _trackedLootboxes)
            {
                if (kv.Value.marker != null)
                {
                    try
                    {
                        UnityEngine.Object.Destroy(kv.Value.marker);
                    }
                    catch { }
                }
            }

            _trackedLootboxes.Clear();
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

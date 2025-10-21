# 任务目标位置标记 - 实现计划

## 概述

在3D ESP和小地图中标记游戏中的任务目标位置（到达指定范围），允许玩家清晰地看到需要前往的位置。

---

## 核心需求

### 输入源
- **CountDownArea** - 任务目标区域的主要组件
- **ShowLocationInMap** - 任务名称和显示信息
- **Collider** - 定义到达的范围
- **BoundaryGenerator** (可选) - 复杂多边形边界

### 输出
1. **小地图标记**：显示任务位置点
2. **3D ESP圆形**：显示到达范围（GL绘制）
3. **文字信息**：任务名称、距离、进度

---

## 实现步骤

### 步骤1：定义数据结构

```csharp
/// <summary>
/// 任务区域标记数据
/// </summary>
public class QuestZoneMarkerData
{
    public CountDownArea countDownArea;
    public ShowLocationInMap locationMarker;
    public Collider triggerCollider;
    public GameObject miniMapMarker;
    
    public string displayName;
    public Vector3 centerPosition;
    public float radius;
    public float requiredTime;
    public bool isActive;
    public float progress;  // 0-1 进度
    public float remainingTime;
}
```

### 步骤2：扫描任务区域

```csharp
private Dictionary<CountDownArea, QuestZoneMarkerData> _trackedQuestZones = 
    new Dictionary<CountDownArea, QuestZoneMarkerData>();

private void ScanQuestZones()
{
    try
    {
        // 查找所有CountDownArea
        CountDownArea[] questAreas = FindObjectsOfType<CountDownArea>();
        
        foreach (CountDownArea area in questAreas)
        {
            if (area == null || !area.gameObject.activeInHierarchy)
                continue;
            
            if (_trackedQuestZones.ContainsKey(area))
                continue;
            
            // 获取显示信息
            ShowLocationInMap locationMarker = area.GetComponent<ShowLocationInMap>();
            if (locationMarker == null)
                continue;
            
            // 获取触发范围
            Collider collider = area.GetComponent<Collider>();
            if (collider == null)
                continue;
            
            // 创建标记数据
            QuestZoneMarkerData markerData = new QuestZoneMarkerData
            {
                countDownArea = area,
                locationMarker = locationMarker,
                triggerCollider = collider,
                displayName = locationMarker.DisplayName,
                centerPosition = collider.bounds.center,
                radius = GetColliderRadius(collider),
                requiredTime = area.RequiredExtrationTime,
                isActive = area.gameObject.activeInHierarchy
            };
            
            _trackedQuestZones[area] = markerData;
            
            // 创建小地图标记
            CreateQuestZoneMarker(markerData);
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"DuckovESP: 扫描任务区域时出错 - {ex.Message}");
    }
}
```

### 步骤3：获取Collider范围

```csharp
private float GetColliderRadius(Collider collider)
{
    if (collider == null)
        return 5f;
    
    // 根据碰撞体类型计算半径
    if (collider is SphereCollider sphereCollider)
    {
        return sphereCollider.radius * collider.transform.lossyScale.x;
    }
    else if (collider is CapsuleCollider capsuleCollider)
    {
        return capsuleCollider.radius * Mathf.Max(collider.transform.lossyScale.x, 
                                                   collider.transform.lossyScale.z);
    }
    else if (collider is BoxCollider boxCollider)
    {
        Vector3 size = boxCollider.size;
        float maxSize = Mathf.Max(size.x, size.z);
        return maxSize * 0.5f * Mathf.Max(collider.transform.lossyScale.x, 
                                          collider.transform.lossyScale.z);
    }
    else
    {
        // 其他类型的碰撞体，使用bounds
        return collider.bounds.extents.magnitude;
    }
}
```

### 步骤4：创建小地图标记

```csharp
private void CreateQuestZoneMarker(QuestZoneMarkerData markerData)
{
    try
    {
        GameObject markerObj = new GameObject($"QuestZoneMarker_{markerData.countDownArea.GetInstanceID()}");
        markerObj.transform.position = markerData.centerPosition;
        
        // 添加地图标记组件
        SimplePointOfInterest poi = markerObj.AddComponent<SimplePointOfInterest>();
        poi.Color = Color.yellow;  // 任务位置通常是黄色
        poi.ShadowColor = Color.black;
        poi.ShadowDistance = 0f;
        
        markerData.miniMapMarker = markerObj;
    }
    catch (Exception ex)
    {
        Debug.LogError($"DuckovESP: 创建任务区域标记时出错 - {ex.Message}");
    }
}
```

### 步骤5：更新任务区域状态

```csharp
private void UpdateQuestZonesStatus()
{
    try
    {
        CharacterMainControl player = CharacterMainControl.Main;
        if (player == null)
            return;
        
        Vector3 playerPos = player.transform.position;
        List<CountDownArea> toRemove = new List<CountDownArea>();
        
        foreach (var kvp in _trackedQuestZones)
        {
            CountDownArea area = kvp.Key;
            QuestZoneMarkerData markerData = kvp.Value;
            
            if (area == null || !area.gameObject.activeInHierarchy)
            {
                toRemove.Add(area);
                continue;
            }
            
            // 更新位置
            markerData.centerPosition = area.GetComponent<Collider>().bounds.center;
            
            // 更新进度
            markerData.progress = area.Progress;
            markerData.remainingTime = area.RemainingTime;
            markerData.isActive = area.gameObject.activeInHierarchy;
            
            // 更新小地图标记位置
            if (markerData.miniMapMarker != null)
            {
                markerData.miniMapMarker.transform.position = markerData.centerPosition;
            }
        }
        
        // 清理已失效的标记
        foreach (var area in toRemove)
        {
            if (_trackedQuestZones.TryGetValue(area, out var markerData))
            {
                if (markerData.miniMapMarker != null)
                {
                    UnityEngine.Object.Destroy(markerData.miniMapMarker);
                }
                _trackedQuestZones.Remove(area);
            }
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"DuckovESP: 更新任务区域状态时出错 - {ex.Message}");
    }
}
```

### 步骤6：在3D ESP中绘制圆形范围

```csharp
private void DrawQuestZones3DESP()
{
    try
    {
        if (_trackedQuestZones.Count == 0)
            return;
        
        CharacterMainControl player = CharacterMainControl.Main;
        if (player == null)
            return;
        
        Vector3 playerPos = player.transform.position;
        
        foreach (var markerData in _trackedQuestZones.Values)
        {
            if (!markerData.isActive)
                continue;
            
            Vector3 screenPos = _mainCamera.WorldToScreenPoint(markerData.centerPosition);
            
            // 在摄像机前方才绘制
            if (screenPos.z <= 0)
                continue;
            
            float distance = Vector3.Distance(playerPos, markerData.centerPosition);
            
            // 如果距离太远，不绘制
            if (distance > 500f)
                continue;
            
            // 绘制圆形范围
            DrawQuestZoneCircle(markerData, screenPos);
        }
    }
    catch (Exception ex)
    {
        Debug.LogWarning($"DuckovESP: 绘制任务区域3D ESP时出错 - {ex.Message}");
    }
}

private void DrawQuestZoneCircle(QuestZoneMarkerData markerData, Vector3 screenPos)
{
    // 在屏幕坐标中绘制圆形
    Color zoneColor = Color.yellow;  // 任务位置颜色
    
    // 根据进度改变颜色
    if (markerData.progress > 0)
    {
        // 进行中：渐变颜色
        zoneColor = Color.Lerp(Color.yellow, Color.green, markerData.progress);
    }
    
    // 使用GL绘制圆形（类似撤离点指示的方式）
    DrawCircleGL(new Vector2(screenPos.x, Screen.height - screenPos.y), 
                 markerData.radius * 10, zoneColor, 24);
}
```

### 步骤7：在OnGUI中绘制文字信息

```csharp
private void DrawQuestZonesInfo()
{
    try
    {
        if (_trackedQuestZones.Count == 0)
            return;
        
        CharacterMainControl player = CharacterMainControl.Main;
        if (player == null)
            return;
        
        Vector3 playerPos = player.transform.position;
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = 12;
        labelStyle.normal.textColor = Color.yellow;
        labelStyle.alignment = TextAnchor.MiddleCenter;
        
        foreach (var markerData in _trackedQuestZones.Values)
        {
            if (!markerData.isActive)
                continue;
            
            Vector3 screenPos = _mainCamera.WorldToScreenPoint(markerData.centerPosition);
            
            if (screenPos.z <= 0)
                continue;
            
            float distance = Vector3.Distance(playerPos, markerData.centerPosition);
            
            // 绘制任务名称和信息
            string infoText = $"{markerData.displayName}\n[{distance:F0}m]";
            
            if (markerData.progress > 0)
            {
                infoText += $"\n进度: {markerData.progress * 100:F0}% ({markerData.remainingTime:F0}s)";
            }
            
            GUI.Label(new Rect(screenPos.x - 100, Screen.height - screenPos.y - 30, 200, 80), 
                     infoText, labelStyle);
        }
    }
    catch (Exception ex)
    {
        Debug.LogWarning($"DuckovESP: 绘制任务区域信息时出错 - {ex.Message}");
    }
}
```

### 步骤8：集成到主更新循环

在 `Update()` 中添加：
```csharp
if (!isInBase)
{
    _scanTimer -= Time.unscaledDeltaTime;
    if (_scanTimer <= 0f)
    {
        _scanTimer = SCAN_INTERVAL;
        ScanQuestZones();  // 【新增】扫描任务区域
    }
    
    UpdateQuestZonesStatus();  // 【新增】更新任务状态
}
```

在 `OnRenderObject()` 中添加：
```csharp
if (_config.EnableEnemyESP)  // 可以添加新的配置选项
{
    DrawQuestZones3DESP();  // 【新增】绘制任务区域3D ESP
}
```

在 `OnGUI()` 中添加：
```csharp
if (_config.EnableEnemyESP)  // 可以添加新的配置选项
{
    DrawQuestZonesInfo();  // 【新增】绘制任务信息文字
}
```

---

## 配置选项 (ESPConfig)

建议添加的配置字段：

```csharp
[SerializeField]
public bool EnableQuestZoneMarkers = true;  // 启用任务区域标记

[SerializeField]
public Color QuestZoneColor = Color.yellow;  // 任务区域颜色

[SerializeField]
public Color QuestZoneInProgressColor = Color.green;  // 进行中的颜色

[SerializeField]
public bool ShowQuestZoneDistance = true;  // 显示距离

[SerializeField]
public bool ShowQuestZoneProgress = true;  // 显示进度
```

---

## 翻译键 (Localization)

需要添加的翻译键：

```json
{
  "UI.Toggle.EnableQuestZoneMarkers": "启用任务区域标记",
  "UI.Label.QuestZoneColor": "任务区域颜色",
  "UI.Label.QuestZoneDistance": "显示任务距离",
  "UI.Label.QuestZoneProgress": "显示任务进度"
}
```

---

## 测试场景

1. 进入有任务目标的关卡
2. 验证小地图上显示任务位置标记
3. 验证3D ESP中显示圆形范围指示
4. 验证显示正确的任务名称和距离
5. 靠近任务区域，验证进度显示
6. 进入任务区域，验证计时开始
7. 完成任务后，验证标记消失

---

## 已知限制

1. **复杂边界** - BoundaryGenerator定义的多边形边界暂不支持
2. **嵌套区域** - 不支持多层嵌套的任务区域
3. **动态生成** - 假设任务区域在关卡加载时就存在

---

## 后续优化

1. 添加淡入淡出效果
2. 支持BoundaryGenerator复杂边界
3. 添加任务完成音效提示
4. 支持任务进度追踪
5. 添加罗盘指向最近的任务区域


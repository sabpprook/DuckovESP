# DuckovESP 性能优化说明

## 优化内容

### 关键性能问题及解决方案

#### 1. ❌ **严重问题：每帧调用 FindObjectsOfType**
**问题**：
- 在 `OnGUI()` 中每帧调用 `FindObjectsOfType<InteractableLootbox>()`
- OnGUI 每帧可能被调用多次（每个事件一次）
- 导致极其昂贵的对象查找和大量 GC 分配

**解决方案**：
- ✅ 使用缓存的 `_trackedLootboxes` 字典
- ✅ 在 `Update()` 中每秒扫描一次（而不是在 OnGUI）
- ✅ 3D ESP 使用缓存数据，每 0.2 秒更新一次

#### 2. ❌ **严重问题：每次绘制都创建和销毁 Texture2D**
**问题**：
```csharp
private void DrawRect(Rect rect, Color color)
{
    Texture2D texture = new Texture2D(1, 1);  // 每次创建新纹理
    texture.SetPixel(0, 0, color);
    texture.Apply();
    GUI.DrawTexture(rect, texture);
    UnityEngine.Object.Destroy(texture);      // 立即销毁
}
```
- 每个 ESP 框需要多次调用（背景、4条边框、连接线）
- 导致大量 GC 分配和 DrawCall

**解决方案**：
- ✅ 创建共享白色纹理 `_whiteTexture`（在 Awake 中创建一次）
- ✅ 使用 `GUI.color` 调整颜色
- ✅ 重用同一个纹理进行所有绘制

**优化后代码**：
```csharp
private void DrawRectFast(Rect rect, Color color)
{
    GUI.color = color;
    GUI.DrawTexture(rect, _whiteTexture);  // 重用共享纹理
    GUI.color = Color.white;
}
```

#### 3. ❌ **性能问题：在 OnGUI 中进行复杂计算**
**问题**：
- 在 OnGUI 中计算距离、过滤物品、判断品质
- OnGUI 每帧多次调用，导致重复计算

**解决方案**：
- ✅ 创建 `ESPData` 缓存类
- ✅ 在 `UpdateESPCache()` 中预计算所有数据
- ✅ OnGUI 只负责绘制，不进行计算

#### 4. ✅ **优化：降低更新频率**
**改进**：
- 小地图标记：每 1 秒扫描一次
- 3D ESP 缓存：每 0.2 秒更新一次
- OnGUI：只负责绘制缓存数据

## 性能提升预期

### 优化前
- **FPS下降**：严重（可能下降 50% 以上）
- **CPU占用**：每帧调用 FindObjectsOfType，CPU 峰值高
- **内存分配**：大量 GC 分配（每帧数百个 Texture2D 对象）
- **DrawCall**：每个 ESP 框约 6-10 次 DrawCall

### 优化后
- **FPS影响**：轻微（预计影响 < 5%）
- **CPU占用**：平滑，无峰值
- **内存分配**：极少 GC（只有文本构建）
- **DrawCall**：每个 ESP 框约 6-10 次，但重用纹理

### 具体改进
| 项目 | 优化前 | 优化后 | 改善幅度 |
|------|--------|--------|----------|
| FindObjectsOfType 调用 | 每帧多次 | 每秒1次 | **99%↓** |
| Texture2D 创建 | 每帧数百次 | 1次（启动时） | **99.9%↓** |
| 品质判断计算 | 每帧多次 | 每0.2秒 | **80%↓** |
| 内存 GC 分配 | 数 MB/帧 | < 1KB/帧 | **99%↓** |

## 配置建议

### 高性能模式（推荐）
```
启用3D ESP: 是
最大显示距离: 100m
最大显示物品数: 3
显示距离: 否
显示连接线: 否
3D ESP最小品质: 橙色(传说) 或更高
```

### 平衡模式
```
启用3D ESP: 是
最大显示距离: 150m
最大显示物品数: 5
显示距离: 是
显示连接线: 是
3D ESP最小品质: 紫色(史诗)
```

### 完整信息模式（性能消耗较高）
```
启用3D ESP: 是
最大显示距离: 500m
最大显示物品数: 10
显示距离: 是
显示连接线: 是
3D ESP最小品质: 白色(普通) - 显示所有
```

## 代码结构改进

### 新增类和字段
```csharp
// ESP数据缓存
private class ESPData
{
    public Vector3 worldPosition;
    public List<Item> items;
    public float distance;
    public ItemValueLevel maxLevel;
}

private readonly List<ESPData> _espDataCache = new List<ESPData>(100);
private float _espCacheTimer = 0f;
private const float ESP_CACHE_INTERVAL = 0.2f;

// 共享纹理
private static Texture2D _whiteTexture;
```

### 执行流程
```
启动 (Awake)
  └─ 创建 _whiteTexture（一次性）

每秒 (Update)
  └─ ScanAllLootboxes() - 扫描新箱子，更新 _trackedLootboxes

每 0.2 秒 (Update)
  └─ UpdateESPCache() - 预计算所有 ESP 数据
      ├─ 遍历 _trackedLootboxes（不是 FindObjectsOfType！）
      ├─ 过滤距离、视野、品质
      ├─ 计算最高品质
      └─ 存入 _espDataCache

每帧 (OnGUI)
  └─ 遍历 _espDataCache
      └─ DrawESPBox() - 只负责绘制
          └─ DrawRectFast() - 使用共享纹理
```

## 监控性能

### 使用Unity Profiler
1. 打开 Unity Profiler（如果有开发者权限）
2. 关注以下指标：
   - **CPU Usage > Scripts**: 应该很低
   - **Memory > GC Alloc**: 应该接近0（OnGUI期间）
   - **Rendering > SetPass Calls**: 每个ESP框应该是1-2次

### 游戏内观察
- **帧率稳定**：FPS应该保持稳定，无明显波动
- **无卡顿**：移动和战斗时无明显卡顿
- **箱子数量**：即使显示50+个箱子，也应该流畅

## 进一步优化空间

如果仍然有性能问题，可以考虑：

1. **减少ESP更新频率**
   - 将 `ESP_CACHE_INTERVAL` 从 0.2 秒增加到 0.5 秒

2. **使用LOD（距离分级）**
   - 近距离箱子：显示详细信息
   - 远距离箱子：只显示图标或简化信息

3. **视野剔除优化**
   - 提前剔除屏幕外的箱子

4. **分帧处理**
   - 将所有箱子分成多组，每帧只更新一组

## 总结

通过以上优化，DuckovESP 的性能开销应该从"严重影响游戏性能"降低到"几乎无影响"。主要改进点是：

1. ✅ 消除了每帧的 FindObjectsOfType 调用
2. ✅ 消除了大量的 Texture2D 创建和销毁
3. ✅ 将复杂计算移出 OnGUI
4. ✅ 使用缓存和定时更新策略

如果还有性能问题，请检查：
- 箱子数量是否过多（>100个）
- 其他mod是否也在使用OnGUI
- 游戏本身的性能问题

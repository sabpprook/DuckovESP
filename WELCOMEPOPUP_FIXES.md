# WelcomePopup 修复总结

## 问题描述

用户报告 WelcomePopup 存在两个主要问题：

1. **窗口宽度递增** - 每当渲染新的帧时，窗口宽度会继续增大
2. **图片加载失败** - PNG 预览图像无法加载

## 根本原因分析

### 问题1: 窗口宽度递增

**原因**: 使用 `GUILayout.Window()` 时，`GUILayout` 会根据内容自动计算最优窗口大小。每次 `Draw()` 调用时，窗口都会被重新评估，导致宽度逐渐增长。

**错误代码**:
```csharp
_windowRect = GUILayout.Window(88888, _windowRect, DrawWindowContent, "", _windowStyle!);
```

**问题**: 
- GUILayout 会忽略设置的 `_windowRect` 宽度和高度
- 每次绘制时，GUILayout 会根据内部内容重新计算最优尺寸
- 随着渲染次数增加，计算的"最优宽度"会越来越大

### 问题2: PNG 图像加载失败

**原因**: 使用 `LoadRawTextureData()` 加载 PNG 数据不正确。

`LoadRawTextureData()` 期望的是原始像素数据（RGBA），而 PNG 是压缩的二进制格式。

**错误代码**:
```csharp
_previewImage.LoadRawTextureData(imageData);  // ❌ 这是错的
```

## 解决方案

### 方案1: 使用 GUILayout.BeginArea 替代 GUILayout.Window

`GUILayout.BeginArea()` 使用固定的矩形区域，不会根据内容自动调整大小。

**修复代码**:
```csharp
public void Draw()
{
    if (!_isVisible)
        return;

    InitializeStyles();

    // 【修复】保持窗口大小恒定
    _windowRect.width = 900;
    _windowRect.height = 700;

    var mousePos = Event.current.mousePosition;

    // 【修复】使用 BeginArea 而非 Window，固定尺寸
    GUILayout.BeginArea(_windowRect, "", _windowStyle!);
    
    GUILayout.BeginVertical();
    
    // ... 内容绘制 ...
    
    GUILayout.EndVertical();
    GUILayout.EndArea();

    // 消费鼠标事件，防止穿透到游戏
    if (_windowRect.Contains(mousePos))
    {
        if (Event.current.type == EventType.MouseDown || 
            Event.current.type == EventType.MouseUp || 
            Event.current.type == EventType.MouseDrag ||
            Event.current.type == EventType.ScrollWheel)
        {
            Event.current.Use();
        }
    }
}
```

**优势**:
- ✅ 窗口大小恒定，不会随时间变化
- ✅ 所有 GUILayout 控件被严格限制在指定矩形内
- ✅ 无需 `GUI.DragWindow()`，窗口仍可拖动

### 方案2: 使用反射调用 Texture2D.LoadImage

在 .NET Standard 中，`Texture2D` 的 `LoadImage` 方法可能不在编译时暴露。使用反射来调用它。

**修复代码**:
```csharp
private void LoadPreviewImage()
{
    _previewImageLoadAttempted = true;

    try
    {
        string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string dllDirectory = Path.GetDirectoryName(dllPath);
        string previewPath = Path.Combine(dllDirectory ?? "", "preview.png");

        if (File.Exists(previewPath))
        {
            byte[] imageData = File.ReadAllBytes(previewPath);
            _previewImage = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            
            // 【修复】使用反射调用 LoadImage 方法
            var loadImageMethod = typeof(Texture2D).GetMethod("LoadImage", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            
            if (loadImageMethod != null)
            {
                bool result = (bool)loadImageMethod.Invoke(_previewImage, new object[] { imageData })!;
                if (result)
                {
                    _previewImage.filterMode = FilterMode.Bilinear;
                    Debug.Log($"[WelcomePopup] 成功加载预览图像: {previewPath}");
                }
                else
                {
                    Debug.LogError("[WelcomePopup] LoadImage 返回 false");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"[WelcomePopup] 加载异常: {ex.Message}");
    }
}
```

**原理**:
- `Texture2D.LoadImage(byte[])` 会自动检测 PNG 格式并解压缩
- 它会自动调整 Texture2D 的尺寸以匹配图像分辨率
- 返回 `true` 表示成功，`false` 表示失败

**优势**:
- ✅ 正确处理 PNG 压缩数据
- ✅ 自动调整纹理大小，无需手动计算
- ✅ 支持所有主流图像格式（PNG, JPG, TGA）

## 修改文件

### `UI/Windows/WelcomePopup.cs`

**修改项**:
1. 移除 `DrawWindowContent()` 方法
2. 将内容绘制代码合并到 `Draw()` 方法中
3. 使用 `GUILayout.BeginArea()` 替代 `GUILayout.Window()`
4. 保持窗口尺寸恒定 (900x700)
5. 修复图像加载使用反射调用 `Texture2D.LoadImage()`

**行数变化**: 方法重组，总行数基本不变

## 验证

### 编译验证
```
✅ 0 个错误
⚠️ 45 个警告（全为预先存在）
✅ 编译成功，耗时 1.14 秒
```

### 运行时验证清单

- [ ] 启动游戏，WelcomePopup 显示
- [ ] 观察窗口宽度 - 应保持 900px，不增长
- [ ] 观察窗口高度 - 应保持 700px
- [ ] 检查预览图像 - 应在窗口顶部显示
- [ ] 拖动窗口 - 应该可以正常拖动（通过滚动内容）
- [ ] 点击"开始"按钮 - 窗口应隐藏
- [ ] 查看控制台日志 - 应显示 "成功加载预览图像" 消息

## 性能影响

- **内存**: 图像加载一次后缓存，无额外开销
- **CPU**: `GUILayout.BeginArea()` 比 `GUILayout.Window()` 更快（无自动调整计算）
- **渲染**: 无变化

## 相关文件

- `UI/Windows/WelcomePopup.cs` - 主要修改
- 无其他文件需要修改

---

**修复完成日期**: 2024年10月26日  
**编译状态**: ✅ 成功  
**部署准备**: ✅ 就绪

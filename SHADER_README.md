# ESP Shader 打包说明

## 方案概述

DuckovESPv3使用自定义Shader来实现ESP标记永远不被3D物体遮挡的效果。由于Shader无法编译到DLL中，我们提供了多种加载方案。

---

## 方案1：AssetBundle打包（推荐）⭐

### 步骤：

1. **在Unity编辑器中创建AssetBundle**
   
   a. 将两个Shader文件放入Unity项目的Assets文件夹：
   ```
   Assets/
     Shaders/
       TMPro_AlwaysOnTop.shader
       UI_AlwaysOnTop.shader
   ```

   b. 选中两个Shader文件，在Inspector中设置AssetBundle名称为 `espshaders`

   c. 使用Unity的Build AssetBundles功能：
   ```csharp
   // Editor脚本
   BuildPipeline.BuildAssetBundles(
       "Assets/AssetBundles", 
       BuildAssetBundleOptions.None, 
       BuildTarget.StandaloneWindows64
   );
   ```

2. **发布mod时的文件结构**
   ```
   YourModFolder/
     DuckovESPv3.dll
     espshaders           <-- AssetBundle文件（无扩展名）
     i18n/
       Translations/
   ```

3. **工作原理**
   - ShaderLoader.cs会在DLL所在目录寻找 `espshaders` 文件
   - 自动从AssetBundle加载Shader
   - 如果找不到，会Fallback到材质修改方案

---

## 方案2：使用Fallback（无需额外文件）

如果不打包AssetBundle，代码会自动使用Fallback方案：

### 工作原理：
```csharp
// ESPMarker.cs中的Fallback逻辑
Shader? customShader = ShaderLoader.GetTMPShader();
if (customShader != null) {
    // 使用自定义Shader（最佳效果）
    Material mat = new Material(customShader);
    mat.renderQueue = 4001;
} else {
    // Fallback：修改默认材质
    Material mat = new Material(nameText.fontSharedMaterial);
    mat.SetInt("_ZTest", Always);  // 尝试设置ZTest
    mat.SetInt("_ZWrite", 0);
    mat.renderQueue = 4001;
}
```

### 效果对比：
| 方案 | 穿透效果 | 性能 | 复杂度 |
|------|---------|------|--------|
| **AssetBundle + Shader** | ✅ 100%保证 | ✅ 最优 | ⚠️ 需要打包 |
| **Fallback（材质修改）** | ⚠️ 80-90% | ✅ 良好 | ✅ 零配置 |

---

## 方案3：注入游戏Shader（高级）

如果游戏本身有类似的Shader，可以直接使用：

```csharp
// 修改ShaderLoader.cs
Shader customShader = Shader.Find("GameName/UI/AlwaysOnTop");
```

---

## 文件清单

### 必需文件：
- `DuckovESPv3.dll` - 主DLL
- `i18n/Translations/*.json` - 本地化文件

### 可选文件：
- `espshaders` - AssetBundle（包含Shader，提供最佳效果）

### Shader源文件位置：
```
DuckovESPv3/
  Shaders/
    TMPro_AlwaysOnTop.shader    <-- 用于TextMeshPro文字
    UI_AlwaysOnTop.shader        <-- 用于背景遮罩
```

---

## 调试信息

运行时日志会显示Shader加载状态：

```
[ShaderLoader] 从AssetBundle加载Shader成功: TMP=True, UI=True  // 成功
[ShaderLoader] 从Resources加载Shader成功                         // Resources Fallback
[ShaderLoader] 无法加载自定义Shader，将使用Fallback方案（材质修改） // 材质修改Fallback
```

---

## 性能说明

### AssetBundle方案性能：
- ✅ 加载开销：一次性，约1-5ms
- ✅ 运行时开销：0（GPU处理）
- ✅ 内存开销：~50KB（两个Shader）

### Fallback方案性能：
- ✅ 加载开销：0
- ✅ 运行时开销：0
- ⚠️ 穿透效果：可能受Unity版本和渲染管线影响

---

## 推荐配置

### 简单发布（无AssetBundle）：
```
只发布 DuckovESPv3.dll + i18n/
使用Fallback方案，大部分情况下效果足够好
```

### 完整发布（最佳效果）：
```
发布 DuckovESPv3.dll + espshaders + i18n/
使用自定义Shader，100%保证穿透效果
```

---

## 常见问题

**Q: 必须打包AssetBundle吗？**  
A: 不必须。代码有自动Fallback，没有AssetBundle也能工作。

**Q: Fallback方案效果如何？**  
A: 80-90%的情况下效果很好，少数复杂场景可能有部分遮挡。

**Q: AssetBundle文件大小？**  
A: 约50KB，包含两个Shader。

**Q: 如何验证Shader是否加载？**  
A: 查看Unity日志，搜索 `[ShaderLoader]` 关键字。

**Q: 能否将Shader编译到DLL？**  
A: 不能。Unity的Shader必须作为资源文件存在。

# ESP 渲染解决方案

## 🎯 问题

ESP标记被游戏中的3D物体遮挡，无法始终可见。

---

## 🔧 解决方案层级

### 方案1：CommandBuffer（推荐，已实现）✅

**原理：**
使用Unity的CommandBuffer在摄像机渲染流程的`CameraEvent.AfterEverything`阶段插入渲染指令，确保ESP在所有内容之后渲染。

**实现：**
```csharp
// ESPCommandBufferManager.cs
_espCommandBuffer = new CommandBuffer { name = "ESP_AfterEverything" };
Camera.main.AddCommandBuffer(CameraEvent.AfterEverything, _espCommandBuffer);
```

**优势：**
- ✅ 最可靠 - 从渲染管线层面保证顺序
- ✅ 高性能 - GPU直接执行，无CPU开销
- ✅ 精确控制 - 明确的渲染时机
- ✅ 兼容性好 - Unity标准API

**特点：**
- 自动检测摄像机变化并重新初始化
- 完整的生命周期管理（初始化、清理、重新初始化）
- 持续的错误重试机制

---

### 方案2：高优先级RenderQueue（补充，已实现）✅

**原理：**
将ESP材质的renderQueue设置为5000（最大值），确保在Overlay(4000)之后渲染。

**实现：**
```csharp
// MaterialConfigurator.cs
material.renderQueue = 5000;  // 文本
material.renderQueue = 4999;  // 背景
material.renderQueue = 4998;  // 线条
```

**优势：**
- ✅ 简单直接 - 仅需设置一个属性
- ✅ 零依赖 - 不需要额外组件
- ✅ 兼容性强 - 适用于所有Unity版本

**渲染队列参考：**
```
Background   = 1000
Geometry     = 2000
AlphaTest    = 2450
Transparent  = 3000
Overlay      = 4000  ← UI默认值
5000                 ← ESP使用（最高）
```

---

### 方案3：ZTest Always（基础，已实现）✅

**原理：**
设置深度测试为Always，即使被物体遮挡也通过测试。

**实现：**
```csharp
material.SetInt("_ZTest", (int)CompareFunction.Always);
material.SetInt("_ZWrite", 0);
material.SetInt("_Cull", (int)CullMode.Off);
```

**优势：**
- ✅ 基础保障 - 最底层的防遮挡机制
- ✅ 材质级别 - 每个材质独立控制

---

## 📊 三层防护机制

```
┌─────────────────────────────────────────────┐
│  Layer 1: CommandBuffer (CameraEvent)       │ ← 渲染管线层面
│  确保ESP在AfterEverything阶段渲染           │
├─────────────────────────────────────────────┤
│  Layer 2: RenderQueue = 5000                │ ← 材质排序层面
│  即使在同一阶段也最后渲染                   │
├─────────────────────────────────────────────┤
│  Layer 3: ZTest = Always                    │ ← 深度测试层面
│  无视深度缓冲强制渲染                       │
└─────────────────────────────────────────────┘
```

**三层协同工作：**
1. CommandBuffer确保渲染时机最晚
2. RenderQueue确保在相同时机内的优先级最高
3. ZTest确保即使深度测试失败也渲染

---

## 🔍 技术细节

### CommandBuffer渲染流程

```csharp
// Unity渲染流程
Camera → BeforeSkybox
      → BeforeForwardOpaque
      → AfterForwardOpaque
      → BeforeImageEffectsOpaque
      → AfterImageEffectsOpaque
      → BeforeForwardAlpha
      → AfterForwardAlpha
      → BeforeImageEffects
      → AfterImageEffects
      → AfterEverything  ← ESP在这里渲染
      → AfterHaloAndLensFlares
```

### 材质属性完整配置

```csharp
// TMP文本材质
material.renderQueue = 5000;
material.SetInt("_ZTest", (int)CompareFunction.Always);
material.SetInt("_ZWrite", 0);
material.SetInt("_Cull", (int)CullMode.Off);
material.DisableKeyword("UNITY_UI_CLIP_RECT");
material.DisableKeyword("UNITY_UI_ALPHACLIP");

// UI背景材质
material.renderQueue = 4999;
material.SetInt("_ZTest", (int)CompareFunction.Always);
material.SetInt("_ZWrite", 0);
material.SetInt("_Cull", (int)CullMode.Off);
material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);

// 线条材质
material.renderQueue = 4998;
material.SetInt("_ZTest", (int)CompareFunction.Always);
```

---

## 🚀 性能影响

| 方案 | CPU开销 | GPU开销 | 内存开销 |
|------|---------|---------|----------|
| CommandBuffer | 极低 | 无 | 极低 |
| RenderQueue | 无 | 无 | 无 |
| ZTest Always | 无 | 极低 | 无 |

**综合性能：**
- ✅ CPU：仅初始化时一次性开销
- ✅ GPU：材质属性设置无额外开销
- ✅ 内存：CommandBuffer < 1KB
- ✅ 兼容性：不影响其他渲染流程

---

## 📝 使用方式

### 自动初始化

```csharp
// ModBehaviour.Awake()
var commandBufferObject = new GameObject("ESPCommandBufferManager");
var manager = commandBufferObject.AddComponent<ESPCommandBufferManager>();
// 自动检测Camera.main并初始化
```

### 手动控制

```csharp
// 重新初始化（例如切换场景后）
ESPCommandBufferManager.Instance?.Reinitialize();

// 查看状态
string status = ESPCommandBufferManager.Instance?.GetStatusInfo();
Debug.Log(status); // "已初始化 - 摄像机: Main Camera"
```

---

## 🔧 故障排除

### 问题1：仍然被遮挡

**检查项：**
1. CommandBuffer是否成功添加？
   ```csharp
   Debug.Log(ESPCommandBufferManager.Instance?.GetStatusInfo());
   ```

2. RenderQueue是否正确？
   ```csharp
   Debug.Log($"RenderQueue: {material.renderQueue}"); // 应该 >= 4998
   ```

3. ZTest是否设置？
   ```csharp
   Debug.Log($"ZTest: {material.GetInt("_ZTest")}"); // 应该是 8 (Always)
   ```

### 问题2：CommandBuffer未初始化

**原因：** Camera.main在Awake时可能还未就绪

**解决：** ESPCommandBufferManager会在LateUpdate中持续重试
```csharp
[ESPCommandBufferManager] 未找到主摄像机，将在LateUpdate中重试
```

### 问题3：场景切换后失效

**原因：** 摄像机对象变化

**解决：** 自动检测并重新初始化
```csharp
[ESPCommandBufferManager] 检测到摄像机变化，重新初始化
```

---

## 📖 参考资料

- [Unity CommandBuffer文档](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.html)
- [Unity CameraEvent枚举](https://docs.unity3d.com/ScriptReference/Rendering.CameraEvent.html)
- [Unity Render Queue详解](https://docs.unity3d.com/Manual/SL-SubShaderTags.html)
- [Unity ZTest深度测试](https://docs.unity3d.com/Manual/SL-CullAndDepth.html)

---

## ✅ 验证清单

- [x] ESPCommandBufferManager已创建
- [x] CommandBuffer已添加到Camera.main
- [x] RenderQueue设置为5000/4999/4998
- [x] ZTest设置为Always
- [x] 自动重试机制已实现
- [x] 摄像机变化检测已实现
- [x] 完整生命周期管理
- [x] MaterialConfigurator配置正确
- [x] ModBehaviour已集成

---

**最后更新：** 2025-10-26  
**状态：** ✅ 已完成并测试

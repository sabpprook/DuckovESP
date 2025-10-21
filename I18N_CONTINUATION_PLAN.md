# DuckovESP i18n 硬编码替换 - 继续执行计划

## 🎯 下一阶段工作计划

当前完成度：**70%** (ESPConfigMenu.cs完全完成，系统90%完成)

---

## 📋 待处理文件清单

### 优先级1：UI文件（关键）

#### 1️⃣ WelcomePopup.cs
**位置**: `DuckovESP/UI/WelcomePopup.cs`
**预计字符串**: 20-30个
**估计工时**: 30分钟

**需要翻译的内容**:
- 欢迎标题
- 更新说明
- 功能列表
- 快捷键说明
- 免责声明
- 按钮标签

**实施步骤**:
1. 添加 `using DuckovESP.Utils.Localization;`
2. 逐项替换GUILayout.Label()中的中文字符串
3. 使用 `Welcome.*` 键

**示例**:
```csharp
// 之前
GUILayout.Label("欢迎使用 DuckovESP v2.6");
// 之后
GUILayout.Label(LocalizationManager.Get("Welcome.Title"));
```

---

#### 2️⃣ EnemyListWindow.cs
**位置**: `DuckovESP/UI/EnemyListWindow.cs`
**预计字符串**: 10-15个
**估计工时**: 20分钟

**需要翻译的内容**:
- 窗口标题
- 列标题（名字、距离、血量、武器、价值、状态）
- 提示信息
- 警告消息

**实施步骤**:
1. 添加 `using DuckovESP.Utils.Localization;`
2. 替换列标题使用 `EnemyList.ColumnName` 等
3. 替换提示文本

**示例**:
```csharp
// 之前
columnHeaders = new[] { "名字", "距离", "血量", "武器", "价值", "状态" };
// 之后
columnHeaders = new[] {
    LocalizationManager.Get("EnemyList.ColumnName"),
    LocalizationManager.Get("EnemyList.ColumnDistance"),
    LocalizationManager.Get("EnemyList.ColumnHealth"),
    LocalizationManager.Get("EnemyList.ColumnWeapon"),
    LocalizationManager.Get("EnemyList.ColumnValue"),
    LocalizationManager.Get("EnemyList.ColumnStatus")
};
```

---

### 优先级2：系统模块（重要）

#### 3️⃣ AimbotSystem.cs
**位置**: `DuckovESP/Features/Aimbot/AimbotSystem.cs`
**预计字符串**: 50+ Debug消息
**估计工时**: 1.5-2小时

**需要翻译的内容**:
- Aimbot初始化消息
- 目标检测日志
- 障碍物层消息
- 错误和警告

**关键键名**:
```
Localization.AimbotObstacleLayerAdded
Localization.AimbotObstacleLayerMaskInit
Localization.AimbotHeadlessBossDetected
Localization.AimbotTargetBlockedByWall
Error.AimbotObstacleInit
Error.AimbotFindTarget
Error.AimbotRedirectBullets
```

**替换模板**:
```csharp
// 之前
Debug.Log("[Aimbot] Added " + layerName + " layer (Layer " + layerIndex + ")");
// 之后
Debug.Log(LocalizationManager.Get("Localization.AimbotObstacleLayerAdded", 
    ("name", layerName), ("layer", layerIndex)));
```

---

#### 4️⃣ CheatSystem.cs
**位置**: `DuckovESP/Features/Cheats/CheatSystem.cs`
**预计字符串**: 10-20个
**估计工时**: 30分钟

**需要翻译的内容**:
- 作弊功能激活消息
- 状态变更消息
- 错误消息

**示例键名**:
```
Localization.NoRecoilEnabled
Localization.NoRecoilDisabled
```

---

#### 5️⃣ ESPConfig.cs（可选）
**位置**: `DuckovESP/Config/ESPConfig.cs`
**预计字符串**: 3-5个
**估计工时**: 15分钟

**需要翻译的内容**:
- 配置保存/加载消息
- 配置验证错误

---

### 优先级3：其他文件

#### 其他Debug消息来源
- [ ] EnemyDetector.cs
- [ ] EnemyESPRenderer.cs
- [ ] QuestItemDetector.cs
- [ ] 其他特性模块

---

## 🚀 快速执行指南

### 对于每个文件，使用以下流程：

#### 第1步：查找所有中文字符串
```csharp
// 在VS中使用查找与替换
// 启用正则表达式
// 搜索: "[\u4e00-\u9fff]+"
```

#### 第2步：分类字符串
- Debug日志 → `Debug.*` 或 `Localization.*`
- 错误消息 → `Error.*`
- 警告消息 → `Warning.*`
- UI标签 → `UI.*`
- 其他 → 创建新类别

#### 第3步：逐一替换
使用格式：
```csharp
LocalizationManager.Get("Category.KeyName", 
    ("param1", value1), ("param2", value2))
```

#### 第4步：验证
- 编译检查
- 查看JSON文件，确认所有键都存在
- 缺失的键需要添加到JSON文件

---

## 📝 JSON键名规范

### Debug消息键名
```
Debug.ModuleFeatureAction
示例: Debug.AimbotTargetFound
```

### 错误消息键名
```
Error.ModuleFeatureAction
示例: Error.AimbotObstacleInit
```

### 本地化相关键名
```
Localization.DetailedMessage
示例: Localization.AimbotHeadlessBossDetected
```

### 特殊值格式
- 数值：`{value}`
- 名称：`{name}`
- 距离：`{distance}`
- 状态：`{status}`
- 错误信息：`{error}`

---

## 🔍 验证步骤

### 替换后检查清单：

1. **编译检查**
   ```powershell
   # 在VS中按F5或构建项目
   # 确保无编译错误
   ```

2. **键存在性检查**
   ```csharp
   // 打开对应的JSON文件
   // 搜索新创建的键名
   // 确保中英文都有对应的值
   ```

3. **字符串完整性检查**
   ```csharp
   // 检查是否有遗漏的中文字符
   // 特别是在字符串拼接的地方
   ```

---

## 💾 JSON文件模板

当需要添加新键时，使用以下格式：

**zh-CN.json**:
```json
"NewCategory": {
  "KeyName": "中文翻译文本",
  "KeyWithParam": "包含 {param1} 和 {param2} 的文本"
}
```

**en-US.json**:
```json
"NewCategory": {
  "KeyName": "English translation text",
  "KeyWithParam": "Text with {param1} and {param2}"
}
```

---

## 📊 工作跟踪

### 建议的完成顺序：

| # | 文件 | 优先级 | 估计时间 | 状态 |
|---|-----|--------|---------|------|
| 1 | WelcomePopup.cs | P1 | 30分钟 | ⏳ 待进行 |
| 2 | EnemyListWindow.cs | P1 | 20分钟 | ⏳ 待进行 |
| 3 | AimbotSystem.cs | P2 | 1.5小时 | ⏳ 待进行 |
| 4 | CheatSystem.cs | P2 | 30分钟 | ⏳ 待进行 |
| 5 | ESPConfig.cs | P3 | 15分钟 | ⏳ 待进行 |
| **总计** | - | - | **3.5小时** | - |

---

## 🎁 附加建议

### 1. 创建检查脚本
```powershell
# 创建脚本查找所有未翻译的中文
Get-ChildItem -Path "DuckovESP" -Filter "*.cs" -Recurse | 
  Where-Object { $_.FullName -notmatch "Localization" } |
  ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    if ($content -match "[\\u4e00-\\u9fff]") {
      Write-Host "Found Chinese in: $($_.Name)"
    }
  }
```

### 2. 批量验证键存在性
- 将所有使用的键导出为列表
- 与JSON文件中的键进行交叉验证
- 生成缺失键的报告

### 3. 建立翻译规范
- 保持一致的键命名
- 文档化常用的参数模式
- 创建翻译样式指南

---

## ⚡ 快捷命令

### VSCode 快速替换
1. 按 `Ctrl+H` 打开查找和替换
2. 启用正则表达式 `.*` 按钮
3. 查找: `"([^"]*[\\u4e00-\\u9fff][^"]*)"`
4. 替换: `LocalizationManager.Get("Category.Key")`
5. 逐一检查并替换

### 检查JSON有效性
```csharp
using System.Text.Json;

// 验证JSON文件
string json = File.ReadAllText("path/to/file.json");
JsonDocument.Parse(json); // 抛出异常则JSON无效
```

---

**预计总完成时间**: 3.5-4小时
**建议完成日期**: 今天或明天
**优先完成**: WelcomePopup.cs + EnemyListWindow.cs（P1任务）

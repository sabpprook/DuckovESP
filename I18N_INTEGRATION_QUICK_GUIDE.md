# DuckovESP i18n 快速集成指南

## 🎯 目标

快速将剩余的UI类和系统模块与i18n系统集成。

---

## 📋 集成检查清单

### 优先级1：关键UI类

#### [ ] WelcomePopup.cs
**位置**: `DuckovESP/UI/WelcomePopup.cs`

**所需修改**:
1. 添加 using:
```csharp
using DuckovESP.Utils.Localization;
```

2. 替换所有中文字符串:
```csharp
// 之前
GUILayout.Label("欢迎使用 DuckovESP v2.6");
// 之后
GUILayout.Label(LocalizationManager.Get("Welcome.Title"));

// 之前
GUILayout.Label("🔧 v2.6 Major Fixes");
// 之后
GUILayout.Label(LocalizationManager.Get("Welcome.MajorFixes"));
```

**需要的键**:
- `Welcome.Title`
- `Welcome.LatestUpdate`
- `Welcome.MajorFixes`
- `Welcome.About`
- `Welcome.Features`
- `Welcome.Hotkeys`
- `Welcome.HotkeysWarning`
- `Welcome.Tips`
- `Welcome.Disclaimer`
- `Welcome.FirstRun`

---

#### [ ] EnemyListWindow.cs
**位置**: `DuckovESP/UI/EnemyListWindow.cs`

**所需修改**:
1. 添加 using:
```csharp
using DuckovESP.Utils.Localization;
```

2. 替换列标题:
```csharp
// 之前
"名字", "距离", "血量", "武器", "价值", "状态"
// 之后
LocalizationManager.Get("EnemyList.ColumnName"),
LocalizationManager.Get("EnemyList.ColumnDistance"),
LocalizationManager.Get("EnemyList.ColumnHealth"),
LocalizationManager.Get("EnemyList.ColumnWeapon"),
LocalizationManager.Get("EnemyList.ColumnValue"),
LocalizationManager.Get("EnemyList.ColumnStatus")
```

**需要的键**:
- `EnemyList.NoEnemiesDetected`
- `EnemyList.EnemyCount`
- `EnemyList.TotalValue`
- `EnemyList.AimingWarning`
- `EnemyList.ColumnName`
- `EnemyList.ColumnDistance`
- `EnemyList.ColumnHealth`
- `EnemyList.ColumnWeapon`
- `EnemyList.ColumnValue`
- `EnemyList.ColumnStatus`
- `EnemyList.Hint`

---

### 优先级2：系统模块

#### [ ] AimbotSystem.cs
**位置**: `DuckovESP/Features/Aimbot/AimbotSystem.cs`

**所需修改**:
1. 添加 using:
```csharp
using DuckovESP.Utils.Localization;
```

2. 替换Debug.Log消息 (约50+条):
```csharp
// 之前
Debug.Log("[Aimbot] Error initializing obstacle layer: " + ex.Message);
// 之后
Debug.Log(LocalizationManager.Get("Error.AimbotObstacleInit", ("error", ex.Message)));
```

**关键Debug消息映射**:
```
"[Aimbot] Added {name} layer" → "Localization.AimbotObstacleLayerAdded"
"[Aimbot] Obstacle layer mask initialized" → "Localization.AimbotObstacleLayerMaskInit"
"[Aimbot] Error initializing" → "Error.AimbotObstacleInit"
"[Aimbot] Detected headless Boss" → "Localization.AimbotHeadlessBossDetected"
"[Aimbot] Target blocked" → "Localization.AimbotTargetBlockedByWall"
```

---

#### [ ] CheatSystem.cs
**位置**: `DuckovESP/Features/Cheats/CheatSystem.cs`

**所需修改**:
1. 添加 using:
```csharp
using DuckovESP.Utils.Localization;
```

2. 替换作弊功能的Debug消息

**需要的键**:
- Cheat functions status messages (如有)
- Error messages for cheat operations

---

#### [ ] TriggerBot 相关
**文件**: 可能在 AimbotSystem.cs 或单独的 TriggerBotSystem.cs

**关键消息**:
```
"[TriggerBot] Initialized" → "Localization.TriggerBotInitialized"
"[TriggerBot] Current target" → "Localization.TriggerBotCurrentTarget"
"[TriggerBot] Target blocked" → "Localization.TriggerBotTargetBlocked"
"[TriggerBot] Fired" → "Localization.TriggerBotFired"
"[TriggerBot] No weapon held" → "Localization.TriggerBotNoWeaponHeld"
```

---

#### [ ] NoRecoil 相关
**文件**: 可能在 CheatSystem.cs 或单独的文件

**关键消息**:
```
"No Recoil enabled" → "Localization.NoRecoilEnabled"
"No Recoil disabled" → "Localization.NoRecoilDisabled"
Error messages related to no recoil
```

---

## 🔄 集成步骤模板

### 对于每个需要集成的文件：

1. **添加 using 语句**
```csharp
using DuckovESP.Utils.Localization;
```

2. **查找所有中文字符串**
```powershell
# PowerShell命令查找中文字符串
Get-Content "filename.cs" | Select-String "[\\u4e00-\\u9fff]"
```

3. **分类字符串**
   - Debug.Log 消息 → `Debug.*` 或 `Localization.*`
   - UI 标签 → `UI.*`
   - 错误消息 → `Error.*`
   - 警告消息 → `Warning.*`

4. **逐一替换**
```csharp
// 模板
Debug.Log(LocalizationManager.Get("Category.KeyName", ("param", value)));
GUILayout.Label(LocalizationManager.Get("Category.KeyName"));
```

5. **验证翻译键存在**
   - 检查 zh-CN.json 和 en-US.json 中是否有对应键
   - 如果缺少，添加新键到两个JSON文件

---

## 📝 ESPConfigMenu 完整UI翻译

这是最复杂的文件，需要翻译120+个UI标签。

### 分段替换策略

#### 第1部分：ESP设置部分 (已完成)
```csharp
// 已完成
UI.Menu.ESP3DSettings
UI.Toggle.Enable3DESP
UI.Label.MaxDisplayDistance
UI.Label.FontSize
UI.Label.MaxDisplayItems
```

#### 第2部分：质量过滤设置
```csharp
// 需要添加键
UI.Menu.FilterSettings
UI.Label.ESP3DMinQuality
UI.Label.MapMarkerMinQuality
UI.Toggle.ShowEmptyBoxes
```

#### 第3部分：视觉设置
```csharp
UI.Menu.AppearanceSettings
UI.Label.BackgroundAlpha
UI.Label.BorderThickness
UI.Toggle.ShowDistance
UI.Toggle.ShowConnectLine
```

#### 第4部分：敌人ESP设置
```csharp
UI.Menu.EnemyESPSettings
UI.Toggle.EnableEnemyESP
UI.Toggle.ShowEnemyHealth
UI.Toggle.ShowEnemyWeapon
UI.Toggle.ShowEnemyValue
UI.Label.MaxEnemyDistance
UI.Label.ConnectLineMaxDistance
```

#### 第5部分：任务物品设置
```csharp
UI.Menu.QuestItemSettings
UI.Toggle.HighlightQuestItems
UI.Toggle.HighlightBuildingMaterials
UI.Label.QuestItemDescription
```

#### 第6部分：自动瞄准设置
```csharp
UI.Menu.AimbotSettings
UI.Toggle.EnableAimbot
UI.Toggle.AimbotAimAtHead
UI.Toggle.AimbotPredictMovement
UI.Toggle.AimbotIgnoreWalls
UI.Label.AimbotFOV
UI.Label.AimbotSmoothness
UI.Label.AimbotMaxDistance
```

#### 第7部分：自动扳机设置
```csharp
UI.Menu.TriggerBotSettings
UI.Toggle.EnableTriggerBot
UI.Toggle.TriggerBotOnlyADS
UI.Toggle.TriggerBotTeamCheck
UI.Label.TriggerBotDelay
```

#### 第8部分：No Recoil设置
```csharp
UI.Menu.NoRecoilSettings
UI.Toggle.EnableNoRecoil
UI.Label.NoRecoilDescription
UI.Label.NoRecoilMechanism
```

#### 第9部分：作弊快捷键
```csharp
UI.Menu.CheatFunctions
UI.CheatKeys.GodMode
UI.CheatKeys.OneHitKill
UI.CheatKeys.SpeedBoost
UI.CheatKeys.InfiniteWeight
UI.CheatKeys.InfiniteAmmo
UI.CheatKeys.InfiniteStamina
```

#### 第10部分：语言设置
```csharp
UI.Menu.LanguageSettings
UI.Label.ChangeLanguage
```

---

## ✅ 验证清单

完成每个文件后，确认：

- [ ] 添加了 using 语句
- [ ] 所有中文字符串都已替换
- [ ] 没有遗漏的中文字符串
- [ ] 编译无错误
- [ ] 所有翻译键都存在于两个JSON文件中
- [ ] 参数替换语法正确（{paramName}）
- [ ] Debug 消息格式正确
- [ ] UI 标签显示完整

---

## 🔍 查找遗漏的中文字符串

### 使用 grep 查找未翻译的中文
```powershell
# 在PowerShell中查找所有仍含中文的.cs文件
Get-ChildItem -Path "DuckovESP" -Filter "*.cs" -Recurse | 
  ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    if ($content -match "[\\u4e00-\\u9fff]") {
      Write-Host "Found Chinese in: $($_.Name)"
    }
  }
```

### 使用 Visual Studio 查找
1. Ctrl+H 打开查找和替换
2. 使用正则表达式：`[\u4e00-\u9fff]+`
3. 这将高亮所有中文文本

---

## 📚 参考资源

- 翻译键名约定：见 `I18N_IMPLEMENTATION_STATUS.md`
- 完整翻译文件：
  - `DuckovESP/i18n/Translations/zh-CN.json`
  - `DuckovESP/i18n/Translations/en-US.json`
- 核心系统：`DuckovESP/Utils/Localization/LocalizationManager.cs`

---

## 🚀 快速集成命令

如果您正在VSCode或VisualStudio中：

1. **打开文件**: Ctrl+P → 输入文件名
2. **查找中文**: Ctrl+F → 输入任意中文字符
3. **逐一替换**: Ctrl+H → 对每个字符串进行替换
4. **验证翻译**: 检查JSON文件确保键存在

---

**预计完成时间**: 
- WelcomePopup: 15分钟
- EnemyListWindow: 20分钟
- ESPConfigMenu (完整): 1小时
- AimbotSystem: 45分钟
- CheatSystem: 30分钟
- **总计**: 2.5-3小时完全集成

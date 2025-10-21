# DuckovESP i18n 目录结构指南

## 📁 翻译文件位置

### 标准位置

根据 `LocalizationManager.cs` 中的代码（第93行）：

```csharp
string baseDir = AppContext.BaseDirectory;
_translationPath = Path.Combine(baseDir, "i18n", "Translations");
```

**翻译文件应放在相对于游戏可执行文件的路径：**

```
[游戏根目录]/
├── i18n/
│   └── Translations/
│       ├── zh-CN.json
│       ├── en-US.json
│       ├── ja-JP.json  (未来)
│       └── ...其他语言
```

### 实际项目路径

在您的开发环境中：

```
DuckovESP/
├── DuckovESP/
│   ├── bin/
│   │   ├── Debug/
│   │   │   └── netstandard2.1/
│   │   │       └── [DLL文件]
│   │   └── Release/
│   │       └── netstandard2.1/
│   │           └── [DLL文件]
│   ├── i18n/                          ← 翻译文件目录
│   │   └── Translations/
│   │       ├── zh-CN.json            ✅ 已存在
│   │       └── en-US.json            ✅ 已存在
│   ├── Utils/
│   │   └── Localization/
│   │       ├── LocalizationManager.cs
│   │       └── LocalizationDataStore.cs
│   └── ...其他文件
```

## 🎯 编译后的文件位置

### 调试版本 (Debug)
```
bin/Debug/netstandard2.1/
├── DuckovESP.dll
└── [MOD加载后，i18n目录应位于同级位置]
```

### 发布版本 (Release)
```
bin/Release/netstandard2.1/
├── DuckovESP.dll
└── [MOD加载后，i18n目录应位于同级位置]
```

## 🔍 路径解析流程

1. **获取DLL所在目录**（新增）
   ```csharp
   // LocalizationManager 使用三步策略
   var assembly = typeof(LocalizationManager).Assembly;
   
   // 步骤1：尝试 Assembly.Location（普通DLL）
   string location = assembly.Location;
   
   // 步骤2：尝试 Assembly.CodeBase（备选方案）
   string codeBase = assembly.CodeBase;
   
   // 步骤3：使用 AppContext.BaseDirectory（所有情况通用）
   string baseDir = AppContext.BaseDirectory;
   ```

2. **构建翻译路径**
   ```csharp
   _translationPath = Path.Combine(baseDir, "i18n", "Translations");
   // 结果示例: "C:\SteamApps\common\DuckovModGame\Mods\DuckovESP\i18n\Translations"
   ```

3. **加载JSON文件**
   ```csharp
   string[] jsonFiles = Directory.GetFiles(_translationPath, "*.json");
   // 查找所有 .json 文件，如 zh-CN.json, en-US.json
   ```

## ⚡ 部署步骤

### 步骤1：编译Mod
```bash
# 在Visual Studio中编译或使用命令行
dotnet build --configuration Release
```

### 步骤2：准备文件
编译后的DLL位于：
```
DuckovESP/bin/Release/netstandard2.1/DuckovESP.dll
```

### 步骤3：创建Mod目录结构
在游戏Mods目录中（假设为 `DuckovESP` 文件夹）：
```
[游戏目录]/Mods/DuckovESP/
├── DuckovESP.dll              ← 编译的Mod DLL
├── info.ini                   ← Mod信息（已存在）
└── i18n/                      ← 创建此目录
    └── Translations/          ← 创建此目录
        ├── zh-CN.json         ← 复制此文件
        └── en-US.json         ← 复制此文件
```

### 步骤4：验证路径
启动游戏后，检查日志中是否出现：
```
[Localization] Translation path: [游戏目录]/Mods/DuckovESP/i18n/Translations
[Localization] Loaded zh-CN: 557 keys
[Localization] Loaded en-US: 557 keys
[Localization] Initialized, current language: zh-CN
```

## 🛠️ 开发调试时的路径

### 从Visual Studio运行
当从Visual Studio中调试时，`AppContext.BaseDirectory` 通常指向：
```
DuckovESP/bin/Debug/netstandard2.1/
```

因此，确保在编译后复制 `i18n` 文件夹到该位置：
```
DuckovESP/bin/Debug/netstandard2.1/
├── DuckovESP.dll
└── i18n/
    └── Translations/
        ├── zh-CN.json
        └── en-US.json
```

### 自动复制设置（推荐）
在 `.csproj` 文件中添加：
```xml
<ItemGroup>
    <None Update="i18n\Translations\*.json">
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
</ItemGroup>
```

这样每次编译时都会自动复制JSON文件到输出目录。

## ✅ 验证检查清单

- [ ] `i18n/Translations/` 目录已创建
- [ ] `zh-CN.json` 文件位于 `i18n/Translations/` 中
- [ ] `en-US.json` 文件位于 `i18n/Translations/` 中
- [ ] JSON文件使用UTF-8编码
- [ ] 所有翻译键都有对应的值
- [ ] 可以正确加载日志显示所有文件
- [ ] 运行时不显示"Translation directory not found"警告

## 📝 常见问题

### Q: 如果翻译文件找不到怎么办？
**A:** 检查以下几点：
1. 验证 `i18n/Translations/` 目录存在
2. 检查 JSON 文件名是否正确（区分大小写）
3. 查看调试日志中的 Translation path 是否正确
4. 确保 JSON 文件有读取权限

### Q: 可以将翻译文件放在其他地方吗？
**A:** 可以，但需要修改 `LocalizationManager.cs` 第93行：
```csharp
// 修改前
_translationPath = Path.Combine(baseDir, "i18n", "Translations");

// 修改后（示例）
_translationPath = Path.Combine(baseDir, "Translations");
// 或
_translationPath = @"C:\FixedPath\Translations";
```

### Q: 多个Mod共享翻译文件可以吗？
**A:** 不建议。每个Mod应该有自己的翻译文件副本，以避免依赖问题。

## 🚀 最终结构确认

```
✅ 当前已正确放置：
c:\Users\diaoz\source\repos\DuckovESP\
└── DuckovESP\
    └── i18n\
        └── Translations\
            ├── zh-CN.json      (557+ 键)
            └── en-US.json      (557+ 键)
```

**状态：✅ 位置正确，无需更改**

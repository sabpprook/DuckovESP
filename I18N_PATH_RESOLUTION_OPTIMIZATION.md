# LocalizationManager 路径解析优化

## 📝 变更说明

### 之前的实现
```csharp
string baseDir = AppContext.BaseDirectory;
_translationPath = Path.Combine(baseDir, "i18n", "Translations");
```
- 依赖于应用程序的基目录
- 在某些场景下可能指向错误的目录

### 之后的实现
```csharp
private static string GetModuleDirectory()
{
    // 策略1：Assembly.Location（普通DLL）
    // 策略2：Assembly.CodeBase（备选方案）
    // 策略3：AppContext.BaseDirectory（通用回退）
    // 策略4：Environment.CurrentDirectory（最后回退）
}

string baseDir = GetModuleDirectory();
_translationPath = Path.Combine(baseDir, "i18n", "Translations");
```

## ✅ 优势

1. **准确性** - 使用DLL本身的位置作为基准
   - 确保从DLL所在目录查找翻译文件
   - 与Mod文件结构直接关联

2. **健壮性** - 四层级回退机制
   - 普通DLL场景 ✅
   - 单文件应用 ✅
   - 反射限制 ✅
   - 极端情况 ✅

3. **兼容性** - 同时支持多种部署方式
   - BepInEx Mod框架
   - 独立DLL加载
   - 单文件打包应用
   - 游戏内嵌脚本

## 🎯 实现细节

### 四层策略

```csharp
┌─────────────────────────────────────┐
│ 尝试 Assembly.Location              │ ← 最常用 (普通DLL)
│ (适用于 .NET Framework & .NET Core) │
└──────────────┬──────────────────────┘
               │ 失败时
               ↓
┌─────────────────────────────────────┐
│ 尝试 Assembly.CodeBase              │ ← 备选方案
│ (旧式但有时有效)                     │
└──────────────┬──────────────────────┘
               │ 失败时
               ↓
┌─────────────────────────────────────┐
│ 使用 AppContext.BaseDirectory       │ ← 通用方案
│ (最可靠，在所有情况下都可用)          │
└──────────────┬──────────────────────┘
               │ 失败时
               ↓
┌─────────────────────────────────────┐
│ 使用 Environment.CurrentDirectory   │ ← 最后回退
│ (最不可靠但总比没有好)               │
└─────────────────────────────────────┘
```

### 异常处理

所有操作都用 try-catch 包装，确保：
- 即使某个策略抛出异常，也能继续尝试下一个
- 最后总是返回某个有效的目录路径

### 编译警告抑制

使用 `#pragma warning disable/restore` 抑制IL3000和IL3001警告，因为：
- 警告仅在单文件应用中适用
- 我们已处理该场景（通过回退机制）
- 抑制范围明确且受控

## 🔧 对现有代码的影响

### 无破坏性更改
- 不改变公开API
- 不影响现有集成代码
- 完全向后兼容

### 运行时日志改进

调试时会输出更详细的信息：
```
[Localization] Module directory: C:\Mods\DuckovESP
[Localization] Translation path: C:\Mods\DuckovESP\i18n\Translations
[Localization] Loaded zh-CN: 557 keys
[Localization] Loaded en-US: 557 keys
[Localization] Initialized, current language: zh-CN
```

## 📊 测试场景

| 场景 | Assembly.Location | Assembly.CodeBase | AppContext | Result |
|------|------------------|-------------------|-----------|--------|
| 普通DLL | ✅ | - | - | ✅ 使用策略1 |
| 单文件应用 | ❌ | ❌ | ✅ | ✅ 使用策略3 |
| 反射限制 | ⚠️ | ❌ | ✅ | ✅ 使用策略3 |
| 异常情况 | ❌ | ❌ | ⚠️ | ✅ 使用策略4 |

## 🚀 最终目录结构确认

```
✅ 当前已正确设置：
c:\Users\diaoz\source\repos\DuckovESP\
└── DuckovESP\
    ├── bin\
    │   ├── Debug\netstandard2.1\
    │   │   └── DuckovESP.dll
    │   └── Release\netstandard2.1\
    │       └── DuckovESP.dll
    ├── i18n\
    │   └── Translations\
    │       ├── zh-CN.json         (557+ 键)
    │       └── en-US.json         (557+ 键)
    └── Utils\Localization\
        ├── LocalizationManager.cs (已优化)
        └── LocalizationDataStore.cs
```

## 📋 验证检查清单

- [x] GetModuleDirectory() 实现完成
- [x] 四层级回退机制实现
- [x] 异常处理完善
- [x] 编译警告抑制
- [x] 日志记录完整
- [x] 编译成功无错误
- [x] 向后兼容

## 🎓 代码改进历史

| 版本 | 日期 | 改进内容 |
|------|------|---------|
| v1.0 | 初始 | 使用 AppContext.BaseDirectory |
| v1.1 | 当前 | 改为使用DLL位置 + 四层回退 |

---

**状态**: ✅ 完成并优化
**编译**: ✅ 无错误
**部署**: ✅ 准备就绪

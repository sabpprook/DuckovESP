# DuckovESP 配置指南

## 修改游戏路径

在编译之前，需要修改项目文件中的DLL引用路径。

### 步骤1: 找到游戏安装目录

常见的Steam游戏安装路径：
- `C:\Program Files (x86)\Steam\steamapps\common\Escape from Duckov`
- `D:\SteamLibrary\steamapps\common\Escape from Duckov`
- `E:\Games\Steam\steamapps\common\Escape from Duckov`

你可以在Steam中右键游戏 -> 管理 -> 浏览本地文件 来找到游戏目录。

### 步骤2: 修改 DuckovESP.csproj

打开 `DuckovESP\DuckovESP.csproj` 文件，找到所有 `<HintPath>` 标签，将路径修改为你的游戏安装路径。

例如，如果你的游戏安装在 `E:\Games\Duckov`，则修改为：

```xml
<Reference Include="TeamSoda.Duckov.Core">
  <HintPath>E:\Games\Duckov\Duckov_Data\Managed\TeamSoda.Duckov.Core.dll</HintPath>
  <Private>False</Private>
</Reference>
```

需要修改的DLL引用：
- TeamSoda.Duckov.Core.dll
- UnityEngine.CoreModule.dll
- UnityEngine.dll
- UnityEngine.UI.dll
- Unity.TextMeshPro.dll

### 步骤3: 修改构建脚本

打开 `build.ps1`，修改默认游戏路径：

```powershell
param(
    [string]$GamePath = "你的游戏路径",  # 在这里修改
    [switch]$Release
)
```

或者在运行构建脚本时指定路径：

```powershell
.\build.ps1 -GamePath "E:\Games\Duckov"
```

## 高级配置

### 修改扫描频率

在 `ModBehaviour.cs` 中修改：

```csharp
private const float SCAN_INTERVAL = 1f; // 改为 0.5f 扫描更快，改为 2f 扫描更慢
```

### 启用品质过滤

在 `ModBehaviour.cs` 中修改：

```csharp
private int _minQualityFilter = 2; // 只显示优秀及以上品质的物品
```

品质等级：
- 0 = 普通
- 1 = 常见
- 2 = 优秀
- 3 = 稀有
- 4 = 史诗
- 5 = 传说

### 显示空箱子

```csharp
private bool _showLowValueItems = true; // 改为true显示所有箱子，包括空箱子
```

### 修改标记颜色

在 `GetMarkerColorByQuality` 方法中自定义颜色：

```csharp
private Color GetMarkerColorByQuality(List<Item> items)
{
    // 自定义颜色方案
    switch (maxQuality)
    {
        case 5: return Color.red;  // 传说改为红色
        // ... 其他品质
    }
}
```

## 故障排除

### 编译错误: 找不到引用

- 确认游戏路径正确
- 确认DLL文件存在
- 检查游戏版本是否匹配

### Mod不加载

1. 检查文件结构：
   ```
   Duckov_Data\Mods\DuckovESP\
       ├── DuckovESP.dll
       └── info.ini
   ```

2. 检查 info.ini 的 name 字段必须是 `DuckovESP`

3. 查看游戏日志文件（通常在 `Duckov_Data\` 目录）

### 标记不显示

- 等待物品加载完成（几秒钟）
- 检查箱子是否激活
- 打开小地图查看

## 开发调试

### 启用详细日志

在代码中添加更多 Debug.Log：

```csharp
Debug.Log($"详细信息: {变量}");
```

### 查看游戏日志

Windows日志位置：
- `%USERPROFILE%\AppData\LocalLow\TeamSoda\Escape from Duckov\Player.log`

### 使用Unity Explorer（高级）

可以安装 Unity Explorer mod 来实时查看游戏对象和组件。

# DuckovESP 构建脚本
# 自动编译mod并复制到游戏目录

param(
    [string]$GamePath = "D:\SteamLibrary\steamapps\common\Escape from Duckov",
    [switch]$Release
)

$ErrorActionPreference = "Stop"

Write-Host "=== DuckovESP Mod 构建脚本 ===" -ForegroundColor Cyan
Write-Host ""

# 检测游戏路径
if (-not (Test-Path $GamePath)) {
    Write-Host "错误: 游戏路径不存在: $GamePath" -ForegroundColor Red
    Write-Host "请使用 -GamePath 参数指定正确的游戏安装路径" -ForegroundColor Yellow
    Write-Host "例如: .\build.ps1 -GamePath 'C:\Games\Escape from Duckov'" -ForegroundColor Yellow
    exit 1
}

$ModsPath = Join-Path $GamePath "Duckov_Data\Mods"
$ModFolder = Join-Path $ModsPath "DuckovESP"

Write-Host "游戏路径: $GamePath" -ForegroundColor Green
Write-Host "Mods路径: $ModsPath" -ForegroundColor Green
Write-Host ""

# 编译配置
$Configuration = if ($Release) { "Release" } else { "Debug" }
Write-Host "编译配置: $Configuration" -ForegroundColor Yellow

# 清理旧的构建
Write-Host "清理旧的构建文件..." -ForegroundColor Yellow
dotnet clean -c $Configuration

# 编译项目
Write-Host "编译项目..." -ForegroundColor Yellow
$buildResult = dotnet build .\DuckovESP\DuckovESP.csproj -c $Configuration

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "编译失败！" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "编译成功！" -ForegroundColor Green
Write-Host ""

# 创建Mod文件夹
if (-not (Test-Path $ModsPath)) {
    Write-Host "创建Mods文件夹: $ModsPath" -ForegroundColor Yellow
    New-Item -Path $ModsPath -ItemType Directory -Force | Out-Null
}

if (-not (Test-Path $ModFolder)) {
    Write-Host "创建Mod文件夹: $ModFolder" -ForegroundColor Yellow
    New-Item -Path $ModFolder -ItemType Directory -Force | Out-Null
}

# 复制文件
$DllSource = ".\DuckovESP\bin\$Configuration\netstandard2.1\DuckovESP.dll"
$DllDest = Join-Path $ModFolder "DuckovESP.dll"
$IniSource = ".\DuckovESP\info.ini"
$IniDest = Join-Path $ModFolder "info.ini"

Write-Host "复制文件到游戏目录..." -ForegroundColor Yellow
Copy-Item $DllSource $DllDest -Force
Copy-Item $IniSource $IniDest -Force

Write-Host ""
Write-Host "=== 构建完成！===" -ForegroundColor Green
Write-Host ""
Write-Host "文件已复制到:" -ForegroundColor Cyan
Write-Host "  DLL: $DllDest" -ForegroundColor White
Write-Host "  INI: $IniDest" -ForegroundColor White
Write-Host ""
Write-Host "下一步:" -ForegroundColor Yellow
Write-Host "  1. 启动游戏" -ForegroundColor White
Write-Host "  2. 进入Mod管理器" -ForegroundColor White
Write-Host "  3. 启用 '箱子物品透视 ESP'" -ForegroundColor White
Write-Host "  4. 进入游戏查看效果" -ForegroundColor White
Write-Host ""

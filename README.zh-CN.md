# Switchboard

[English](README.md) | [한국어](README.ko.md) | [中文](README.zh-CN.md) | [日本語](README.ja.md)

Switchboard 是一款 Windows 11 桌面叠加层，可让窗口切换更清晰、更可靠。它以易读的标题和不裁切的预览显示已打开的窗口，并根据窗口数量和可用屏幕空间调整布局。

**当前版本：** [v0.3.1](https://github.com/ai-blink/alt-tab/releases/tag/v0.3.1) · **发布形式：** Windows 11 x64 Portable

> 这是免安装的便携预览版：解压后即可运行。未进行代码签名的构建可能会触发 Windows SmartScreen 警告。

## 功能

- 按一次 Alt+Tab 即可显示或隐藏 Switchboard
- 显示完整源窗口的 DWM 预览
- 为超过 25 个窗口提供响应式行列布局和滚动
- 网格、紧凑和列表视图
- 拖动空白区域保存叠加层位置、一次性九方向位置控制器，以及返回已保存位置
- 按最近使用、应用、显示器、标题或收藏夹排序
- 透明、深色和浅色主题；不透明度；整体 UI 缩放（60/70/80/100/125/150/200%）；以及缩略图大小设置
- 自定义辅助快捷键（默认：Ctrl+Alt+Space）
- 始终置顶选项、系统托盘驻留，以及指示窗口切换的专用应用图标
- 每个窗口卡片均有一个请求正常关闭目标窗口的按钮
- 仅在窗口列表变化时刷新，以尽量减少轮询闪烁

## 安装与运行

1. 将 Switchboard-v0.3.1-win-x64-Portable.zip 解压到一个新文件夹。
2. 运行 Switchboard.App.exe。
3. 如果出现 SmartScreen，请确认来源后选择 **更多信息 → 仍要运行**。
4. 如需退出，在任务栏通知区域中右键 Switchboard 图标，然后选择 **Exit**。

自包含发布包已包含 .NET 运行时，无需单独安装运行时。

## 操作

| 输入 | 操作 |
| --- | --- |
| Alt+Tab | 显示或隐藏叠加层 |
| Ctrl+Alt+Space | 显示叠加层（默认辅助快捷键） |
| Tab 或方向键 | 移动窗口卡片选择 |
| Enter | 激活选中的窗口 |
| Esc | 隐藏叠加层 |
| 双击鼠标 | 激活该窗口 |
| 窗口卡片上的 X 按钮 | 请求正常关闭该窗口 |

顶部的设置按钮会打开一个单独的模态窗口。左侧标签将位置与移动、外观与大小、默认行为分开设置。拖动叠加层空白区域会自动保存新位置；位置控制器只会移动一次，不会改变已保存的位置。整体 UI 缩放会同时调整文字、按钮、间距和窗口尺寸，紧凑模式不会额外缩小。图钉按钮用于切换 **始终置顶**。

关闭按钮发送的是标准 Windows 关闭请求，并不会强制退出应用程序。如果目标应用有未保存的工作，仍可能显示其正常的保存确认对话框。

## 设置文件

设置会自动保存到：

~~~
%APPDATA%\Switchboard\settings.json
~~~

要恢复默认值，请退出 Switchboard，删除该文件，然后重新启动 Switchboard。

## 已知限制

- 由于 Windows 的焦点限制，激活以管理员权限运行的窗口或安全桌面上的窗口可能会失败。
- 当前发布包没有数字代码签名，因此可能出现 SmartScreen。
- 目前没有单独的设置用于禁用 Alt+Tab 钩子。请退出 Switchboard 后使用默认的 Windows Alt+Tab 行为。
- V1 不包含虚拟桌面管理、自动窗口放置和时间线功能。

## 开发环境

- Windows 11
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- WPF / Win32 / DWM

~~~powershell
git clone https://github.com/ai-blink/alt-tab.git
cd alt-tab
dotnet build Switchboard.slnx --nologo
dotnet test Switchboard.slnx --nologo
dotnet run --project src/Switchboard.App/Switchboard.App.csproj
~~~

## 发布构建

使用以下命令创建 Windows x64 自包含单文件可执行文件：

~~~powershell
dotnet publish src/Switchboard.App/Switchboard.App.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  --nologo `
  -o artifacts/release/win-x64 `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  -p:PublishTrimmed=false `
  -p:DebugType=None `
  -p:DebugSymbols=false
~~~

发布 ZIP 文件使用 Switchboard-v{version}-win-x64-Portable.zip 命名格式。请参阅 [CHANGELOG.md](CHANGELOG.md) 了解各版本的变更。

## 项目文档

| 文档 | 用途 |
| --- | --- |
| [CLAUDE.md](CLAUDE.md) | 项目工作规则和标准命令的唯一规范入口 |
| [当前上下文](rules/dev-context.md) · [进度](rules/dev-progress.md) · [路线图](rules/dev-roadmap.md) | 当前切片、验证状态和下一步工作 |
| [架构](rules/dev-arch.md) · [UX 原则](rules/dev-ux.md) · [决策](rules/dev-decisions.md) | 持续适用的实现边界和产品决策 |
| [计划](notes/plans/) · [运行记录](notes/runs/) · [Stitch 参考资料](references/stitch/) | 历史计划和验证依据；不是当前状态的事实来源 |

## 项目结构

- src/Switchboard.App：WPF 外壳、视图、视图模型和用户设置
- src/Switchboard.Core：窗口模型、筛选、排序和布局计算
- src/Switchboard.Native：Win32/DWM 窗口枚举、快捷键和前台激活
- tests/Switchboard.Tests：Core、输入和刷新行为测试

请在 [GitHub Issues](https://github.com/ai-blink/alt-tab/issues) 中报告问题，并附上复现步骤和 Windows 版本。

<div align="center">

<img src="https://raw.githubusercontent.com/LiuYan-xwx/MonitorIsland/refs/heads/master/icon2.png" alt="MonitorIsland Logo" height="100" width="125">

# MonitorIsland

![GitHub License](https://img.shields.io/github/license/LiuYan-xwx/MonitorIsland)
![GitHub top language](https://img.shields.io/github/languages/top/LiuYan-xwx/MonitorIsland)
![GitHub Downloads (all assets, all releases)](https://img.shields.io/github/downloads/LiuYan-xwx/MonitorIsland/total?label=%E6%80%BB%E4%B8%8B%E8%BD%BD%E9%87%8F)
![GitHub Repo stars](https://img.shields.io/github/stars/LiuYan-xwx/MonitorIsland)


</div>

## 简介

这是一个 [ClassIsland](https://github.com/ClassIsland/ClassIsland) 插件，提供了一个 `监控` 组件，可以实时监控和显示系统资源的使用状态。

从 3.0.0 版本开始，MonitorIsland 支持 **Windows、Linux 和 macOS**。

**目前支持的监控项：**

| 监控项 | Windows | Linux | macOS |
| --- | :---: | :---: | :---: |
| 内存使用量 | 支持 | 支持 | 支持 |
| 内存使用率 | 支持 | 支持 | 支持 |
| CPU 使用率 | 支持 | 支持 | 支持 |
| 驱动器剩余空间 | 支持 | 支持 | 支持 |
| ClassIsland 内存使用 | 支持 | 支持 | 支持 |
| 硬件传感器 | 支持 | 不支持 | 不支持 |

> 硬件传感器监控通过 LibreHardwareMonitor 实现，目前仅支持 Windows，可以显示 CPU、GPU、主板等设备提供的温度传感器数据。

## 截图

**主界面**  
<img width="1063" height="73" alt="主界面示例" src="https://github.com/user-attachments/assets/1deb2c70-f7df-40dc-aaca-5af5575f52ae" />

**设置**  
<img width="1313" height="786" alt="设置界面示例" src="https://github.com/user-attachments/assets/6cc137fb-e7eb-4cf8-8063-938c1aedd26a" />

<img width="1222" height="194" alt="image" src="https://github.com/user-attachments/assets/b665b924-2a3c-4c76-9334-45a24f5a8247" />

> 图片仅为示例，实际效果以当前版本为准。

## 安装

- 在插件市场找到 `MonitorIsland` 并安装。
- 或者，您可以前往 [Releases](https://github.com/LiuYan-xwx/MonitorIsland/releases) 找到您想要的版本，下载 `.cipx` 文件手动安装。
- Linux 和 macOS 用户请使用 **3.0.0 或更高版本**。

## 版本说明

| ClassIsland 版本 | MonitorIsland 版本 | 说明 |
| --- | --- | --- |
| ClassIsland 2.0（Avalonia） | 3.x（推荐） | 支持 Windows、Linux 和 macOS |
| ClassIsland 2.0（Avalonia） | 1.8 - 2.x | 仅支持 Windows |
| ClassIsland 1.0（WPF） | 1.7.x | 由 `ci1.0` 分支维护 |

## 使用方法

1. 添加 `监控` 组件到主界面，在组件设置中根据需求自行设置即可。
2. 各设置项的用处应该易懂，这里不再阐述。

## 常见问题

- **Q: 某个数值为 `N/A` ？**

  A: 这表示数据获取失败，可能是权限不足、硬件不支持或系统接口返回异常。请先查看日志，然后你或许可以尝试使用管理员身份运行程序。如果仍无法解决时可以前往 [Issues](https://github.com/LiuYan-xwx/MonitorIsland/issues) 反馈。

- **Q: Linux 或 macOS 中为什么没有硬件传感器选项？**

  A: 硬件传感器目前依赖 LibreHardwareMonitor，仅在 Windows 中提供。CPU、内存、磁盘等基础监控项不受影响。

## 开发

根据 [ClassIsland 开发文档](https://docs.classisland.tech/dev/get-started/devlopment-plugins.html) 配置插件开发环境。  

- `master` 分支面向 ClassIsland 2.0，`ci1.0` 分支面向 ClassIsland 1.0。

- Windows 使用 `PerformanceCounter` 和 Win32 API 获取 CPU、内存数据。
- Linux 读取 `/proc/stat` 和 `/proc/meminfo` 获取 CPU、内存数据。
- macOS 使用 Mach API 和 `sysctl` 获取 CPU、内存数据。
- 驱动器剩余空间使用 .NET `DriveInfo` 获取。
- Windows 硬件传感器通过独立平台后端和 LibreHardwareMonitor 获取。

## 反馈与贡献

- 如果遇到问题、Bug，并且确定是由本插件引起的，或有功能建议，请提交 [Issue](https://github.com/LiuYan-xwx/MonitorIsland/issues)
- 提交 Bug 时请附带详细的日志、报错内容和复现步骤等
- 欢迎各位的 PR 和建议

## 致谢

- 插件图标由 [<img src="https://github.com/LiPolymer.png" width="20" height="20"/>](https://github.com/LiPolymer)[@LiPolymer](https://github.com/LiPolymer) 提供😋😋
- 本项目使用了以下的第三方库：
  - [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor)
  - [ByteSize](https://github.com/omar/ByteSize)
- 开发过程中使用了 Copilot 辅助

## 贡献者

非常感谢以下人员对本仓库做出的贡献：

<a href="https://github.com/LiuYan-xwx/MonitorIsland/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=LiuYan-xwx/MonitorIsland" />
</a>

---

欢迎 star 或提出宝贵意见！

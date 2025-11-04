<div align="center">

<img src="https://github.com/LiuYan-xwx/MonitorIsland/blob/master/icon2.png" alt="MonitorIsland Logo" height="100">

# MonitorIsland

![GitHub License](https://img.shields.io/github/license/LiuYan-xwx/MonitorIsland)
![GitHub top language](https://img.shields.io/github/languages/top/LiuYan-xwx/MonitorIsland)
![GitHub Downloads (all assets, all releases)](https://img.shields.io/github/downloads/LiuYan-xwx/MonitorIsland/total?label=%E6%80%BB%E4%B8%8B%E8%BD%BD%E9%87%8F)
![GitHub Repo stars](https://img.shields.io/github/stars/LiuYan-xwx/MonitorIsland)


</div>

> [!TIP]
> 如果您正在使用 ClassIsland 2.0 预览版本，可以前往 Actions 下载 **ci2.0** 分支的构建产物，其已完成对 CI 2.0 的基本兼容

## 简介

这是一个 [ClassIsland](https://github.com/ClassIsland/ClassIsland) 插件，提供了一个 `监控` 组件，可以实时监控和显示系统各类硬件资源的使用状态。

**目前支持的监控项：**
- `内存使用量`：显示内存占用情况
- `内存使用率`：显示内存当前使用率
- `CPU 利用率`：显示 CPU 当前使用率
- `CPU 温度`：显示指定 CPU 传感器的温度
- `磁盘空间`：显示指定磁盘的剩余空间

*以后会有更多（）*

## 截图

**主界面**  
![主界面示例](https://github.com/user-attachments/assets/d1d6f477-d211-4aa8-8a20-b4c7aee08632)  

**设置**  
![设置示例](https://github.com/user-attachments/assets/f279d0e8-d0a0-40b8-919a-4d0058ded8fa)  

> 图片仅为示例，实际效果以当前版本为准。

## 安装

> [!WARNING]
> ~~由于我也不知道什么原因~~，插件需要 ClassIsland 版本 **≥ 1.6.2.0** 才能运行。

- 在插件市场找到 `MonitorIsland` 并安装。
- 有时插件索引还没更新或者访问不了，可以前往 [Releases](https://github.com/LiuYan-xwx/MonitorIsland/releases) 下载最新 `.cipx` 文件，手动安装。

## 使用方法

1. 确保 ClassIsland 版本符合要求。
2. 添加 `监控` 组件到主界面，在组件设置中根据需求自行设置即可。
3. 各设置项的用处应该易懂，这里不再阐述。

## 常见问题

- **Q: 某个数值为 `N/A` ？**  
  A: 这是数据获取失败了，可能是由于权限不足或者硬件不支持等奇奇怪怪的原因，建议**查看日志**，然后可以前往 [Issues](https://github.com/LiuYan-xwx/MonitorIsland/issues) 反馈。

## 开发

根据 [ClassIsland 开发文档](https://docs.classisland.tech/dev/get-started/devlopment-plugins.html) 配置插件开发环境。  

- `内存使用量`、`内存使用率` 使用了 `PerformanceCounter` 获取可用内存，并通过 `kernel32` API 获取总内存
- `CPU 利用率` 使用了 `PerformanceCounter` 获取
- `磁盘空间` 使用了 `DriveInfo` 获取
- `CPU 温度` 使用了第三方库获取

## 反馈与贡献

- 如果遇到问题、Bug，并且确定是由本插件引起的，或有功能建议，请提交 [Issue](https://github.com/LiuYan-xwx/MonitorIsland/issues)
- 提交 Bug 时请附带详细的日志、报错内容和复现步骤等
- 欢迎各位的 PR 和建议

## 致谢

- 插件图标由 [<img src="https://github.com/LiPolymer.png" width="20" height="20"/>](https://github.com/LiPolymer)[@LiPolymer](https://github.com/LiPolymer) 提供😋😋
- 本项目使用了以下的第三方库：
  - [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) - 用于获取 CPU 温度
- 开发过程中使用了 Copilot 辅助

## 贡献者

非常感谢以下人员对本仓库做出的贡献：

<a href="https://github.com/LiuYan-xwx/MonitorIsland/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=LiuYan-xwx/MonitorIsland" />
</a>

---

欢迎 star 或提出宝贵意见！

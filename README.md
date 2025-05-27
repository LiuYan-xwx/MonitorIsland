<div align="center">

<img src="./icon2.png" alt="MonitorIsland Logo" style="height: 100px;">

# MonitorIsland

</div>

## 简介
这是一个 [ClassIsland](https://github.com/ClassIsland/ClassIsland) 插件，提供了一个`监控`组件，它可以显示您电脑当前的一些信息。  
目前支持的有：  
- 内存使用量
- CPU 利用率
- CPU 温度

## 图片
**主界面**  
![image](https://github.com/user-attachments/assets/d1d6f477-d211-4aa8-8a20-b4c7aee08632)  
**设置**  
![image](https://github.com/user-attachments/assets/f279d0e8-d0a0-40b8-919a-4d0058ded8fa)  

## 安装
>[!warning]
> ~~由于我也不知道什么原因~~，本插件需要 ClassIsland 版本大于或等于 **1.6.2.0** 才能正常运行！

在插件市场找到本插件，安装即可。  
有时插件索引还没来得及更新，您也可以从 [Releases](https://github.com/LiuYan-xwx/MonitorIsland/releases) 中下载 .cipx 文件，再进行手动安装。  

## 使用
**请先确保您的 ClassIsland 版本大于或等于 1.6.2.0**  
本插件有一个叫 `监控` 的**组件**，把它放到主界面上，然后在组件设置中按照您的喜好自行设置就行。  
每个设置项在应用内的描述应该易懂，这里不再阐述。  

## 开发
根据 [ClassIsland 开发文档](https://docs.classisland.tech/dev/get-started/devlopment-plugins.html)配置插件开发环境，使用 [Visual Studio 2022](https://visualstudio.microsoft.com/) 打开即可。  

- 插件中的`内存使用量` 和 `CPU 利用率`我使用了 `PerformanceCounter` 获取  
- `CPU 温度`使用了第三方库获取

## 反馈
如果您在使用本插件的过程中**遇到了问题 / bugs**，并且确定是由本插件引起，可以提交 [Issues](https://github.com/LiuYan-xwx/MonitorIsland/issues) 让我知道，我会尽快回复。  
如果您对本插件有**功能请求**或者其他要求，也可提交 [Issues](https://github.com/LiuYan-xwx/MonitorIsland/issues)。  
- 提交 bug 反馈时您需要附上相关的日志，报错内容，如何复现等
- 提交功能请求时请描述清楚您想要的功能，想要的原因

如果您在使用的过程中发现某个监控项返回的数值为 **-1**，这是刻意为之的，表明获取数据时发生了错误。点开 ClassIsland 的日志应该可以发现相关的报错。

## 致谢
插件图标由 [<img src="https://github.com/LiPolymer.png" width="20" height="20"/>](https://github.com/LiPolymer)[@LiPolymer](https://github.com/LiPolymer) 提供😋😋

本项目使用了以下的第三方库：
- [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) - 用于获取 CPU 温度

开发时使用了 Github Copilot 辅助

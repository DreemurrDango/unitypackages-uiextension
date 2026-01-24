# 更新日志

> 此文件记录了该软件包所有重要的变更
> 文件格式基于 [Keep a Changelog](http://keepachangelog.com/en/1.0.0/) 更新日志规范，且此项目版本号遵循 [语义化版本](http://semver.org/spec/v2.0.0.html) 规范

## [1.3.0] - 2025-10-31
### 更改
- 将视频播放控制相关脚本和预制体统一移动到 `Assets/Plugins/DreemurrStudio/UIExtension/VideoPlayControl` 目录下
- 统一使用命名空间 `DreemurrStudio.UIExtension.VideoPlayControl`，规范化脚本归属

## [1.2.0] - 2025-08-29
### 修复
- 修复了再次播放上次播放的视频时播放失败的 BUG

## [1.1.0] - 2025-06-11
### 新增
- **视频播放进度滑条 (`MediaPlayProgressSlider`)**:
  - 基于 `Slider` 组件实现的视频进度控制 UI，可拖动进度条调整当前播放时间
  - 支持在拖动进度条时自动暂停播放、拖动结束后继续播放
  - 提供格式化的时间显示文本（当前进度/总时长）
- **播放状态切换开关 (`MediaPlayStateSwitchToggle`)**:
  - 基于 `Toggle` 组件实现的一键播放/暂停控制
  - 与视频播放器状态联动，自动同步 UI 状态

### 更改
- 将视频播放控制模块整理为可复用的 UI 扩展组件，并打包到 `UIExtension` 功能包中

## [1.0.0] - 2025-06-11
### 新增
- **初始版本发布**: 提供了一个基础的视频播放控制模板
- 提供完整的 `VideoPlayPanel` 预制体，包含根 UI 面板、视频播放组件与控制器，可直接调整 UI 后使用
- 提供独立的 `VideoController` 控制器预制体，绑定视频播放组件后即可单独复用
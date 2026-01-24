# Unity UI 组件扩展模块 (UI Extension)

## 概述

本模块为 Unity 项目提供了一套常用的 UI 扩展组件，目前包含 **视频播放控制** 功能。它基于现成的视频播放器插件（如 AVPro Video 的 `MediaPlayer`），封装了播放进度控制和播放/暂停切换的 UI 逻辑，使你可以通过拖拽和少量配置快速搭建视频播放面板

## 当前包含的功能

*   **视频播放进度控制 (`MediaPlayProgressSlider`)**
    *   基于 `Slider` 的视频进度条组件
    *   支持拖动进度条调整播放进度
    *   支持在拖动时自动暂停、结束拖动后继续播放
    *   支持在文本中显示“当前进度 / 总时长”的时间信息

*   **播放状态切换开关 (`MediaPlayStateSwitchToggle`)**
    *   基于 `Toggle` 的播放/暂停状态开关
    *   勾选状态表示“暂停中”，未勾选表示“播放中”
    *   与视频播放器事件联动，自动同步 UI 状态（例如响应播放/暂停事件）

> 注：视频播放依赖外部视频播放插件（如 AVPro Video），本模块只负责 UI 控制层

---

## 快速开始

### 1. 环境准备

1. 在项目中导入你所使用的视频播放插件（例如 AVPro Video）
2. 在场景中创建并配置一个 `MediaPlayer`（或对应插件的播放组件），确保其可以正常播放视频

### 2. 使用 `MediaPlayProgressSlider`

`MediaPlayProgressSlider` 用于显示和控制视频播放进度

1. 在 UI 上创建一个 `Slider` 对象
2. 将脚本 `MediaPlayProgressSlider` 挂载到该 `Slider` 上
3. 在 Inspector 中进行如下绑定：
    * `Media Player`: 绑定对应的 `MediaPlayer` 组件
    * `Progress Time Text`: 绑定一个 `TMP_Text` 用于显示时间
    * `Time Format String`: 设置时间显示格式，例如：`{0:D2}:{1:D2} / {2:D2}:{3:D2}`
    * `Pause On Seek`: 勾选后，在拖动进度条时会自动暂停播放，拖动结束后自动继续播放

脚本会自动：
* 在视频打开后获取总时长
* 在每帧更新播放进度和时间显示
* 在拖动进度条时调用 `MediaPlayer.Control.Seek(...)` 调整播放位置

### 3. 使用 `MediaPlayStateSwitchToggle`

`MediaPlayStateSwitchToggle` 用于控制视频播放/暂停状态

1. 在 UI 上创建一个 `Toggle` 对象（可以使用自定义的开关样式）
2. 将脚本 `MediaPlayStateSwitchToggle` 挂载到该 `Toggle` 上
3. 在 Inspector 中将 `Media Player` 字段绑定到对应的 `MediaPlayer` 组件

行为说明：

* 当 `Toggle` 为 **勾选** 状态时：视为暂停，脚本会调用 `mediaPlayer.Pause()`
* 当 `Toggle` 为 **未勾选** 状态时：视为播放，脚本会调用 `mediaPlayer.Play()`
* 同时会监听 `MediaPlayer` 的事件，如：
    * `Unpaused` 时自动将 `Toggle` 设为未勾选
    * `Paused` 时自动将 `Toggle` 设为勾选

这样可以保证 UI 与实际播放状态始终同步

---

## 推荐使用方式

一般情况下，你可以创建一个统一的视频播放面板预制体，例如：

* 根对象：`VideoPlayPanel`
    * 子对象：`MediaPlayer`（视频输出与控制）
    * 子对象：`ProgressSlider`（挂载 `MediaPlayProgressSlider`）
    * 子对象：`PlayToggle`（挂载 `MediaPlayStateSwitchToggle`）
    * 可选子对象：时间显示文本、标题、关闭按钮等

在不同场景中直接实例化 `VideoPlayPanel` 并更换视频源即可复用完整的视频播放控制 UI

--- 

本模块后续可以扩展更多 UI 功能（如循环滚动视图、分页控件等），但当前版本主要聚焦于视频播放控制 UI 的封装
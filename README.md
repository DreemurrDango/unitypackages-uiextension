# Unity UI 组件扩展模块 (UI Extension)

## 概述

本模块为 Unity 项目提供了一套常用的 UI 扩展组件，目前包含 **视频播放控制** 与 **循环滚动视图** 功能。它通过封装常见交互逻辑与 UI 行为，帮助你快速构建可复用的界面组件

## 当前包含的功能

*   **循环滚动视图 (`LoopSelectScrollView`)**
    *   支持循环滚动的选择型 UI 视图，中心项自动放大
    *   支持索引按钮与下方 Toggle 快速跳转
    *   支持自动生成按钮实例或绑定已有 UI
    *   支持拖动滚动与索引跳转的平滑移动动画

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

### 一、循环滚动视图 `LoopSelectScrollView`

`LoopSelectScrollView` 提供循环滚动、中心项放大、索引跳转等常用交互，适合用于角色选择、卡牌轮播、商品展示等场景

#### 1. 场景准备

1. 创建一个 `ScrollRect` 并设置为横向滚动
2. 准备一个用于显示内容按钮的 `HorizontalLayoutGroup`
3. 准备一个用于索引跳转的 `ToggleGroup`
4. 在场景中创建空对象并挂载 `LoopSelectScrollView` 脚本
5. 在 Inspector 中绑定以下字段：
   * `Scroll Rect`
   * `Button Group`
   * `Toggle Group`
   * `Scale Check Area`（用于中心缩放检测）
   * `Button Prefab`、`Toggle Prefab`

#### 2. 运行时初始化

你可以调用 `Init(List<Sprite> sprites, int defaultIndex)` 在运行时初始化

示例逻辑：
- 如果 `Create Instance` 为 `true`，脚本会自动生成按钮与 Toggle
- 如果为 `false`，脚本会绑定现有 UI
- 调用 `MoveToIndex` 可快速跳转

---

### 二、视频播放控制

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

### 视频播放控制预制体结构

* 根对象：`VideoPlayPanel`
    * 子对象：`MediaPlayer`（视频输出与控制）
    * 子对象：`ProgressSlider`（挂载 `MediaPlayProgressSlider`）
    * 子对象：`PlayToggle`（挂载 `MediaPlayStateSwitchToggle`）
    * 可选子对象：时间显示文本、标题、关闭按钮等

### 循环滚动视图结构建议

* 根对象：`LoopSelectScrollView`
    * `ScrollRect`
        * `Content`（绑定为 `Button Group`）
    * `ToggleGroup`（绑定为索引跳转组）

---

本模块后续可以扩展更多 UI 功能，目前版本已包含视频播放控制与循环滚动视图
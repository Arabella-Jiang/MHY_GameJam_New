# CutsceneManager Inspector 配置指南

## 步骤1：创建 CutsceneManager GameObject

1. 在场景中创建一个空的 GameObject，命名为 "CutsceneManager"
2. 添加 `CutsceneManager` 组件

## 步骤2：配置视频文件引用

在 Inspector 中，将 `Assets/过场动画/` 文件夹中的视频文件拖拽到对应字段：

### 第一关过场动画
- **level1_LightConverge** ← `第一关-光芒汇聚到主角身上.mp4`
- **level1_FireIgnite** ← `第一关-生火.mp4`
- **level1_WaterFreeze** ← `第一关-湖水冻结.mp4`

### 第二关过场动画
- **level2_FlowersGrow** ← `第二关-石碑旁边长出花草.mp4`

### 第三关过场动画
- **level3_TreeRevive** ← `第三关-枯树复活.mp4`
- **level3_WaterFlowToTree** ← `第三关-碎裂湖水流到树上.mp4`

## 步骤3：配置 VideoPlayer（如果使用 Prefab）

### 方式A：使用 Prefab（推荐）

1. **创建 VideoPlayer GameObject：**
   - 在 CutsceneManager 下创建子对象，命名为 "VideoPlayer"
   - 添加 `VideoPlayer` 组件

2. **配置 VideoPlayer 组件：**
   - **Render Mode**: `Render Texture`
   - **Target Texture**: 创建一个新的 RenderTexture（1920x1080）
     - 在 Project 窗口右键 → Create → Render Texture
     - 命名为 "CutsceneRenderTexture"
     - 设置尺寸为 1920x1080
   - **Play On Awake**: ❌ 取消勾选
   - **Wait For First Frame**: ✅ 勾选
   - **Loop**: ❌ 取消勾选
   - **Skip On Drop**: ✅ 勾选

3. **在 CutsceneManager 中设置引用：**
   - **videoPlayer** ← 拖拽 VideoPlayer GameObject

### 方式B：让代码自动创建（简单）

- 不设置 `videoPlayer` 字段（留空）
- 代码会在运行时自动创建

## 步骤4：配置视频播放 UI（如果使用 Prefab）

### 方式A：使用 Prefab（推荐）

1. **创建 Canvas：**
   - 在 CutsceneManager 下创建子对象，命名为 "CutsceneCanvas"
   - 添加 `Canvas` 组件
   - **Render Mode**: `Screen Space - Overlay`
   - **Sort Order**: `3000`（确保在最上层）
   - 添加 `Canvas Scaler` 组件：
     - **UI Scale Mode**: `Scale With Screen Size`
     - **Reference Resolution**: `1920 x 1080`
     - **Match**: `0.5`
   - 添加 `Graphic Raycaster` 组件

2. **创建 VideoPanel：**
   - 在 CutsceneCanvas 下创建子对象，命名为 "VideoPanel"
   - 添加 `Image` 组件
   - **Color**: 黑色 (0, 0, 0, 255)
   - RectTransform 设置：
     - **Anchor Min**: (0, 0)
     - **Anchor Max**: (1, 1)
     - **Size Delta**: (0, 0)
     - **Anchored Position**: (0, 0)
   - 初始状态：取消勾选 Active（代码会自动显示）

3. **创建 VideoRawImage：**
   - 在 VideoPanel 下创建子对象，命名为 "VideoRawImage"
   - 添加 `RawImage` 组件
   - **Color**: 白色 (255, 255, 255, 255)
   - RectTransform 设置：
     - **Anchor Min**: (0, 0)
     - **Anchor Max**: (1, 1)
     - **Size Delta**: (0, 0)
     - **Anchored Position**: (0, 0)
   - **Texture**: 留空（代码会自动设置）

4. **在 CutsceneManager 中设置引用：**
   - **videoPanel** ← 拖拽 VideoPanel GameObject
   - **videoRenderTarget** ← 拖拽 VideoRawImage 的 RawImage 组件

### 方式B：让代码自动创建（简单）

- 不设置 `videoPanel` 和 `videoRenderTarget` 字段（留空）
- 代码会在运行时自动创建

## 步骤5：配置其他设置

- **pauseGameDuringCutscene**: ✅ 勾选（播放时暂停游戏）
- **autoHideOnComplete**: ✅ 勾选（播放完成后自动隐藏）

## 完整 Prefab 结构示例

```
CutsceneManager (GameObject + CutsceneManager component)
├── VideoPlayer (GameObject + VideoPlayer component)
│   └── [VideoPlayer 组件配置]
│       - Render Mode: Render Texture
│       - Target Texture: CutsceneRenderTexture
│       - Play On Awake: false
│       - Wait For First Frame: true
│       - Loop: false
└── CutsceneCanvas (GameObject + Canvas component)
    └── VideoPanel (GameObject + Image component)
        └── VideoRawImage (GameObject + RawImage component)
```

## 注意事项

1. **如果使用 Prefab：**
   - 确保所有引用都已正确设置
   - VideoPlayer 的 Target Texture 必须设置
   - VideoRawImage 的 Texture 可以留空（代码会自动设置）

2. **如果让代码自动创建：**
   - 所有字段都可以留空
   - 代码会在运行时自动创建所需组件

3. **单例模式：**
   - CutsceneManager 只需要在 MainMenu 场景中创建一次
   - 使用 `DontDestroyOnLoad`，会在场景切换时保留

4. **测试：**
   - 运行游戏后，查看 Console 中的 `[CutsceneManager]` 日志
   - 如果看到初始化日志，说明配置成功


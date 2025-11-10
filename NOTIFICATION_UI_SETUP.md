# Notification UI 配置指南

本文档说明如何配置游戏中的通知消息显示系统。

## 系统概述

Notification UI系统会自动捕获并显示：
- **ShowMessage()** 调用（来自LevelManager）
- **Debug.Log** 输出（通过DebugLogHandler捕获）

所有消息都会显示在屏幕左下角的Notification Panel中，带有淡入淡出动画效果。

## 配置步骤

### 1. 在Canvas上添加NotificationUIManager组件

1. 在Unity编辑器中打开Scene
2. 在Hierarchy中找到Canvas（路径：`Prefab/UI/Canvas.prefab`）
3. 选中Canvas GameObject
4. 在Inspector中点击"Add Component"
5. 搜索并添加"NotificationUIManager"组件

### 2. 在NotificationPanel下添加TextMeshPro组件

如果NotificationPanel下还没有TextMeshPro组件，需要手动添加：

1. 在Hierarchy中展开Canvas → NotificationPanel
2. 右键点击NotificationPanel → UI → Text - TextMeshPro
3. 将新创建的Text对象重命名为"NotificationText"
4. 配置TextMeshPro组件：
   - **RectTransform**: 
     - Anchor: Stretch (左右上下都拉伸)
     - Left/Right/Top/Bottom: 10 (留出边距)
   - **TextMeshProUGUI**:
     - Font Size: 18
     - Alignment: Center (水平居中) + Middle (垂直居中)
     - Color: 黑色或白色（根据背景调整）
     - Word Wrapping: 启用
     - Overflow: Overflow

### 3. 配置NotificationUIManager组件引用

在Inspector中配置以下字段：

#### UI引用部分
- **Notification Text**: 拖入 `Canvas/NotificationPanel/NotificationText` 的TextMeshProUGUI组件
- **Notification Panel**: 拖入 `Canvas/NotificationPanel` GameObject

#### 字体设置部分
- **Chinese Font Asset**: 拖入支持中文的TextMeshPro字体资源
  - ⚠️ **重要**: 如果显示中文消息，必须指定中文字体资源
  - 参考 `CHINESE_FONT_SETUP.md` 创建中文字体资源
  - 如果留空，中文可能显示为方块

#### 显示设置部分
- **Display Duration**: 3（消息显示持续时间，秒）
- **Fade Duration**: 0.3（淡入淡出时间，秒）
- **Auto Hide**: ✓（自动隐藏消息）
- **Max Queue Length**: 5（最大消息队列长度，0表示无限制）
- **Enable Queue**: ✓（启用消息队列）

### 4. 在Canvas上添加DebugLogHandler组件（可选）

如果需要捕获Debug.Log输出：

1. 选中Canvas GameObject
2. 在Inspector中点击"Add Component"
3. 搜索并添加"DebugLogHandler"组件
4. 配置DebugLogHandler组件：
   - **Enable Log Capture**: ✓
   - **Show Debug Log**: ✓（是否显示Debug.Log）
   - **Show Log Warning**: ✓（是否显示Debug.LogWarning）
   - **Show Log Error**: ✓（是否显示Debug.LogError）
   - **Show UI Prompt Messages**: ✗（不显示包含"UI提示"的消息，避免重复）
   - **Filter Keywords**: 可以添加需要过滤的关键词列表

## 工作流程

### ShowMessage流程
1. **LevelManager.ShowMessage()** 被调用
2. 查找**NotificationUIManager**实例
3. 调用**NotificationUIManager.ShowNotification()**
4. 消息被添加到队列或直接显示
5. 在Notification Panel中显示消息，带有淡入效果
6. 等待显示时间后，淡出并隐藏

### Debug.Log流程
1. **Debug.Log()** 被调用
2. **DebugLogHandler** 捕获日志消息
3. 检查过滤条件（关键词、日志类型等）
4. 清理日志消息（移除堆栈跟踪等）
5. 转发到**NotificationUIManager.ShowNotification()**
6. 显示在Notification Panel中

## 消息队列系统

NotificationUIManager支持消息队列功能：

- **启用队列**：多条消息会依次显示，不会互相覆盖
- **队列长度限制**：可以设置最大队列长度，超出后自动移除最旧的消息
- **消息间隔**：队列中的消息之间有0.2秒的间隔

### 队列示例
```
消息1显示 → 淡入 → 显示3秒 → 淡出 → 等待0.2秒 → 消息2显示 → ...
```

## 自定义设置

### 调整显示时间
在NotificationUIManager组件中修改：
- **Display Duration**: 消息显示持续时间（秒）
- **Fade Duration**: 淡入淡出动画时间（秒）

### 调整消息过滤
在DebugLogHandler组件中：
- 添加**Filter Keywords**列表，包含这些关键词的日志将被忽略
- 调整**Show Debug Log/Warning/Error**开关

### 调整UI样式
在NotificationText的TextMeshProUGUI组件中：
- 修改**Font Size**、**Color**、**Alignment**等
- 调整**RectTransform**的位置和大小

## 测试验证

配置完成后，在Unity编辑器中运行游戏：

1. **测试ShowMessage**：
   - 在代码中调用 `LevelManager.ShowMessage("测试消息")`
   - 观察左下角是否显示通知

2. **测试Debug.Log**：
   - 在代码中调用 `Debug.Log("测试日志")`
   - 观察左下角是否显示通知

3. **测试消息队列**：
   - 快速连续调用多个ShowMessage
   - 观察消息是否依次显示

## 常见问题

### Q: 中文显示为方块？
**A:** 
1. 需要在Unity编辑器中创建支持中文的TextMeshPro字体资源
2. 参考 `CHINESE_FONT_SETUP.md` 文档创建中文字体
3. 在NotificationUIManager组件中指定 `Chinese Font Asset`
4. 或者直接在NotificationText的TextMeshProUGUI组件中设置Font Asset

### Q: 消息不显示？
**A:** 检查以下几点：
1. NotificationUIManager组件是否正确配置了所有引用
2. NotificationText对象是否存在且激活
3. NotificationPanel是否激活
4. Console中是否有错误信息

### Q: 消息显示位置不对？
**A:** 调整NotificationPanel的RectTransform：
- 修改**AnchoredPosition**调整位置
- 修改**SizeDelta**调整大小

### Q: 消息显示时间太短/太长？
**A:** 在NotificationUIManager组件中调整**Display Duration**

### Q: 不想显示某些Debug.Log？
**A:** 在DebugLogHandler组件中：
- 添加过滤关键词
- 或关闭对应的日志类型开关

### Q: 消息重叠显示？
**A:** 确保**Enable Queue**选项已启用

## 相关文件

- **脚本**: 
  - `Assets/Script/core/NotificationUIManager.cs`
  - `Assets/Script/core/DebugLogHandler.cs`
  - `Assets/Script/core/LevelManager.cs`
- **Canvas Prefab**: `Assets/Prefab/UI/Canvas.prefab`
- **TextMeshPro资源**: `Assets/TextMesh Pro/Resources/Fonts & Materials/`

## API使用示例

### 在代码中显示通知

```csharp
// 方法1: 通过LevelManager（推荐）
LevelManager levelManager = FindObjectOfType<LevelManager>();
if (levelManager != null)
{
    levelManager.ShowMessage("这是一条通知消息");
}

// 方法2: 直接使用NotificationUIManager
NotificationUIManager notificationManager = FindObjectOfType<NotificationUIManager>();
if (notificationManager != null)
{
    notificationManager.ShowNotification("这是一条通知消息");
}

// 方法3: 使用Debug.Log（如果启用了DebugLogHandler）
Debug.Log("这条日志会显示在通知面板中");
```

## 更新日志

- 2024-11-09: 创建Notification UI系统
  - 添加NotificationUIManager组件
  - 添加DebugLogHandler组件
  - 修改LevelManager.ShowMessage使用NotificationUIManager
  - 支持消息队列和淡入淡出动画


# 中文字体设置指南

## 问题说明

TextMeshPro默认使用的LiberationSans字体不支持中文字符，导致中文显示为方块。需要创建一个支持中文的TextMeshPro字体资源。

## 解决方案

### 方法1：使用系统字体创建TextMeshPro字体（推荐）

1. **在Unity编辑器中创建字体资源**
   - 打开 `Window` → `TextMeshPro` → `Font Asset Creator`
   - 在 `Source Font File` 中，点击 `Select Font Asset` 按钮
   - 这会打开文件浏览器，需要手动导航到系统字体文件夹：
     - **macOS**: 
       - 系统字体位置：`/Library/Fonts/` 或 `/System/Library/Fonts/`
       - 推荐字体：`PingFangSC-Regular.ttf`（苹方）或 `STHeiti Light.ttc`（黑体）
       - 在文件浏览器中，按 `Cmd + Shift + G` 打开"前往文件夹"对话框
       - 输入 `/Library/Fonts/` 或 `/System/Library/Fonts/`
       - 找到并选择 `PingFangSC-Regular.ttf` 或 `STHeiti Light.ttc`
     - **Windows**: 
       - 系统字体位置：`C:\Windows\Fonts\`
       - 推荐字体：`msyh.ttf`（微软雅黑）或 `simhei.ttf`（黑体）
     - **如果没有系统字体**：可以从网上下载支持中文的字体文件（.ttf格式），放到项目Assets文件夹中

2. **配置字体设置**
   - **Atlas Resolution**: 1024 x 1024（或更高，如2048 x 2048）
   - **Character Set**: 选择 `Unicode Range (Hex)` 或 `Characters from File`
   - **如果选择Unicode Range**：
     - 添加常用中文范围：
       - `4E00-9FFF`（CJK统一汉字）
       - `3000-303F`（CJK符号和标点）
       - `FF00-FFEF`（全角ASCII、全角标点）
   - **如果选择Characters from File**：
     - 创建一个文本文件，包含所有可能用到的中文字符
     - 或者直接使用 `Common Characters` 选项

3. **生成字体资源**
   - 点击 `Generate Font Atlas` 按钮
   - 等待字体图集生成完成
   - 点击 `Save` 或 `Save as...` 按钮
   - 将字体资源保存到 `Assets/TextMesh Pro/Resources/Fonts & Materials/` 文件夹
   - 命名为 `ChineseFont` 或类似名称

4. **应用字体到NotificationText**
   - 在Project窗口中打开 `Assets/Prefab/UI/Canvas.prefab`
   - 展开 Canvas → NotificationPanel → NotificationText
   - 在Inspector中，找到 `TextMeshProUGUI` 组件
   - 将 `Font Asset` 设置为刚创建的中文字体资源

### 方法2：使用Unity内置字体（临时方案）

如果暂时无法创建字体资源，可以使用Unity的旧版Text组件（但不推荐，因为TextMeshPro效果更好）：

1. 将NotificationText的组件从 `TextMeshProUGUI` 改为 `Text`
2. 在 `Text` 组件中设置 `Font` 为系统字体（Arial或中文字体）
3. 注意：需要修改NotificationUIManager脚本以支持Text组件

### 方法3：下载现成的中文字体资源

可以从以下资源获取现成的TextMeshPro中文字体：
- Unity Asset Store搜索 "Chinese Font TextMeshPro"
- GitHub上的开源中文字体资源

## 快速设置步骤（macOS示例）

1. **打开Font Asset Creator**
   ```
   Window → TextMeshPro → Font Asset Creator
   ```

2. **选择系统字体文件**
   - 点击 `Source Font File` 旁边的 `Select Font Asset` 按钮
   - 这会打开文件浏览器
   - 按 `Cmd + Shift + G` 打开"前往文件夹"对话框
   - 输入 `/Library/Fonts/` 并回车
   - 在文件夹中找到以下字体之一：
     - `PingFangSC-Regular.ttf`（苹方-常规）
     - `PingFangSC-Medium.ttf`（苹方-中等）
     - `STHeiti Light.ttc`（华文黑体-细体）
     - `STHeiti Medium.ttc`（华文黑体-中等）
   - 选择其中一个字体文件并点击"打开"
   
   **或者更简单的方法：**
   - 将系统字体复制到项目文件夹：
     1. 打开Finder，按 `Cmd + Shift + G`，输入 `/Library/Fonts/`
     2. 找到 `PingFangSC-Regular.ttf`，复制它
     3. 在Unity的Project窗口中，右键点击 `Assets` 文件夹 → `Create` → `Folder`，命名为 `Fonts`
     4. 将复制的字体文件拖入 `Assets/Fonts/` 文件夹
     5. 在Font Asset Creator中，点击 `Select Font Asset`，选择 `Assets/Fonts/PingFangSC-Regular.ttf`

3. **设置字符集**
   - 在 `Character Set` 下拉菜单中选择 `Unicode Range (Hex)`
   - 在 `Character Sequence` 或 `Unicode Range` 字段中，点击 `+` 按钮添加范围：
     - 第一个范围：`0020-007F` (ASCII基本字符，包括英文和数字)
     - 第二个范围：`4E00-9FFF` (CJK统一汉字，常用汉字)
     - 第三个范围：`3000-303F` (CJK符号和标点)
     - 第四个范围：`FF00-FFEF` (全角ASCII、全角标点)
   - 或者选择 `Characters from File`，创建一个包含常用中文的文本文件

4. **生成字体**
   - Atlas Resolution: `1024 x 1024`
   - 点击 `Generate Font Atlas`
   - 等待生成完成

5. **保存字体资源**
   - 点击 `Save` 按钮
   - 保存到: `Assets/TextMesh Pro/Resources/Fonts & Materials/ChineseFont.asset`

6. **应用到NotificationText**
   - 打开 Canvas.prefab
   - 选择 NotificationText
   - 在Inspector中，将 Font Asset 设置为 ChineseFont

## 验证

设置完成后，运行游戏并显示中文消息，应该能够正常显示中文而不是方块。

## 常见问题

### Q: 某些中文字符显示为方块（如"石"字）？
**A:** 
这是最常见的问题！说明字体资源中没有包含这些字符。解决方法：

1. **方法1：使用更大的字符范围（推荐）**
   - 重新打开 Font Asset Creator
   - 在 Character Set 中选择 `Unicode Range (Hex)`
   - 确保添加了以下范围：
     - `0020-007F` (ASCII基本字符)
     - `4E00-9FFF` (CJK统一汉字 - **这个范围包含了几乎所有常用汉字**)
     - `3000-303F` (CJK符号和标点)
     - `FF00-FFEF` (全角ASCII、全角标点)
   - 重新生成字体图集
   - 如果图集太大，可以增加 Atlas Resolution 到 2048 x 2048 或 4096 x 4096

2. **方法2：使用Characters from File（精确控制）**
   - 在项目中创建一个文本文件（如 `Assets/Fonts/ChineseCharacters.txt`）
   - 将所有游戏中会用到的中文字符都写进这个文件
   - 在 Font Asset Creator 中，选择 `Characters from File`
   - 选择这个文本文件
   - 生成字体图集
   - 这样只会包含文件中出现的字符，文件更小

3. **方法3：检查字符是否在范围内**
   - 字符"石"的Unicode是 `\u77F3`（十六进制：`77F3`）
   - 这个值在 `4E00-9FFF` 范围内，应该被包含
   - 如果仍然显示为方块，可能是：
     - 字体文件本身不包含这个字符
     - 字符范围设置错误
     - 需要重新生成字体图集

### Q: 字体图集太大怎么办？
**A:** 
- 使用 `Characters from File` 方法，只包含游戏中实际用到的字符
- 增加Atlas Resolution（如2048 x 2048或4096 x 4096）
- 使用多个字体资源（分别用于不同用途）
- 或者使用字体子集工具

### Q: 生成的字体文件很大？
**A:** 
- 这是正常的，中文字体包含大量字符
- 可以使用 `Characters from File` 方法，只生成游戏中用到的字符
- 参考项目中的 `Assets/Fonts/ChineseCharacters.txt` 文件

### Q: 可以同时支持中文和英文吗？
**A:** 
- 可以，在Character Set中添加：
  - `0020-007F` (ASCII)
  - `4E00-9FFF` (中文)
  - 或者其他需要的字符范围

### Q: 运行时能否动态切换字体？
**A:** 
- 可以，在代码中修改TextMeshProUGUI的fontAsset属性
- 例如：`notificationText.fontAsset = chineseFontAsset;`

## 推荐设置

对于游戏中的通知消息，建议：
- **字体**: PingFang SC (macOS) 或 Microsoft YaHei (Windows)
- **字符范围**: 
  - `0020-007F` (ASCII基本字符)
  - `4E00-9FFF` (CJK统一汉字)
  - `3000-303F` (CJK符号和标点)
- **Atlas Resolution**: 1024 x 1024 或 2048 x 2048
- **字体大小**: 18-24（根据UI设计调整）

## 相关文件

- **Canvas Prefab**: `Assets/Prefab/UI/Canvas.prefab`
- **NotificationUIManager**: `Assets/Script/core/NotificationUIManager.cs`
- **TextMeshPro资源**: `Assets/TextMesh Pro/Resources/Fonts & Materials/`


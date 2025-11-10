# 修复缺失字符问题

## 问题描述

如果某些中文字符（如"石"字）显示为方块，说明字体资源中没有包含这些字符。

错误信息示例：
```
The character with Unicode value \u77F3 was not found in the [经典宋体简 SDF] font asset
```

## 解决方案

### 方案1：重新生成字体，使用完整的字符范围（推荐）

1. **打开Font Asset Creator**
   - `Window` → `TextMeshPro` → `Font Asset Creator`

2. **选择源字体文件**
   - 在 `Source Font File` 中选择你的中文字体（如"经典宋体简"）

3. **设置字符集为Unicode Range**
   - Character Set: `Unicode Range (Hex)`
   - 添加以下范围（确保全部添加）：
     ```
     0020-007F  (ASCII基本字符，英文和数字)
     4E00-9FFF  (CJK统一汉字 - 包含几乎所有常用汉字，包括"石"字)
     3000-303F  (CJK符号和标点)
     FF00-FFEF  (全角ASCII、全角标点)
     ```
   - **重要**：`4E00-9FFF` 这个范围包含了字符"石"（Unicode: 77F3）

4. **增加Atlas Resolution**
   - 设置为 `2048 x 2048` 或 `4096 x 4096`
   - 因为包含大量字符，需要更大的图集

5. **生成字体图集**
   - 点击 `Generate Font Atlas`
   - 等待生成完成（可能需要一些时间）
   - 检查是否有错误或警告

6. **保存字体资源**
   - 点击 `Save` 或 `Save as...`
   - 覆盖现有的字体资源，或创建新的资源
   - 保存到：`Assets/TextMesh Pro/Resources/Fonts & Materials/`

7. **应用到UI**
   - 确保NotificationText使用了新生成的字体资源
   - 运行游戏测试

### 方案2：使用Characters from File（精确控制）

如果你知道游戏中会用到哪些字符，可以使用这种方法：

1. **创建字符文件**
   - 在项目中创建 `Assets/Fonts/ChineseCharacters.txt`
   - 将所有游戏中会用到的中文字符写入文件
   - 可以参考项目中的 `Assets/Fonts/ChineseCharacters.txt` 文件
   - 也可以直接调用 `GameMessageCatalog.WriteCharactersToFile("Assets/Fonts/ChineseCharacters.txt")` 自动生成所有提示文本所需字符（需在编辑器模式下执行，例如放在菜单或自定义工具中）

2. **在Font Asset Creator中**
   - Character Set: `Characters from File`
   - 选择 `ChineseCharacters.txt` 文件
   - 这样只会生成文件中出现的字符

3. **生成并保存字体资源**

### 方案3：检查字体文件

如果问题仍然存在，可能是字体文件本身不包含某些字符：

1. **尝试不同的字体文件**
   - PingFang SC (macOS)
   - 微软雅黑 (Windows)
   - 思源黑体 (开源，支持完整的中文字符集)

2. **下载支持完整字符集的字体**
   - 推荐：思源黑体（Noto Sans CJK）
   - 下载地址：https://github.com/adobe-fonts/source-han-sans

## 验证字符是否包含

字符"石"的Unicode信息：
- Unicode值：`\u77F3`
- 十六进制：`77F3`
- 应该在范围 `4E00-9FFF` 内

如果设置了 `4E00-9FFF` 范围但仍然显示为方块，可能是：
1. 字体文件本身不包含这个字符
2. 需要尝试不同的字体文件
3. 或者使用支持完整字符集的字体（如思源黑体）

## 快速修复步骤

1. 打开 Font Asset Creator
2. 选择你的中文字体文件
3. Character Set: `Unicode Range (Hex)`
4. 添加范围：`4E00-9FFF`（确保这个范围被添加）
5. Atlas Resolution: `2048 x 2048`
6. 点击 `Generate Font Atlas`
7. 保存字体资源
8. 运行游戏测试

## 推荐设置

- **字体**: 思源黑体（Noto Sans CJK）或 PingFang SC
- **字符范围**: 
  - `0020-007F` (ASCII)
  - `4E00-9FFF` (CJK统一汉字) ← **这个最重要**
  - `3000-303F` (中文标点)
- **Atlas Resolution**: `2048 x 2048` 或 `4096 x 4096`

## 完成！

重新生成字体后，所有中文字符应该都能正常显示了！


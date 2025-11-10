# 中文字体设置 - 简化版指南

## 问题
TextMeshPro显示中文为方块，因为默认字体不支持中文。

## 解决方案（3步）

### 步骤1：复制系统字体到项目（macOS）

1. 打开Finder
2. 按 `Cmd + Shift + G`
3. 输入 `/Library/Fonts/` 并回车
4. 找到 `PingFangSC-Regular.ttf`（如果没有，找 `STHeiti Light.ttc`）
5. 复制这个文件
6. 在Unity的Project窗口中：
   - 右键点击 `Assets` 文件夹
   - 选择 `Create` → `Folder`
   - 命名为 `Fonts`
7. 将复制的字体文件粘贴到 `Assets/Fonts/` 文件夹

### 步骤2：创建TextMeshPro字体资源

1. 在Unity菜单栏：`Window` → `TextMeshPro` → `Font Asset Creator`
2. 在 `Source Font File` 字段：
   - 点击 `Select Font Asset` 按钮
   - 在文件浏览器中找到 `Assets/Fonts/PingFangSC-Regular.ttf`
   - 选择并打开
3. 在 `Character Set` 下拉菜单：
   - 选择 `Unicode Range (Hex)`
4. 在字符范围设置中，添加以下范围（点击 `+` 按钮）：
   - `0020-007F` (英文和数字)
   - `4E00-9FFF` (常用汉字)
   - `3000-303F` (中文标点)
5. 设置 `Atlas Resolution`：
   - 选择 `1024 x 1024` 或 `2048 x 2048`
6. 点击 `Generate Font Atlas` 按钮
7. 等待生成完成（可能需要几秒钟）
8. 点击 `Save` 或 `Save as...` 按钮
9. 保存到：`Assets/TextMesh Pro/Resources/Fonts & Materials/`
10. 命名为：`ChineseFont` 或 `PingFangFont`

### 步骤3：应用到NotificationText

**方法1：在NotificationUIManager中设置（推荐）**
1. 打开 `Assets/Prefab/UI/Canvas.prefab`
2. 选择 `NotificationManager` GameObject
3. 在Inspector中找到 `NotificationUIManager` 组件
4. 找到 `Chinese Font Asset` 字段
5. 将创建的 `ChineseFont` 字体资源拖入该字段
6. 保存prefab

**方法2：直接在NotificationText上设置**
1. 打开 `Assets/Prefab/UI/Canvas.prefab`
2. 展开 `Canvas` → `NotificationPanel` → `NotificationText`
3. 选择 `NotificationText` GameObject
4. 在Inspector中找到 `TextMeshProUGUI` 组件
5. 找到 `Font Asset` 字段
6. 将创建的 `ChineseFont` 字体资源拖入该字段
7. 保存prefab

## 验证

运行游戏，显示中文消息，应该能正常显示中文而不是方块。

## 常见问题

### Q: 找不到PingFangSC-Regular.ttf？
**A:** 
- 尝试找 `STHeiti Light.ttc` 或 `STHeiti Medium.ttc`
- 或者从网上下载思源黑体（Noto Sans CJK）等免费中文字体

### Q: 字体图集生成失败？
**A:** 
- 检查字体文件是否有效
- 尝试降低Atlas Resolution（如512 x 512）
- 减少字符范围（只添加 `4E00-9FFF`）

### Q: 生成的字体文件很大？
**A:** 
- 这是正常的，中文字体包含大量字符
- 可以考虑只生成常用字符（使用 `Characters from File` 选项）

### Q: 可以同时支持中文和英文吗？
**A:** 
- 可以，在字符范围中添加 `0020-007F`（ASCII）即可
- 生成的字体资源会同时支持中英文

## 推荐设置

- **字体**: PingFangSC-Regular.ttf (macOS) 或 Microsoft YaHei (Windows)
- **字符范围**: 
  - `0020-007F` (ASCII)
  - `4E00-9FFF` (常用汉字)
  - `3000-303F` (中文标点)
- **Atlas Resolution**: 1024 x 1024
- **字体大小**: 18-24

## 完成！

设置完成后，中文消息应该能正常显示了！🎉


# Level 2 石碑文字点亮设置指南

## 概述

Level 2 的石碑（StoneTable）需要在充能三个组件（木、羽、日）时点亮对应的文字部分。本指南说明如何在 Unity Editor 中设置 `SpringTabletEffect` 组件。

## 文字结构

根据 `Assets/Prefab/word/level2.prefab` 的结构：

- **木（wood）**：`branch` 父对象，包含多个子对象（木左上、木左下、木右等），每个子对象都有 Renderer 组件
- **羽（feather）**：`羽` 单个对象，有 Renderer 组件
- **日（sun）**：`sun` 父对象，包含多个子对象（日上、日中、日下面等），每个子对象都有 Renderer 组件

## 设置步骤

### 1. 找到 Level 2 场景中的 StoneTable 对象

在 Level 2 场景中找到 StoneTable prefab 实例（或直接编辑 `Assets/Prefab/level2/StoneTable.prefab`）。

### 2. 添加 SpringTabletEffect 组件

如果 StoneTable 对象上还没有 `SpringTabletEffect` 组件，请添加：
- 选中 StoneTable 对象
- 点击 Inspector 面板的 "Add Component"
- 搜索 "SpringTabletEffect" 并添加

### 3. 配置文字渲染器引用

在 `SpringTabletEffect` 组件的 Inspector 中，配置以下字段：

#### 文字渲染器（Text Renderers）

1. **Feather Text Renderer**
   - 展开 StoneTable 对象，找到文字部分的层级结构
   - 找到名为 `羽` 的对象（通常在 `word` 或 `level2` 子对象下）
   - 将 `羽` 对象的 `Renderer` 组件拖拽到 `Feather Text Renderer` 字段

2. **Wood Text Parent**
   - 找到名为 `branch` 的对象（通常在 `word` 或 `level2` 子对象下）
   - 将 `branch` 对象的 `Transform` 组件拖拽到 `Wood Text Parent` 字段
   - **注意**：这里需要的是父对象的 Transform，不是单个 Renderer

3. **Sun Text Parent**
   - 找到名为 `sun` 的对象（通常在 `word` 或 `level2` 子对象下）
   - 将 `sun` 对象的 `Transform` 组件拖拽到 `Sun Text Parent` 字段
   - **注意**：这里需要的是父对象的 Transform，不是单个 Renderer

#### 点亮材质（Lit Material）

1. **Lit Material**
   - 创建一个材质（Material），用于文字点亮后的效果
   - 或者使用已有的点亮材质（应该与 Level 3 使用的点亮材质相同）
   - 将材质拖拽到 `Lit Material` 字段

### 4. 配置 Level2Manager 引用

1. 在 Level 2 场景中找到 `Level2Manager` 对象（或创建它）
2. 在 `Level2Manager` 组件的 Inspector 中，找到 `Tablet Text Effect` 字段
3. 将 StoneTable 对象上的 `SpringTabletEffect` 组件拖拽到该字段

### 5. 验证设置

运行游戏并测试：

1. 充能羽毛组件 → "羽" 文字应该被点亮
2. 充能木组件 → "木" 文字的所有部分应该被点亮
3. 充能日组件 → "日" 文字的所有部分应该被点亮
4. 所有组件充能完成后 → 石碑最终效果应该被触发

## 常见问题

### Q: 文字没有被点亮

**A:** 检查以下几点：
- `Feather Text Renderer` 是否指向正确的 Renderer 组件
- `Wood Text Parent` 和 `Sun Text Parent` 是否指向正确的父对象 Transform
- `Lit Material` 是否已设置
- `Level2Manager` 的 `Tablet Text Effect` 字段是否已设置
- 查看 Console 中的错误信息，通常会有详细的错误提示

### Q: 如何找到文字对象？

**A:** 
1. 在 Hierarchy 面板中展开 StoneTable 对象
2. 查找名为 `word` 或 `level2` 的子对象
3. 在该子对象下应该能找到 `branch`、`羽`、`sun` 等对象
4. 如果找不到，可以查看 `Assets/Prefab/word/level2.prefab` 的结构作为参考

### Q: 木和日的文字部分没有被全部点亮

**A:** 
- 确保 `Wood Text Parent` 和 `Sun Text Parent` 指向的是父对象，而不是单个子对象
- 脚本会自动查找父对象下的所有 Renderer 组件并点亮它们
- 如果某些子对象没有 Renderer 组件，它们不会被点亮（这是正常的）

### Q: 能否使用不同的材质点亮不同的文字？

**A:** 
- 当前实现中，所有文字（木、羽、日）使用同一个 `litMaterial`
- 如果需要不同的材质，需要修改 `SpringTabletEffect.cs` 脚本，添加单独的材质字段

## 代码说明

### SpringTabletEffect 组件

- `LightUpFeatherText()`: 点亮"羽"文字
- `LightUpWoodText()`: 点亮"木"文字（点亮父对象下的所有子对象）
- `LightUpSunText()`: 点亮"日"文字（点亮父对象下的所有子对象）
- `AreAllTextsLit()`: 检查是否所有文字都已点亮

### Level2Manager 调用

- `ChargeFeather()` → 调用 `tabletTextEffect.LightUpFeatherText()`
- `ChargeWood()` → 调用 `tabletTextEffect.LightUpWoodText()`
- `ChargeSun()` → 调用 `tabletTextEffect.LightUpSunText()`

## 参考

- Level 3 的 `StarTabletEffect` 组件实现（`Assets/Script/StarTabletEffect.cs`）
- Level 3 的设置文档（`STONETABLE_COMPONENT_SETUP.md`）


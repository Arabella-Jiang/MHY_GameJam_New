# "星点"组件获取方法配置说明

## 📋 概述

有两种方法可以获取"星点"组件：
1. **方法1**：将石头的"硬"和冰锥的"透"同时赋予雪面，使其呈现镜面质感，从而获得反射星光的能力
2. **方法2**：将【透】赋予大块石头，石头可以映射出星光

两种方法的效果都是：改变材质，并将物体标记为"星点"组件（可拾取）。

## 🔧 配置步骤

### 方法1：雪面镜面效果（SnowPlane）

#### 1. 在 `snowPlane.prefab` 上添加组件

- **添加 `SnowMirrorEffect` 组件**
  - 位置：在 `snowPlane` GameObject 上
  - 脚本路径：`Assets/Script/SnowMirrorEffect.cs`

#### 2. 配置 `SnowMirrorEffect` 组件

- **Mirror Material**（可选）：
  - 拖拽镜面材质到这个字段
  - 如果不设置，会使用代码创建的默认镜面材质（淡蓝色，高反光）

- **Star Prefab**（必需）：
  - 拖拽 `gold star.prefab` 到这个字段
  - 路径：`Assets/Prefab/level3/gold star.prefab`

- **Star Spawn Height**（可选）：
  - 星星生成高度（相对于物体表面）
  - 默认值：0.5

#### 3. 配置 `InteractableObject` 组件的属性组合

在 `snowPlane` 的 `InteractableObject` 组件中：

- **Property Combinations**：
  - 点击 `+` 添加一个新的组合
  - **Required Properties**：
    - 添加 `Hard`（硬）
    - 添加 `Transparent`（透）
  - **Effect Component Name**：
    - 输入：`SnowMirrorEffect`

#### 4. 配置 `InteractableObject` 组件的可理解属性

- **Understandable Properties**：
  - 确保包含 `Cool`、`White`（雪面的基础属性）
  - 这些属性可以被玩家理解

---

### 方法2：大石头透明效果（BigStone）

#### 1. 在 `BigStone.prefab` 上添加组件

- **添加 `StoneTransparentEffect` 组件**
  - 位置：在 `BigStone` GameObject 上
  - 脚本路径：`Assets/Script/StoneTransparentEffect.cs`

#### 2. 配置 `StoneTransparentEffect` 组件

- **Transparent Material**（可选）：
  - 拖拽透明/反射材质到这个字段
  - 如果不设置，会使用代码创建的默认透明材质（淡蓝色，半透明，高反光）

- **Transparency**（可选）：
  - 透明度值（0-1），仅在创建默认材质时使用
  - 默认值：0.5

- **Star Prefab**（必需）：
  - 拖拽 `gold star.prefab` 到这个字段
  - 路径：`Assets/Prefab/level3/gold star.prefab`

- **Star Spawn Height**（可选）：
  - 星星生成高度（相对于物体表面）
  - 默认值：1.0

#### 3. 配置 `InteractableObject` 组件的属性组合

在 `BigStone` 的 `InteractableObject` 组件中：

- **Property Combinations**：
  - 点击 `+` 添加一个新的组合
  - **Required Properties**：
    - 添加 `Transparent`（透）
  - **Effect Component Name**：
    - 输入：`StoneTransparentEffect`

#### 4. 配置 `InteractableObject` 组件的可理解属性

- **Understandable Properties**：
  - 确保包含 `Heavy`、`Hard`（石头的基础属性）
  - 这些属性可以被玩家理解

---

## ✅ 工作流程

### 方法1：雪面镜面效果

1. 玩家理解石头，获得 `Hard` 属性
2. 玩家理解冰锥，获得 `Transparent` 属性
3. 玩家对雪面应用 `Hard` 属性（短按E）
4. 玩家对雪面应用 `Transparent` 属性（短按E）
5. 当雪面同时拥有 `Hard` 和 `Transparent` 属性时：
   - `SnowMirrorEffect.TriggerEffect()` 被触发
   - 雪面材质变为镜面材质
   - **在雪面上生成星星prefab**（`gold star.prefab`）
   - 星星prefab自动添加 `StarPointCollector` 组件
   - 星星prefab可以被拾取（`canBePickedUp = true`）
6. 玩家拾取星星（F键），获得"星点"组件

### 方法2：大石头透明效果

1. 玩家理解冰锥，获得 `Transparent` 属性
2. 玩家对大石头应用 `Transparent` 属性（短按E）
3. 当大石头拥有 `Transparent` 属性时：
   - `StoneTransparentEffect.TriggerEffect()` 被触发
   - 石头材质变为透明/反射材质
   - **在石头上生成星星prefab**（`gold star.prefab`）
   - 星星prefab自动添加 `StarPointCollector` 组件
   - 星星prefab可以被拾取（`canBePickedUp = true`）
4. 玩家拾取星星（F键），获得"星点"组件

---

## 📝 检查清单

### SnowPlane Prefab

- [ ] 已添加 `SnowMirrorEffect` 组件
- [ ] `SnowMirrorEffect` 的 `mirrorMaterial` 已配置（可选）
- [ ] `SnowMirrorEffect` 的 `starPrefab` 已配置（必需）：指向 `gold star.prefab`
- [ ] `SnowMirrorEffect` 的 `starSpawnHeight` 已设置（默认0.5，可选）
- [ ] `InteractableObject` 的 `propertyCombinations` 已配置：
  - [ ] Required Properties: `Hard` + `Transparent`
  - [ ] Effect Component Name: `SnowMirrorEffect`
- [ ] `InteractableObject` 的 `understandableProperties` 包含 `Cool`、`White`
- [ ] `InteractableObject` 的 `canBePickedUp` 为 `false`（雪面本身不能被拾取）

### BigStone Prefab

- [ ] 已添加 `StoneTransparentEffect` 组件
- [ ] `StoneTransparentEffect` 的 `transparentMaterial` 已配置（可选）
- [ ] `StoneTransparentEffect` 的 `transparency` 已设置（默认0.5）
- [ ] `StoneTransparentEffect` 的 `starPrefab` 已配置（必需）：指向 `gold star.prefab`
- [ ] `StoneTransparentEffect` 的 `starSpawnHeight` 已设置（默认1.0，可选）
- [ ] `InteractableObject` 的 `propertyCombinations` 已配置：
  - [ ] Required Properties: `Transparent`
  - [ ] Effect Component Name: `StoneTransparentEffect`
- [ ] `InteractableObject` 的 `understandableProperties` 包含 `Heavy`、`Hard`
- [ ] `InteractableObject` 的 `canBePickedUp` 为 `false`（大石头本身不能被拾取）

---

## 🎯 效果说明

### SnowMirrorEffect

- **触发条件**：雪面同时拥有 `Hard` 和 `Transparent` 属性
- **效果**：
  - 改变材质为镜面材质（高反光，淡蓝色）
  - **在雪面上生成星星prefab**（`gold star.prefab`）
  - 星星prefab自动添加 `StarPointCollector` 组件
  - 星星prefab设置为可拾取（`canBePickedUp = true`）
- **结果**：雪面变成镜面，可以反射星光，星星出现在雪面上，玩家可以拾取星星获得"星点"组件

### StoneTransparentEffect

- **触发条件**：大石头拥有 `Transparent` 属性
- **效果**：
  - 改变材质为透明/反射材质（半透明，高反光，淡蓝色）
  - **在石头上生成星星prefab**（`gold star.prefab`）
  - 星星prefab自动添加 `StarPointCollector` 组件
  - 星星prefab设置为可拾取（`canBePickedUp = true`）
- **结果**：石头变得透明，可以映射星光，星星出现在石头上，玩家可以拾取星星获得"星点"组件

---

## 🔍 调试提示

如果效果没有触发，请检查：

1. **属性组合配置**：
   - 确保 `InteractableObject` 的 `propertyCombinations` 中正确配置了所需的属性
   - 确保 `Effect Component Name` 与脚本类名完全匹配（区分大小写）

2. **组件添加**：
   - 确保 `SnowMirrorEffect` 或 `StoneTransparentEffect` 组件已添加到 GameObject 上
   - 确保组件已启用（Enabled）

3. **属性应用**：
   - 确保玩家已理解所需的属性（通过理解物体获得）
   - 确保玩家对目标物体应用了所有必需的属性

4. **日志输出**：
   - 查看 Console 中的日志，脚本会输出详细的调试信息
   - 如果看到警告或错误，按照日志提示修复

---

## 📌 注意事项

1. **材质配置**：
   - 如果不在 Inspector 中配置材质，脚本会创建默认材质
   - 默认材质使用 Standard Shader，可能不是最佳的视觉效果
   - 建议在 Unity Editor 中创建专门的镜面/透明材质，并在 Inspector 中配置

2. **星星生成**：
   - 效果触发后，脚本会在物体表面上方生成星星prefab
   - 生成位置基于物体的 `Renderer.bounds` 计算
   - 可以通过 `starSpawnHeight` 调整生成高度
   - 星星prefab会自动添加 `StarPointCollector` 和 `InteractableObject` 组件

3. **StarPointCollector**：
   - 生成的星星prefab会自动添加 `StarPointCollector` 组件
   - 玩家拾取星星时，`StarPointCollector` 会自动通知 `Level3Manager`
   - 不需要手动配置 `StarPointCollector` 的引用

4. **重要提示**：
   - **雪面和大石头本身不能被拾取**（`canBePickedUp = false`）
   - 只有生成的星星prefab可以被拾取
   - 星星prefab会出现在镜面/透明物体的表面上


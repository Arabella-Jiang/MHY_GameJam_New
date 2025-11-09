# Level 2 木组件获取方式设置指南

## 概述

Level 2 中获取"木"组件有两种路线：

**路线1（Soft特性）**：
- 玩家吸收羽毛的特性：Soft（软）
- 赋予 woodoriginal
- woodoriginal 有 Soft 特性，按 Q 可以被折断
- 掉落 woodpart
- 拿着 woodpart 到石碑充能

**路线2（Thin特性）**：
- 玩家有特性：Thin（细）
- 赋予 woodoriginal
- woodoriginal 变 inactive
- woodthin 变 active
- 玩家拾取 woodthin
- 拿着 woodthin 到石碑充能

## Unity Editor 配置

### 1. Wood Prefab（woodoriginal）配置

在 Wood.prefab 的 `InteractableObject` 组件中：

#### 1.1 添加组件
- 添加 `WoodSoftEffect` 组件
- 添加 `WoodThinEffect` 组件

#### 1.2 配置 WoodSoftEffect
- **woodPartPrefab**: 指定 `WoodPart.prefab`
- **woodPartSpawnPoint**: （可选）指定掉落位置
- **woodPartSpawnOffset**: 生成偏移（建议：Vector3.down）
- **breakEffect**: （可选）折断特效
- **breakSound**: （可选）折断音效

#### 1.3 配置 WoodThinEffect
- **woodThinObject**: 指定 WoodThin GameObject（可以是场景中的对象或子对象）
- **transformEffect**: （可选）变形特效
- **transformSound**: （可选）变形音效

#### 1.4 配置 propertyCombinations
```yaml
propertyCombinations:
  - requiredProperties:
      - Soft
    effectComponentName: WoodSoftEffect
  - requiredProperties:
      - Thin
    effectComponentName: WoodThinEffect
```

#### 1.5 初始设置
- **canBePickedUp**: false（原始木头不可拾取）

### 2. WoodPart Prefab 配置

在 WoodPart.prefab 的 `InteractableObject` 组件中：
- **canBePickedUp**: true
- 确保有 Collider（用于玩家检测）
- 确保有 Rigidbody（设置为 kinematic）

### 3. WoodThin GameObject/Prefab 配置

WoodThin 可以作为：
1. Wood.prefab 的子对象（初始设为 inactive）
2. 或者场景中的独立对象（初始设为 inactive）

在 WoodThin 的 `InteractableObject` 组件中：
- **canBePickedUp**: true
- 确保有 Collider（用于玩家检测）
- 确保有 Rigidbody（设置为 kinematic）
- **初始状态**: Inactive

### 4. 羽毛配置（提供Soft特性）

羽毛需要提供 Soft 特性给玩家。在羽毛的 `InteractableObject` 组件中：
- **understandableProperties**: 添加 `Soft`

## 工作流程

### 路线1：Soft特性
1. 玩家拾取鸟巢，获得羽毛
2. 玩家长按E对着羽毛，理解 Soft 特性
3. 玩家按数字键选择 Soft 特性（1/2/3）
4. 玩家对着 Wood（woodoriginal）短按E，赋予 Soft 特性
5. Wood 触发 `WoodSoftEffect`，变软了，现在可以被折断
6. 玩家对着 Wood 按 Q 键（空手状态），折断木头
7. 掉落 WoodPart（Wood 本体还在，可以继续折断）
8. 玩家拾取 WoodPart（F键）
9. 手持 WoodPart 到石碑，按 Q 键充能"木"文字

### 路线2：Thin特性
1. 玩家从老藤理解 Thin 特性
2. 玩家按数字键选择 Thin 特性（1/2/3）
3. 玩家对着 Wood（woodoriginal）短按E，赋予 Thin 特性
4. Wood 触发 `WoodThinEffect`
5. Wood（woodoriginal）变 inactive
6. WoodThin 变 active
7. 玩家拾取 WoodThin（F键）
8. 手持 WoodThin 到石碑，按 Q 键充能"木"文字

## 代码逻辑说明

### WoodSoftEffect
- `TriggerEffect()`: 当 Wood 获得 Soft 特性时调用，设置 `canBreak = true`
- `TryBreak()`: 由 Level2Manager 调用，尝试折断木头
- `BreakWood()`: 折断木头，生成 WoodPart（Wood 本体保持存在，可以多次折断）

### Level2Manager.HandlePlayerUse
- 检测玩家是否对着有 `WoodSoftEffect` 的木头按 Q 键
- 如果是，调用 `WoodSoftEffect.TryBreak()`
- 处理石碑充能逻辑

### WoodThinEffect
- `TriggerEffect()`: 当 Wood 获得 Thin 特性时调用
  - woodoriginal 变 inactive
  - woodthin 变 active
  - 设置 woodthin 可拾取

### Level2Manager
- `HandlePlayerPickup()`: 检测拾取的是否是 WoodPart 或 WoodThin（通过名称判断）
- `TryChargeComponent()`: 检测手持的是否是 WoodPart 或 WoodThin（通过名称判断）

## 注意事项

1. **羽毛特性**: 确保羽毛的 `understandableProperties` 包含 `Soft` 特性
2. **Wood 初始状态**: Wood（woodoriginal）初始不可拾取（`canBePickedUp = false`）
3. **WoodThin 初始状态**: WoodThin 初始设为 inactive
4. **折断条件**: 只有在 Wood 获得 Soft 特性后，才能按 Q 键折断
5. **特性理解**: 玩家需要先理解特性，然后选择特性，再赋予木头
6. **充能条件**: 只有手持 WoodPart 或 WoodThin 才能充能

## 测试 Checklist

- [ ] 路线1：从羽毛理解 Soft 特性
- [ ] 路线1：对 Wood 使用 Soft 特性
- [ ] 路线1：按 Q 键折断 Wood，生成 WoodPart
- [ ] 路线1：拾取 WoodPart，充能到石碑
- [ ] 路线2：从老藤理解 Thin 特性
- [ ] 路线2：对 Wood 使用 Thin 特性
- [ ] 路线2：Wood 变 inactive，WoodThin 变 active
- [ ] 路线2：拾取 WoodThin，充能到石碑
- [ ] 石碑上的"木"文字被点亮


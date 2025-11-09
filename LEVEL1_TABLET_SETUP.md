# Level 1 石碑配置指南

## 概述

Level 1 的石碑需要点亮两个文字部分：

1. **Fire 文字**：手持点燃的树枝走到石碑前按 Q
2. **Human 文字**：空手走到石碑前按 Q

两个文字都点亮后，教程完成，跳转到 Level 2。

---

## Unity Editor 配置

### 1. StoneTable Prefab 配置

在 `Assets/Prefab/level1/StoneTable.prefab` 上：

#### StoneTableEffect 组件（应该已有）

**新增字段**：

**Fire 文字部分**：
- `fireTextRenderer`: 拖入 Fire 文字的 Renderer（单个 Renderer）

**Human 文字部分**：
- `humanTextRenderer`: 拖入 Human 文字的 Renderer（单个 Renderer）

**点亮材质**：
- `litMaterial`: 指定点亮后的材质（Fire 和 Human 共用）

**旧版字段（保留兼容性）**：
- `wordRenderer`: 可保留不填
- `litWordMaterial`: 可保留不填

---

### 2. TutorialManager 配置

在场景中的 TutorialManager 对象上：

#### 字段配置
- `player`: 拖入 Player
- `empowerment`: 拖入 Player 的 EmpowermentAbility（或留空自动查找）
- `rock`: 拖入石头（用于理解 Hard 特性）
- `thinBranch`: 拖入细树枝
- `thickBranch`: 拖入粗树枝
- `waterHard`: 拖入水面的 WaterHardEffect
- `stoneTablet`: 拖入石碑
- `tabletTextEffect`: 拖入石碑的 StoneTableEffect 组件（或留空自动查找）

---

## 游戏流程

### 教程步骤
1. 靠近石头，长按 E 理解 Hard 特性
2. 对水面短按 E，赋予 Hard 特性 → 水硬化
3. 对两根树枝分别短按 E，赋予 Hard 特性 → 树枝变硬
4. 空手对着任意树枝按 Q → 摩擦生火，细树枝被点燃
5. **充能石碑**（两种方式，顺序任意）：
   - 拾起点燃的树枝（F键），走到石碑按 Q → 点亮 Fire 文字
   - 空手走到石碑按 Q → 点亮 Human 文字
6. 两个文字都点亮 → 教程完成 → 跳转 Level 2

---

## 配置检查清单

### StoneTable Prefab
- [ ] 有 StoneTableEffect 组件
- [ ] StoneTableEffect.fireTextRenderer 已指定（Fire 文字的 Renderer）
- [ ] StoneTableEffect.humanTextRenderer 已指定（Human 文字的 Renderer）
- [ ] StoneTableEffect.litMaterial 已指定（点亮后的材质）

### TutorialManager
- [ ] player 已指定
- [ ] rock 已指定
- [ ] thinBranch 已指定
- [ ] thickBranch 已指定
- [ ] waterHard 已指定
- [ ] stoneTablet 已指定
- [ ] tabletTextEffect 已指定（或留空自动查找）

---

## 测试步骤

### Fire 文字测试
1. 运行游戏，完成前面的教程步骤
2. 点燃细树枝后，按 F 拾起树枝
3. 走到石碑附近（距离 < 5米）
4. 按 Q 键
5. Console 应显示："✅ Fire文字已点亮！"
6. Fire 文字材质应变为 litMaterial

### Human 文字测试
1. 运行游戏，完成前面的教程步骤
2. 确保空手（按 F 放下物品，或不拾取任何东西）
3. 走到石碑附近（距离 < 5米）
4. 按 Q 键
5. Console 应显示："✅ Human文字已点亮！"
6. Human 文字材质应变为 litMaterial

### 通关测试
1. 点亮两个文字（顺序任意）
2. Console 应显示："✅ 石碑两个文字部分都已点亮！教程完成！"
3. 1秒后应跳转到 Level 2

---

## 常见配置错误

### ❌ 错误1：fireTextRenderer 或 humanTextRenderer 未指定
- **现象**：Console 显示错误 "fireTextRenderer未指定"
- **解决**：在 StoneTableEffect 中拖入对应的 Renderer

### ❌ 错误2：litMaterial 未指定
- **现象**：Console 显示错误 "litMaterial未指定"
- **解决**：在 StoneTableEffect 中指定点亮后的材质

### ❌ 错误3：按Q键没反应
- **现象**：按 Q 键没有点亮文字
- **解决**：
  - 检查是否在石碑附近（距离 < 5米）
  - 检查 player.currentInteractTarget 是否是石碑
  - 检查 Console 是否有错误日志

### ❌ 错误4：手持树枝但提示需要点燃
- **现象**：手持树枝按 Q，提示"树枝还没有点燃"
- **解决**：确保细树枝已通过摩擦点火（空手按Q对着树枝）

---

## 调试技巧

### Console 日志
- "✅ Fire文字已点亮！" → Fire 充能成功
- "✅ Human文字已点亮！" → Human 充能成功
- "✅ 石碑两个文字部分都已点亮！" → 教程完成
- "Fire文字已经点亮过了" → 重复充能
- 红色错误 → 检查组件配置

### Inspector 实时查看
- 运行时查看 TutorialManager.fireCharged 和 humanCharged
- 应该看到充能后变为 true

### 调试步骤
1. 如果 Fire 文字没有点亮，检查：
   - 是否手持 thinBranch
   - thinBranch.IsIgnited() 是否为 true
   - tabletTextEffect.fireTextRenderer 是否已指定

2. 如果 Human 文字没有点亮，检查：
   - player.playerHoldItem.heldObject 是否为 null（空手）
   - tabletTextEffect.humanTextRenderer 是否已指定

---

## 与 Level 2 和 Level 3 的对比

### 相同点
- 都是石碑点亮文字的逻辑
- 都通过 LevelManager/TutorialManager 处理充能
- 都有专门的 TabletEffect 脚本处理文字点亮

### 不同点
| Level | 文字部分 | 充能条件 | TabletEffect 脚本 |
|-------|---------|---------|------------------|
| Level 1 | Fire, Human | 手持点燃树枝/空手 | StoneTableEffect |
| Level 2 | 木, 羽, 日 | 手持组件/空手有光源 | SpringTabletEffect |
| Level 3 | Life, Star Point | 手持组件 | StarTabletEffect |

---

## 脚本说明

- `StoneTableEffect.cs`: 处理 Fire 和 Human 文字的点亮（新增 `LightUpFireText()` 和 `LightUpHumanText()` 方法）
- `TutorialManager.cs`: 管理教程流程，处理石碑充能逻辑

参考 `SpringTabletEffect.cs` 和 `StarTabletEffect.cs` 的实现。

## 修改内容

在现有的 `StoneTableEffect.cs` 中新增：
- `fireTextRenderer` 和 `humanTextRenderer` 字段
- `litMaterial` 字段（用于两个文字共用的点亮材质）
- `LightUpFireText()` 方法（点亮 Fire 文字）
- `LightUpHumanText()` 方法（点亮 Human 文字）
- `AreAllTextsLit()` 方法（检查是否全部点亮）

旧版字段（`wordRenderer`, `litWordMaterial`）保留以兼容现有配置。


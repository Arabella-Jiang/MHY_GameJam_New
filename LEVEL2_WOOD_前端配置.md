# Level 2 木组件 - Unity Editor 前端配置步骤

## 一、Wood.prefab（原始木头）配置

### 1. 添加组件
在 Wood.prefab 上添加以下组件：
- `InteractableObject`（应该已有）
- `WoodSoftEffect`（新增）
- `WoodThinEffect`（新增）

### 2. InteractableObject 配置

**基础设置**：
- `canBePickedUp`: ❌ **false**（原始木头不可拾取）

**propertyCombinations（特性组合）**：
点击 + 号添加 2 个元素：

**元素 [0] - Soft 路线**：
- `requiredProperties`: 
  - Size: 1
  - Element 0: `Soft`
- `effectComponentName`: `WoodSoftEffect`

**元素 [1] - Thin 路线**：
- `requiredProperties`:
  - Size: 1
  - Element 0: `Thin`
- `effectComponentName`: `WoodThinEffect`

### 3. WoodSoftEffect 配置

**必填字段**：
- `woodPartPrefab`: 拖入 **WoodPart.prefab**

**可选字段**：
- `woodPartSpawnPoint`: 不填（会使用木头位置）
- `woodPartSpawnOffset`: (0, -1, 0) 建议向下偏移
- `breakEffect`: 可选的折断粒子特效
- `breakSound`: 可选的折断音效

### 4. WoodThinEffect 配置

**必填字段**：
- `woodThinObject`: 
  - **方案A**：如果 WoodThin 是 Wood 的子对象，拖入该子对象
  - **方案B**：如果 WoodThin 是场景独立对象，拖入场景中的 WoodThin

**可选字段**：
- `transformEffect`: 可选的变形粒子特效
- `transformSound`: 可选的变形音效

---

## 二、WoodPart.prefab（折断后的部件）配置

### InteractableObject 配置
- `canBePickedUp`: ✅ **true**

### 物理组件（必须）
- **Collider**：确保有 BoxCollider 或其他 Collider
- **Rigidbody**：
  - `isKinematic`: ✅ true
  - `useGravity`: ❌ false

---

## 三、WoodThin（变细后的木头）配置

### 位置方案

**推荐方案A：作为 Wood 的子对象**
1. 在 Wood.prefab 下创建子对象 WoodThin
2. 设置 WoodThin 为 **Inactive**（默认隐藏）

**方案B：场景独立对象**
1. 在场景中放置 WoodThin GameObject
2. 设置 WoodThin 为 **Inactive**（默认隐藏）

### InteractableObject 配置
- `canBePickedUp`: ✅ **true**

### 物理组件（必须）
- **Collider**：确保有 BoxCollider 或其他 Collider
- **Rigidbody**：
  - `isKinematic`: ✅ true
  - `useGravity`: ❌ false

### ⚠️ 重要提醒
- **初始状态必须是 Inactive**（在 Inspector 顶部取消勾选 Active）

---

## 四、羽毛（Feather/BirdNest）配置

### InteractableObject 配置

**understandableProperties（可理解特性）**：
点击 + 号添加：
- Element 0: `Soft`

这样玩家才能从羽毛理解 Soft 特性。

---

## 五、Level2Manager 配置

在场景中的 Level2Manager 对象上：

### 字段配置
- `stoneTablet`: 拖入石碑
- `oldVine`: 拖入老藤
- `birdNest`: 拖入鸟巢
- `tabletTextEffect`: 拖入石碑上的 SpringTabletEffect 组件

---

## 六、配置检查清单

### Wood.prefab
- [ ] 有 InteractableObject 组件
- [ ] canBePickedUp = false
- [ ] propertyCombinations 有 2 个元素（Soft, Thin）
- [ ] 有 WoodSoftEffect 组件
- [ ] WoodSoftEffect.woodPartPrefab 已指定
- [ ] 有 WoodThinEffect 组件
- [ ] WoodThinEffect.woodThinObject 已指定

### WoodPart.prefab
- [ ] 有 InteractableObject 组件
- [ ] canBePickedUp = true
- [ ] 有 Collider
- [ ] 有 Rigidbody（isKinematic = true）

### WoodThin
- [ ] **初始状态 Inactive**
- [ ] 有 InteractableObject 组件
- [ ] canBePickedUp = true
- [ ] 有 Collider
- [ ] 有 Rigidbody（isKinematic = true）

### 羽毛
- [ ] understandableProperties 包含 Soft

---

## 七、测试步骤

### Soft 路线测试
1. 运行游戏
2. 拾取羽毛，长按 E 理解 Soft 特性
3. 按数字键选择 Soft
4. 对着 Wood 短按 E，赋予 Soft 特性
5. Console 应显示："✅ 木头变软了！现在可以按Q键折断它。"
6. 按 0 切换到空手
7. 对着 Wood 按 Q
8. 应该掉落 WoodPart
9. Wood 本体还在（可以继续按 Q 折断）

### Thin 路线测试
1. 运行游戏
2. 从老藤理解 Thin 特性
3. 按数字键选择 Thin
4. 对着 Wood 短按 E，赋予 Thin 特性
5. Wood 应该消失（inactive）
6. WoodThin 应该出现（active）
7. 按 F 拾取 WoodThin

---

## 八、常见配置错误

### ❌ 错误1：WoodThin 初始是 Active
- **现象**：游戏开始就能看到两个木头
- **解决**：确保 WoodThin 初始设为 Inactive

### ❌ 错误2：woodPartPrefab 未指定
- **现象**：按 Q 没有掉落 WoodPart，Console 有错误
- **解决**：在 WoodSoftEffect 中拖入 WoodPart.prefab

### ❌ 错误3：woodThinObject 未指定
- **现象**：使用 Thin 特性后什么都没发生，Console 有错误
- **解决**：在 WoodThinEffect 中拖入 WoodThin GameObject

### ❌ 错误4：WoodPart/WoodThin 没有 Collider
- **现象**：拾取提示不出现
- **解决**：添加 BoxCollider 或其他 Collider

### ❌ 错误5：羽毛没有 Soft 特性
- **现象**：无法从羽毛理解 Soft 特性
- **解决**：在羽毛的 understandableProperties 中添加 Soft

---

## 九、调试技巧

### 查看 Console 日志
- "✅ 木头变软了！" → Soft 特性赋予成功
- "✅ 木头被折断了！" → 折断成功
- "✅ 木头变细了！" → Thin 特性赋予成功
- 红色错误 → 检查字段是否配置正确

### Inspector 实时查看
- 运行时查看 Wood 的 InteractableObject.currentProperties
- 应该能看到 Soft 或 Thin 特性被添加

### Hierarchy 查看
- Thin 路线触发后，Wood 应该显示为灰色（inactive）
- WoodThin 应该变为白色（active）

---

## 十、推荐配置顺序

1. **先配置 WoodPart.prefab**（最简单）
2. **配置 WoodThin**（设为 Inactive）
3. **配置 Wood.prefab**（添加组件，设置字段）
4. **配置羽毛**（添加 Soft 特性）
5. **测试 Soft 路线**
6. **测试 Thin 路线**

配置完成后，两条路线都应该能正常工作！


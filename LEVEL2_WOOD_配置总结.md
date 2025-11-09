# Level 2 木组件配置总结

## 快速配置清单

### 1. Wood Prefab（woodoriginal）

**添加组件**：
- `InteractableObject`（应该已有）
- `WoodSoftEffect`
- `WoodThinEffect`

**InteractableObject 配置**：
```
canBePickedUp: false（原始木头不可拾取）

propertyCombinations (2个):
  [0]:
    requiredProperties: Soft
    effectComponentName: WoodSoftEffect
  [1]:
    requiredProperties: Thin
    effectComponentName: WoodThinEffect
```

**WoodSoftEffect 配置**：
- `woodPartPrefab`: 拖入 `WoodPart.prefab`
- `woodPartSpawnOffset`: (0, -1, 0) 或根据需要调整
- 其他可选

**WoodThinEffect 配置**：
- `woodThinObject`: 拖入场景中的 WoodThin GameObject（或 Wood 的子对象）
- 其他可选

---

### 2. WoodPart Prefab

**配置**：
- 确保有 `InteractableObject` 组件
- `canBePickedUp: true`
- 确保有 `Collider`（用于玩家检测）
- 确保有 `Rigidbody`（isKinematic = true）

---

### 3. WoodThin GameObject

**位置**：可以是：
- Wood Prefab 的子对象（推荐）
- 场景中的独立对象

**配置**：
- **初始状态**: Inactive（非常重要！）
- 确保有 `InteractableObject` 组件
- `canBePickedUp: true`
- 确保有 `Collider`（用于玩家检测）
- 确保有 `Rigidbody`（isKinematic = true）

---

### 4. 羽毛（Feather）

**InteractableObject 配置**：
```
understandableProperties (添加):
  - Soft
```

这样玩家才能从羽毛理解 Soft 特性。

---

## 游戏流程

### 路线1（Soft → 折断）
1. 拾取羽毛 → 理解 Soft 特性
2. 对 Wood 使用 Soft 特性 → Wood 变软
3. 对着 Wood 按 Q 键 → 折断，掉落 WoodPart（Wood 本体还在）
4. 拾取 WoodPart → 获得木组件
5. 手持 WoodPart 到石碑按 Q → 充能"木"文字

### 路线2（Thin → 变细）
1. 从老藤理解 Thin 特性
2. 对 Wood 使用 Thin 特性 → Wood 变 inactive，WoodThin 变 active
3. 拾取 WoodThin → 获得木组件
4. 手持 WoodThin 到石碑按 Q → 充能"木"文字

---

## 常见问题

**Q: 木头不能折断？**
- 检查是否已赋予 Soft 特性
- 检查玩家是否空手（按数字键0切换到空手）
- 检查是否按 Q 键（不是 E 键）

**Q: WoodThin 没有出现？**
- 检查 WoodThin 初始是否设为 Inactive
- 检查 WoodThinEffect 的 woodThinObject 字段是否已指定

**Q: 拾取不了 WoodPart/WoodThin？**
- 检查 canBePickedUp 是否为 true
- 检查是否有 Collider 和 Rigidbody

**Q: 无法充能石碑？**
- 检查是否手持 WoodPart 或 WoodThin
- 检查物体名称是否包含 "WoodPart" 或 "WoodThin"

**Q: 按 Q 键没有掉落 WoodPart？**
- 检查 WoodSoftEffect 的 woodPartPrefab 是否已指定
- 检查 Console 是否有错误日志

**Q: Wood 消失了？**
- Soft 路线：Wood 不应该消失，只是掉落 WoodPart
- Thin 路线：Wood 会 inactive，WoodThin 会 active（这是正常的）

---

## 脚本说明

- `WoodSoftEffect.cs`: 处理木头获得 Soft 特性，按 Q 折断
- `WoodThinEffect.cs`: 处理木头获得 Thin 特性，切换到 WoodThin
- `Level2Manager.cs`: 处理木组件拾取和充能逻辑

详细说明见 `LEVEL2_WOOD_SETUP.md`


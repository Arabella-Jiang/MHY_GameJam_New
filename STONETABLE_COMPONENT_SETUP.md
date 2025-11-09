# StoneTable Prefab 组件配置说明

## 📋 组件挂载位置

### 1. **StarTabletEffect 组件**
- **位置**：添加到 `default` 子对象上（和 `InteractableObject` 组件在同一个对象）
- **原因**：
  - `Level3Manager` 通过 `stoneTablet.GetComponent<CombinationEffect>()` 获取这个组件
  - `stoneTablet` 引用的是 `default` 子对象（有 `InteractableObject` 的那个）
  - 所以 `StarTabletEffect` 必须和 `InteractableObject` 在同一个对象上

### 2. **InteractableObject 组件**
- **位置**：已在 `default` 子对象上（已有，无需添加）

### 3. **Collider 组件**
- **位置**：已在 `default` 子对象上（已有 `MeshCollider`，无需添加）

## 🔧 Inspector 配置步骤

### 在 StoneTable Prefab 中：

1. **选择 `default` 子对象**
   - 添加 `StarTabletEffect` 组件（如果还没有的话）

2. **配置 StarTabletEffect 组件的 Inspector：**
   - **Life Text Renderer**：
     - 拖拽 `word/life` 对象到这个字段
     - 或者手动选择：`StoneTable` → `word` → `life` → 获取其 `MeshRenderer` 组件
   
   - **Star Point Text Parent**：
     - 拖拽 `word/Star` 对象到这个字段
     - 或者手动选择：`StoneTable` → `word` → `Star` → 获取其 `Transform` 组件
   
   - **Lit Material**：
     - 拖拽点亮后的材质（Material）到这个字段
     - 这个材质会应用到"生"和"星点"文字上

### 在 Level3Manager 中：

1. **Stone Tablet 引用**：
   - 拖拽 `default` 子对象到 `stoneTablet` 字段
   - （因为 `default` 有 `InteractableObject` 组件）

2. **Tablet Text Effect 引用**：
   - 拖拽 `default` 子对象到 `tabletTextEffect` 字段
   - （因为 `StarTabletEffect` 组件在 `default` 上）

## 📁 Prefab 结构

```
StoneTable (主对象)
├── default (子对象)
│   ├── InteractableObject ✅
│   ├── MeshCollider ✅
│   ├── MeshRenderer ✅
│   ├── MeshFilter ✅
│   └── StarTabletEffect ⚠️ 需要添加
│
└── word (子对象)
    ├── life (子对象)
    │   └── MeshRenderer ✅ (用于 lifeTextRenderer)
    │
    └── Star (子对象，父对象)
        ├── Retopo_polySurface7 (子对象，有 Renderer)
        ├── Retopo_polySurface9 (子对象，有 Renderer)
        ├── Retopo_polySurface10 (子对象，有 Renderer)
        ├── Retopo_polySurface11 (子对象，有 Renderer)
        ├── Retopo_polySurface12 (子对象，有 Renderer)
        └── ... (其他子对象，都有 Renderer)
```

## ✅ 检查清单

- [ ] `default` 子对象上有 `StarTabletEffect` 组件
- [ ] `StarTabletEffect` 的 `lifeTextRenderer` 指向 `word/life` 的 `Renderer`
- [ ] `StarTabletEffect` 的 `starPointTextParent` 指向 `word/Star` 的 `Transform`
- [ ] `StarTabletEffect` 的 `litMaterial` 已设置点亮后的材质
- [ ] `Level3Manager` 的 `stoneTablet` 引用指向 `default` 子对象
- [ ] `Level3Manager` 的 `tabletTextEffect` 引用指向 `default` 子对象

## 🎯 工作原理

1. **充能"生"组件**：
   - 玩家手持"生"组件花，对着石碑按 Q
   - `Level3Manager.ChargeLife()` 被调用
   - `tabletTextEffect.LightUpLifeText()` 被调用
   - "生"文字的材质被替换为 `litMaterial`

2. **充能"星点"组件**：
   - 玩家手持"星点"组件，对着石碑按 Q
   - `Level3Manager.ChargeStarPoint()` 被调用
   - `tabletTextEffect.LightUpStarPointText()` 被调用
   - "星点"文字的所有 Renderer 的材质被替换为 `litMaterial`

3. **触发通关效果**：
   - 当"生"和"星点"都充能完成后
   - `Level3Manager` 检测到所有组件已充能
   - 调用 `stoneTablet.GetComponent<CombinationEffect>().TriggerEffect()`
   - `StarTabletEffect.TriggerEffect()` 被调用，输出通关日志


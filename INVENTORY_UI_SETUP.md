# Inventory UI 配置指南

本文档说明如何配置玩家特性背包的UI显示系统。

## 系统概述

当玩家吸收物品的特性时，对应的图标会显示在屏幕左上角的Inventory Panel中。

**支持的物品类型：**
- 🪶 **羽毛** (Feather) - 对应特性：Light（轻）
- 🪨 **石头** (Stone) - 对应特性：Hard（坚硬）、Heavy（重）
- 🌿 **老藤** (OldVine) - 对应特性：Flexible（柔韧）、Soft（软）、Long（长）
- 🧊 **冰锥** (Icicle) - 对应特性：Transparent（透明）、Sharp（尖锐）、Cool（凉）、White（白）

## 配置步骤

### 1. 在Canvas上添加InventoryUIManager组件

1. 在Unity编辑器中打开Scene
2. 在Hierarchy中找到或创建Canvas（路径：`Prefab/UI/Canvas.prefab`）
3. 选中Canvas GameObject
4. 在Inspector中点击"Add Component"
5. 搜索并添加"InventoryUIManager"组件

### 2. 配置InventoryUIManager组件引用

在Inspector中配置以下字段：

#### UI引用部分
- **Property Icon 1**: 拖入 `Canvas/InventoryPanel/InventoryPanel1/RawImage/propertyIcon`
- **Property Icon 2**: 拖入 `Canvas/InventoryPanel/InventoryPanel2/RawImage/propertyIcon`

> ⚠️ **注意**：这两个propertyIcon对象在prefab中默认是未激活的（IsActive = false），这是正常的。脚本会在需要时自动激活它们。

#### 特性图标资源部分
将以下图标资源拖入对应字段（位于 `Assets/Prefab/UI/InventoryUI/` 文件夹）：

- **Feather Icon**: 拖入 `Feather.png`
- **Stone Icon**: 拖入 `Stone.png`
- **Old Vine Icon**: 拖入 `OldVine.png`
- **Icicle Icon**: 拖入 `Icicle.png`

#### 组件引用部分
- **Empowerment Ability**: 拖入玩家GameObject上的 `EmpowermentAbility` 组件
  - 如果留空，脚本会自动查找带有"Player"标签的GameObject上的该组件

### 3. 确保Player对象设置正确

1. 找到Player GameObject（通常在 `Prefab/Player/Player.prefab`）
2. 确保Player上已经挂载了 `EmpowermentAbility` 组件
3. 确保Player的Tag设置为 "Player"

## 工作流程

1. **玩家长按E键**理解物品特性
2. **EmpowermentAbility**组件将特性添加到背包槽位（最多2个）
3. **EmpowermentAbility**触发 `OnPropertyChanged` 事件
4. **InventoryUIManager**收到事件通知
5. **InventoryUIManager**根据特性类型查找对应图标
6. 在左上角的Inventory Panel中**显示图标**

## 测试验证

配置完成后，在Unity编辑器中运行游戏：

1. 操作玩家角色靠近可交互物体（如羽毛、石头等）
2. 长按E键吸收特性
3. 观察左上角的Inventory Panel：
   - 第一次吸收特性时，应该在InventoryPanel1中显示对应图标
   - 第二次吸收特性时，应该在InventoryPanel2中显示对应图标
4. Console中应显示类似信息：
   ```
   特性 [Light] 已存入背包格 1
   UI更新：槽位1显示特性 [Light] 的图标
   ```

## 特性到图标的映射表

| 特性 (ObjectProperty) | 对应图标 | 来源物品 |
|---------------------|---------|---------|
| Light | Feather.png | 羽毛 |
| Hard | Stone.png | 石头 |
| Heavy | Stone.png | 石头 |
| Flexible | OldVine.png | 老藤 |
| Soft | OldVine.png | 老藤 |
| Long | OldVine.png | 老藤 |
| Flammable | OldVine.png | 树枝/老藤 |
| Thin | OldVine.png | 树枝/老藤 |
| Transparent | Icicle.png | 冰锥 |
| Sharp | Icicle.png | 冰锥 |
| Cool | Icicle.png | 冰锥 |
| White | Icicle.png | 冰锥 |

## 扩展和自定义

### 添加新的特性图标

如果需要为某个特性添加独立的图标：

1. 在 `InventoryUIManager` 组件中添加新的 `Texture2D` 字段
2. 修改 `InitializePropertyIconMap()` 方法，添加新的映射关系：
```csharp
propertyIconMap[ObjectProperty.YourNewProperty] = yourNewIcon;
```

### 调整UI布局

- Inventory Panel的位置：修改 `Canvas/InventoryPanel` 的 RectTransform
- 图标大小：修改 `propertyIcon` 对象的 Scale
- 图标间距：修改 `InventoryPanel1` 和 `InventoryPanel2` 的位置

## 常见问题

### Q: 图标不显示？
**A:** 检查以下几点：
1. InventoryUIManager组件是否正确配置了所有图标资源
2. propertyIcon对象的父级RawImage是否正确引用
3. Console中是否有错误信息
4. 使用Inspector查看运行时propertyIcon的Active状态

### Q: 图标显示位置不对？
**A:** 调整Canvas的布局设置：
- 确保Canvas Scaler的UI Scale Mode设置正确
- 检查InventoryPanel的Anchor和Pivot设置
- 调整InventoryPanel1和InventoryPanel2的AnchoredPosition

### Q: 如何调试？
**A:** 
1. 在InventoryUIManager组件的右键菜单中选择"刷新UI显示"
2. 查看Console中的日志输出
3. 在Inspector中实时查看propertyIcon的texture引用

## 相关文件

- **脚本**: `Assets/Script/core/InventoryUIManager.cs`
- **EmpowermentAbility**: `Assets/Script/core/EmpowermentAbility.cs`
- **Canvas Prefab**: `Assets/Prefab/UI/Canvas.prefab`
- **图标资源**: `Assets/Prefab/UI/InventoryUI/*.png`

## 更新日志

- 2024-11-09: 创建Inventory UI系统
  - 添加InventoryUIManager组件
  - 在EmpowermentAbility中添加事件通知机制
  - 建立特性到图标的映射关系


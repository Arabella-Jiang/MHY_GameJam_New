# 组件关系图

## 核心系统架构

```
┌─────────────────────────────────────────────────────────────┐
│                        Player (主控制器)                      │
├─────────────────────────────────────────────────────────────┤
│  - PlayerMovement (移动控制)                                 │
│  - MouseLook (视角控制)                                      │
│  - EmpowermentAbility (特性背包)                             │
│  - PlayerHoldItem (物品拾取)                                 │
│  - Camera (相机)                                             │
└─────────────────────────────────────────────────────────────┘
                            │
                            ├─────────────────┐
                            │                 │
                            ▼                 ▼
        ┌───────────────────────────┐  ┌──────────────────────┐
        │  InteractableObject       │  │  LevelManager        │
        │  (可交互物体基类)          │  │  (关卡管理器)         │
        ├───────────────────────────┤  ├──────────────────────┤
        │  - currentProperties      │  │  - TutorialManager   │
        │  - understandableProperties│  │  - Level2Manager     │
        │  - propertyCombinations   │  │                      │
        │  - canBePickedUp          │  │                      │
        └───────────────────────────┘  └──────────────────────┘
                    │
                    ├──────────────────────────────────────┐
                    │                                      │
                    ▼                                      ▼
        ┌───────────────────────────┐        ┌──────────────────────────┐
        │  CombinationEffect        │        │  特殊效果组件             │
        │  (组合效果基类)            │        │  - BranchIgnition        │
        ├───────────────────────────┤        │  - FeatherCollector      │
        │  - TriggerEffect()        │        │  - SunCollector          │
        └───────────────────────────┘        └──────────────────────────┘
                    │
        ┌───────────┼───────────┬───────────┬───────────┐
        │           │           │           │           │
        ▼           ▼           ▼           ▼           ▼
┌───────────┐ ┌───────────┐ ┌───────────┐ ┌───────────┐ ┌───────────┐
│WaterHard  │ │BranchHard │ │StoneLong  │ │StoneFlex  │ │StoneTable │
│Effect     │ │Effect     │ │Effect     │ │Effect     │ │Effect     │
└───────────┘ └───────────┘ └───────────┘ └───────────┘ └───────────┘
        │
        ▼
┌───────────────────────────┐
│  AirWallBlockMessage      │
│  (空气墙阻挡消息)          │
└───────────────────────────┘
```

## 交互流程图

### 特性理解流程
```
玩家长按E键 (2秒)
    │
    ▼
Player.StartUnderstanding()
    │
    ▼
获取物体可理解特性列表
    │
    ├─── 单个特性 ────→ EmpowermentAbility.UnderstandProperty()
    │                                          │
    │                                          ▼
    └─── 多个特性 ────→ 玩家选择 ────→ 存储到propertySlots[0/1]
```

### 特性赋予流程
```
玩家短按E键 (选择特性槽后)
    │
    ▼
Player.HandleApplyProperty()
    │
    ▼
EmpowermentAbility.ApplyProperty()
    │
    ▼
InteractableObject.ReceiveProperty()
    │
    ▼
检查propertyCombinations
    │
    ├─── 匹配组合 ────→ 通过反射查找CombinationEffect
    │                          │
    │                          ▼
    │                    TriggerEffect()
    │                          │
    │                          ▼
    └─── 不匹配 ────→ 仅添加特性到currentProperties
```

### 物品拾取流程
```
玩家按F键
    │
    ▼
Player.HandleItemPickupDrop()
    │
    ▼
PlayerHoldItem.PickupItem()
    │
    ├─── 设置物品为kinematic
    ├─── 禁用碰撞体
    ├─── 播放拾取动画
    ├─── 设置父对象为holdPosition
    └─── 通知LevelManager.HandlePlayerPickup()
```

### 基础交互流程
```
玩家按Q键
    │
    ▼
Player.HandleBaseInteraction()
    │
    ├─── 检查是否有BranchIgnition组件
    │         │
    │         └───→ TryIgnite() (摩擦点火)
    │
    └─── 通知LevelManager.HandlePlayerUse()
                │
                ├─── TutorialManager → 检查石碑充能
                └─── Level2Manager → 检查组件充能
```

## Level1 (教程关卡) 流程

```
开始
  │
  ▼
学习Hard特性 (靠近石头长按E)
  │
  ▼
硬化水面 (对水面短按E，使用Hard特性)
  │
  ▼
通过河流
  │
  ▼
硬化两根树枝 (对树枝短按E，使用Hard特性)
  │
  ▼
摩擦点火 (空手对树枝按Q键)
  │
  ▼
拾取点燃的树枝 (按F键)
  │
  ▼
带到石碑旁边 (移动到石碑附近)
  │
  ▼
充能石碑 (按Q键)
  │
  ▼
触发StoneTableEffect.TriggerEffect()
  │
  ▼
关卡完成 → 切换到Level2
```

## Level2 流程

```
开始
  │
  ▼
理解老藤特性 (Long/Flexible/Thin)
  │
  ├─── 路径1: 石头变高 (Long特性)
  │         │
  │         └───→ 够到树冠 → 获得羽毛
  │
  ├─── 路径2: 石头变软 (Flexible特性)
  │         │
  │         └───→ 跳板效果 → 够到树冠 → 获得羽毛
  │
  ├─── 路径3: 树干变细 (Thin特性)
  │         │
  │         └───→ 鸟巢掉落 → 获得羽毛
  │
  └─── 路径4: 空气变暖 (Warm特性)
            │
            └───→ 上升气流 → 够到树冠 → 获得羽毛
  │
  ▼
获得木组件 (让树干变细到可抓取)
  │
  ▼
获得日组件 (使用光能量特性，从Level1继承)
  │
  ▼
充能石碑 (手持组件按E，或使用光能量特性)
  │
  ├─── 充能羽毛
  ├─── 充能木组件
  └─── 充能日组件
  │
  ▼
触发SpringTabletEffect.TriggerEffect()
  │
  ▼
关卡完成
```

## 特性组合表

| 物体 | 所需特性 | 效果组件 | 结果 |
|------|---------|---------|------|
| 水 | Hard | WaterHardEffect | 水硬化，可通过河流 |
| 树枝 | Hard | BranchHardEffect | 树枝变硬且易燃 |
| 石头 | Long | StoneLongEffect | 石头变高，用作平台 |
| 石头 | Flexible | StoneFlexibleEffect | 石头变软，用作跳板 |
| 石碑(Level1) | Flammable (通过点燃的树枝) | StoneTableEffect | 石碑点亮，关卡完成 |
| 树干 | Thin | TreeTrunkThinEffect | 树干变细，鸟巢掉落 |
| 树干 | Thin (更细) | TreeTrunkGraspableEffect | 树干可抓取，获得木组件 |
| 林间空气 | Warm | AirWarmEffect | 形成上升暖流 |

## 特性来源表

| 特性 | 来源物体 | 获取方式 |
|------|---------|---------|
| Hard | 石头 | 长按E理解 |
| Flammable | 点燃的树枝 | 长按E理解 |
| Long | 老藤 | 长按E理解 |
| Flexible | 老藤 | 长按E理解 |
| Thin | 老藤 | 长按E理解 |
| Warm | 光能量 (从Level1继承) | 已拥有 |
| Light | 光能量 (从Level1继承) | 已拥有 |

## 组件通信图

```
┌─────────────┐
│   Player    │
└──────┬──────┘
       │
       ├──────────────┐
       │              │
       ▼              ▼
┌──────────────┐  ┌──────────────┐
│Empowerment   │  │PlayerHoldItem│
│Ability       │  └──────┬───────┘
└──────┬───────┘         │
       │                 │
       │                 ▼
       │          ┌──────────────┐
       │          │LevelManager  │
       │          └──────┬───────┘
       │                 │
       │                 ▼
       │          ┌──────────────┐
       │          │TutorialManager│
       │          │Level2Manager │
       │          └──────────────┘
       │
       ▼
┌──────────────┐
│Interactable  │
│Object        │
└──────┬───────┘
       │
       ├─────────────────┐
       │                 │
       ▼                 ▼
┌──────────────┐  ┌──────────────┐
│Combination   │  │BranchIgnition│
│Effect        │  │FeatherCollector│
│              │  │SunCollector  │
└──────────────┘  └──────────────┘
```

## 数据流向

### 特性数据流
```
物体.understandableProperties
    │
    ▼
玩家理解 (长按E)
    │
    ▼
EmpowermentAbility.propertySlots[0/1]
    │
    ▼
玩家赋予 (短按E)
    │
    ▼
目标物体.currentProperties
    │
    ▼
检查propertyCombinations
    │
    ▼
触发CombinationEffect.TriggerEffect()
```

### 物品数据流
```
InteractableObject (canBePickedUp = true)
    │
    ▼
玩家拾取 (按F)
    │
    ▼
PlayerHoldItem.heldObject
    │
    ├─── 手持状态
    │
    └─── 充能到石碑 (按E/Q)
            │
            ▼
        LevelManager处理充能逻辑
```

## 关键接口和抽象

### CombinationEffect (抽象类)
```csharp
public abstract class CombinationEffect : MonoBehaviour
{
    public abstract void TriggerEffect();
}
```

### LevelManager (抽象类)
```csharp
public abstract class LevelManager : MonoBehaviour
{
    public abstract void HandlePlayerPickup(InteractableObject target, bool hasUnlockedEmpowerment);
    public abstract void HandlePlayerUse(InteractableObject target, int selectedSlot, bool hasUnlockedEmpowerment);
}
```

## 扩展点

1. **添加新特性** - 在ObjectProperty枚举中添加
2. **添加新组合效果** - 继承CombinationEffect类
3. **添加新关卡** - 继承LevelManager类
4. **添加新收集组件** - 创建类似FeatherCollector的组件
5. **添加新交互类型** - 在Player.HandleBaseInteraction()中扩展


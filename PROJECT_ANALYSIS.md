# 项目深度分析文档

## 项目概述
这是一个Unity游戏项目，主题似乎是关于"赋能"（Empowerment）的谜题解谜游戏。玩家通过"理解"物体的特性，然后将这些特性"赋予"其他物体，创造组合效果来解决谜题。

## 核心游戏机制

### 1. 特性系统 (Property System)
- **ObjectProperty枚举**：定义了游戏中所有可用的特性
  - `Hard` (坚硬) - 从石头理解
  - `Soft` (柔软)
  - `Flexible` (柔韧)
  - `Flammable` (可燃) - 从树枝理解
  - `Liquid` (液体)
  - `Long` (长)
  - `Thin` (细)
  - `Warm` (暖)
  - `Light` (光) - 从上一关获得

### 2. 玩家交互系统

#### 核心组件：
- **Player.cs** - 主玩家控制器
  - 管理所有交互输入
  - 使用触发器检测范围内的可交互物体
  - 视角评分系统（选择最面向玩家的物体）

#### 输入控制：
- **长按E键** (2秒) - 理解特性（Understand Property）
  - 从物体中学习特性并存入背包
  - 如果物体有多个可理解特性，玩家可以选择
- **短按E键** - 赋予特性（Apply Property）
  - 仅在选择特性槽时有效（数字键1/2选择槽位）
  - 将背包中的特性赋予目标物体
- **Q键** - 基础交互（Base Interaction）
  - 不依赖特性槽的交互
  - 用于树枝摩擦点火、石碑充能等
- **F键** - 拾取/放下物品
  - 拾取可交互物体
  - 再次按F放下物品
- **数字键1/2** - 切换特性槽位
- **数字键0** - 切换到空手状态

#### 背包系统 (EmpowermentAbility.cs)：
- 2个特性槽位（propertySlots）
- 特性可以从物体中"理解"并存储
- 特性可以"赋予"给其他物体
- 槽位满时自动替换第一个槽位

### 3. 特性组合效果系统

#### CombinationEffect基类：
所有组合效果都继承自`CombinationEffect`，通过`TriggerEffect()`方法触发。

#### 组合效果列表：

1. **WaterHardEffect** - 水硬化效果
   - 触发条件：水 + Hard特性
   - 效果：禁用空气墙碰撞体，允许玩家通过河流
   - 材质替换为硬化版本
   - 启用水面平面碰撞体

2. **BranchHardEffect** - 树枝硬化效果
   - 触发条件：树枝 + Hard特性
   - 效果：树枝变硬且易燃，颜色变深

3. **BranchIgnition** - 树枝点火系统
   - 需要两根硬化的树枝
   - 空手对树枝按Q键进行摩擦点火
   - 只有细树枝会被点燃
   - 点燃后获得Flammable特性

4. **StoneLongEffect** - 石头变高效果
   - 触发条件：石头 + Long特性
   - 效果：石头X/Z轴放大3倍，Y轴放大5倍
   - 用作平台让玩家够到高处

5. **StoneFlexibleEffect** - 石头变软效果（跳板）
   - 触发条件：石头 + Flexible特性
   - 效果：玩家踩上后重力减小，跳跃高度增加
   - 通过反射修改PlayerMovement的重力和跳跃参数

6. **StoneTableEffect** - 石碑效果（Level1通关）
   - 触发条件：点燃的树枝充能到石碑
   - 效果：点亮石碑，播放火焰特效，改变文字材质

7. **SpringTabletEffect** - 春字石碑效果（Level2通关）
   - 触发条件：收集木、羽、日三个组件并充能
   - 效果：点亮石碑，播放春天特效，生成花草

8. **TreeTrunkThinEffect** - 树干变细效果
   - 触发条件：树干 + Thin特性
   - 效果：树干变细，鸟巢掉落

9. **TreeTrunkGraspableEffect** - 树干可抓取效果
   - 触发条件：树干 + Thin特性（更细的版本）
   - 效果：树干变得可拾取，获得木组件

10. **AirWarmEffect** - 空气变暖效果
    - 触发条件：林间空气 + Warm特性
    - 效果：形成上升暖流，玩家在范围内会获得向上的力

### 4. 关卡系统

#### LevelManager基类（抽象类）：
- 管理关卡逻辑
- 处理玩家拾取和使用
- 显示消息和触发过场动画

#### TutorialManager (Level1)：
教程关卡，引导玩家学习核心机制：
1. 学习Hard特性（从石头）
2. 硬化水面，通过河流
3. 硬化两根树枝
4. 摩擦点火（两根硬树枝按Q）
5. 拾取点燃的树枝，带到石碑充能

#### Level2Manager：
第二关，需要收集三个组件：
1. **木组件** - 通过让树干变细获得
2. **羽组件** - 从掉落的鸟巢中获得
3. **日组件** - 使用光能量特性（从Level1继承的Flammable/Light）

关卡流程：
- 理解老藤的特性（Long/Flexible/Thin）
- 获得羽毛（通过路径3：让树干变细，鸟巢掉落）
- 获得木组件（通过让树干变细到可抓取）
- 充能石碑（手持组件按E，或使用光能量特性）

### 5. 物体交互系统

#### InteractableObject.cs：
所有可交互物体的基类
- **currentProperties** - 物体当前拥有的特性列表
- **understandableProperties** - 物体可被理解的特性列表
- **propertyCombinations** - 特性组合配置
- **canBePickedUp** - 是否可被拾取

特性组合触发机制：
- 当物体获得新特性时，检查是否满足组合条件
- 通过反射查找对应的CombinationEffect组件
- 调用TriggerEffect()触发效果

### 6. 物品拾取系统

#### PlayerHoldItem.cs：
- 管理玩家手持物品
- **PickupItem()** - 拾取物品
  - 设置物品为kinematic
  - 禁用碰撞体
  - 播放拾取动画（支持Animation和Animator）
  - 通知LevelManager
- **DropItem()** - 放下物品
  - 恢复物理状态
  - 重新启用碰撞体

特殊处理：
- 树枝拾取时旋转-90度（竖着拿）
- 自动查找PlayerBody > Group模型根对象播放动画

### 7. 玩家移动系统

#### PlayerMovement.cs：
- 使用CharacterController
- WASD移动
- 左Shift奔跑
- Space跳跃
- 重力系统

#### MouseLook.cs：
- 鼠标视角控制
- 垂直旋转限制在-90°到90°
- 水平旋转控制玩家身体

### 8. 相机系统

#### CameraManager.cs：
- 管理Cinemachine虚拟相机
- 播放过场动画时禁用玩家控制
- 支持日落分镜等过场

#### CameraSystemFix.cs：
- 修复相机系统初始化问题
- 确保MouseLook正常工作
- 禁用不需要的Cinemachine虚拟相机

### 9. 收集组件系统

#### FeatherCollector.cs：
- 标记羽毛物品
- 拾取时通知Level2Manager
- 播放收集特效

#### SunCollector.cs：
- 标记日组件物品
- 拾取时通知Level2Manager
- 播放收集特效和光源

### 10. 辅助系统

#### AirWallBlockMessage.cs：
- 空气墙阻挡消息
- 当玩家碰到未硬化的水流空气墙时显示提示
- 与WaterHardEffect配合工作

## 场景结构

### 场景列表：
1. **MainMenu.unity** - 主菜单
2. **Level1.unity** - 教程关卡（使用TutorialManager）
3. **Level2.unity** - 第二关（使用Level2Manager）
4. **Level3.unity** - 第三关（待实现）

## Prefab结构

### 主要Prefab：
- **Player.prefab** - 玩家预制体
  - 包含Player、PlayerMovement、MouseLook、EmpowermentAbility、PlayerHoldItem组件
  - PlayerBody模型（包含Group子对象，有动画）
  
- **Level1StoneTable.prefab** - Level1石碑
- **Level2StoneTable.prefab** - Level2石碑（春字）
- **Canvas.prefab** - UI画布
- **word/level1.prefab** - Level1文字
- **word/level2.prefab** - Level2文字
- **word/level3.prefab** - Level3文字

### 其他Prefab：
- 光照系统（Directional Light、Fire_Light）
- 粒子特效（Particles_Fire、Particles_Fireflies）
- 地图元素（Terrain、Water、OakTree04）
- 控制器（PlayerBody.controller、Group.controller）

## 数据流和依赖关系

### 核心数据流：
1. **玩家输入** → Player.cs → 触发交互/移动/拾取
2. **特性理解** → EmpowermentAbility → 存储到propertySlots
3. **特性赋予** → InteractableObject.ReceiveProperty() → 检查组合 → 触发CombinationEffect
4. **物品拾取** → PlayerHoldItem → LevelManager.HandlePlayerPickup()
5. **基础交互** → LevelManager.HandlePlayerUse() → 触发特殊逻辑

### 组件依赖关系：
```
Player
├── PlayerMovement (移动)
├── MouseLook (视角)
├── EmpowermentAbility (特性背包)
├── PlayerHoldItem (物品拾取)
└── Camera (相机)

InteractableObject
├── CombinationEffect[] (组合效果)
└── 其他效果组件 (BranchIgnition, FeatherCollector等)

LevelManager (抽象)
├── TutorialManager (Level1)
└── Level2Manager (Level2)
```

## 关键设计模式

1. **组合模式** - CombinationEffect基类，所有效果继承
2. **策略模式** - 不同的LevelManager实现不同的关卡逻辑
3. **观察者模式** - LevelManager监听玩家拾取和使用事件
4. **反射机制** - InteractableObject通过反射查找CombinationEffect组件

## 待完善的功能

1. **UI系统** - 目前大部分提示使用Debug.Log，需要实现UI显示
2. **关卡切换** - Level1完成后自动切换到Level2
3. **Level3实现** - 第三关尚未实现
4. **过场动画** - TriggerCutscene方法目前只是占位
5. **音效系统** - 缺少音效支持
6. **存档系统** - 没有存档功能

## 技术亮点

1. **灵活的特性组合系统** - 通过配置propertyCombinations实现不同的组合效果
2. **反射机制** - 动态查找和触发效果组件，提高扩展性
3. **物理交互** - 使用CharacterController和Rigidbody实现真实的物理交互
4. **触发器检测** - 使用CapsuleCollider触发器优化交互检测性能
5. **视角评分系统** - 根据玩家视角方向选择最佳交互目标

## 注意事项

1. **PlayerBody模型结构** - 动画系统依赖于PlayerBody > Group的层级结构
2. **空气墙系统** - WaterHardEffect需要正确配置airwallcollider父对象
3. **特性槽位限制** - 目前只有2个槽位，槽位满时会自动替换
4. **物理状态管理** - 拾取物品时需要正确设置kinematic和碰撞体状态
5. **反射性能** - InteractableObject使用反射查找组件，可能影响性能（可优化为缓存）

## 扩展建议

1. 添加更多特性类型和组合效果
2. 实现特性槽位扩展系统
3. 添加特性使用次数限制
4. 实现特性升级系统
5. 添加更多视觉效果和粒子特效
6. 实现完整的UI系统
7. 添加音效和背景音乐
8. 实现存档和读档功能
9. 添加成就系统
10. 实现关卡编辑器


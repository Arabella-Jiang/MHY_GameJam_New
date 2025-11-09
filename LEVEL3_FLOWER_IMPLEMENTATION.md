# Level3 花朵和石碑充能功能实现说明

## 功能概述
1. 植物复活后，地图上激活一朵花（代表"生"组件）
2. 玩家拾取花朵后获得"生"组件
3. 玩家手持花朵到石碑附近，对着石碑按E键充能
4. 充能后，"life"文字部分的材质会改变，达到点亮效果

## 实现步骤

### 1. 在Unity Editor中设置

#### 步骤1：准备花朵Prefab
1. 创建一个花朵GameObject（或使用已有的花朵模型）
2. 确保花朵初始状态为**未激活**（inactive）
3. 添加以下组件：
   - `InteractableObject` 组件
   - `LifeCollector` 组件（可选，PlantRevivalEffect会自动添加）
   - `Collider` 组件（用于拾取检测）
   - `Rigidbody` 组件（可选，用于物理效果）

#### 步骤2：配置PlantRevivalEffect
1. 在植物复活效果的GameObject上找到 `PlantRevivalEffect` 组件
2. 在Inspector中：
   - 将花朵GameObject拖入 `Life Flower` 槽位
   - 配置其他已有的设置（deadTree, treeLive等）

#### 步骤3：配置石碑文字
1. 在石碑GameObject上添加 `StarTabletTextEffect` 组件
2. 在Inspector中配置：
   - **Life Text Renderer**: 将"生"文字部分的Renderer拖入（只有一个）
   - **Star Point Text Parent**: 将"星点"文字部分的父对象Transform拖入（包含多个子对象）
   - **Lit Material**: 将文字点亮后的材质拖入（"生"和"星点"共用同一个材质）

#### 步骤4：配置Level3Manager
1. 在Level3Manager的Inspector中：
   - 将 `StarTabletTextEffect` 组件拖入 `Tablet Text Effect` 槽位
   - 确保其他引用都已正确配置

## 工作流程

### 玩家操作流程：
1. **植物复活**：玩家通过冰面解冻获得水，浇灌植物
2. **花朵出现**：植物复活后，花朵自动激活并出现在地图上
3. **拾取花朵**：玩家靠近花朵，按F键拾取
4. **获得组件**：拾取后，玩家获得"生"组件（hasLife = true）
5. **充能石碑**：玩家手持花朵，靠近石碑，对着石碑按E键（短按E）
6. **文字点亮**："生"文字部分的材质被替换为点亮材质，视觉效果改变

## 代码修改说明

### 1. PlantRevivalEffect.cs
- 添加了 `lifeFlower` 字段，用于指定花朵GameObject
- 在 `PerformRevival()` 方法中：
  - 激活花朵
  - 自动添加 `LifeCollector` 组件（如果不存在）
  - 确保花朵可以被拾取

### 2. Level3Manager.cs
- 添加了 `tabletTextEffect` 字段，用于引用文字点亮效果组件
- 修改了 `ChargeLife()` 方法：
  - 充能时调用 `tabletTextEffect.LightUpLifeText()` 点亮文字
- 修改了 `ChargeStarPoint()` 方法：
  - 充能时调用 `tabletTextEffect.LightUpStarPointText()` 点亮文字

### 3. StarTabletTextEffect.cs（新建）
- 管理石碑文字的点亮效果
- 提供了 `LightUpLifeText()` 和 `LightUpStarPointText()` 方法
- 自动保存原始材质，支持重置（用于测试）

### 4. LifeCollector.cs
- 保持不变，继续处理"生"组件的收集逻辑

## 注意事项

1. **花朵初始状态**：花朵必须在场景中但初始状态为未激活（inactive）
2. **文字结构**：
   - "生"文字只有一个Renderer，直接指定Renderer
   - "星点"文字有多个子对象，需要指定父对象的Transform
3. **材质设置**：准备好点亮后的材质（"生"和"星点"共用同一个材质）
4. **组件引用**：确保Level3Manager正确引用了StarTabletTextEffect组件

## 测试步骤

1. 运行Level3场景
2. 完成植物复活流程（冰面解冻 → 浇灌植物）
3. 确认花朵出现
4. 按F键拾取花朵
5. 检查是否获得"生"组件（hasLife应该为true）
6. 手持花朵，靠近石碑
7. 对着石碑按E键（短按E，需要选择特性槽位，但如果只是充能可以不需要）
8. 检查"生"文字部分是否点亮（材质是否改变）

## 故障排除

- **花朵不出现**：检查PlantRevivalEffect的lifeFlower引用是否正确
- **无法拾取花朵**：检查花朵是否有InteractableObject组件，且canBePickedUp为true
- **充能不工作**：检查Level3Manager的tabletTextEffect引用是否正确
- **文字不点亮**：检查StarTabletTextEffect的lifeTextRenderer、starPointTextParent和litMaterial是否正确配置


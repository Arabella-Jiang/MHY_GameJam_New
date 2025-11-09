using UnityEngine;
using System.Collections;

public class Level3Manager : LevelManager
{
    [Header("关卡目标引用")]
    public InteractableObject stoneTablet;      // 石碑（星字）
    public InteractableObject stone;            // 石头（可理解 Heavy/Hard）
    public InteractableObject snow;             // 雪（可理解 Cool/White）
    public InteractableObject iceCone;          // 冰锥（可理解 Hard/Transparent/Sharp）
    public InteractableObject deadPlant;        // 枯死的植物
    public InteractableObject frozenLake;       // 结冰的湖面
    public InteractableObject iceSurface;       // 冰面
    public InteractableObject largeStone;       // 大块石头（用于反射星光）

    [Header("收集状态")]
    public bool hasLife = false;                // 是否获得"生"组件
    public bool hasStarPoint = false;           // 是否获得"星点"组件

    [Header("充能状态")]
    public bool lifeCharged = false;            // "生"组件是否已充能到石碑
    public bool starPointCharged = false;       // "星点"组件是否已充能到石碑

    private enum Step
    {
        Start,
        LearnProperties,
        GetLife,
        GetStarPoint,
        ChargeTablet,
        Complete
    }

    private Step step = Step.Start;

    protected override void InitializeLevel()
    {
        base.InitializeLevel();

        ShowMessage("春,多么充满希望的字。万物复苏,草木葳蕤,知春便知世界的律动。");
        ShowMessage("石碑需要两个组件：生、星点。继续探索吧。");
        step = Step.LearnProperties;
    }

    void Update()
    {
        switch (step)
        {
            case Step.LearnProperties:
                // 提示玩家理解物体的特性
                if (player != null && player.currentInteractTarget != null)
                {
                    InteractableObject target = player.currentInteractTarget;
                    if (target == stone || target == snow || target == iceCone)
                    {
                        // 可以在这里添加提示
                    }
                }
                break;

            case Step.GetLife:
                if (hasLife)
                {
                    ShowMessage("已获得\"生\"组件。接下来需要获取\"星点\"组件。");
                    step = Step.GetStarPoint;
                }
                break;

            case Step.GetStarPoint:
                if (hasStarPoint)
                {
                    ShowMessage("已获得\"星点\"组件。前往石碑充能（短按E）。");
                    step = Step.ChargeTablet;
                }
                break;

            case Step.ChargeTablet:
                // 检查是否所有组件都已充能完成
                if (lifeCharged && starPointCharged && stoneTablet != null)
                {
                    // 触发石碑最终效果（使用反射避免编译错误）
                    CombinationEffect effect = stoneTablet.GetComponent<CombinationEffect>();
                    if (effect != null)
                    {
                        effect.TriggerEffect();
                    }
                    else
                    {
                        // 尝试使用SendMessage触发效果
                        stoneTablet.gameObject.SendMessage("TriggerEffect", SendMessageOptions.DontRequireReceiver);
                        Debug.LogWarning("StarTabletEffect component not found on stone tablet!");
                    }
                    
                    ShowMessage("石碑被点亮！星星,照应我们所处的位置在宇宙的何方,知道脚下在哪里,才明白未来何去何从。");
                    step = Step.Complete;
                    TriggerCutscene("StarComplete");
                }
                // 提示玩家充能进度
                else if (hasLife || hasStarPoint)
                {
                    int chargedCount = (lifeCharged ? 1 : 0) + (starPointCharged ? 1 : 0);
                    if (chargedCount < 2)
                    {
                        string missing = "";
                        if (!lifeCharged && hasLife) missing += "生 ";
                        if (!starPointCharged && hasStarPoint) missing += "星点 ";
                        
                        if (missing != "")
                        {
                            ShowMessage($"还需要充能：{missing.Trim()} ({chargedCount}/2)");
                        }
                    }
                }
                break;
        }
    }

    public void OnLifeObtained()
    {
        hasLife = true;
        ShowMessage("获得了\"生\"组件！");
    }

    public void OnStarPointObtained()
    {
        hasStarPoint = true;
        ShowMessage("获得了\"星点\"组件！");
    }

    /// <summary>
    /// 充能"生"组件到石碑
    /// </summary>
    public void ChargeLife()
    {
        if (lifeCharged)
        {
            ShowMessage("\"生\"组件已经充能过了");
            return;
        }
        
        lifeCharged = true;
        ShowMessage("✅ \"生\"组件已充能到石碑！");
        
        CheckAllComponentsCharged();
    }

    /// <summary>
    /// 充能"星点"组件到石碑
    /// </summary>
    public void ChargeStarPoint()
    {
        if (starPointCharged)
        {
            ShowMessage("\"星点\"组件已经充能过了");
            return;
        }
        
        starPointCharged = true;
        ShowMessage("✅ \"星点\"组件已充能到石碑！");
        
        CheckAllComponentsCharged();
    }

    private void CheckAllComponentsCharged()
    {
        if (lifeCharged && starPointCharged)
        {
            step = Step.ChargeTablet; // 确保进入充能步骤，触发完成
        }
    }

    public override void HandlePlayerPickup(InteractableObject target, bool hasUnlockedEmpowerment)
    {
        // 处理玩家拾取逻辑
        // 检查是否是"生"组件（通过LifeCollector组件标记，使用反射避免编译错误）
        Component lifeCollector = target.GetComponent("LifeCollector");
        if (lifeCollector != null && !hasLife)
        {
            lifeCollector.SendMessage("OnPickup", SendMessageOptions.DontRequireReceiver);
            return;
        }

        // 检查是否是"星点"组件（通过StarPointCollector组件标记，使用反射避免编译错误）
        Component starPointCollector = target.GetComponent("StarPointCollector");
        if (starPointCollector != null && !hasStarPoint)
        {
            starPointCollector.SendMessage("OnPickup", SendMessageOptions.DontRequireReceiver);
            return;
        }
    }

    public override void HandlePlayerUse(InteractableObject target, int selectedSlot, bool hasUnlockedEmpowerment)
    {
        // 处理玩家使用特性逻辑
        // 如果玩家对着石碑交互（短按E），检查是否手持组件并充能
        if (target == stoneTablet && player != null && player.playerHoldItem != null)
        {
            GameObject heldItem = player.playerHoldItem.heldObject;
            
            // 如果手持物品，尝试充能"生"或"星点"组件
            if (heldItem != null)
            {
                TryChargeComponent(heldItem);
            }
            else
            {
                ShowMessage("需要手持组件才能充能石碑");
            }
        }
    }

    /// <summary>
    /// 尝试充能组件到石碑
    /// </summary>
    private void TryChargeComponent(GameObject heldItem)
    {
        // 检查是否是"生"组件（使用反射避免编译错误）
        Component lifeCollector = heldItem.GetComponent("LifeCollector");
        if (lifeCollector != null && !lifeCharged)
        {
            ChargeLife();
            return;
        }

        // 检查是否是"星点"组件（使用反射避免编译错误）
        Component starPointCollector = heldItem.GetComponent("StarPointCollector");
        if (starPointCollector != null && !starPointCharged)
        {
            ChargeStarPoint();
            return;
        }

        ShowMessage("当前手持的物品不是需要的组件，或已经充能过了");
    }
}


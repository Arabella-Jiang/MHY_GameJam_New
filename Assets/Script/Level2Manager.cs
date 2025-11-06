using UnityEngine;
using System.Collections;

public class Level2Manager : LevelManager
{
    [Header("关卡目标引用")]
    public InteractableObject stoneTablet;      // 石碑（春字）
    public InteractableObject oldVine;          // 老藤（可理解 Long/Flexible/Thin）
    public InteractableObject treeTrunk;       // 树干（用于获取木组件）
    public InteractableObject birdNest;         // 鸟巢（获得羽毛）
    public InteractableObject forestAir;        // 林间空气（可赋予 Warm）
    public InteractableObject elevatedStone;    // 高处的石头（用于路径1和2）
    public InteractableObject sunItem;          // 日组件物品（可选，如果需要在关卡内拾取）

    [Header("收集状态")]
    public bool hasFeather = false;            // 是否获得羽毛
    public bool hasWood = false;               // 是否获得木组件
    public bool hasSun = true;                // 是否获得日组件

    [Header("充能状态")]
    public bool featherCharged = false;       // 羽毛是否已充能到石碑
    public bool woodCharged = false;          // 木组件是否已充能到石碑
    public bool sunCharged = false;           // 日组件是否已充能到石碑

    private enum Step
    {
        Start,
        LearnVineProperties,
        GetFeather,
        GetWood,
        GetSun,
        ChargeTablet,
        Complete
    }

    private Step step = Step.Start;

    protected override void InitializeLevel()
    {
        base.InitializeLevel();

        // 检查是否有光能量（从上一关继承）- 这就是"日"组件
        if (player != null && player.empowermentAbility != null)
        {
            foreach (var prop in player.empowermentAbility.propertySlots)
            {
                if (prop == ObjectProperty.Light || prop == ObjectProperty.Flammable)
                {
                    hasSun = true;
                    ShowMessage("你已拥有光能量（日组件）");
                    break;
                }
            }
        }

        if (hasSun)
        {
            ShowMessage("石碑需要三个组件：木、羽、日。你已有光能量，继续收集其他组件吧。");
        }
        else
        {
            ShowMessage("石碑需要三个组件：木、羽、日。继续探索吧。");
        }
        step = Step.LearnVineProperties;
    }

    void Update()
    {
        switch (step)
        {
            case Step.LearnVineProperties:
                // 提示玩家理解老藤的特性
                if (oldVine != null && player.currentInteractTarget == oldVine)
                {
                    ShowMessage("老藤有多种特性：长、柔韧、细。选择一个最适合的。");
                }
                break;

            case Step.GetFeather:
                if (hasFeather)
                {
                    ShowMessage("已获得羽毛。接下来需要获取木组件。");
                    step = Step.GetWood;
                }
                break;

            case Step.GetWood:
                if (hasWood)
                {
                    if (!hasSun)
                    {
                        ShowMessage("已获得木组件。你需要先获得光能量（日组件）。");
                        step = Step.GetSun;
                    }
                    else
                    {
                        ShowMessage("已获得木组件。手持组件前往石碑充能（短按E）。");
                        step = Step.ChargeTablet;
                    }
                }
                break;

            case Step.GetSun:
                if (hasSun)
                {
                    ShowMessage("已拥有光能量（日组件）。前往石碑充能（使用特性或手持物品）。");
                    step = Step.ChargeTablet;
                }
                break;

            case Step.ChargeTablet:
                // 检查是否所有组件都已充能完成
                if (featherCharged && woodCharged && sunCharged && stoneTablet != null)
                {
                    // 触发石碑最终效果
                    SpringTabletEffect effect = stoneTablet.GetComponent<SpringTabletEffect>();
                    if (effect != null)
                    {
                        effect.TriggerEffect();
                    }
                    
                    ShowMessage("石碑被点亮！春的意义，即是唤醒世界上一切律动的能力。");
                    step = Step.Complete;
                    TriggerCutscene("SpringComplete");
                }
                // 提示玩家充能进度
                else if (hasFeather || hasWood || hasSun)
                {
                    int chargedCount = (featherCharged ? 1 : 0) + (woodCharged ? 1 : 0) + (sunCharged ? 1 : 0);
                    if (chargedCount < 3)
                    {
                        string missing = "";
                        if (!featherCharged && hasFeather) missing += "羽毛 ";
                        if (!woodCharged && hasWood) missing += "木 ";
                        if (!sunCharged && hasSun) missing += "日 ";
                        
                        if (missing != "")
                        {
                            ShowMessage($"还需要充能：{missing.Trim()} ({chargedCount}/3)");
                        }
                    }
                }
                break;
        }
    }

    public void OnFeatherObtained()
    {
        hasFeather = true;
        ShowMessage("获得了羽毛！");
    }

    public void OnWoodObtained()
    {
        hasWood = true;
        ShowMessage("获得了木组件！");
    }

    public void OnSunObtained()
    {
        hasSun = true;
        ShowMessage("获得了日组件！");
    }

    /// <summary>
    /// 充能羽毛到石碑
    /// </summary>
    public void ChargeFeather()
    {
        if (featherCharged)
        {
            ShowMessage("羽毛已经充能过了");
            return;
        }
        
        featherCharged = true;
        ShowMessage("✅ 羽毛已充能到石碑！");
        
        // 如果这是最后一个组件，会自动触发完成
        CheckAllComponentsCharged();
    }

    /// <summary>
    /// 充能木组件到石碑
    /// </summary>
    public void ChargeWood()
    {
        if (woodCharged)
        {
            ShowMessage("木组件已经充能过了");
            return;
        }
        
        woodCharged = true;
        ShowMessage("✅ 木组件已充能到石碑！");
        
        CheckAllComponentsCharged();
    }

    /// <summary>
    /// 充能日组件到石碑（使用光能量特性，不需要手持物品）
    /// </summary>
    public void ChargeSun()
    {
        if (sunCharged)
        {
            ShowMessage("日组件已经充能过了");
            return;
        }
        
        if (!hasSun)
        {
            ShowMessage("你还没有光能量（日组件）");
            return;
        }
        
        sunCharged = true;
        ShowMessage("✅ 日组件已充能到石碑！");
        
        CheckAllComponentsCharged();
    }

    private void CheckAllComponentsCharged()
    {
        // 检查是否所有组件都已充能（在ChargeTablet步骤的Update中会自动处理完成逻辑）
        if (featherCharged && woodCharged && sunCharged)
        {
            step = Step.ChargeTablet; // 确保进入充能步骤，触发完成
        }
    }

    private bool CheckAllComponentsCollected()
    {
        // 检查是否所有组件都已收集（不要求充能）
        return hasFeather && hasWood && hasSun;
    }

    private bool HasAllComponentsCharged()
    {
        if (stoneTablet == null) return false;
        
        // 检查石碑是否拥有所有需要的特性组合
        return stoneTablet.currentProperties.Contains(ObjectProperty.Flammable) || 
               HasProperty(stoneTablet, ObjectProperty.Light);
    }

    private bool HasProperty(InteractableObject obj, ObjectProperty prop)
    {
        return obj != null && obj.currentProperties != null && obj.currentProperties.Contains(prop);
    }

    public override void HandlePlayerPickup(InteractableObject target, bool hasUnlockedEmpowerment)
    {
        // 处理玩家拾取逻辑
        // 检查是否是羽毛（通过FeatherCollector组件标记）
        var featherCollector = target.GetComponent<FeatherCollector>();
        if (featherCollector != null && !hasFeather)
        {
            featherCollector.OnPickup();
            // OnPickup内部已经调用了OnFeatherObtained()
            return;
        }

        // 检查是否是树干（获取木组件）
        if (target == treeTrunk && !hasWood && HasProperty(treeTrunk, ObjectProperty.Thin))
        {
            OnWoodObtained();
            return;
        }

        // 检查是否是日组件（通过SunCollector组件标记）
        var sunCollector = target.GetComponent<SunCollector>();
        if (sunCollector != null && !hasSun)
        {
            sunCollector.OnPickup();
            // OnPickup内部已经调用了OnSunObtained()
            return;
        }
    }

    public override void HandlePlayerUse(InteractableObject target, int selectedSlot, bool hasUnlockedEmpowerment)
    {
        // 处理玩家使用特性逻辑
        // 如果玩家对着石碑交互（短按E），检查是否手持组件并充能，或使用光能量特性充能日组件
        if (target == stoneTablet && player != null && player.playerHoldItem != null)
        {
            GameObject heldItem = player.playerHoldItem.heldObject;
            
            // 如果手持物品，尝试充能羽毛或木组件
            if (heldItem != null)
            {
                TryChargeComponent(heldItem);
            }
            // 如果没有手持物品，检查是否使用光能量特性充能日组件
            else if (selectedSlot >= 0 && selectedSlot < player.empowermentAbility.propertySlots.Length)
            {
                ObjectProperty property = player.empowermentAbility.propertySlots[selectedSlot];
                if ((property == ObjectProperty.Light || property == ObjectProperty.Flammable) && !sunCharged)
                {
                    ChargeSun();
                }
                else if (property != ObjectProperty.None)
                {
                    ShowMessage("当前特性不能用于充能石碑");
                }
                else
                {
                    ShowMessage("需要手持组件或使用光能量特性才能充能石碑");
                }
            }
            else
            {
                ShowMessage("需要手持组件或使用光能量特性才能充能石碑");
            }
        }
    }

    /// <summary>
    /// 尝试充能组件到石碑
    /// </summary>
    private void TryChargeComponent(GameObject heldItem)
    {
        // 检查是否是羽毛
        if (heldItem.GetComponent<FeatherCollector>() != null && !featherCharged)
        {
            ChargeFeather();
            return;
        }

        // 检查是否是木组件（通过TreeTrunkGraspableEffect标记或直接是treeTrunk）
        if ((heldItem == treeTrunk?.gameObject || heldItem.GetComponent<TreeTrunkGraspableEffect>() != null) 
            && !woodCharged && HasProperty(heldItem.GetComponent<InteractableObject>(), ObjectProperty.Thin))
        {
            ChargeWood();
            return;
        }

        // 日组件不需要手持，而是使用光能量特性（在HandlePlayerUse中处理）

        ShowMessage("当前手持的物品不是需要的组件，或已经充能过了");
    }
}


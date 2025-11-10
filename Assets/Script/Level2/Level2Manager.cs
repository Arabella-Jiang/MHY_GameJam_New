using UnityEngine;
using System.Collections;

public class Level2Manager : LevelManager
{
    [Header("关卡目标引用")]
    public InteractableObject stoneTablet;      // 石碑（春字）
    public InteractableObject oldVine;          // 老藤（可理解 Long/Flexible/Thin）
    public InteractableObject birdNest;         // 鸟巢（获得羽毛）
    public InteractableObject sunItem;          // 日组件物品（可选，如果需要在关卡内拾取）
    
    [Header("石碑文字效果")]
    public SpringTabletEffect tabletTextEffect; // 石碑文字点亮效果组件

    [Header("通关设置")]
    public float returnToLevel3Delay = 3f;    // 通关后跳转到Level3的延迟时间（秒）

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
    private bool hasShownVinePropertyHint = false; // 是否已经显示过老藤特性提示
    private bool transitionTriggered = false;

    protected override void InitializeLevel()
    {
        base.InitializeLevel();

        // 检查是否有跟随光源（Fire_Light）- 这是从上一关继承的"日"组件
        if (player != null)
        {
            // 查找PlayerBody子对象，然后在其下查找Fire_Light
            Transform playerBody = player.transform.Find("PlayerBody");
            if (playerBody != null)
            {
                Transform fireLightTransform = playerBody.Find("Fire_Light");
                // 如果找到了Fire_Light对象且它是激活的，说明玩家有跟随光源
                if (fireLightTransform != null && fireLightTransform.gameObject.activeSelf)
                {
                    hasSun = true;
                    GameNotification.ShowByTrigger("Level2", "开场日组建说明");
                }
            }
        }

        if (hasSun)
        {
            GameNotification.ShowByTrigger("Level2", "开场石碑信息");
        }
        step = Step.LearnVineProperties;
    }

    void Update()
    {
        switch (step)
        {
            case Step.LearnVineProperties:
                // 检测玩家是否已经从老藤理解了特性（Long、Flexible或Thin）
                if (player != null && player.empowermentAbility != null)
                {
                    bool hasLearnedVineProperty = false;
                    for (int i = 0; i < player.empowermentAbility.propertySlots.Length; i++)
                    {
                        ObjectProperty prop = player.empowermentAbility.propertySlots[i];
                        if (prop == ObjectProperty.Long || prop == ObjectProperty.Flexible || prop == ObjectProperty.Thin)
                        {
                            hasLearnedVineProperty = true;
                            break;
                        }
                    }
                    
                    if (hasLearnedVineProperty)
                    {
                        
                        step = Step.GetFeather;
                        break;
                    }
                }
                
                // 只在第一次靠近老藤时显示一次提示，告诉玩家如何选择特性
                if (oldVine != null && player.currentInteractTarget == oldVine && !hasShownVinePropertyHint)
                {
                    GameNotification.ShowByTrigger("Level2", "第一次注视老藤时的提示1");
                    hasShownVinePropertyHint = true;
                }
                break;

            case Step.GetFeather:
                if (hasFeather)
                {
                    GameNotification.ShowByTrigger("Level2", "获得羽毛后");
                    step = Step.GetWood;
                }
                break;

            case Step.GetWood:
                if (hasWood)
                {
                    if (!hasSun)
                    {
                        GameNotification.ShowByTrigger("Level2", "获得木组件，但没有日组件");
                        step = Step.GetSun;
                    }
                    else
                    {
                        GameNotification.ShowByTrigger("Level2", "获得木组件且已有日组件");
                        step = Step.ChargeTablet;
                    }
                }
                break;

            case Step.GetSun:
                if (hasSun)
                {
                    GameNotification.ShowByTrigger("Level2", "获得日组件后");
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
                    
                    GameNotification.ShowByTrigger("Level2", "三个组件全部充能完成");
                    step = Step.Complete;
                    TriggerCutscene("SpringComplete");
                    TriggerLevelComplete();
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
                            // 使用动态消息
                            string progressMessage = $"还需要充能：{missing.Trim()} ({chargedCount}/3)";
                            GameNotification.ShowRaw(progressMessage);
                        }
                    }
                }
                break;
        }
    }

    public void OnFeatherObtained()
    {
        hasFeather = true;
        GameNotification.ShowByTrigger("Level2", "拾取羽毛时");
    }

    public void OnWoodObtained()
    {
        hasWood = true;
        GameNotification.ShowByTrigger("Level2", "拾取木组件时");
    }

    public void OnSunObtained()
    {
        hasSun = true;
        GameNotification.ShowByTrigger("Level2", "拾取日组件时");
    }

    /// <summary>
    /// 充能羽毛到石碑
    /// </summary>
    public void ChargeFeather()
    {
        if (featherCharged)
        {
            GameNotification.ShowByTrigger("Level2", "羽毛重复充能");
            return;
        }
        
        featherCharged = true;
        GameNotification.ShowByTrigger("Level2", "羽毛充能成功");
        
        // 点亮"羽"文字部分
        if (tabletTextEffect != null)
        {
            tabletTextEffect.LightUpFeatherText();
        }
        else
        {
            Debug.LogWarning("Level2Manager: tabletTextEffect未指定，无法点亮文字");
        }
        
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
            GameNotification.ShowByTrigger("Level2", "木组件重复充能");
            return;
        }
        
        woodCharged = true;
        GameNotification.ShowByTrigger("Level2", "木组件充能成功");
        
        // 点亮"木"文字部分
        if (tabletTextEffect != null)
        {
            tabletTextEffect.LightUpWoodText();
        }
        else
        {
            Debug.LogWarning("Level2Manager: tabletTextEffect未指定，无法点亮文字");
        }
        
        CheckAllComponentsCharged();
    }

    /// <summary>
    /// 充能日组件到石碑（使用光能量特性，不需要手持物品）
    /// </summary>
    public void ChargeSun()
    {
        if (sunCharged)
        {
            GameNotification.ShowByTrigger("Level2", "日组件重复充能");
            return;
        }
        
        if (!hasSun)
        {
            GameNotification.ShowByTrigger("Level2", "缺少日组件尝试充能");
            return;
        }
        
        sunCharged = true;
        GameNotification.ShowByTrigger("Level2", "日组件充能成功");
        
        // 点亮"日"文字部分
        if (tabletTextEffect != null)
        {
            tabletTextEffect.LightUpSunText();
        }
        else
        {
            Debug.LogWarning("Level2Manager: tabletTextEffect未指定，无法点亮文字");
        }
        
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

        // 检查是否是木组件（woodpart 或 woodthin）
        // 通过名称判断，或者可以添加特定的组件标记
        if (!hasWood && (target.name.Contains("WoodPart") || target.name.Contains("WoodThin") || target.name.Contains("Wood")))
        {
            // 检查是否是可拾取的木组件（排除原始的不可拾取的木头）
            if (target.canBePickedUp)
            {
                OnWoodObtained();
                return;
            }
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
        
        // 检查是否对着有WoodSoftEffect的木头按Q键（用于折断木头）
        if (selectedSlot == -1)  // 空手按Q键
        {
            var woodSoftEffect = target.GetComponent<WoodSoftEffect>();
            if (woodSoftEffect != null)
            {
                woodSoftEffect.TryBreak();
                return;
            }
        }
        
        // 如果玩家对着石碑交互（按Q键），检查是否手持组件并充能，或空手充能日组件
        if (target == stoneTablet && player != null && player.playerHoldItem != null)
        {
            GameObject heldItem = player.playerHoldItem.heldObject;
            
            // 如果手持物品，尝试充能羽毛或木组件
            if (heldItem != null)
            {
                TryChargeComponent(heldItem);
            }
            // 如果没有手持物品（空手），检查玩家是否有日组件（从上一关继承的跟随光源），如果有就直接充能日组件
            else
            {
                // 玩家空手按Q键，检查是否有日组件（跟随光源是上一关的奖励）
                if (hasSun && !sunCharged)
                {
                    // 玩家拥有日组件（跟随光源），空手按Q键即可充能日组件
                    ChargeSun();
                }
                else if (sunCharged)
                {
                    GameNotification.ShowByTrigger("Level2", "日组件重复充能");
                }
                else
                {
                    GameNotification.ShowByTrigger("Level2", "缺少手持组件时提示");
                }
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

        // 检查是否是木组件（woodpart 或 woodthin）
        if (!woodCharged && (heldItem.name.Contains("WoodPart") || heldItem.name.Contains("WoodThin")))
        {
            ChargeWood();
            return;
        }

        // 日组件不需要手持，而是使用光能量特性（在HandlePlayerUse中处理）

        GameNotification.ShowByTrigger("Level2", "尝试充能但手持物品错误");
    }

    private void TriggerLevelComplete()
    {
        if (transitionTriggered) return;
        transitionTriggered = true;

        ShowConclusionAndLoad("Level2", "结语", "Level3", returnToLevel3Delay, "春的意义，即是唤醒世界上一切律动的能力。");
    }
}


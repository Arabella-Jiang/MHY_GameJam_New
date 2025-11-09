using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Level3Manager : LevelManager
{
    [Header("关卡目标引用")]
    public InteractableObject stoneTablet;      // 石碑（星字）
    public InteractableObject stone;            // 石头（可理解 Heavy/Hard）
    public InteractableObject snow;             // 雪（可理解 Cool/White）
    public InteractableObject iceCone;          // 冰锥（可理解 Hard/Transparent/Sharp）
    public InteractableObject deadPlant;        // 枯死的植物
    public InteractableObject iceSurface;       // 冰面（支持两种触发方式：1.冰锥交互 2.玩家变重）
    public InteractableObject largeStone;       // 大块石头（用于反射星光）
    
    [Header("石碑文字效果")]
    public StarTabletEffect tabletTextEffect; // 石碑文字点亮效果组件（合并了文字点亮和充能效果）

    [Header("收集状态")]
    public bool hasLife = false;                // 是否获得"生"组件
    public bool hasStarPoint = false;           // 是否获得"星点"组件

    [Header("充能状态")]
    public bool lifeCharged = false;            // "生"组件是否已充能到石碑
    public bool starPointCharged = false;       // "星点"组件是否已充能到石碑

    [Header("冰面状态")]
    private bool iceSurfaceTransformed = false; // 冰面是否已转换（防止重复触发）
    
    [Header("通关设置")]
    public float returnToMainMenuDelay = 3f;    // 通关后返回主界面的延迟时间（秒）

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

        ShowMessage("星光照亮寻找神灵的旅途，连世界也会为勇敢者歌唱赞歌");
        ShowMessage("石碑需要两个组件：生、星点。继续探索吧。");
        step = Step.LearnProperties;
    }

    void Update()
    {
        // 处理方式2：玩家变重站在冰面上（持续检测）
        if (iceSurface != null && !iceSurfaceTransformed && player != null)
        {
            CheckHeavyPlayerOnIce();
        }

        // 关卡流程逻辑
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
                    // 触发石碑最终效果
                    CombinationEffect effect = stoneTablet.GetComponent<CombinationEffect>();
                    if (effect != null)
                    {
                        effect.TriggerEffect();
                    }
                    else
                    {
                        Debug.LogWarning("StarTabletEffect component not found on stone tablet!");
                    }
                    
                    ShowMessage("石碑被点亮！星星,照应我们所处的位置在宇宙的何方,知道脚下在哪里,才明白未来何去何从。");
                    step = Step.Complete;
                    TriggerCutscene("StarComplete");
                    
                    // 延迟后返回主界面
                    StartCoroutine(ReturnToMainMenu());
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
        
        // 点亮"生"文字部分
        if (tabletTextEffect != null)
        {
            tabletTextEffect.LightUpLifeText();
        }
        else
        {
            Debug.LogWarning("Level3Manager: tabletTextEffect未指定，无法点亮文字");
        }
        
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
        
        // 点亮"星点"文字部分
        if (tabletTextEffect != null)
        {
            tabletTextEffect.LightUpStarPointText();
        }
        else
        {
            Debug.LogWarning("Level3Manager: tabletTextEffect未指定，无法点亮文字");
        }
        
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
        if (player == null)
        {
            Debug.LogWarning("Level3Manager: player is null");
            return;
        }
        
        if (player.playerHoldItem == null)
        {
            Debug.LogWarning("Level3Manager: player.playerHoldItem is null");
            return;
        }
        
        GameObject heldItem = player.playerHoldItem.heldObject;
        
        // 处理玩家使用冰锥凿开冰面（方式1：冰锥交互）
        // 玩家手持冰锥，对着冰面按Q键即可触发
        // 通过检查target是否有IceSurfaceEffect组件来判断是否是冰面（更可靠，不依赖引用匹配）
        Component iceSurfaceEffect = target.GetComponent("IceSurfaceEffect");
        bool isIceSurface = (iceSurfaceEffect != null) || (target == iceSurface);
        
        if (isIceSurface)
        {
            Debug.Log($"Level3Manager: 玩家对着冰面按Q键，目标: {target.name}，手持物品: {(heldItem != null ? heldItem.name : "null")}");
            
            if (heldItem != null)
            {
                // 检查手持物品是否有Hard属性
                TryBreakIceWithIceCone(heldItem, target);
            }
            else
            {
                ShowMessage("需要手持有Hard属性的物品才能凿开冰面");
            }
            return;
        }
        
        // 处理玩家对着石碑交互（按Q键），检查是否手持组件并充能
        if (target == stoneTablet)
        {
            // 如果手持物品，尝试充能"生"或"星点"组件
            if (heldItem != null)
            {
                TryChargeComponent(heldItem);
            }
            else
            {
                // 如果没有手持物品，提示玩家需要手持组件
                ShowMessage("需要手持组件才能充能石碑。请先拾取" + (hasLife ? "" : "\"生\"") + (hasStarPoint ? "" : (hasLife ? "或\"星点\"" : "或\"星点\"")) + "组件。");
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

    #region 冰面转换逻辑（两种触发方式）

    /// <summary>
    /// 方式1：尝试使用冰锥凿开冰面（由HandlePlayerUse调用）
    /// 玩家手持冰锥，对着冰面按Q键即可触发
    /// </summary>
    private void TryBreakIceWithIceCone(GameObject heldItem, InteractableObject targetIceSurface)
    {
        Debug.Log($"TryBreakIceWithIceCone: 开始处理，手持物品: {heldItem?.name}，目标冰面: {targetIceSurface?.name}");
        
        if (heldItem == null)
        {
            Debug.LogWarning("TryBreakIceWithIceCone: heldItem is null");
            return;
        }
        if (targetIceSurface == null)
        {
            Debug.LogWarning("TryBreakIceWithIceCone: targetIceSurface is null");
            return;
        }
        if (iceSurfaceTransformed)
        {
            Debug.Log("TryBreakIceWithIceCone: 冰面已经转换过了");
            return;
        }
        
        // 首先检查手持物品是否是冰锥（通过引用比较或名称匹配）
        InteractableObject heldItemIO = heldItem.GetComponent<InteractableObject>();
        if (heldItemIO == null)
        {
            Debug.LogWarning($"Level3Manager: 手持物品 {heldItem.name} 不是InteractableObject");
            return;
        }
        
        // 检查是否是冰锥（通过引用比较）
        bool isIceCone = (heldItem == iceCone?.gameObject) || (heldItemIO == iceCone);
        
        // 如果引用不匹配，尝试通过名称匹配（兼容性检查）
        if (!isIceCone && iceCone != null)
        {
            // 可以通过名称或其他方式匹配
            isIceCone = heldItem.name.Contains("IceCone") || heldItem.name.Contains("iceCone") || 
                        heldItem.name.Contains("冰锥");
        }
        
        if (!isIceCone)
        {
            ShowMessage("需要手持冰锥才能凿开冰面");
            Debug.Log($"TryBreakIceWithIceCone: 手持物品 {heldItem.name} 不是冰锥");
            return;
        }
        
        Debug.Log($"TryBreakIceWithIceCone: 确认是冰锥，检查属性，当前属性: {string.Join(", ", heldItemIO.currentProperties)}");
        
        // 检查冰锥是否有Hard属性
        if (!heldItemIO.currentProperties.Contains(ObjectProperty.Hard))
        {
            ShowMessage("冰锥还不够坚硬，无法凿开冰面");
            Debug.Log($"TryBreakIceWithIceCone: 冰锥没有Hard属性，无法凿开冰面");
            return;
        }
        
        Debug.Log("✅ 使用冰锥凿开冰面！");
        Debug.Log($"TryBreakIceWithIceCone: 准备调用TriggerIceSurfaceTransformation，targetIceSurface: {targetIceSurface?.name}");
        
        // 触发冰面转换效果（两种方式都会触发植物灌溉）
        TriggerIceSurfaceTransformation(targetIceSurface);
        
        Debug.Log("TryBreakIceWithIceCone: TriggerIceSurfaceTransformation调用完成");
    }

    /// <summary>
    /// 方式2：检测玩家变重后站在冰面上（由Update持续检测）
    /// </summary>
    private void CheckHeavyPlayerOnIce()
    {
        if (iceSurface == null) return;
        if (iceSurfaceTransformed) return;
        if (player == null) return;
        if (player.empowermentAbility == null) return;

        // 检查玩家的属性槽中是否有Heavy属性
        bool hasHeavy = false;
        for (int i = 0; i < player.empowermentAbility.propertySlots.Length; i++)
        {
            if (player.empowermentAbility.propertySlots[i] == ObjectProperty.Heavy)
            {
                hasHeavy = true;
                break;
            }
        }

        // 如果玩家没有Heavy属性，不检测
        if (!hasHeavy) return;

        // 检查玩家是否在冰面上
        // 方法1: 使用Collider的bounds检查（考虑Y轴高度差）
        Collider iceCollider = iceSurface.GetComponent<Collider>();
        bool playerOnIce = false;
        
        if (iceCollider != null)
        {
            Bounds iceBounds = iceCollider.bounds;
            Vector3 playerPos = player.transform.position;
            
            // 检查X和Z轴是否在bounds内，Y轴高度差不要太大（5f以内）
            bool inXZ = playerPos.x >= iceBounds.min.x && playerPos.x <= iceBounds.max.x &&
                       playerPos.z >= iceBounds.min.z && playerPos.z <= iceBounds.max.z;
            bool inY = Mathf.Abs(playerPos.y - iceBounds.center.y) <= 5f;
            
            playerOnIce = inXZ && inY;
        }

        if (!playerOnIce) return;

        Debug.Log("✅ 玩家变重且站在冰面上，触发冰面转换效果！");
        // 玩家变重了，触发冰面转换效果
        TriggerIceSurfaceTransformation(iceSurface);
    }

    /// <summary>
    /// 触发冰面转换效果（两种方式都调用这个方法）
    /// </summary>
    private void TriggerIceSurfaceTransformation(InteractableObject targetIceSurface)
    {
        Debug.Log($"[TriggerIceSurfaceTransformation] 方法被调用，targetIceSurface: {(targetIceSurface != null ? targetIceSurface.name : "null")}");
        
        if (targetIceSurface == null)
        {
            Debug.LogError("[TriggerIceSurfaceTransformation] targetIceSurface is null，无法继续");
            return;
        }
        
        if (iceSurfaceTransformed)
        {
            Debug.LogWarning("[TriggerIceSurfaceTransformation] 冰面已经转换过了，跳过");
            return;
        }

        Debug.Log($"[TriggerIceSurfaceTransformation] 开始触发效果，目标: {targetIceSurface.name}");

        // 方法1: 通过组件名称查找（使用反射）
        Component iceSurfaceEffect = targetIceSurface.GetComponent("IceSurfaceEffect");
        if (iceSurfaceEffect == null)
        {
            // 方法2: 尝试通过GameObject查找
            iceSurfaceEffect = targetIceSurface.gameObject.GetComponent("IceSurfaceEffect");
        }
        
        if (iceSurfaceEffect != null)
        {
            Debug.Log($"[TriggerIceSurfaceTransformation] ✅ 找到IceSurfaceEffect组件: {iceSurfaceEffect.GetType().Name}");
            
            // 直接转换为CombinationEffect调用（更可靠）
            CombinationEffect effect = iceSurfaceEffect as CombinationEffect;
            if (effect != null)
            {
                Debug.Log("[TriggerIceSurfaceTransformation] 通过CombinationEffect.TriggerEffect()触发");
                effect.TriggerEffect();
            }
            else
            {
                // 备用方案：使用SendMessage
                Debug.Log("[TriggerIceSurfaceTransformation] 使用SendMessage触发效果");
                iceSurfaceEffect.SendMessage("TriggerEffect", SendMessageOptions.DontRequireReceiver);
            }
            
            iceSurfaceTransformed = true;
            Debug.Log("[TriggerIceSurfaceTransformation] ✅ 冰面转换状态已更新");

            // 两种方式都会触发植物灌溉
            if (deadPlant != null)
            {
                Debug.Log("[TriggerIceSurfaceTransformation] ✅ 开始触发植物灌溉协程");
                StartCoroutine(TriggerPlantWateringAfterDelay());
            }
            else
            {
                Debug.LogError("[TriggerIceSurfaceTransformation] ❌ deadPlant is null，无法触发植物灌溉！请检查Level3Manager的deadPlant引用");
            }
        }
    }

    /// <summary>
    /// 延迟触发植物灌溉（等待冰面转换动画完成）
    /// </summary>
    private IEnumerator TriggerPlantWateringAfterDelay()
    {
        // 等待冰面转换动画完成（假设2秒）
        yield return new WaitForSeconds(2f);

        // 触发植物灌溉
        if (deadPlant != null)
        {
            Component plantRevival = deadPlant.GetComponent("PlantRevivalEffect");
            if (plantRevival != null)
            {
                plantRevival.SendMessage("StartWateringAnimation", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    #endregion

    /// <summary>
    /// 返回主界面
    /// </summary>
    private IEnumerator ReturnToMainMenu()
    {
        // 等待指定时间（让玩家看到通关信息）
        yield return new WaitForSeconds(returnToMainMenuDelay);
        
        Debug.Log("✅ Level3通关！返回主界面...");
        
        // 加载主界面场景
        SceneManager.LoadScene("MainMenu");
    }
}


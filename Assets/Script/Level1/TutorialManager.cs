using UnityEngine;
public class TutorialManager : MonoBehaviour
{
    public Player player;
    public EmpowermentAbility empowerment;

    [Header("关卡目标引用")]
    public InteractableObject rock;             // 可理解 Hard 的石头
    public BranchIgnition thinBranch;           // 细树枝（被点燃目标）
    public BranchIgnition thickBranch;          // 粗树枝（用于摩擦）
    public WaterHardEffect waterHard;           // 水流硬化效果
    public InteractableObject stoneTablet;      // 终点石碑
    public StoneTableEffect tabletTextEffect;   // 石碑文字点亮效果组件
    
    [Header("充能状态")]
    public bool fireCharged = false;            // Fire文字是否已充能
    public bool humanCharged = false;           // Human文字是否已充能

    private enum Step
    {
        Start,
        LearnHard,
        HardenWater,
        HardenBranches,
        Ignite,
        ChargeTablet,
        Complete
    }

    private Step step = Step.Start;
    private bool branchHintShown = false;
    private bool stoneHintShown = false;
    private bool stoneMessageShown = false;
    private bool transitionTriggered = false;

    void Start()
    {
        if (player == null) player = FindObjectOfType<Player>();
        if (empowerment == null && player != null) empowerment = player.empowermentAbility;
        
        // 自动查找 StoneTableEffect（如果没有手动指定）
        if (tabletTextEffect == null && stoneTablet != null)
        {
            tabletTextEffect = stoneTablet.GetComponent<StoneTableEffect>();
            if (tabletTextEffect == null)
            {
                Debug.LogWarning("TutorialManager: 石碑对象上找不到 StoneTableEffect 组件！");
            }
            else
            {
                Debug.Log("TutorialManager: 已找到 StoneTableEffect 组件");
            }
        }

        // 验证引用
        if (stoneTablet == null) Debug.LogWarning("TutorialManager: stoneTablet 引用为空！");
        if (player == null) Debug.LogWarning("TutorialManager: player 引用为空！");
        if (empowerment == null) Debug.LogWarning("TutorialManager: empowerment 引用为空！");

        ShowLevelMessage("夜幕将临");
        ShowLevelMessage("石碑说明");
        ShowLevelMessage("石碑指引");


        step = Step.LearnHard;
    }

    void Update()
    {
        if (!stoneHintShown && player != null)
        {
            if (player.currentInteractTarget == rock)
            {
                ShowLevelMessage("靠近石头");
                stoneHintShown = true;
            }
            else if (!stoneMessageShown && rock != null && IsPlayerFacing(rock.transform))
            {
                ShowLevelMessage("靠近石头");
                stoneHintShown = true;
                stoneMessageShown = true;
            }
        }

        if (!branchHintShown && player != null)
        {
            InteractableObject currentTarget = player.currentInteractTarget;
            if (currentTarget != null)
            {
                InteractableObject thinInteractable = thinBranch != null ? thinBranch.interactableObject : null;
                InteractableObject thickInteractable = thickBranch != null ? thickBranch.interactableObject : null;
                if (currentTarget == thinInteractable || currentTarget == thickInteractable)
                {
                    ShowLevelMessage("靠近树枝");
                    branchHintShown = true;
                }
            }
            else if (thinBranch != null && thickBranch != null)
            {
                if (IsPlayerFacing(thinBranch.transform) || IsPlayerFacing(thickBranch.transform))
                {
                    ShowLevelMessage("靠近树枝");
                    branchHintShown = true;
                }
            }
        }

        switch (step)
        {
            case Step.LearnHard:
                if (InventoryHas(ObjectProperty.Hard))
                {
                    ShowLevelMessage("石头特性获取成功");
                    ShowLevelMessage("获得Hard后");
                    step = Step.HardenWater;
                }
                break;

            case Step.HardenWater:
                if (waterHard != null && waterHard.canPass)
                {
                    if (!branchHintShown)
                    {
                        ShowLevelMessage("靠近树枝");
                        branchHintShown = true;
                    }
                    step = Step.HardenBranches;
                }
                break;

            case Step.HardenBranches:
                if (thinBranch != null && thickBranch != null && thinBranch.HasHardened() && thickBranch.HasHardened())
                {
                    ShowLevelMessage("两根树枝已硬化");
                    step = Step.Ignite;
                }
                break;

            case Step.Ignite:
                if (thinBranch != null && thinBranch.IsIgnited())
                {
                    ShowLevelMessage("细树枝已点燃说明");
                    ShowLevelMessage("充能提示");
                    step = Step.ChargeTablet;
                }
                break;

            case Step.ChargeTablet:
                // 检查玩家是否在石碑附近
                if (player != null && stoneTablet != null)
                {
                    float dist = Vector3.Distance(player.transform.position, stoneTablet.transform.position);
                    
                    // 检查玩家是否在石碑附近，并且当前交互目标是石碑
                    if (dist < 5f && player.currentInteractTarget == stoneTablet)
                    {
                        // 检查是否按了Q键（普通交互）
                        if (Input.GetKeyDown(KeyCode.Q))
                        {
                            HandleTabletCharge();
                        }
                    }
                }
                
                // 检查是否全部完成
                if (fireCharged && humanCharged)
                {
                    ShowLevelMessage("所有文字点亮");
                    TriggerLevelComplete();
                    step = Step.Complete;
                }
                break;

            case Step.Complete:
                break;
        }
    }

    /// <summary>
    /// 处理石碑充能逻辑
    /// </summary>
    private void HandleTabletCharge()
    {
        if (player == null || player.playerHoldItem == null) return;

        GameObject heldItem = player.playerHoldItem.heldObject;

        // 检查是否手持点燃的树枝
        if (heldItem != null && heldItem == thinBranch.gameObject)
        {
            // 检查树枝是否点燃
            if (thinBranch.IsIgnited())
            {
                ChargeFireText();
            }
            else
            {
                ShowLevelMessage("树枝未点燃，充能失败");
            }
        }
        // 检查是否空手
        else if (heldItem == null)
        {
            ChargeHumanText();
        }
        else
        {
            ShowLevelMessage("充能提示");
        }
    }

    /// <summary>
    /// 充能Fire文字（手持点燃的树枝）
    /// </summary>
    private void ChargeFireText()
    {
        if (fireCharged)
        {
            ShowLevelMessage("‘火’文字重复充能");
            return;
        }

        fireCharged = true;
        ShowLevelMessage("‘火’文字点亮");

        // 点亮Fire文字
        if (tabletTextEffect != null)
        {
            tabletTextEffect.LightUpFireText();
        }
        else
        {
            Debug.LogWarning("TutorialManager: tabletTextEffect未指定，无法点亮文字");
        }
    }

    /// <summary>
    /// 充能Human文字（空手）
    /// </summary>
    private void ChargeHumanText()
    {
        if (humanCharged)
        {
            ShowLevelMessage("‘人’文字重复充能");
            return;
        }

        humanCharged = true;
        ShowLevelMessage("‘人’文字点亮");

        // 点亮Human文字
        if (tabletTextEffect != null)
        {
            tabletTextEffect.LightUpHumanText();
        }
        else
        {
            Debug.LogWarning("TutorialManager: tabletTextEffect未指定，无法点亮文字");
        }
    }

    private bool HasProperty(InteractableObject obj, ObjectProperty prop)
    {
        return obj != null && obj.currentProperties != null && obj.currentProperties.Contains(prop);
    }

    private bool InventoryHas(ObjectProperty prop)
    {
        if (empowerment == null) return false;
        foreach (var p in empowerment.propertySlots)
        {
            if (p == prop) return true;
        }
        return false;
    }

    private void ShowLevelMessage(string trigger)
    {
        if (!GameNotification.ShowByTrigger("Level1", trigger))
        {
            Debug.LogWarning($"TutorialManager: 未找到触发消息 {trigger}");
        }
    }

    private void TriggerLevelComplete()
    {
        if (transitionTriggered) return;
        transitionTriggered = true;

        // 显示跳转消息
        GameNotification.ShowByTrigger("Level1", "跳转下一关");
        
        LevelManager.ShowConclusionAndLoadStatic("Level1", "结语", "Level2", 1.8f, "石碑两个文字部分都已点亮！教程完成！");
    }

    private bool IsPlayerFacing(Transform target)
    {
        if (player == null || target == null) return false;

        Vector3 toTarget = target.position - player.transform.position;
        toTarget.y = 0f;
        if (toTarget.sqrMagnitude < 0.0001f)
        {
            return false;
        }

        Vector3 forward = player.transform.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.0001f)
        {
            return false;
        }

        toTarget.Normalize();
        forward.Normalize();

        return Vector3.Dot(forward, toTarget) >= 0.5f;
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
                Log("警告：石碑对象上找不到 StoneTableEffect 组件！");
            }
            else
            {
                Log("已找到 StoneTableEffect 组件");
            }
        }

        // 验证引用
        if (stoneTablet == null) Log("警告：stoneTablet 引用为空！");
        if (player == null) Log("警告：player 引用为空！");
        if (empowerment == null) Log("警告：empowerment 引用为空！");

        Log("教程开始：靠近石头长按E理解 Hard（数字1切换使用）");
        step = Step.LearnHard;
    }

    void Update()
    {
        switch (step)
        {
            case Step.LearnHard:
                if (InventoryHas(ObjectProperty.Hard))
                {
                    Log("已获得 Hard。先对水面短按E进行硬化，穿过河流");
                    step = Step.HardenWater;
                }
                break;

            case Step.HardenWater:
                if (waterHard != null && waterHard.canPass)
                {
                    Log("水已硬化，可通过。接着对两根树枝分别短按E赋予 Hard");
                    step = Step.HardenBranches;
                }
                break;

            case Step.HardenBranches:
                if (thinBranch != null && thickBranch != null && thinBranch.HasHardened() && thickBranch.HasHardened())
                {
                    Log("两根树枝已变硬。空手对着任意树枝按Q进行摩擦点火");
                    step = Step.Ignite;
                }
                break;

            case Step.Ignite:
                if (thinBranch != null && thinBranch.IsIgnited())
                {
                    Log("细树枝已点燃。现在有两种方式充能石碑：");
                    Log("1. 拾起点燃的树枝（F键），带到石碑按Q → 点亮Fire文字");
                    Log("2. 空手走到石碑按Q → 点亮Human文字");
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
                    Log("✅ 石碑两个文字部分都已点亮！教程完成！");
                    step = Step.Complete;
                }
                break;

            case Step.Complete:
                // 教程完成，跳转到Level2场景
                Log("正在跳转到Level2...");
                StartCoroutine(LoadLevel2AfterDelay(1f));
                step = Step.Start; // 防止重复触发
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
                Log("树枝还没有点燃，无法充能");
            }
        }
        // 检查是否空手
        else if (heldItem == null)
        {
            ChargeHumanText();
        }
        else
        {
            Log("需要手持点燃的树枝或空手才能充能石碑");
        }
    }

    /// <summary>
    /// 充能Fire文字（手持点燃的树枝）
    /// </summary>
    private void ChargeFireText()
    {
        if (fireCharged)
        {
            Log("Fire文字已经点亮过了");
            return;
        }

        fireCharged = true;
        Log("✅ Fire文字已点亮！");

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
            Log("Human文字已经点亮过了");
            return;
        }

        humanCharged = true;
        Log("✅ Human文字已点亮！");

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

    private void Log(string msg)
    {
        Debug.Log($"[Tutorial] {msg}");
    }

    private IEnumerator LoadLevel2AfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Log($"尝试加载场景: Level2");
        SceneManager.LoadScene("Level2");
    }
}

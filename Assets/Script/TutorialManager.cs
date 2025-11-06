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
    public StoneTableEffect stoneTableEffect;   // 石碑效果组件（用于检测是否完成）

    private enum Step
    {
        Start,
        LearnHard,
        HardenWater,
        HardenBranches,
        Ignite,
        CarryBranchToTablet,
        Complete
    }

    private Step step = Step.Start;

    void Start()
    {
        if (player == null) player = FindObjectOfType<Player>();
        if (empowerment == null && player != null) empowerment = player.empowermentAbility;
        
        // 自动查找 StoneTableEffect（如果没有手动指定）
        if (stoneTableEffect == null && stoneTablet != null)
        {
            stoneTableEffect = stoneTablet.GetComponent<StoneTableEffect>();
            if (stoneTableEffect == null)
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
                    Log("细树枝已点燃。拾起细树枝（F键），带到石碑旁边并按Q键充能完成关卡");
                    step = Step.CarryBranchToTablet;
                }
                break;

            case Step.CarryBranchToTablet:
                // 检查玩家是否拿着点燃的细树枝，在石碑附近按Q键
                if (player != null && player.playerHoldItem != null && stoneTablet != null)
                {
                    bool holdingThin = player.playerHoldItem.heldObject != null && 
                                      player.playerHoldItem.heldObject == thinBranch.gameObject;
                    bool branchIsIgnited = thinBranch != null && thinBranch.IsIgnited();
                    
                    if (holdingThin && branchIsIgnited)
                    {
                        float dist = Vector3.Distance(player.transform.position, stoneTablet.transform.position);
                        
                        // 检查玩家是否在石碑附近，并且当前交互目标是石碑
                        if (dist < 5f && player.currentInteractTarget == stoneTablet)
                        {
                            // 检查是否按了Q键（普通交互）
                            if (Input.GetKeyDown(KeyCode.Q))
                            {
                                // 触发石碑充能效果
                                if (stoneTableEffect != null)
                                {
                                    stoneTableEffect.TriggerEffect();
                                    Log("已将点燃的树枝充能到石碑！触发石碑效果");
                                }
                                else
                                {
                                    Log("警告：StoneTableEffect 组件未找到，但仍标记为完成");
                                }
                                
                                Log("教程完成！");
                                step = Step.Complete;
                            }
                            else
                            {
                                // 每2秒提示一次
                                if (Time.frameCount % 120 == 0)
                                {
                                    Log("提示：按Q键对石碑进行充能");
                                }
                            }
                        }
                        else if (dist < 10f)
                        {
                            // 如果距离较远，提示靠近
                            if (Time.frameCount % 180 == 0) // 每3秒提示一次
                            {
                                Log("提示：靠近石碑（当前距离: " + dist.ToString("F1") + "米）");
                            }
                        }
                    }
                    else if (!holdingThin && branchIsIgnited)
                    {
                        // 提示拾起树枝
                        if (Time.frameCount % 180 == 0)
                        {
                            Log("提示：按F键拾起点燃的细树枝");
                        }
                    }
                }
                break;

            case Step.Complete:
                // 教程完成，跳转到Level2场景
                Log("正在跳转到Level2...");
                StartCoroutine(LoadLevel2AfterDelay(1f));
                break;
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
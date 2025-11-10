using UnityEngine;
using System.Collections;

/// <summary>
/// 植物复活效果（自动触发：湖面解冻后自动浇灌植物）
/// </summary>
public class PlantRevivalEffect : CombinationEffect
{
    [Header("植物对象")]
    public GameObject deadTree;                 // 枯死的植物（需要在Unity中手动指定）
    public GameObject treeLive;                 // 复活后的植物（需要在Unity中手动指定）
    
    [Header("视觉效果")]
    public ParticleSystem revivalEffect;        // 复活特效
    public Light plantLight;                    // 植物光源（复活后点亮）
    
    [Header("生组件花")]
    public GameObject lifeFlower;               // "生"组件花（植物复活后激活，带LifeCollector组件）

    private bool effectTriggered = false;
    private bool isRevived = false;

    void Start()
    {
        // 确保初始状态：deadTree激活，treeLive未激活
        if (deadTree != null)
        {
            deadTree.SetActive(true);
        }
        if (treeLive != null)
        {
            treeLive.SetActive(false);
        }
        
        // 确保"生"组件花初始状态为未激活
        if (lifeFlower != null)
        {
            lifeFlower.SetActive(false);
        }
    }

    public override void TriggerEffect()
    {
        // 这个效果不会通过属性组合触发，而是通过外部调用StartWateringAnimation()触发
        Debug.Log("PlantRevivalEffect: 需要通过StartWateringAnimation()方法触发");
    }

    /// <summary>
    /// 开始播放灌溉动画（由FrozenLakeEffect在冰化水动画完成后调用）
    /// </summary>
    public void StartWateringAnimation()
    {
        if (isRevived) return;

        // 启动协程处理完整的动画序列
        StartCoroutine(PlayWateringSequence());
    }

    /// <summary>
    /// 播放完整的灌溉动画序列
    /// </summary>
    private IEnumerator PlayWateringSequence()
    {
        // 先播放湖水汇入枯树过场动画，等待播放完成
        if (CutsceneManager.Instance != null)
        {
            bool waterFlowFinished = false;
            CutsceneManager.Instance.PlayCutscene("WaterFlowToTree", () => {
                waterFlowFinished = true;
            });
            
            // 等待过场动画播放完成
            while (!waterFlowFinished)
            {
                yield return null;
            }
        }
        else
        {
            // 如果没有CutsceneManager，等待一小段时间
            yield return new WaitForSeconds(2f);
        }

        // 播放枯树复活动画
        if (CutsceneManager.Instance != null)
        {
            bool treeRevivalFinished = false;
            CutsceneManager.Instance.PlayCutscene("TreeRevive", () => {
                treeRevivalFinished = true;
            });
            
            // 等待枯树复活动画播放完成
            while (!treeRevivalFinished)
            {
                yield return null;
            }
        }
        else
        {
            // 如果没有CutsceneManager，等待一小段时间
            yield return new WaitForSeconds(2f);
        }

        // 动画播放完成后，执行植物复活
        PerformRevival();
    }

    /// <summary>
    /// 执行植物复活
    /// </summary>
    private void PerformRevival()
    {
        // 播放复活特效
        if (revivalEffect != null)
        {
            revivalEffect.Play();
        }

        // 播放植物复苏音效
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayPlantRevivalSound();
        }

        // 点亮植物光源
        if (plantLight != null)
        {
            plantLight.enabled = true;
        }

        // 切换植物：deadTree inactive，treeLive active
        if (deadTree != null)
        {
            deadTree.SetActive(false);
        }
        if (treeLive != null)
        {
            treeLive.SetActive(true);
        }

        // 激活"生"组件花
        if (lifeFlower != null)
        {
            lifeFlower.SetActive(true);
            Debug.Log("✅ \"生\"组件花已激活！");
            
            // 确保花有LifeCollector组件
            LifeCollector collector = lifeFlower.GetComponent<LifeCollector>();
            if (collector == null)
            {
                collector = lifeFlower.AddComponent<LifeCollector>();
                Debug.Log("✅ 已为\"生\"组件花添加LifeCollector组件");
            }
            
            // 确保花有InteractableObject组件且可被拾取
            InteractableObject io = lifeFlower.GetComponent<InteractableObject>();
            if (io == null)
            {
                io = lifeFlower.AddComponent<InteractableObject>();
            }
            io.canBePickedUp = true;
        }
        else
        {
            Debug.LogWarning("PlantRevivalEffect: lifeFlower未指定，无法生成\"生\"组件");
        }

        // 注意：不在这里通知Level3Manager，而是在玩家拾取花时才通知
        // 这样玩家必须先拾取花才能获得"生"组件

        Debug.Log("✅ 植物复苏了！\"生\"组件花已出现。");
    }
}


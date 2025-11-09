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
    
    [Header("动画设置")]
    public Animator waterAnimation;             // 浇灌动画（可选）
    public string waterAnimationName = "WaterPlant"; // 浇灌动画名称
    
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

        // 播放浇灌动画
        StartCoroutine(PlayWaterAnimation());

        Debug.Log("✅ 开始播放浇灌动画，植物即将复苏！");
    }

    /// <summary>
    /// 播放浇灌动画
    /// </summary>
    private IEnumerator PlayWaterAnimation()
    {
        // 播放浇灌动画（如果有）
        if (waterAnimation != null && !string.IsNullOrEmpty(waterAnimationName))
        {
            waterAnimation.Play(waterAnimationName);
            Debug.Log($"播放浇灌动画: {waterAnimationName}");
            
            // 等待动画播放完成
            yield return new WaitForEndOfFrame(); // 等待一帧，确保动画开始播放
            yield return new WaitUntil(() => 
            {
                AnimatorStateInfo stateInfo = waterAnimation.GetCurrentAnimatorStateInfo(0);
                return stateInfo.IsName(waterAnimationName) && stateInfo.normalizedTime >= 1f;
            });
        }
        else
        {
            // 如果没有动画，等待一小段时间
            yield return new WaitForSeconds(1f);
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


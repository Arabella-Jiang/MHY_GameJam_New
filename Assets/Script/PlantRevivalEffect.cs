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

        // 通知Level3Manager
        Level3Manager level3 = FindObjectOfType<Level3Manager>();
        if (level3 != null)
        {
            level3.OnLifeObtained();
        }

        Debug.Log("✅ 植物复苏了！获得了\"生\"组件。");
    }
}


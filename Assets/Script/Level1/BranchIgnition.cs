using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchIgnition : MonoBehaviour
{
    [Header("分组设置")]
    public bool isThinBranch = true;                    // 细树枝：被点燃的目标
    public BranchIgnition otherBranch;                  // 成对的另一根树枝

    [Header("引用")]
    public InteractableObject interactableObject;       // 自身交互脚本
    public ParticleSystem fireEffect;                   // 点燃后的火焰特效（可为空）
    public Light fireLight;                             // 点燃后的光源（可为空）

    [Header("点火配置")]
    public float rubDuration = 1.0f;                    // 摩擦点火需要时间（占位）

    private bool isIgnited = false;
    private bool isIgniting = false;

    void Awake()
    {
        if (interactableObject == null)
        {
            interactableObject = GetComponent<InteractableObject>();
        }
    }

    public bool HasHardened()
    {
        return interactableObject != null && interactableObject.currentProperties != null &&
               interactableObject.currentProperties.Contains(ObjectProperty.Hard);
    }

    public bool IsIgnited()
    {
        return isIgnited;
    }

    // 由 Player 在空手短按E时调用
    public void TryIgnite()
    {
        if (isIgnited || isIgniting) return;
        if (otherBranch == null || interactableObject == null) return;

        // 两根树枝都需要是 Hard
        if (!HasHardened() || !otherBranch.HasHardened())
        {
            GameNotification.ShowByTrigger("Level1", "树枝摩擦失败");
            Debug.Log("需要两根树枝都变得坚硬后，再尝试摩擦点火");
            return;
        }

        // 只点燃细树枝
        BranchIgnition target = isThinBranch ? this : (otherBranch != null && otherBranch.isThinBranch ? otherBranch : null);
        if (target == null)
        {
            Debug.Log("未找到细树枝，无法点燃");
            return;
        }

        StartCoroutine(IgniteRoutine(target));
    }

    private IEnumerator IgniteRoutine(BranchIgnition target)
    {
        isIgniting = true;
        GameNotification.ShowRaw("开始摩擦生火...");
        Debug.Log("开始摩擦生火...");
        
        // 播放摩擦木头音效
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayRubWoodGrindIceSound();
        }
        
        yield return new WaitForSeconds(rubDuration);
        isIgniting = false;

        target.SetIgnited();
    }

    private void SetIgnited()
    {
        if (isIgnited) return;
        isIgnited = true;

        // 给细树枝添加可理解/可赋予的可燃特性
        if (interactableObject != null)
        {
            if (!interactableObject.currentProperties.Contains(ObjectProperty.Flammable))
            {
                interactableObject.currentProperties.Add(ObjectProperty.Flammable);
            }
            if (!interactableObject.understandableProperties.Contains(ObjectProperty.Flammable))
            {
                interactableObject.understandableProperties.Add(ObjectProperty.Flammable);
            }
        }

        // 视觉表现（占位）
        if (fireEffect != null) fireEffect.Play();
        if (fireLight != null) fireLight.enabled = true;
        
        // 播放柴火燃烧音效
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayFireBurnSound();
        }

        // 触发生火过场动画
        if (CutsceneManager.Instance != null)
        {
            CutsceneManager.Instance.PlayCutscene("FireIgnite");
        }

        GameNotification.ShowByTrigger("Level1", "细树枝已点燃说明");
        Debug.Log($"✅ 细树枝已被点燃，可理解/赋予 {GameLocalization.GetPropertyDisplayName(ObjectProperty.Flammable)}");
    }
}



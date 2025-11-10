using UnityEngine;
using System.Collections;

/// <summary>
/// 冰面转换效果（播放动画、替换材质、禁用碰撞体）
/// 冰面状态：玩家可以走在冰面上（有Collider）
/// 水面状态：玩家可以下沉到水中（禁用Collider）
/// </summary>
public class IceSurfaceEffect : CombinationEffect
{
    [Header("材质设置")]
    public Material targetMaterial;               // 转换后的材质（水面材质）
    public Renderer iceRenderer;                  // 冰面Renderer组件（如果不指定则自动查找）

    [Header("动画设置")]
    public Animator iceAnimation;                 // 冰面动画（可选）
    public string animationName = "IceTransform"; // 动画名称

    private bool isTransformed = false;           // 是否已经转换

    void Start()
    {
        // 如果未手动指定iceRenderer，自动查找
        if (iceRenderer == null)
        {
            iceRenderer = GetComponent<Renderer>();
            if (iceRenderer == null)
            {
                iceRenderer = GetComponentInChildren<Renderer>();
            }
        }
    }

    public override void TriggerEffect()
    {
        if (isTransformed) return;

        isTransformed = true;
        Debug.Log("✅ 冰面开始转换！");
        
        // 执行转换效果（使用协程处理动画序列）
        StartCoroutine(PerformTransformationSequence());
    }

    /// <summary>
    /// 执行转换效果序列（协程）
    /// </summary>
    private IEnumerator PerformTransformationSequence()
    {
        // 替换材质
        if (targetMaterial != null)
        {
            ReplaceMaterial(targetMaterial);
        }

        // 禁用碰撞体（让玩家可以下沉到水中）
        DisableColliders();
        
        // 播放冰面裂开过场动画，等待播放完成
        if (CutsceneManager.Instance != null)
        {
            bool iceBreakFinished = false;
            CutsceneManager.Instance.PlayCutscene("IceBreak", () => {
                iceBreakFinished = true;
            });
            
            // 等待 IceBreak 动画播放完成
            while (!iceBreakFinished)
            {
                yield return null;
            }
        }
        else
        {
            // 如果没有CutsceneManager，等待一小段时间
            yield return new WaitForSeconds(2f);
        }

        Debug.Log("✅ 冰面已转换！");

        // 等待 1.5 秒
        yield return new WaitForSeconds(1.5f);

        // 触发植物灌溉动画序列
        Level3Manager level3Manager = FindObjectOfType<Level3Manager>();
        if (level3Manager != null && level3Manager.deadPlant != null)
        {
            PlantRevivalEffect plantRevival = level3Manager.deadPlant.GetComponent<PlantRevivalEffect>();
            if (plantRevival != null)
            {
                Debug.Log("✅ 开始触发植物灌溉动画序列");
                plantRevival.StartWateringAnimation();
            }
            else
            {
                Debug.LogWarning("IceSurfaceEffect: 未找到 PlantRevivalEffect 组件");
            }
        }
        else
        {
            Debug.LogWarning("IceSurfaceEffect: 未找到 Level3Manager 或 deadPlant 引用");
        }
    }

    /// <summary>
    /// 替换材质
    /// </summary>
    private void ReplaceMaterial(Material newMaterial)
    {
        if (iceRenderer == null)
        {
            Debug.LogWarning("IceSurfaceEffect: 未找到Renderer组件！");
            return;
        }

        if (newMaterial != null)
        {
            iceRenderer.material = newMaterial;
            Debug.Log($"✅ 冰面材质已替换为: {newMaterial.name}");
        }
        else
        {
            Debug.LogWarning("IceSurfaceEffect: 未指定目标材质！");
        }
    }

    /// <summary>
    /// 禁用碰撞体（让玩家可以下沉到水中）
    /// </summary>
    private void DisableColliders()
    {
        // 查找所有非Trigger Collider并禁用它们
        Collider[] allColliders = GetComponents<Collider>();
        int disabledCount = 0;
        foreach (Collider col in allColliders)
        {
            // 只禁用非Trigger的Collider（冰面collider）
            // Trigger Collider保留（如果有用于检测的trigger）
            if (!col.isTrigger)
            {
                col.enabled = false;
                disabledCount++;
            }
        }

        if (disabledCount > 0)
        {
            Debug.Log($"✅ 已禁用 {disabledCount} 个碰撞体，玩家现在可以下沉到水中");
        }
    }
}

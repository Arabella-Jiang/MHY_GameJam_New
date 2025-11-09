using UnityEngine;

/// <summary>
/// 星字石碑充能效果（获得生、星点后充能）
/// 负责点亮石碑上的文字部分，并在所有组件充能完成后触发通关效果
/// </summary>
public class StarTabletEffect : CombinationEffect
{
    [Header("文字渲染器")]
    public Renderer lifeTextRenderer;          // "生"文字部分的Renderer（只有一个）
    public Transform starPointTextParent;      // "星点"文字部分的父对象（包含多个子对象）
    
    [Header("点亮材质")]
    public Material litMaterial;               // 文字点亮后的材质（"生"和"星点"共用）

    private Renderer[] starPointRenderers;     // "星点"文字的所有Renderer
    private bool lifeTextLit = false;
    private bool starPointTextLit = false;
    private bool effectTriggered = false;

    void Start()
    {
        // 查找"星点"文字的所有Renderer
        if (starPointTextParent != null)
        {
            starPointRenderers = starPointTextParent.GetComponentsInChildren<Renderer>();
            
            if (starPointRenderers != null && starPointRenderers.Length > 0)
            {
                Debug.Log($"✅ 找到 {starPointRenderers.Length} 个\"星点\"文字Renderer");
            }
        }
    }

    /// <summary>
    /// 点亮"生"文字部分
    /// </summary>
    public void LightUpLifeText()
    {
        if (lifeTextLit)
        {
            Debug.LogWarning("StarTabletEffect: \"生\"文字已经点亮过了");
            return;
        }

        if (lifeTextRenderer == null)
        {
            Debug.LogError("StarTabletEffect: lifeTextRenderer未指定！请在Inspector中指定\"生\"文字的Renderer。");
            return;
        }

        if (litMaterial == null)
        {
            Debug.LogError("StarTabletEffect: litMaterial未指定！请在Inspector中指定点亮后的材质。");
            return;
        }

        lifeTextRenderer.material = litMaterial;
        lifeTextLit = true;
        Debug.Log("✅ \"生\"文字部分已点亮！");
    }

    /// <summary>
    /// 点亮"星点"文字部分（点亮父对象下的所有子对象）
    /// </summary>
    public void LightUpStarPointText()
    {
        if (starPointTextLit)
        {
            Debug.LogWarning("StarTabletEffect: \"星点\"文字已经点亮过了");
            return;
        }

        if (starPointTextParent == null)
        {
            Debug.LogError("StarTabletEffect: starPointTextParent未指定！请在Inspector中指定\"星点\"文字的父对象Transform。");
            return;
        }

        if (litMaterial == null)
        {
            Debug.LogError("StarTabletEffect: litMaterial未指定！请在Inspector中指定点亮后的材质。");
            return;
        }

        // 如果还没有获取Renderer，重新获取
        if (starPointRenderers == null || starPointRenderers.Length == 0)
        {
            starPointRenderers = starPointTextParent.GetComponentsInChildren<Renderer>();
        }

        if (starPointRenderers == null || starPointRenderers.Length == 0)
        {
            Debug.LogError("StarTabletEffect: 在starPointTextParent下未找到任何Renderer！请检查父对象及其子对象是否有Renderer组件。");
            return;
        }

        // 点亮所有"星点"文字的Renderer
        int litCount = 0;
        foreach (Renderer renderer in starPointRenderers)
        {
            if (renderer != null)
            {
                renderer.material = litMaterial;
                litCount++;
            }
        }

        starPointTextLit = true;
        Debug.Log($"✅ \"星点\"文字部分已点亮！（共 {litCount} 个Renderer）");
    }

    /// <summary>
    /// 检查是否所有文字都已点亮
    /// </summary>
    public bool AreAllTextsLit()
    {
        return lifeTextLit && starPointTextLit;
    }

    /// <summary>
    /// 触发石碑最终效果（所有组件充能完成后调用）
    /// </summary>
    public override void TriggerEffect()
    {
        if (effectTriggered) return;

        effectTriggered = true;

        Debug.Log("✅ 石碑被点亮！星星,照应我们所处的位置在宇宙的何方,知道脚下在哪里,才明白未来何去何从。");
    }
}


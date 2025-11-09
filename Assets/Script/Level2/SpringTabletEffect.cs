using UnityEngine;

/// <summary>
/// 春字石碑充能效果（获得木、羽、日后充能）
/// 负责点亮石碑上的文字部分，并在所有组件充能完成后触发通关效果
/// </summary>
public class SpringTabletEffect : CombinationEffect
{
    [Header("文字渲染器")]
    public Renderer featherTextRenderer;       // "羽"文字部分的Renderer（只有一个）
    public Transform woodTextParent;            // "木"文字部分的父对象（包含多个子对象）
    public Transform sunTextParent;             // "日"文字部分的父对象（包含多个子对象）
    
    [Header("点亮材质")]
    public Material litMaterial;               // 文字点亮后的材质（木、羽、日共用）

    [Header("石碑效果")]
    public Light tabletLight;                  // 石碑光源
    public ParticleSystem springEffect;         // 春天特效（花草生长等）

    private Renderer[] woodTextRenderers;      // "木"文字的所有Renderer
    private Renderer[] sunTextRenderers;       // "日"文字的所有Renderer
    private bool featherTextLit = false;
    private bool woodTextLit = false;
    private bool sunTextLit = false;
    private bool effectTriggered = false;

    void Start()
    {
        // 查找"木"文字的所有Renderer
        if (woodTextParent != null)
        {
            woodTextRenderers = woodTextParent.GetComponentsInChildren<Renderer>();
            
            if (woodTextRenderers != null && woodTextRenderers.Length > 0)
            {
                Debug.Log($"✅ 找到 {woodTextRenderers.Length} 个\"木\"文字Renderer");
            }
        }

        // 查找"日"文字的所有Renderer
        if (sunTextParent != null)
        {
            sunTextRenderers = sunTextParent.GetComponentsInChildren<Renderer>();
            
            if (sunTextRenderers != null && sunTextRenderers.Length > 0)
            {
                Debug.Log($"✅ 找到 {sunTextRenderers.Length} 个\"日\"文字Renderer");
            }
        }
    }

    /// <summary>
    /// 点亮"羽"文字部分
    /// </summary>
    public void LightUpFeatherText()
    {
        if (featherTextLit)
        {
            Debug.LogWarning("SpringTabletEffect: \"羽\"文字已经点亮过了");
            return;
        }

        if (featherTextRenderer == null)
        {
            Debug.LogError("SpringTabletEffect: featherTextRenderer未指定！请在Inspector中指定\"羽\"文字的Renderer。");
            return;
        }

        if (litMaterial == null)
        {
            Debug.LogError("SpringTabletEffect: litMaterial未指定！请在Inspector中指定点亮后的材质。");
            return;
        }

        // 使用materials数组来设置材质（支持多个材质的情况）
        Material[] materials = new Material[featherTextRenderer.materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = litMaterial;
        }
        featherTextRenderer.materials = materials;
        
        featherTextLit = true;
        Debug.Log("✅ \"羽\"文字部分已点亮！");
    }

    /// <summary>
    /// 点亮"木"文字部分（点亮父对象下的所有子对象）
    /// </summary>
    public void LightUpWoodText()
    {
        if (woodTextLit)
        {
            Debug.LogWarning("SpringTabletEffect: \"木\"文字已经点亮过了");
            return;
        }

        if (woodTextParent == null)
        {
            Debug.LogError("SpringTabletEffect: woodTextParent未指定！请在Inspector中指定\"木\"文字的父对象Transform。");
            return;
        }

        if (litMaterial == null)
        {
            Debug.LogError("SpringTabletEffect: litMaterial未指定！请在Inspector中指定点亮后的材质。");
            return;
        }

        // 如果还没有获取Renderer，重新获取
        if (woodTextRenderers == null || woodTextRenderers.Length == 0)
        {
            woodTextRenderers = woodTextParent.GetComponentsInChildren<Renderer>();
        }

        if (woodTextRenderers == null || woodTextRenderers.Length == 0)
        {
            Debug.LogError("SpringTabletEffect: 在woodTextParent下未找到任何Renderer！请检查父对象及其子对象是否有Renderer组件。");
            return;
        }

        // 点亮所有"木"文字的Renderer
        int litCount = 0;
        foreach (Renderer renderer in woodTextRenderers)
        {
            if (renderer != null)
            {
                // 使用materials数组来设置材质（支持多个材质的情况）
                Material[] materials = new Material[renderer.materials.Length];
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = litMaterial;
                }
                renderer.materials = materials;
                litCount++;
            }
        }

        woodTextLit = true;
        Debug.Log($"✅ \"木\"文字部分已点亮！（共 {litCount} 个Renderer）");
    }

    /// <summary>
    /// 点亮"日"文字部分（点亮父对象下的所有子对象）
    /// </summary>
    public void LightUpSunText()
    {
        if (sunTextLit)
        {
            Debug.LogWarning("SpringTabletEffect: \"日\"文字已经点亮过了");
            return;
        }

        if (sunTextParent == null)
        {
            Debug.LogError("SpringTabletEffect: sunTextParent未指定！请在Inspector中指定\"日\"文字的父对象Transform。");
            return;
        }

        if (litMaterial == null)
        {
            Debug.LogError("SpringTabletEffect: litMaterial未指定！请在Inspector中指定点亮后的材质。");
            return;
        }

        // 如果还没有获取Renderer，重新获取
        if (sunTextRenderers == null || sunTextRenderers.Length == 0)
        {
            sunTextRenderers = sunTextParent.GetComponentsInChildren<Renderer>();
        }

        if (sunTextRenderers == null || sunTextRenderers.Length == 0)
        {
            Debug.LogError("SpringTabletEffect: 在sunTextParent下未找到任何Renderer！请检查父对象及其子对象是否有Renderer组件。");
            return;
        }

        // 点亮所有"日"文字的Renderer
        int litCount = 0;
        foreach (Renderer renderer in sunTextRenderers)
        {
            if (renderer != null)
            {
                // 使用materials数组来设置材质（支持多个材质的情况）
                Material[] materials = new Material[renderer.materials.Length];
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = litMaterial;
                }
                renderer.materials = materials;
                litCount++;
            }
        }

        sunTextLit = true;
        Debug.Log($"✅ \"日\"文字部分已点亮！（共 {litCount} 个Renderer）");
    }

    /// <summary>
    /// 检查是否所有文字都已点亮
    /// </summary>
    public bool AreAllTextsLit()
    {
        return featherTextLit && woodTextLit && sunTextLit;
    }

    /// <summary>
    /// 触发石碑最终效果（所有组件充能完成后调用）
    /// </summary>
    public override void TriggerEffect()
    {
        if (effectTriggered) return;

        effectTriggered = true;

        // 点亮石碑
        if (tabletLight != null)
        {
            tabletLight.enabled = true;
        }

        // 播放春天特效
        if (springEffect != null)
        {
            springEffect.Play();
        }

        // 改变石碑材质，表示被点亮
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = new Color(1f, 0.95f, 0.8f); // 温暖的春天色调
        }

        Debug.Log("✅ 石碑被点亮！春的意义，即是唤醒世界上一切律动的能力。");
    }
}


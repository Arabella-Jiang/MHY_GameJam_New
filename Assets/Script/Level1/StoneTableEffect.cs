using UnityEngine;

public class StoneTableEffect : CombinationEffect
{
    [Header("通关效果")]
    public Light tabletLight; // 石碑光源
    public ParticleSystem fireEffect; // 火焰特效
    
    [Header("Fire文字部分（手持点燃的树枝）")]
    public Renderer fireTextRenderer;           // Fire文字的Renderer（单个）
    
    [Header("Human文字部分（空手）")]
    public Renderer humanTextRenderer;          // Human文字的Renderer（单个）
    
    [Header("点亮材质")]
    public Material litMaterial;                 // 点亮后的材质（Fire和Human共用）

    private bool fireTextLit = false;
    private bool humanTextLit = false;
    
    [Header("旧版字段（保留兼容性）")]
    public Renderer wordRenderer; // 旧版文字的Renderer组件
    public Material litWordMaterial; // 旧版点亮后的文字材质
    
    public override void TriggerEffect()
    {
        Debug.Log("✅ 文字被点亮！关卡通关！");
        
        // 点亮石碑（可选，如果不需要可以注释掉）
        if (tabletLight != null)
        {
            tabletLight.enabled = true;
        }
        
        // 播放火焰特效
        if (fireEffect != null)
        {
            fireEffect.Play();
        }
        
        // 改变文字材质，表示被点亮
        if (wordRenderer != null)
        {
            if (litWordMaterial != null)
            {
                // 如果指定了新的材质，使用新材质
                wordRenderer.material = litWordMaterial;
            }
            else
            {
                // 如果没有指定新材质，只改变颜色
                wordRenderer.material.color = Color.yellow;
            }
        }
        else
        {
            // 如果没有手动指定wordRenderer，尝试在子对象中查找
            Transform wordTransform = transform.Find("wenzi");
            if (wordTransform == null)
            {
                // 如果找不到"wenzi"，尝试搜索所有子对象中的Renderer
                Renderer[] renderers = GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    if (renderer.name.Contains("wenzi") || renderer.name.Contains("word") || renderer.name.Contains("文字"))
                    {
                        wordRenderer = renderer;
                        break;
                    }
                }
            }
            else
            {
                // 在wenzi对象或其子对象中查找Renderer
                wordRenderer = wordTransform.GetComponentInChildren<Renderer>();
            }
            
            // 如果找到了文字Renderer，应用材质
            if (wordRenderer != null)
            {
                if (litWordMaterial != null)
                {
                    wordRenderer.material = litWordMaterial;
                }
                else
                {
                    wordRenderer.material.color = Color.yellow;
                }
            }
            else
            {
                Debug.LogWarning("StoneTableEffect: 找不到文字Renderer！请在Inspector中手动指定wordRenderer。");
            }
        }
        
        // 触发通关事件
        // GameManager.Instance.CompleteLevel();
    }

    /// <summary>
    /// 点亮Fire文字（手持点燃的树枝按Q）
    /// </summary>
    public void LightUpFireText()
    {
        if (fireTextLit)
        {
            Debug.LogWarning("StoneTableEffect: Fire文字已经点亮过了");
            return;
        }

        if (fireTextRenderer == null)
        {
            Debug.LogError("StoneTableEffect: fireTextRenderer未指定！请在Inspector中指定Fire文字的Renderer。");
            return;
        }

        if (litMaterial == null)
        {
            Debug.LogError("StoneTableEffect: litMaterial未指定！请在Inspector中指定点亮后的材质。");
            return;
        }

        // 使用materials数组来设置材质（支持多个材质槽）
        Material[] materials = new Material[fireTextRenderer.materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = litMaterial;
        }
        fireTextRenderer.materials = materials;
        
        fireTextLit = true;
        Debug.Log("✅ Fire文字部分已点亮！");
    }

    /// <summary>
    /// 点亮Human文字（空手按Q）
    /// </summary>
    public void LightUpHumanText()
    {
        if (humanTextLit)
        {
            Debug.LogWarning("StoneTableEffect: Human文字已经点亮过了");
            return;
        }

        if (humanTextRenderer == null)
        {
            Debug.LogError("StoneTableEffect: humanTextRenderer未指定！请在Inspector中指定Human文字的Renderer。");
            return;
        }

        if (litMaterial == null)
        {
            Debug.LogError("StoneTableEffect: litMaterial未指定！请在Inspector中指定点亮后的材质。");
            return;
        }

        // 使用materials数组来设置材质（支持多个材质槽）
        Material[] materials = new Material[humanTextRenderer.materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = litMaterial;
        }
        humanTextRenderer.materials = materials;
        
        humanTextLit = true;
        Debug.Log("✅ Human文字部分已点亮！");
    }

    /// <summary>
    /// 检查是否所有文字都已点亮
    /// </summary>
    public bool AreAllTextsLit()
    {
        return fireTextLit && humanTextLit;
    }
}
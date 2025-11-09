using UnityEngine;
using System.Collections;

/// <summary>
/// 石头透明效果（大块石头 + Transparent属性 = 可以映射星光，获得"星点"组件）
/// </summary>
public class StoneTransparentEffect : CombinationEffect
{
    [Header("透明设置")]
    public Material transparentMaterial;        // 透明材质
    public float transparency = 0.5f;           // 透明度（0-1）
    
    [Header("星光反射")]
    public Light starLight;                     // 星光光源
    public GameObject starPointPrefab;          // "星点"组件Prefab
    public Transform starPointSpawnPoint;       // "星点"生成位置
    
    [Header("视觉效果")]
    public ParticleSystem reflectionEffect;     // 反射特效

    private bool effectTriggered = false;
    private bool starPointGenerated = false;

    public override void TriggerEffect()
    {
        if (effectTriggered) return;

        InteractableObject stone = GetComponent<InteractableObject>();
        if (stone == null) return;

        // 检查是否拥有Transparent属性
        if (!stone.currentProperties.Contains(ObjectProperty.Transparent))
        {
            Debug.LogWarning("StoneTransparentEffect: 石头还没有Transparent属性！");
            return;
        }

        effectTriggered = true;

        // 改变石头材质为透明/反射材质
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            if (transparentMaterial != null)
            {
                renderer.material = transparentMaterial;
            }
            else
            {
                // 创建一个透明/反射材质
                Material newMat = new Material(Shader.Find("Standard"));
                newMat.SetFloat("_Mode", 3); // Transparent mode
                newMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                newMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                newMat.SetInt("_ZWrite", 0);
                newMat.DisableKeyword("_ALPHATEST_ON");
                newMat.EnableKeyword("_ALPHABLEND_ON");
                newMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                newMat.renderQueue = 3000;
                newMat.SetFloat("_Metallic", 0.8f);
                newMat.SetFloat("_Glossiness", 0.9f);
                Color color = new Color(0.9f, 0.9f, 1f, transparency);
                newMat.color = color;
                renderer.material = newMat;
            }
        }

        Debug.Log("✅ 石头变得透明，可以映射星光了！");

        // 开始检测星光映射
        StartCoroutine(CheckStarLightReflection());
    }

    /// <summary>
    /// 检测星光映射
    /// </summary>
    private System.Collections.IEnumerator CheckStarLightReflection()
    {
        while (!starPointGenerated)
        {
            yield return new WaitForSeconds(1f); // 每秒检查一次
            
            // 如果效果已触发且还没有生成"星点"，生成它
            if (effectTriggered && !starPointGenerated)
            {
                GenerateStarPoint();
                break;
            }
        }
    }

    /// <summary>
    /// 生成"星点"组件
    /// </summary>
    private void GenerateStarPoint()
    {
        if (starPointGenerated) return;

        starPointGenerated = true;

        // 播放反射特效
        if (reflectionEffect != null)
        {
            reflectionEffect.Play();
        }

        // 生成"星点"组件
        if (starPointPrefab != null)
        {
            Vector3 spawnPos = starPointSpawnPoint != null ? starPointSpawnPoint.position : transform.position + Vector3.up * 1f;
            GameObject starPoint = Instantiate(starPointPrefab, spawnPos, Quaternion.identity);
            
            // 确保有StarPointCollector组件
            StarPointCollector collector = starPoint.GetComponent<StarPointCollector>();
            if (collector == null)
            {
                collector = starPoint.AddComponent<StarPointCollector>();
            }
            
            // 确保有InteractableObject组件
            InteractableObject io = starPoint.GetComponent<InteractableObject>();
            if (io == null)
            {
                io = starPoint.AddComponent<InteractableObject>();
            }
            io.canBePickedUp = true;
        }
        else
        {
            Debug.LogWarning("StoneTransparentEffect: 未指定\"星点\"组件Prefab！");
        }

        // 通知Level3Manager
        Level3Manager level3 = FindObjectOfType<Level3Manager>();
        if (level3 != null)
        {
            level3.OnStarPointObtained();
        }

        Debug.Log("✅ 石头映射了星光，获得了\"星点\"组件！");
    }
}


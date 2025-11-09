using UnityEngine;

/// <summary>
/// 雪面镜面效果（雪面 + Hard属性 + Transparent属性 = 镜面，可以反射星光，获得"星点"组件）
/// </summary>
public class SnowMirrorEffect : CombinationEffect
{
    [Header("镜面设置")]
    public Material mirrorMaterial;             // 镜面材质
    public float reflectionIntensity = 1f;      // 反射强度
    
    [Header("星光反射")]
    public Light starLight;                     // 星光光源（天空中的星星）
    public GameObject starPointPrefab;          // "星点"组件Prefab
    public Transform starPointSpawnPoint;       // "星点"生成位置
    
    [Header("视觉效果")]
    public ParticleSystem reflectionEffect;     // 反射特效

    private bool effectTriggered = false;
    private bool starPointGenerated = false;

    void Start()
    {
        // 初始状态是普通雪面
    }

    public override void TriggerEffect()
    {
        if (effectTriggered) return;

        InteractableObject snow = GetComponent<InteractableObject>();
        if (snow == null) return;

        // 检查是否同时拥有Hard和Transparent属性
        if (!snow.currentProperties.Contains(ObjectProperty.Hard) || 
            !snow.currentProperties.Contains(ObjectProperty.Transparent))
        {
            Debug.LogWarning("SnowMirrorEffect: 雪面需要同时拥有Hard和Transparent属性！");
            return;
        }

        effectTriggered = true;

        // 改变雪面材质为镜面
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            if (mirrorMaterial != null)
            {
                renderer.material = mirrorMaterial;
            }
            else
            {
                // 创建一个简单的镜面材质
                Material newMat = new Material(Shader.Find("Standard"));
                newMat.SetFloat("_Metallic", 0.9f);
                newMat.SetFloat("_Glossiness", 0.95f);
                newMat.color = new Color(0.9f, 0.9f, 1f, 1f); // 淡蓝色，像冰面
                renderer.material = newMat;
            }
        }

        Debug.Log("✅ 雪面变成了镜面，可以反射星光了！");

        // 开始检测星光反射
        StartCoroutine(CheckStarLightReflection());
    }

    /// <summary>
    /// 检测星光反射
    /// </summary>
    private System.Collections.IEnumerator CheckStarLightReflection()
    {
        while (!starPointGenerated)
        {
            // 检查是否有星光照射（可以通过检测天空中的光源或特定标记）
            // 这里简化处理，直接生成"星点"组件
            // 实际游戏中可能需要检测玩家视角、天空光源等
            
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
            Vector3 spawnPos = starPointSpawnPoint != null ? starPointSpawnPoint.position : transform.position + Vector3.up * 0.5f;
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
            Debug.LogWarning("SnowMirrorEffect: 未指定\"星点\"组件Prefab！");
        }

        // 通知Level3Manager
        Level3Manager level3 = FindObjectOfType<Level3Manager>();
        if (level3 != null)
        {
            level3.OnStarPointObtained();
        }

        Debug.Log("✅ 雪面反射了星光，获得了\"星点\"组件！");
    }
}


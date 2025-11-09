using UnityEngine;

/// <summary>
/// 雪面镜面效果（雪面 + Hard属性 + Transparent属性 = 镜面，可以反射星光，获得"星点"组件）
/// 效果：改变材质为镜面材质，并在物体上生成星星prefab（可拾取）
/// </summary>
public class SnowMirrorEffect : CombinationEffect
{
    [Header("镜面材质")]
    public Material mirrorMaterial;             // 镜面材质（如果为null，会创建默认镜面材质）
    
    [Header("星星Prefab")]
    public GameObject starPrefab;               // 星星Prefab（"gold star.prefab"）
    public float starSpawnHeight = 0.5f;        // 星星生成高度（相对于物体表面）

    private bool effectTriggered = false;
    private GameObject spawnedStar = null;      // 已生成的星星实例

    public override void TriggerEffect()
    {
        if (effectTriggered) return;

        InteractableObject snow = GetComponent<InteractableObject>();
        if (snow == null)
        {
            Debug.LogError("SnowMirrorEffect: 未找到InteractableObject组件！");
            return;
        }

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
                Debug.Log("✅ 雪面材质已更改为镜面材质！");
            }
           
        }
        else
        {
            Debug.LogWarning("SnowMirrorEffect: 未找到Renderer组件，无法更改材质！");
        }

        // 生成星星prefab
        SpawnStar();

        Debug.Log("✅ 雪面变成了镜面，可以反射星光，星星已出现在雪面上！");
    }

    /// <summary>
    /// 在雪面上生成星星prefab
    /// </summary>
    private void SpawnStar()
    {
        if (starPrefab == null)
        {
            Debug.LogError("SnowMirrorEffect: 未指定星星Prefab！请在Inspector中指定\"gold star.prefab\"");
            return;
        }

        // 如果已经生成过星星，不再重复生成
        if (spawnedStar != null)
        {
            Debug.LogWarning("SnowMirrorEffect: 星星已经生成过了！");
            return;
        }

        // 计算生成位置（物体表面上方）
        Vector3 spawnPosition = CalculateStarSpawnPosition();

        // 实例化星星prefab
        spawnedStar = Instantiate(starPrefab, spawnPosition, Quaternion.identity);

        // 确保星星有StarPointCollector组件
        StarPointCollector collector = spawnedStar.GetComponent<StarPointCollector>();
        if (collector == null)
        {
            collector = spawnedStar.AddComponent<StarPointCollector>();
            Debug.Log("✅ 已为星星添加StarPointCollector组件");
        }

        // 确保星星有InteractableObject组件且可被拾取
        InteractableObject io = spawnedStar.GetComponent<InteractableObject>();
        if (io == null)
        {
            io = spawnedStar.AddComponent<InteractableObject>();
        }
        io.canBePickedUp = true;

        Debug.Log($"✅ 星星已生成在雪面上！位置: {spawnPosition}");
    }

    /// <summary>
    /// 计算星星生成位置（固定Y值为154.3，使用物体的X和Z坐标）
    /// </summary>
    private Vector3 CalculateStarSpawnPosition()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // 使用Renderer的bounds计算X和Z坐标（中心点），Y坐标固定为154.3
            Bounds bounds = renderer.bounds;
            Vector3 center = bounds.center;
            return new Vector3(center.x, 154.3f, center.z);
        }
        else
        {
            // 如果没有Renderer，使用物体位置的X和Z，Y坐标固定为154.3
            return new Vector3(transform.position.x, 154.3f, transform.position.z);
        }
    }
}


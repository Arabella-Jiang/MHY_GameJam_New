using UnityEngine;

/// <summary>
/// 石头透明效果（大块石头 + Transparent属性 = 可以映射星光，获得"星点"组件）
/// 效果：改变材质为透明/反射材质，并在物体上生成星星prefab（可拾取）
/// </summary>
public class StoneTransparentEffect : CombinationEffect
{
    [Header("透明/反射材质")]
    public Material transparentMaterial;        // 透明/反射材质（如果为null，会创建默认透明材质）
    public float transparency = 0.5f;           // 透明度（0-1，仅在创建默认材质时使用）
    
    [Header("星星Prefab")]
    public GameObject starPrefab;               // 星星Prefab（"gold star.prefab"）
    public float starSpawnHeight = 1f;          // 星星生成高度（相对于物体表面）

    private bool effectTriggered = false;
    private GameObject spawnedStar = null;      // 已生成的星星实例

    public override void TriggerEffect()
    {
        if (effectTriggered) return;

        InteractableObject stone = GetComponent<InteractableObject>();
        if (stone == null)
        {
            Debug.LogError("StoneTransparentEffect: 未找到InteractableObject组件！");
            return;
        }

        // 检查是否拥有Transparent属性
        if (!stone.currentProperties.Contains(ObjectProperty.Transparent))
        {
            Debug.LogWarning("StoneTransparentEffect: 石头还没有Transparent属性！");
            return;
        }

        effectTriggered = true;

        // 播放第二个星空折射过场动画（石头变透明时）
        if (CutsceneManager.Instance != null)
        {
            CutsceneManager.Instance.PlayCutscene("SecondStarRefraction");
        }

        // 改变石头材质为透明/反射材质
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            if (transparentMaterial != null)
            {
                renderer.material = transparentMaterial;
                Debug.Log("✅ 石头材质已更改为透明/反射材质！");
            }
           
        }
        else
        {
            Debug.LogWarning("StoneTransparentEffect: 未找到Renderer组件，无法更改材质！");
        }

        // 生成星星prefab
        SpawnStar();

        Debug.Log("✅ 石头变得透明，可以映射星光，星星已出现在石头上！");
    }

    /// <summary>
    /// 在石头上生成星星prefab
    /// </summary>
    private void SpawnStar()
    {
        if (starPrefab == null)
        {
            Debug.LogError("StoneTransparentEffect: 未指定星星Prefab！请在Inspector中指定\"gold star.prefab\"");
            return;
        }

        // 如果已经生成过星星，不再重复生成
        if (spawnedStar != null)
        {
            Debug.LogWarning("StoneTransparentEffect: 星星已经生成过了！");
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

        Debug.Log($"✅ 星星已生成在石头上！位置: {spawnPosition}");
    }

    /// <summary>
    /// 计算星星生成位置（物体表面上方）
    /// </summary>
    private Vector3 CalculateStarSpawnPosition()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // 使用Renderer的bounds计算位置
            Bounds bounds = renderer.bounds;
            Vector3 center = bounds.center;
            Vector3 top = new Vector3(center.x, bounds.max.y, center.z);
            return top + Vector3.up * starSpawnHeight;
        }
        else
        {
            // 如果没有Renderer，使用物体位置上方
            return transform.position + Vector3.up * starSpawnHeight;
        }
    }
}


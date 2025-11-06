using UnityEngine;

/// <summary>
/// 春字石碑充能效果（获得木、羽、日后充能）
/// </summary>
public class SpringTabletEffect : CombinationEffect
{
    [Header("石碑效果")]
    public Light tabletLight;                  // 石碑光源
    public ParticleSystem springEffect;         // 春天特效（花草生长等）
    
    [Header("完成设置")]
    public GameObject[] flowersToSpawn;        // 要生成的花草对象
    public Transform flowerSpawnParent;       // 花草生成父节点

    private bool effectTriggered = false;

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

        // 生成花草（如果有配置）
        if (flowersToSpawn != null && flowersToSpawn.Length > 0)
        {
            SpawnFlowers();
        }

        // 改变石碑材质，表示被点亮
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = new Color(1f, 0.95f, 0.8f); // 温暖的春天色调
        }

        Debug.Log("✅ 石碑被点亮！春的意义，即是唤醒世界上一切律动的能力。");

        // 通知Level2Manager完成
        Level2Manager level2 = FindObjectOfType<Level2Manager>();
        if (level2 != null)
        {
            // level2.OnLevelComplete();
        }
    }

    private void SpawnFlowers()
    {
        if (flowerSpawnParent == null)
        {
            flowerSpawnParent = transform;
        }

        foreach (var flowerPrefab in flowersToSpawn)
        {
            if (flowerPrefab != null)
            {
                Vector3 spawnPos = flowerSpawnParent.position + Random.insideUnitSphere * 2f;
                spawnPos.y = flowerSpawnParent.position.y;
                Instantiate(flowerPrefab, spawnPos, Quaternion.identity, flowerSpawnParent);
            }
        }
    }
}


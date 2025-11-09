using UnityEngine;

/// <summary>
/// 星字石碑充能效果（获得生、星点后充能）
/// </summary>
public class StarTabletEffect : CombinationEffect
{
    [Header("石碑效果")]
    public Light tabletLight;                  // 石碑光源
    public ParticleSystem starEffect;          // 星星特效
    
    [Header("完成设置")]
    public GameObject[] starsToSpawn;         // 要生成的星星对象
    public Transform starSpawnParent;         // 星星生成父节点

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

        // 播放星星特效
        if (starEffect != null)
        {
            starEffect.Play();
        }

        // 生成星星（如果有配置）
        if (starsToSpawn != null && starsToSpawn.Length > 0)
        {
            SpawnStars();
        }

        // 改变石碑材质，表示被点亮
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = new Color(0.9f, 0.9f, 1f, 1f); // 淡蓝色，星星的色调
        }

        Debug.Log("✅ 石碑被点亮！星星,照应我们所处的位置在宇宙的何方,知道脚下在哪里,才明白未来何去何从。");

        // 通知Level3Manager完成
        Level3Manager level3 = FindObjectOfType<Level3Manager>();
        if (level3 != null)
        {
            // level3.OnLevelComplete();
        }
    }

    private void SpawnStars()
    {
        if (starSpawnParent == null)
        {
            starSpawnParent = transform;
        }

        foreach (var starPrefab in starsToSpawn)
        {
            if (starPrefab != null)
            {
                Vector3 spawnPos = starSpawnParent.position + Random.insideUnitSphere * 2f;
                spawnPos.y = starSpawnParent.position.y;
                Instantiate(starPrefab, spawnPos, Quaternion.identity, starSpawnParent);
            }
        }
    }
}


using UnityEngine;

/// <summary>
/// 木头变软效果（路线1：玩家使用Soft特性对woodoriginal，使其可以被折断）
/// 玩家按Q键可以折断木头，掉落woodpart
/// </summary>
public class WoodSoftEffect : CombinationEffect
{
    [Header("折断设置")]
    public GameObject woodPartPrefab;            // 掉落的木头部件（WoodPart.prefab）
    public Transform woodPartSpawnPoint;         // 木头部件生成位置（如果为空，使用木头的位置）
    public Vector3 woodPartSpawnOffset = Vector3.zero; // 生成位置的偏移
    
    [Header("折断效果")]
    public ParticleSystem breakEffect;           // 折断特效
    public AudioSource breakSound;               // 折断音效（可选）

    private bool effectTriggered = false;
    private bool canBreak = false;               // 是否可以被折断（获得Soft特性后为true）
    private GameObject spawnedWoodPart = null;   // 生成的木头部件

    public override void TriggerEffect()
    {
        if (effectTriggered) return;

        effectTriggered = true;
        canBreak = true;  // 木头获得Soft特性后，可以被折断

        Debug.Log("✅ 木头变软了！现在可以按Q键折断它。");
    }

    /// <summary>
    /// 尝试折断木头（由Level2Manager调用，当玩家按Q键时）
    /// </summary>
    public void TryBreak()
    {
        if (!canBreak)
        {
            Debug.LogWarning("木头还不能被折断，需要先赋予Soft特性");
            return;
        }

        BreakWood();
    }

    /// <summary>
    /// 折断木头，掉落woodpart
    /// </summary>
    private void BreakWood()
    {
        // 不检查是否已折断，允许多次折断掉落多个woodpart
        // if (spawnedWoodPart != null)
        // {
        //     Debug.LogWarning("木头已经被折断过了");
        //     return;
        // }

        // 播放折断特效
        if (breakEffect != null)
        {
            breakEffect.Play();
        }

        // 播放折断音效
        if (breakSound != null)
        {
            breakSound.Play();
        }

        // 生成woodpart
        SpawnWoodPart();

        // 原始木头保持存在（只是变软了，可以继续折断掉落更多woodpart）
        // 不需要 inactive 或隐藏

        Debug.Log("✅ 木头被折断了！掉落了木头部件。");
    }

    /// <summary>
    /// 生成可拾取的木头部件
    /// </summary>
    private void SpawnWoodPart()
    {
        if (woodPartPrefab == null)
        {
            Debug.LogError("WoodSoftEffect: woodPartPrefab未指定！请在Inspector中指定WoodPart prefab。");
            return;
        }

        // 计算生成位置
        Vector3 spawnPosition = CalculateWoodPartSpawnPosition();

        // 实例化木头部件
        spawnedWoodPart = Instantiate(woodPartPrefab, spawnPosition, Quaternion.identity);

        // 确保木头部件有InteractableObject组件且可被拾取
        InteractableObject woodPartIO = spawnedWoodPart.GetComponent<InteractableObject>();
        if (woodPartIO == null)
        {
            woodPartIO = spawnedWoodPart.AddComponent<InteractableObject>();
        }
        woodPartIO.canBePickedUp = true;

        Debug.Log($"✅ 木头部件已生成在位置: {spawnPosition}");
    }

    /// <summary>
    /// 计算木头部件的生成位置
    /// </summary>
    private Vector3 CalculateWoodPartSpawnPosition()
    {
        if (woodPartSpawnPoint != null)
        {
            return woodPartSpawnPoint.position + woodPartSpawnOffset;
        }
        else
        {
            // 使用木头的位置，稍微向下偏移
            return transform.position + woodPartSpawnOffset + Vector3.down * 1f;
        }
    }
}


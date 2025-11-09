using UnityEngine;

/// <summary>
/// "星点"组件标记组件（标记这是"星点"组件物品，用于在拾取时通知Level3Manager）
/// </summary>
public class StarPointCollector : MonoBehaviour
{
    [Header("视觉效果")]
    public ParticleSystem collectEffect;       // 收集特效
    public Light starLight;                     // 星光效果

    private Level3Manager level3Manager;

    void Start()
    {
        level3Manager = FindObjectOfType<Level3Manager>();
        
        // 确保物体可以被拾取
        InteractableObject io = GetComponent<InteractableObject>();
        if (io != null)
        {
            io.canBePickedUp = true;
        }
    }

    /// <summary>
    /// 当玩家拾取时调用（由PlayerHoldItem或Level3Manager调用）
    /// </summary>
    public void OnPickup()
    {
        // 播放收集特效
        if (collectEffect != null)
        {
            collectEffect.Play();
        }

        // 通知Level3Manager
        if (level3Manager != null)
        {
            level3Manager.OnStarPointObtained();
        }
        else
        {
            Debug.LogWarning("StarPointCollector: Level3Manager not found!");
        }

        Debug.Log("✅ 获得了\"星点\"组件！");
    }
}


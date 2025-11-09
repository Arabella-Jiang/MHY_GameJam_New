using UnityEngine;

/// <summary>
/// "生"组件标记组件（标记这是"生"组件物品，用于在拾取时通知Level3Manager）
/// </summary>
public class LifeCollector : MonoBehaviour
{
    [Header("视觉效果")]
    public ParticleSystem collectEffect;       // 收集特效（可选）

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
            level3Manager.OnLifeObtained();
        }
        else
        {
            Debug.LogWarning("LifeCollector: Level3Manager not found!");
        }

        Debug.Log("✅ 获得了\"生\"组件！");
    }
}


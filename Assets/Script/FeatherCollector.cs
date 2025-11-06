using UnityEngine;

/// <summary>
/// 羽毛标记组件（标记这是羽毛物品，用于在拾取时通知Level2Manager）
/// 玩家应该通过F键拾取，然后手持羽毛到石碑充能（短按E）
/// </summary>
public class FeatherCollector : MonoBehaviour
{
    [Header("视觉效果")]
    public ParticleSystem collectEffect;       // 收集特效（可选）

    private Level2Manager level2Manager;

    void Start()
    {
        level2Manager = FindObjectOfType<Level2Manager>();
        
        // 确保物体可以被拾取
        InteractableObject io = GetComponent<InteractableObject>();
        if (io != null)
        {
            io.canBePickedUp = true;
        }
    }

    /// <summary>
    /// 当玩家拾取时调用（由PlayerHoldItem或Level2Manager调用）
    /// </summary>
    public void OnPickup()
    {
        // 播放收集特效
        if (collectEffect != null)
        {
            collectEffect.Play();
        }

        // 通知Level2Manager
        if (level2Manager != null)
        {
            level2Manager.OnFeatherObtained();
        }

        Debug.Log("✅ 获得了羽毛！手持羽毛到石碑充能（短按E）");
    }
}


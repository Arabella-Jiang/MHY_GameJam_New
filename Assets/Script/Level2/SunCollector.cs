using UnityEngine;

/// <summary>
/// 日/光组件标记（标记这是日组件物品，用于在拾取时通知Level2Manager）
/// 玩家应该通过F键拾取（就像羽毛和木组件一样）
/// </summary>
public class SunCollector : MonoBehaviour
{
    [Header("视觉效果")]
    public ParticleSystem collectEffect;       // 收集特效
    public Light sunLight;                     // 光源效果

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
            level2Manager.OnSunObtained();
        }

        Debug.Log("✅ 获得了日组件！");
    }
}


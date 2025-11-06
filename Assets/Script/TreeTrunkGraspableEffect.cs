using UnityEngine;

/// <summary>
/// 树干变细到可抓住（获取木组件）
/// </summary>
public class TreeTrunkGraspableEffect : CombinationEffect
{
    [Header("树干设置")]
    public float thinScale = 0.2f;            // 变细后的缩放比例（比掉落鸟巢更细）
    
    [Header("可拾取设置")]
    public bool canBePickedUp = false;         // 是否可拾取（触发后变为true）

    private bool effectTriggered = false;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
        
        // 确保初始不可拾取
        InteractableObject io = GetComponent<InteractableObject>();
        if (io != null)
        {
            io.canBePickedUp = false;
        }
    }

    public override void TriggerEffect()
    {
        if (effectTriggered) return;

        effectTriggered = true;

        // 树干变得更细（X和Z轴缩放大幅减小）
        Vector3 newScale = originalScale;
        newScale.x *= thinScale;
        newScale.z *= thinScale;
        transform.localScale = newScale;

        // 允许拾取
        InteractableObject io = GetComponent<InteractableObject>();
        if (io != null)
        {
            io.canBePickedUp = true;
            canBePickedUp = true;
        }

        // 通知Level2Manager获得木组件
        Level2Manager level2 = FindObjectOfType<Level2Manager>();
        if (level2 != null)
        {
            level2.OnWoodObtained();
        }

        Debug.Log("✅ 树干变细到可以抓住！获得了木组件。");
    }
}


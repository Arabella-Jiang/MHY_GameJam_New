using UnityEngine;

/// <summary>
/// 石头变高效果（路径1：用老藤的"长"属性对石头使用）
/// </summary>
public class StoneLongEffect : CombinationEffect
{
    [Header("石头设置")]
    public Vector3 scaleMultiplier = new Vector3(3f, 5f, 3f);  // 缩放倍数 (x, y, z)
    public Vector3 originalScale;              // 原始缩放（自动保存）
    public float heightOffset = 0f;            // Y轴高度偏移（放大后手动调整，保持底部贴地）
    
    [Header("玩家站台")]
    public Transform playerStandPosition;      // 玩家可以站的位置

    private bool effectTriggered = false;
    private bool originalScaleSaved = false;
    private bool originalPositionSaved = false;
    private Vector3 originalPosition;

    void Start()
    {
        // 保存原始scale和position（在Start时）
        if (!originalScaleSaved)
        {
            originalScale = transform.localScale;
            originalScaleSaved = true;
        }
        
        if (!originalPositionSaved)
        {
            originalPosition = transform.position;
            originalPositionSaved = true;
        }
        
        if (playerStandPosition == null)
        {
            // 自动在石头顶部创建站台位置
            GameObject standPoint = new GameObject("PlayerStandPoint");
            standPoint.transform.SetParent(transform);
            standPoint.transform.localPosition = Vector3.up * (originalScale.y * 0.5f + 0.5f);
            playerStandPosition = standPoint.transform;
        }
    }

    public override void TriggerEffect()
    {
        if (effectTriggered) return;

        effectTriggered = true;

        // 确保已保存原始scale和position（如果还没保存，使用当前的）
        if (!originalScaleSaved)
        {
            originalScale = transform.localScale;
            originalScaleSaved = true;
        }
        
        if (!originalPositionSaved)
        {
            originalPosition = transform.position;
            originalPositionSaved = true;
        }

        // 处理物理组件，防止缩放时弹飞
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // 临时禁用物理
        }

        // 石头整体放大（x和z各3倍，y 5倍）
        // MeshCollider会自动跟随transform.scale变化
        // 其他Collider类型可能需要在Inspector中正确配置
        Vector3 newScale = new Vector3(
            originalScale.x * scaleMultiplier.x,
            originalScale.y * scaleMultiplier.y,
            originalScale.z * scaleMultiplier.z
        );
        
        // 应用缩放，只改变Y轴位置（保持X和Z不变）
        transform.localScale = newScale;
        Vector3 newPosition = originalPosition;
        newPosition.y += heightOffset;
        transform.position = newPosition;

        // 恢复物理状态（石头应该是静态的，保持kinematic）
        if (rb != null)
        {
            rb.isKinematic = true; // 石头变高后应该保持静态
            rb.useGravity = false; // 禁用重力，防止掉落
        }

        // 更新站台位置（使用新的scale计算）
        if (playerStandPosition != null)
        {
            playerStandPosition.localPosition = Vector3.up * (newScale.y * 0.5f + 0.5f);
        }

        // 设置石头为不可拾取（变大后不能拾取）
        InteractableObject interactable = GetComponent<InteractableObject>();
        if (interactable != null)
        {
            interactable.canBePickedUp = false;
        }

        Debug.Log("✅ 石头变高了！玩家可以站上去够到树冠。");
    }
}


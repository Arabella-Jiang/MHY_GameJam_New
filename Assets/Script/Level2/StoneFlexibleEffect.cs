using UnityEngine;

/// <summary>
/// 石头变软成跳板效果（路径2：用老藤的"柔韧"属性对石头使用）
/// 当玩家踩上去时，直接修改玩家的重力或跳跃参数，让玩家跳得更高
/// </summary>
public class StoneFlexibleEffect : CombinationEffect
{
    [Header("跳板设置")]
    public float gravityMultiplier = 0.5f;    // 重力倍数（越小，重力越小，跳得越高）
    public float jumpHeightMultiplier = 1.5f;  // 跳跃高度倍数（越大，跳得越高）

    [Header("视觉效果")]
    public ParticleSystem bounceEffect;      // 弹跳特效

    private bool effectTriggered = false;
    private Vector3 originalScale;
    
    // 玩家状态
    private PlayerMovement playerMovement;
    private bool playerOnSpringboard = false;
    private bool originalValuesSaved = false;  // 是否已保存原始值
    private float originalGravity;
    private float originalJumpHeight;

    public override void TriggerEffect()
    {
        if (effectTriggered) return;

        effectTriggered = true;

        // 视觉变化：石头看起来更柔软
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = new Color(0.8f, 0.8f, 0.9f); // 稍微偏蓝灰，表示柔软
        }

        // 添加弹簧效果（可选）
        // springJoint = gameObject.AddComponent<SpringJoint>();
        // springJoint.spring = 100f;
        // springJoint.damper = 10f;

        // 处理物理组件，防止石头掉落
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // 石头变软后应该保持静态
            rb.useGravity = false; // 禁用重力，防止掉落
        }

        // 设置石头为不可拾取（变软后不能拾取）
        InteractableObject interactable = GetComponent<InteractableObject>();
        if (interactable != null)
        {
            interactable.canBePickedUp = false;
        }

        // 确保石头的Collider不是Trigger（用于物理碰撞支撑玩家）
        Collider[] allColliders = GetComponents<Collider>();
        foreach (var col in allColliders)
        {
            col.isTrigger = false; // 确保不是Trigger，用于物理碰撞
        }

        Debug.Log("✅ 石头变得柔软，可以作为跳板！");
    }

    void Update()
    {
        if (!effectTriggered) return;

        // 检测玩家是否在石头上方
        CheckPlayerOnSpringboard();
    }

    void CheckPlayerOnSpringboard()
    {
        // 查找玩家
        if (playerMovement == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerMovement = player.GetComponent<PlayerMovement>();
                if (playerMovement == null)
                {
                    // 如果Player对象本身没有PlayerMovement，尝试在子对象中查找
                    playerMovement = player.GetComponentInChildren<PlayerMovement>();
                }
            }
        }

        if (playerMovement == null)
        {
            return;
        }

        // 检查玩家是否在石头上方（水平距离和垂直位置）
        Vector3 playerPos = playerMovement.transform.position;
        Vector3 stonePos = transform.position;
        
        // 水平距离（忽略Y轴）
        Vector3 horizontalDiff = new Vector3(playerPos.x - stonePos.x, 0, playerPos.z - stonePos.z);
        float horizontalDistance = horizontalDiff.magnitude;
        
        // 获取Collider的实际大小（更准确）
        Collider col = GetComponent<Collider>();
        float stoneRadius = 1f;
        float stoneHeight = 1f;
        
        if (col != null)
        {
            if (col is BoxCollider boxCol)
            {
                stoneRadius = Mathf.Max(boxCol.size.x, boxCol.size.z) * 0.5f * transform.lossyScale.x;
                stoneHeight = boxCol.size.y * 0.5f * transform.lossyScale.y;
            }
            else if (col is SphereCollider sphereCol)
            {
                stoneRadius = sphereCol.radius * transform.lossyScale.x;
                stoneHeight = stoneRadius;
            }
            else if (col is CapsuleCollider capsuleCol)
            {
                stoneRadius = capsuleCol.radius * transform.lossyScale.x;
                stoneHeight = capsuleCol.height * 0.5f * transform.lossyScale.y;
            }
            else
            {
                // 使用 transform.localScale 作为后备
                stoneRadius = Mathf.Max(transform.lossyScale.x, transform.lossyScale.z) * 0.5f;
                stoneHeight = transform.lossyScale.y * 0.5f;
            }
        }
        else
        {
            // 没有Collider，使用transform.localScale
            stoneRadius = Mathf.Max(transform.lossyScale.x, transform.lossyScale.z) * 0.5f;
            stoneHeight = transform.lossyScale.y * 0.5f;
        }
        
        // 石头顶部位置
        float stoneTopY = stonePos.y + stoneHeight;
        
        // 检查玩家是否在石头范围内（放宽条件）
        // 水平距离：在石头范围内（允许更大的容差）
        // 垂直位置：玩家在石头顶部附近（从顶部-1米到顶部+3米）
        bool isOnStone = horizontalDistance <= stoneRadius * 2f &&  // 放宽到2倍半径
                         playerPos.y >= stoneTopY - 1f && 
                         playerPos.y <= stoneTopY + 3f; // 允许玩家在石头顶部上方3米内

        if (isOnStone && !playerOnSpringboard)
        {
            // 玩家刚踩上石头
            playerOnSpringboard = true;
            originalValuesSaved = false;  // 重置标记，准备保存原始值
            ModifyPlayerMovement();
            
            if (bounceEffect != null) bounceEffect.Play();
            Debug.Log("✅ 玩家踩上跳板，重力减小，跳跃增强！");
        }
        else if (isOnStone && playerOnSpringboard)
        {
            // 玩家持续在石头上，持续应用修改
            ApplyPlayerMovementModification();
        }
        else if (!isOnStone && playerOnSpringboard)
        {
            // 玩家离开石头
            playerOnSpringboard = false;
            RestorePlayerMovement();
            
            Debug.Log("玩家离开跳板，恢复正常重力");
        }
    }

    void ModifyPlayerMovement()
    {
        if (playerMovement == null) return;

        // 只在第一次进入时保存原始值
        if (!originalValuesSaved)
        {
            // 通过反射获取原始重力值
            var gravityField = typeof(PlayerMovement).GetField("gravityValue", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (gravityField != null)
            {
                originalGravity = (float)gravityField.GetValue(playerMovement);
            }

            // 保存原始跳跃高度
            originalJumpHeight = playerMovement.jumpHeight;
            originalValuesSaved = true;
        }

        // 应用修改
        ApplyPlayerMovementModification();
    }

    void ApplyPlayerMovementModification()
    {
        if (playerMovement == null) return;

        // 通过反射修改重力值
        var gravityField = typeof(PlayerMovement).GetField("gravityValue", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (gravityField != null)
        {
            gravityField.SetValue(playerMovement, originalGravity * gravityMultiplier);
        }

        // 修改跳跃高度
        playerMovement.jumpHeight = originalJumpHeight * jumpHeightMultiplier;
    }

    void RestorePlayerMovement()
    {
        if (playerMovement == null) return;

        // 恢复重力
        var gravityField = typeof(PlayerMovement).GetField("gravityValue", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (gravityField != null && originalValuesSaved)
        {
            gravityField.SetValue(playerMovement, originalGravity);
        }

        // 恢复跳跃高度
        if (originalValuesSaved)
        {
            playerMovement.jumpHeight = originalJumpHeight;
        }

        // 重置标记和引用
        originalValuesSaved = false;
        playerMovement = null;
    }
}


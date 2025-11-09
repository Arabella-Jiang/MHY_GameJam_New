using UnityEngine;

/// <summary>
/// 树干变细效果（路径3：用"细"属性对树干使用，鸟巢掉落）
/// </summary>
public class TreeTrunkThinEffect : CombinationEffect
{
    [Header("树干设置")]
    public float thinScale = 0.3f;            // 变细后的缩放比例
    public Transform targetTree;             // 目标树 Transform（如果为空，自动查找 Tree_1）
    
    [Header("鸟巢")]
    public GameObject birdNest;               // 鸟巢对象
    public float nestFallDelay = 1f;          // 鸟巢掉落延迟
    public float groundCheckDistance = 0.1f;  // 地面检测距离
    public LayerMask groundLayer = -1;        // 地面层级

    private bool effectTriggered = false;
    private Vector3 originalScale;
    private Rigidbody nestRigidbody;
    private bool isMonitoringNest = false;
    private Transform targetTreeTransform; // 实际要变细的树（子对象 Tree_1）

    void Start()
    {
        // 如果手动指定了目标树，直接使用
        if (targetTree != null)
        {
            targetTreeTransform = targetTree;
        }
        else
        {
            // 自动查找：先找名为 "Tree_1" 的子对象
            targetTreeTransform = FindChildByName(transform, "Tree_1");
            
            // 如果没找到，尝试第一个子对象的第一个子对象
            if (targetTreeTransform == null && transform.childCount > 0)
            {
                Transform firstChild = transform.GetChild(0);
                if (firstChild.childCount > 0)
                {
                    targetTreeTransform = firstChild.GetChild(0);
                }
            }
            
            // 如果还没找到，使用当前物体
            if (targetTreeTransform == null)
            {
                targetTreeTransform = transform;
            }
        }
        
        originalScale = targetTreeTransform.localScale;
    }

    /// <summary>
    /// 递归查找子对象中指定名称的 Transform
    /// </summary>
    private Transform FindChildByName(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == name)
            {
                return child;
            }
            Transform found = FindChildByName(child, name);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }

    public override void TriggerEffect()
    {
        if (effectTriggered) return;

        effectTriggered = true;

        // 确保目标 Transform 已找到
        if (targetTreeTransform == null)
        {
            targetTreeTransform = FindChildByName(transform, "Tree_1");
            if (targetTreeTransform == null && transform.childCount > 0)
            {
                Transform firstChild = transform.GetChild(0);
                if (firstChild.childCount > 0)
                {
                    targetTreeTransform = firstChild.GetChild(0);
                }
            }
            if (targetTreeTransform == null)
            {
                targetTreeTransform = transform;
            }
            originalScale = targetTreeTransform.localScale;
        }
        
        // 树干变细（X和Z轴缩放减小）
        Vector3 newScale = originalScale;
        newScale.x *= thinScale;
        newScale.z *= thinScale;
        targetTreeTransform.localScale = newScale;

        // 延迟后鸟巢掉落
        StartCoroutine(DropBirdNest());

        Debug.Log("✅ 树干变细了！鸟巢即将掉落。");
    }

    private System.Collections.IEnumerator DropBirdNest()
    {
        yield return new WaitForSeconds(nestFallDelay);

        if (birdNest != null)
        {
            // 确保鸟巢有Collider且不是Trigger
            Collider nestCollider = birdNest.GetComponent<Collider>();
            if (nestCollider == null)
            {
                // 如果没有Collider，添加一个BoxCollider
                nestCollider = birdNest.AddComponent<BoxCollider>();
                Debug.LogWarning("鸟巢缺少Collider，已自动添加BoxCollider");
            }
            nestCollider.isTrigger = false; // 确保不是Trigger，用于物理碰撞

            // 检查MeshCollider，如果是非凸面的，设置为convex（Unity 5+要求）
            MeshCollider meshCollider = nestCollider as MeshCollider;
            if (meshCollider != null && !meshCollider.convex)
            {
                meshCollider.convex = true;
                Debug.Log("已将鸟巢的MeshCollider设置为convex");
            }

            // 添加物理组件让鸟巢掉落
            nestRigidbody = birdNest.GetComponent<Rigidbody>();
            if (nestRigidbody == null)
            {
                nestRigidbody = birdNest.AddComponent<Rigidbody>();
            }
            nestRigidbody.isKinematic = false;
            nestRigidbody.useGravity = true;
            // 设置一些物理参数，防止穿透
            nestRigidbody.mass = 1f;
            nestRigidbody.drag = 0.5f; // 增加阻力，防止过快下落

            // 如果鸟巢是目标树或其子对象的子对象，解除父子关系
            if (targetTreeTransform != null && IsChildOf(birdNest.transform, targetTreeTransform))
            {
                birdNest.transform.SetParent(null);
                Debug.Log("鸟巢已从目标树解除父子关系");
            }

            // 确保鸟巢可以被拾取
            InteractableObject nestInteractable = birdNest.GetComponent<InteractableObject>();
            if (nestInteractable != null)
            {
                nestInteractable.canBePickedUp = true;
            }

            // 开始监控鸟巢是否落地
            isMonitoringNest = true;
            StartCoroutine(MonitorNestLanding());

            Debug.Log("鸟巢掉落！");
        }
    }

    /// <summary>
    /// 监控鸟巢是否落地，落地后停止物理运动
    /// </summary>
    private System.Collections.IEnumerator MonitorNestLanding()
    {
        if (birdNest == null || nestRigidbody == null)
        {
            yield break;
        }

        float checkInterval = 0.1f; // 每0.1秒检查一次
        float maxVelocityThreshold = 0.5f; // 最大速度阈值，低于此值认为已落地

        while (isMonitoringNest && birdNest != null)
        {
            // 方法1: 检查是否在地面上（射线检测）
            RaycastHit hit;
            float rayDistance = groundCheckDistance + 0.1f;
            Vector3 rayOrigin = birdNest.transform.position;
            Vector3 rayDirection = Vector3.down;

            if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance, groundLayer))
            {
                // 检测到地面，检查速度
                if (nestRigidbody.velocity.magnitude < maxVelocityThreshold)
                {
                    // 鸟巢已落地，停止物理运动
                    StopNestFall();
                    yield break;
                }
            }

            // 方法2: 检查垂直速度，如果很小且向下，可能已经落地
            if (Mathf.Abs(nestRigidbody.velocity.y) < 0.1f && nestRigidbody.velocity.magnitude < maxVelocityThreshold)
            {
                // 再次用射线确认是否在地面附近
                if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance, groundLayer))
                {
                    StopNestFall();
                    yield break;
                }
            }

            // 防止无限下落：如果Y位置过低（比如低于-100），停止监控
            if (birdNest.transform.position.y < -100f)
            {
                Debug.LogWarning("鸟巢掉出地图，停止监控");
                isMonitoringNest = false;
                yield break;
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    /// <summary>
    /// 停止鸟巢下落，将其固定在当前位置
    /// </summary>
    private void StopNestFall()
    {
        if (birdNest == null || nestRigidbody == null)
        {
            return;
        }

        // 停止物理运动
        nestRigidbody.velocity = Vector3.zero;
        nestRigidbody.angularVelocity = Vector3.zero;
        nestRigidbody.isKinematic = true; // 设为kinematic，固定在当前位置
        
        isMonitoringNest = false;

        Debug.Log("✅ 鸟巢已落地，停止下落。现在可以拾取羽毛了！");

        // 确保鸟巢仍然可以被拾取
        InteractableObject nestInteractable = birdNest.GetComponent<InteractableObject>();
        if (nestInteractable != null)
        {
            nestInteractable.canBePickedUp = true;
        }
    }

    /// <summary>
    /// 检查 childTransform 是否是 parent 的子对象（包括子对象的子对象）
    /// </summary>
    private bool IsChildOf(Transform childTransform, Transform parent)
    {
        Transform current = childTransform;
        while (current != null)
        {
            if (current.parent == parent)
            {
                return true;
            }
            current = current.parent;
        }
        return false;
    }

    void OnDestroy()
    {
        // 清理时停止监控
        isMonitoringNest = false;
    }
}


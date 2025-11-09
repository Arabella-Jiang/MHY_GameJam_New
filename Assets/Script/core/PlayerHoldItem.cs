using UnityEngine;
using System.Collections;

public class PlayerHoldItem : MonoBehaviour
{
    [Header("手持设置")]
    public Transform holdPosition;
    
    [Header("动画设置")]
    public Animator playerAnimator; // 玩家模型的Animator组件（如果使用Animator）
    public Animation playerAnimation; // 玩家模型的Animation组件（Legacy动画，如果使用Legacy动画系统）
    public GameObject playerModelRoot; // 玩家模型根对象（PlayerBody或包含player贴图模型的GameObject）
    public AnimationClip pickupAnimationClip; // 拾取动画Clip（可以从player贴图.fbx中拖拽）
    public string pickupAnimationName = "take001"; // 拾取动画的名称（如果使用名称查找）
    
    [Header("当前状态")]
    public GameObject heldObject;
    public bool hasUnlockedEmpowerment = false;

    public bool PickupItem(GameObject item)
    {
        if (heldObject != null)
        {
            Debug.Log("手中已有物品，无法拾取新物品");
            return false;
        }
        
        InteractableObject interactable = item.GetComponent<InteractableObject>();
        if (interactable == null)
        {
            Debug.LogError("尝试拾取非交互物体");
            return false;
        }

        if (!interactable.canBePickedUp)
        {
            Debug.Log($"{item.name} 标记为不可拾取");
            return false;
        }

        if (item == null)
        {
            Debug.LogError("尝试拾取空物体");
            return false;
        }
        
        heldObject = item;
        item.transform.SetParent(holdPosition);
        item.transform.localPosition = Vector3.zero;
        
        // 检查是否是树枝（有BranchIgnition组件），如果是则旋转x轴-90度使其竖着
        BranchIgnition branch = item.GetComponent<BranchIgnition>();
        if (branch != null)
        {
            item.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        }
        // 检查是否是星星（有StarPointCollector组件），如果是则旋转x轴-90度使其竖着
        else if (item.GetComponent<StarPointCollector>() != null)
        {
            item.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        }
        else
        {
            item.transform.localRotation = Quaternion.identity;
        }
        
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        
        Collider collider = item.GetComponent<Collider>();
        if (collider != null) collider.enabled = false;
        
        Debug.Log($"物理拾取了: {item.name}");
        
        // 播放拾取动画
        PlayPickupAnimation();
        
        // 通知LevelManager（如果有）- 这会处理羽毛、木组件的收集逻辑
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.HandlePlayerPickup(interactable, hasUnlockedEmpowerment);
        }
        
        // 触发物品的拾取回调（用于羽毛等特殊物品）
        item.SendMessage("OnPickup", SendMessageOptions.DontRequireReceiver);
        
        return true;
    }

    public void DropItem()
    {
        if (heldObject == null) return;
        
        // 保存引用，因为heldObject会在后面置为null
        GameObject itemToDrop = heldObject;
        
        heldObject.transform.SetParent(null);
        
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        
        Collider collider = heldObject.GetComponent<Collider>();
        if (collider != null) collider.enabled = true;
        
        Debug.Log($"放下了: {itemToDrop.name}");
        heldObject = null;
        
        // 确保物体在放下后能被重新检测到
        StartCoroutine(ReenableObjectDetection(itemToDrop));
    }

    private IEnumerator ReenableObjectDetection(GameObject obj)
    {
        // 等待一帧确保物理系统更新
        yield return null;
        
        // 强制重新触发OnTriggerEnter
        Collider collider = obj.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
            yield return null;
            collider.enabled = true;
        }
    }

    public void UnlockEmpowerment()
    {
        hasUnlockedEmpowerment = true;
        Debug.Log("✅ 赋能能力已解锁！");
    }

    /// <summary>
    /// 等待并检查动画是否真的在播放
    /// </summary>
    private IEnumerator WaitAndCheckAnimation(Animation anim, string clipName, string objectName, System.Action onSuccess)
    {
        yield return null; // 等待一帧，让Play生效
        
        if (anim == null)
        {
            Debug.LogError($"Animation组件为null！");
            yield break;
        }
        
        // 检查动画是否在播放
        if (anim.isPlaying)
        {
            // 检查当前播放的clip名称
            if (anim.clip != null && (anim.clip.name == clipName || anim.clip.name == "Take 001"))
            {
                Debug.Log($"✅ 成功播放拾取动画: {clipName} (在 {objectName} 上)");
                onSuccess?.Invoke();
            }
            else
            {
                Debug.LogWarning($"⚠️ 动画正在播放，但clip名称不匹配。当前: {anim.clip?.name ?? "null"}, 期望: {clipName}");
                onSuccess?.Invoke(); // 仍然算成功
            }
        }
        else
        {
            Debug.LogError($"动画无法播放。请检查PlayerBody > Group的Animation组件是否已配置Take 001 clip。");
        }
    }

    /// <summary>
    /// 查找包含实际模型骨骼的GameObject
    /// 在Player prefab中，模型组是 PlayerBody > Group，这是包含所有骨骼和SkinnedMeshRenderer的根对象
    /// </summary>
    private GameObject FindModelRootWithSkeleton()
    {
        // 如果手动指定了模型根对象，优先使用
        if (playerModelRoot != null)
        {
            // 检查这个对象或其子对象是否有SkinnedMeshRenderer（表示有骨骼模型）
            if (playerModelRoot.GetComponentInChildren<SkinnedMeshRenderer>() != null)
            {
                return playerModelRoot;
            }
        }
        
        // 查找 PlayerBody > Group（这是包含所有骨骼模型的根对象）
        Transform playerBody = transform.Find("PlayerBody");
        if (playerBody != null)
        {
            Transform group = playerBody.Find("Group");
            if (group != null)
            {
                // Group是包含DeformationSystem, MotionSystem, FitSkeleton, Geometry的根对象
                // Geometry下包含实际的SkinnedMeshRenderer
                return group.gameObject;
            }
        }
        
        // 备用方案：在PlayerBody及其子对象中查找包含SkinnedMeshRenderer的对象
        if (playerBody != null)
        {
            SkinnedMeshRenderer skinnedMesh = playerBody.GetComponentInChildren<SkinnedMeshRenderer>();
            if (skinnedMesh != null)
            {
                // 返回包含骨骼的根对象（通常是模型的根）
                return skinnedMesh.transform.root.gameObject;
            }
        }
        
        // 如果都找不到，返回PlayerBody
        return playerBody != null ? playerBody.gameObject : gameObject;
    }

    /// <summary>
    /// 播放拾取动画
    /// </summary>
    private void PlayPickupAnimation()
    {
        bool animationPlayed = false;
        
        // 方法1: 如果直接指定了AnimationClip，使用Animation组件播放
        if (pickupAnimationClip != null)
        {
            // 找到包含实际骨骼模型的GameObject
            GameObject modelRoot = FindModelRootWithSkeleton();
            
            // 确保有Animation组件
            if (playerAnimation == null)
            {
                playerAnimation = modelRoot.GetComponent<Animation>();
                
                // 如果找不到，在模型根对象上添加Animation组件
                if (playerAnimation == null)
                {
                    playerAnimation = modelRoot.AddComponent<Animation>();
                    Debug.Log($"在 {modelRoot.name} 上添加了Animation组件");
                }
            }
            
            if (playerAnimation != null)
            {
                // 确保Animation组件已启用
                if (!playerAnimation.enabled)
                {
                    playerAnimation.enabled = true;
                }
                
                // 先停止当前动画
                playerAnimation.Stop();
                
                // 方法1: 尝试播放Animation组件中配置的默认clip（Inspector中配置的）
                if (playerAnimation.clip != null)
                {
                    playerAnimation.Play(playerAnimation.clip.name);
                    StartCoroutine(WaitAndCheckAnimation(playerAnimation, playerAnimation.clip.name, modelRoot.name, () => {
                        animationPlayed = true;
                    }));
                    animationPlayed = true;
                }
                // 方法2: 尝试播放指定的clip名称
                else
                {
                    string[] clipNamesToTry = {
                        pickupAnimationClip.name,  // "Take 001"
                        "Take 001",
                        pickupAnimationName,       // "take001"
                        "take001"
                    };
                    
                    foreach (string clipName in clipNamesToTry)
                    {
                        if (playerAnimation.Play(clipName))
                        {
                            StartCoroutine(WaitAndCheckAnimation(playerAnimation, clipName, modelRoot.name, () => {
                                animationPlayed = true;
                            }));
                            animationPlayed = true;
                            break;
                        }
                    }

                }
            }
        }
        
        // 方法2: 使用Animator（新动画系统）
        if (!animationPlayed && playerAnimator == null)
        {
            // 尝试在PlayerBody或指定的模型根对象上查找Animator
            GameObject targetObject = playerModelRoot != null ? playerModelRoot : gameObject;
            playerAnimator = targetObject.GetComponentInChildren<Animator>();
        }
        
        if (!animationPlayed && playerAnimator != null && playerAnimator.enabled)
        {
            try
            {
                playerAnimator.Play(pickupAnimationName);
                Debug.Log($"通过Animator播放拾取动画: {pickupAnimationName}");
                animationPlayed = true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Animator播放动画失败: {e.Message}");
            }
        }
        
        // 方法3: 使用Legacy Animation组件（通过名称）
        if (!animationPlayed && playerAnimation == null)
        {
            GameObject targetObject = playerModelRoot != null ? playerModelRoot : gameObject;
            playerAnimation = targetObject.GetComponentInChildren<Animation>();
        }
        
        if (!animationPlayed && playerAnimation != null && playerAnimation.enabled)
        {
            if (playerAnimation.GetClip(pickupAnimationName) != null)
            {
                playerAnimation.Play(pickupAnimationName);
                Debug.Log($"通过Legacy Animation播放拾取动画: {pickupAnimationName}");
                animationPlayed = true;
            }
            else
            {
                // 尝试直接播放（Unity可能会自动从FBX中查找）
                if (playerAnimation.Play(pickupAnimationName))
                {
                    Debug.Log($"尝试通过Legacy Animation播放动画: {pickupAnimationName}");
                    animationPlayed = true;
                }
            }
        }
        
        if (!animationPlayed)
        {
            Debug.LogWarning($"PlayerHoldItem: 无法播放拾取动画 '{pickupAnimationName}'。\n");
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [Header("组件引用")]
    //public CharacterController controller;
    public MouseLook mouseLook;
    public PlayerMovement movement;
    public EmpowermentAbility empowermentAbility;
    public Camera playerCamera;
    public PlayerHoldItem playerHoldItem;
    //public Transform holdPosition; //手持道具的位置

    [Header("交互设置")]
    public float interactRange = 3f;
    //public LayerMask interactableLayer;
    public float empowerHoldDuration = 2f; //长按E键 理解赋能的时间

    [Header("当前状态")]
    public InteractableObject currentInteractTarget;
    public int currentSelectedSlot = -1;

    // 触发器区域相关
    private List<InteractableObject> objectsInRange = new List<InteractableObject>();
    private CapsuleCollider interactionTrigger;



    private float empowerHoldTimer = 0f;
    private bool isHoldingEmpower = false;


    void Start()
    {
        //if (controller == null) controller = GetComponent<CharacterController>();
        if (mouseLook == null) mouseLook = GetComponentInChildren<MouseLook>();
        if (movement == null) movement = GetComponent<PlayerMovement>();
        if (empowermentAbility == null) empowermentAbility = GetComponent<EmpowermentAbility>();
        if (playerHoldItem == null) playerHoldItem = GetComponent<PlayerHoldItem>();

        // 创建交互触发器
        CreateInteractionTrigger();

        Cursor.lockState = CursorLockMode.Locked;

    }

    void CreateInteractionTrigger()
    {
        // 添加球形触发器
        interactionTrigger = gameObject.AddComponent<CapsuleCollider>();
        interactionTrigger.isTrigger = true;
        interactionTrigger.radius = interactRange * 1.5f;
        interactionTrigger.height = 3f;
        interactionTrigger.center = new Vector3(0, 1.5f, 0);

        // 调整触发器位置（如果需要）
        // interactionTrigger.center = new Vector3(0, 1, 0); // 根据角色高度调整

    }


    void Update()
    {

        UpdateCurrentInteractTarget();

        HandleEmpowerInput(); //长按E-理解特性， 短按E-赋予特性（仅在选择特性槽时）
        HandlePropertySwitch(); //数字键盘切换特性
        HandleInteraction(); //Q键普通交互（基础交互，不依赖特性槽）

        HandleItemPickupDrop();      // F键捡起/放下物品

        //Game loop 相关
        /* 用ui
        HandleRestart(); // R
        HandleExit(); // ESC
        */

        // 调试信息
        /*
        if (Time.frameCount % 120 == 0)
        {
            Debug.Log($"范围内物体数量: {objectsInRange.Count}, 当前目标: {(currentInteractTarget != null ? currentInteractTarget.name : "无")}");
        }
        */
    }

    void HandleItemPickupDrop()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (playerHoldItem.heldObject == null)
            {
                // 尝试捡起当前目标
                if (currentInteractTarget != null)
                {
                    if (!currentInteractTarget.canBePickedUp)
                    {
                        Debug.Log($"{currentInteractTarget.name} 不可拾取");
                        return;
                    }

                    bool success = playerHoldItem.PickupItem(currentInteractTarget.gameObject);
                    if (success)
                    {
                        // 捡起成功后从交互列表中移除
                        objectsInRange.Remove(currentInteractTarget);
                        currentInteractTarget = null;
                    }
                }
            }
            else
            {
                // 放下手中物品
                playerHoldItem.DropItem();
            }
        }
    }

    void HandleEmpowerInput()
    {
        if (Input.GetKey(KeyCode.E))
        {
            empowerHoldTimer += Time.deltaTime;

            //显示长按进度？
            if (!isHoldingEmpower && empowerHoldTimer >= 0.5f)
            {
                //Debug.Log("长按E理解特质中...");
            }

            //长按完成
            if (empowerHoldTimer >= empowerHoldDuration && !isHoldingEmpower)
            {
                StartUnderstanding();
                isHoldingEmpower = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            //短按E： 如果选择了特性槽，赋予特性；否则不处理（普通交互用Q键）
            if (empowerHoldTimer < empowerHoldDuration && !isHoldingEmpower && currentSelectedSlot != -1)
            {
                HandleApplyProperty();
            }

            empowerHoldTimer = 0f;
            isHoldingEmpower = false;
        }
    }

    void StartUnderstanding()
    {
        UpdateCurrentInteractTarget();

        if (currentInteractTarget == null)
        {
            Debug.Log("没有可理解的目标");
            return;
        }

        List<ObjectProperty> understandableProperties = currentInteractTarget.GetUnderstandableProperties();

        if (understandableProperties.Count == 0)
        {
            Debug.Log($"{currentInteractTarget.name} 没有可理解的特性");
            return;
        }

        //如果有多个特性，进入选择 让玩家选择理解哪个特性
        if (understandableProperties.Count > 1)
        {
            Debug.Log($"选择要理解的特性:");
            for (int i = 0; i < understandableProperties.Count; i++)
            {
                Debug.Log($"[{i + 1}] {understandableProperties[i]}");
            }
            // 处理特性选择
            StartCoroutine(HandleMultiplePropertySelection(understandableProperties));
        }
        else
        {
            // 只有一个特性，直接理解
            empowermentAbility.UnderstandProperty(currentInteractTarget, 0);
        }
    }

    //有多个特性可理解，让玩家选择
    private IEnumerator HandleMultiplePropertySelection(List<ObjectProperty> properties)
    {
        bool propertySelected = false;
        int selectedIndex = -1;

        while (!propertySelected)
        {
            for (int i = 0; i < properties.Count; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    selectedIndex = i;
                    propertySelected = true;
                    break;
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("取消特性理解");
                yield break;
            }

            yield return null;
        }

        if (selectedIndex != -1)
        {
            empowermentAbility.UnderstandProperty(currentInteractTarget, selectedIndex);
        }
    }

    /// <summary>
    /// Q键：普通交互（基础交互，不依赖特性槽）
    /// </summary>
    void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            HandleBaseInteraction();
        }
    }

    /// <summary>
    /// 基础交互逻辑（用于触发树枝摩擦点火、石碑充能等）
    /// </summary>
    void HandleBaseInteraction()
    {
        if (currentInteractTarget == null)
        {
            Debug.Log("没有可交互的目标");
            return;
        }

        // 基础交互（用于触发树枝摩擦点火、石碑充能等）
        var ignitionComponent = currentInteractTarget.GetComponent("BranchIgnition");
        if (ignitionComponent != null)
        {
            // 使用消息调用，避免直接依赖类型
            currentInteractTarget.gameObject.SendMessage("TryIgnite", SendMessageOptions.DontRequireReceiver);
            return;
        }
        
        // 通知LevelManager（用于处理石碑充能等特殊交互）
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.HandlePlayerUse(currentInteractTarget, -1, playerHoldItem.hasUnlockedEmpowerment);
        }
        // 其他基础交互可在此扩展
        // 注意：羽毛应该通过F键拾取，而不是Q键交互
    }

    /// <summary>
    /// 短按E：赋予特性（仅在选择特性槽时调用）
    /// </summary>
    void HandleApplyProperty()
    {
        if (currentInteractTarget == null)
        {
            Debug.Log("没有可交互的目标");
            return;
        }

        if (currentSelectedSlot == -1)
        {
            Debug.Log("未选择特性，无法赋予。使用Q键进行普通交互");
            return;
        }

        // 使用当前选择的特性
        bool success = empowermentAbility.ApplyProperty(currentInteractTarget, currentSelectedSlot);
        if (!success)
        {
            Debug.Log("特性使用失败");
        }
        
        // 通知LevelManager（用于处理石碑充能等特殊交互）
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.HandlePlayerUse(currentInteractTarget, currentSelectedSlot, playerHoldItem.hasUnlockedEmpowerment);
        }
    }

    void HandlePropertySwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchToSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchToSlot(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SwitchToSlot(-1);
        }
    }

    void SwitchToSlot(int slotIndex)
    {
        currentSelectedSlot = slotIndex;

        if (slotIndex == -1)
        {
            Debug.Log("切换到空手状态");
        }
        else
        {
            ObjectProperty property = empowermentAbility.GetProperty(slotIndex);
            if (property == ObjectProperty.None)
            {
                Debug.Log($"背包格{slotIndex + 1}为空");
                currentSelectedSlot = -1;
            }
            else
            {
                Debug.Log($"切换到特性: {property}");
            }
        }
    }

    /// <summary>
    /// 更新当前交互目标（射线检测）
    /// </summary>
    void UpdateCurrentInteractTarget()
    {
        InteractableObject bestTarget = null;
        float bestScore = 0f;

        objectsInRange.RemoveAll(item => item == null);

        foreach (InteractableObject interactable in objectsInRange)
        {
            float score = CalculateViewScore(interactable);

            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = interactable;
            }
        }

        if (bestScore > 0.3f && bestTarget != null)
        {
            //发现新目标
            if (currentInteractTarget != bestTarget)
            {
                if (currentInteractTarget != null)
                {
                    //Debug.Log($"目标切换: {currentInteractTarget.name} -> {bestTarget.name}");
                    currentInteractTarget.OnLoseFocus();
                }
                currentInteractTarget = bestTarget;
                currentInteractTarget.OnFocus();
                //Debug.Log($"选中目标: {bestTarget.name}, 视角分数: {bestScore:F2}");
            }
        }
        else
        {
            // 没有合适的目标
            if (currentInteractTarget != null)
            {
                //Debug.Log($"清除目标原因 - 最佳分数: {bestScore:F2}, 最佳目标: {(bestTarget != null ? bestTarget.name : "null")}, 当前目标: {currentInteractTarget.name}");
                currentInteractTarget.OnLoseFocus();
                currentInteractTarget = null;
            }
        }
    }

    float CalculateViewScore(InteractableObject interactable)
    {
        if (interactable == null) return 0f;

        Vector3 playerForward = playerCamera.transform.forward;
        Vector3 toTarget = (interactable.transform.position - playerCamera.transform.position).normalized;

        // 计算视角方向的点积（1.0表示正前方，0.0表示侧面，-1.0表示后方）
        float viewScore = Vector3.Dot(playerForward, toTarget);

        return viewScore;
    }

    // 触发器事件 - 物体进入范围
    void OnTriggerEnter(Collider other)
    {
        InteractableObject interactable = other.GetComponent<InteractableObject>();
        if (interactable != null && !objectsInRange.Contains(interactable))
        {
            objectsInRange.Add(interactable);
            //Debug.Log($"进入交互范围: {interactable.name}");
        }
    }

    // 触发器事件 - 物体离开范围
    void OnTriggerExit(Collider other)
    {
        InteractableObject interactable = other.GetComponent<InteractableObject>();
        if (interactable != null && objectsInRange.Contains(interactable))
        {
            objectsInRange.Remove(interactable);
            //Debug.Log($"离开交互范围: {interactable.name}");

            // 如果离开的是当前目标，清除它
            if (currentInteractTarget == interactable)
            {
                currentInteractTarget.OnLoseFocus();
                currentInteractTarget = null;
                //Debug.Log($"当前目标离开范围，已清除");
            }
        }
    }


    /// <summary>
    /// 调试绘制
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (playerCamera != null)
        {
            // 绘制交互射线
            Gizmos.color = Color.blue;
            Vector3 rayOrigin = playerCamera.transform.position;
            Vector3 rayDirection = playerCamera.transform.forward * interactRange;
            Gizmos.DrawRay(rayOrigin, rayDirection);
        }
    }

}
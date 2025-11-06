using UnityEngine;
using System.Collections;

public class PlayerHoldItem : MonoBehaviour
{
    [Header("手持设置")]
    public Transform holdPosition;
    
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
        else
        {
            item.transform.localRotation = Quaternion.identity;
        }
        
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        
        Collider collider = item.GetComponent<Collider>();
        if (collider != null) collider.enabled = false;
        
        Debug.Log($"物理拾取了: {item.name}");
        
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
}
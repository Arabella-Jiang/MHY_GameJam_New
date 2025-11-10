using System.Collections.Generic;
using UnityEngine;

public class WaterHardEffect : CombinationEffect
{
    [Header("通行控制")]
    public bool canPass = false;
    [Header("玩家标签")]
    public string playerTag = "Player";

    [Header("提醒信息")]
    public string blockMessage = "水流太急，试试其他办法吧";
    public string successMessage = "水变得坚硬了，可以通过这里";

    [Header("河流碰撞体")]
    public Transform airwallColliderParent; // water/airwallcollider 父对象
    public Collider riverPlaneBoxCollider; // 水面平面碰撞体（硬化后启用）
    
    private List<Collider> airwallColliders = new List<Collider>(); // 存储所有空气墙碰撞体

    [Header("水效果控制")]
    public Renderer waterRenderer;
    public Material hardenedWaterMaterial; // 硬化后的材质

    private bool hasShownBlockMessage = false;
    private bool effectTriggered = false; // 防止重复触发

    void Start()
    {
        // 确保Water对象有Trigger Collider用于交互检测（Player的触发器需要）
        EnsureWaterTriggerCollider();
        
        // 如果未手动指定airwallColliderParent，尝试自动查找
        if (airwallColliderParent == null)
        {
            // 尝试在当前对象层级中查找名为"airwallcollider"的子对象
            Transform waterTransform = transform.parent;
            if (waterTransform != null)
            {
                airwallColliderParent = FindChildByName(waterTransform, "airwallcollider");
            }
            
            // 如果还是找不到，尝试在当前对象的子对象中查找
            if (airwallColliderParent == null)
            {
                airwallColliderParent = FindChildByName(transform, "airwallcollider");
            }
        }
        
        // 收集所有空气墙碰撞体
        CollectAirwallColliders();
        
        // 为每个空气墙cube添加block message组件
        SetupAirwallBlockMessages();
    }
    
    /// <summary>
    /// 确保Water对象有Trigger Collider用于交互检测（Player的触发器需要）
    /// </summary>
    private void EnsureWaterTriggerCollider()
    {
        // 检查当前对象（Water对象）是否有Trigger Collider
        Collider[] allColliders = GetComponents<Collider>();
        bool hasTriggerCollider = false;
        
        foreach (var col in allColliders)
        {
            if (col.isTrigger)
            {
                hasTriggerCollider = true;
                break;
            }
        }
        
        if (!hasTriggerCollider)
        {
            // 没有Trigger Collider，添加一个BoxCollider作为Trigger
            // 这个Collider用于让Player的触发器能够检测到Water（有InteractableObject组件）
            BoxCollider triggerCollider = gameObject.AddComponent<BoxCollider>();
            triggerCollider.isTrigger = true;
            
            // 尝试根据子对象或渲染器的大小来设置Collider大小
            if (waterRenderer != null)
            {
                Bounds bounds = waterRenderer.bounds;
                // 将world space的bounds size转换为local space
                Vector3 lossyScale = transform.lossyScale;
                Vector3 localSize = new Vector3(
                    bounds.size.x / Mathf.Max(lossyScale.x, 0.001f),
                    Mathf.Max(bounds.size.y / Mathf.Max(lossyScale.y, 0.001f), 10f), // 确保高度至少10
                    bounds.size.z / Mathf.Max(lossyScale.z, 0.001f)
                );
                triggerCollider.size = localSize;
                // 调整center，让Collider覆盖水面附近
                Vector3 localCenter = transform.InverseTransformPoint(bounds.center);
                localCenter.y = 0;
                triggerCollider.center = localCenter;
            }
            else
            {
                // 使用默认大小（覆盖整个水区域）
                triggerCollider.size = new Vector3(200f, 20f, 200f);
                triggerCollider.center = Vector3.zero;
            }
            
            Debug.Log("WaterHardEffect: 已自动添加Trigger Collider到Water对象，用于交互检测");
        }
    }
    
    /// <summary>
    /// 为每个空气墙cube添加block message组件
    /// </summary>
    private void SetupAirwallBlockMessages()
    {
        if (airwallColliders.Count == 0)
        {
            Debug.LogWarning("WaterHardEffect: 没有空气墙碰撞体，无法设置阻挡消息");
            return;
        }
        
        // 使用反射获取AirWallBlockMessage类型，避免编译错误
        System.Type blockMessageType = null;
        
        // 方法1: 尝试直接获取类型
        blockMessageType = System.Type.GetType("AirWallBlockMessage");
        
        // 方法2: 如果找不到，尝试从当前程序集查找
        if (blockMessageType == null)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            blockMessageType = assembly.GetType("AirWallBlockMessage");
        }
        
        // 方法3: 遍历所有类型查找
        if (blockMessageType == null)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            foreach (var type in assembly.GetTypes())
            {
                if (type.Name == "AirWallBlockMessage")
                {
                    blockMessageType = type;
                    break;
                }
            }
        }
        
        if (blockMessageType == null)
        {
            Debug.LogError("WaterHardEffect: 找不到AirWallBlockMessage类型，请确保脚本已编译并存在于项目中");
            return;
        }
        
        int addedCount = 0;
        foreach (var collider in airwallColliders)
        {
            if (collider == null) continue;
            
            // 确保collider不是trigger（用于物理阻挡）
            collider.isTrigger = false;
            
            // 检查是否已有组件
            Component existingComponent = collider.GetComponent(blockMessageType);
            if (existingComponent == null)
            {
                // 为每个cube添加AirWallBlockMessage组件
                existingComponent = collider.gameObject.AddComponent(blockMessageType);
                addedCount++;
            }
            
            // 使用反射设置参数
            if (existingComponent != null)
            {
                var waterHardEffectField = blockMessageType.GetField("waterHardEffect");
                if (waterHardEffectField != null)
                {
                    waterHardEffectField.SetValue(existingComponent, this);
                }
                
                var blockMessageField = blockMessageType.GetField("blockMessage");
                if (blockMessageField != null)
                {
                    blockMessageField.SetValue(existingComponent, this.blockMessage);
                }
                
                var playerTagField = blockMessageType.GetField("playerTag");
                if (playerTagField != null)
                {
                    playerTagField.SetValue(existingComponent, this.playerTag);
                }
            }
        }
        
        Debug.Log($"WaterHardEffect: 已为 {airwallColliders.Count} 个空气墙cube设置阻挡消息（新增 {addedCount} 个）");
    }
    
    /// <summary>
    /// 递归查找子对象中指定名称的Transform
    /// </summary>
    private Transform FindChildByName(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == name || child.name.ToLower() == name.ToLower())
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
    
    /// <summary>
    /// 收集airwallcollider下所有子对象的碰撞体
    /// </summary>
    private void CollectAirwallColliders()
    {
        airwallColliders.Clear();
        
        if (airwallColliderParent == null)
        {
            Debug.LogWarning("WaterHardEffect: 未找到airwallcollider父对象，无法收集空气墙碰撞体");
            return;
        }
        
        // 获取所有子对象的Collider组件
        Collider[] colliders = airwallColliderParent.GetComponentsInChildren<Collider>(true);
        foreach (var collider in colliders)
        {
            // 排除airwallColliderParent本身的Collider（如果有的话）
            if (collider.transform != airwallColliderParent)
            {
                airwallColliders.Add(collider);
            }
        }
        
        Debug.Log($"WaterHardEffect: 收集到 {airwallColliders.Count} 个空气墙碰撞体");
    }

    public override void TriggerEffect()
    {
        if (effectTriggered) return; // 防止重复触发

        canPass = true;
        effectTriggered = true;

        // 禁用所有空气墙碰撞体
        DisableAllAirwallColliders();
        
        // 启用水面平面碰撞体（如果配置了）
        if(riverPlaneBoxCollider != null)
        {
            riverPlaneBoxCollider.enabled = true;
            Debug.Log("启用水面平面碰撞体");
        }

        // 替换材质
        ReplaceWaterMaterial();
        
        // 设置waterplane的localPosition y为-0.8（固定值）
        if (waterRenderer != null)
        {
            Vector3 localPos = waterRenderer.transform.localPosition;
            localPos.y = -0.8f;
            waterRenderer.transform.localPosition = localPos;
            //Debug.Log($"已设置waterplane的localPosition y为-0.8");
        }
        
        // 保持原有的transform位置设置
        //this.transform.localPosition = new Vector3(0.000000086511f, 0.58f, 0);

        GameNotification.ShowByTrigger("Level1", "水面硬化成功");
        Debug.Log($"✅ {successMessage}");
    }
    
    /// <summary>
    /// 禁用所有空气墙碰撞体
    /// </summary>
    private void DisableAllAirwallColliders()
    {
        if (airwallColliders.Count == 0)
        {
            // 如果列表为空，尝试重新收集
            CollectAirwallColliders();
        }
        
        int disabledCount = 0;
        foreach (var collider in airwallColliders)
        {
            if (collider != null)
            {
                collider.enabled = false;
                disabledCount++;
            }
        }
        
        Debug.Log($"已禁用 {disabledCount} 个空气墙碰撞体，允许在硬化水面上通行");
    }
    
    private void ReplaceWaterMaterial()
    {
        if (waterRenderer != null && hardenedWaterMaterial != null)
        {
            waterRenderer.material = hardenedWaterMaterial;
            Debug.Log("水材质已替换为硬化版本");
        }
        else
        {
            Debug.LogError("无法替换材质：渲染器或硬化材质为空");
        }
    }

}
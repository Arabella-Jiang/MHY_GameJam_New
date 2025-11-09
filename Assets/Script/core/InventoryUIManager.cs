using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 管理玩家特性背包的UI显示
/// 当玩家吸收特性时，在左上角的Inventory Panel显示对应的图标
/// </summary>
public class InventoryUIManager : MonoBehaviour
{
    [Header("UI引用")]
    [Tooltip("第一个特性槽的图标显示")]
    public RawImage propertyIcon1;
    
    [Tooltip("第二个特性槽的图标显示")]
    public RawImage propertyIcon2;

    [Header("特性图标资源")]
    [Tooltip("羽毛图标 - Soft特性")]
    public Texture2D featherIcon;
    
    [Tooltip("石头图标 - Hard/Heavy特性")]
    public Texture2D stoneIcon;
    
    [Tooltip("老藤图标 - Flexible/Long/Thin特性")]
    public Texture2D oldVineIcon;
    
    [Tooltip("冰锥图标 - Transparent/Sharp/Cool特性")]
    public Texture2D icicleIcon;

    [Header("组件引用")]
    public EmpowermentAbility empowermentAbility;

    // 特性到图标的映射
    private Dictionary<ObjectProperty, Texture2D> propertyIconMap;

    void Awake()
    {
        // 初始化特性到图标的映射
        InitializePropertyIconMap();
    }

    void Start()
    {
        // 查找EmpowermentAbility组件（如果没有手动设置）
        if (empowermentAbility == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                empowermentAbility = player.GetComponent<EmpowermentAbility>();
            }
        }

        if (empowermentAbility == null)
        {
            Debug.LogError("InventoryUIManager: 找不到EmpowermentAbility组件！");
        }

        // 初始隐藏所有图标
        if (propertyIcon1 != null)
        {
            propertyIcon1.gameObject.SetActive(false);
        }
        if (propertyIcon2 != null)
        {
            propertyIcon2.gameObject.SetActive(false);
        }

        // 订阅特性变化事件
        if (empowermentAbility != null)
        {
            empowermentAbility.OnPropertyChanged += UpdateInventoryUI;
        }

        // 初始化UI显示
        UpdateInventoryUI();
    }

    void OnDestroy()
    {
        // 取消订阅
        if (empowermentAbility != null)
        {
            empowermentAbility.OnPropertyChanged -= UpdateInventoryUI;
        }
    }

    /// <summary>
    /// 初始化特性到图标的映射关系
    /// </summary>
    private void InitializePropertyIconMap()
    {
        propertyIconMap = new Dictionary<ObjectProperty, Texture2D>();

        // 羽毛相关特性
        if (featherIcon != null)
        {
            propertyIconMap[ObjectProperty.Soft] = featherIcon;
        }

        // 石头相关特性
        if (stoneIcon != null)
        {
            propertyIconMap[ObjectProperty.Hard] = stoneIcon;
            propertyIconMap[ObjectProperty.Heavy] = stoneIcon;
        }

        // 老藤相关特性
        if (oldVineIcon != null)
        {
            propertyIconMap[ObjectProperty.Flexible] = oldVineIcon;
            propertyIconMap[ObjectProperty.Thin] = oldVineIcon;
            propertyIconMap[ObjectProperty.Long] = oldVineIcon;
        }

        // 冰锥相关特性
        if (icicleIcon != null)
        {
            propertyIconMap[ObjectProperty.Transparent] = icicleIcon;
            propertyIconMap[ObjectProperty.Sharp] = icicleIcon;
            propertyIconMap[ObjectProperty.Cool] = icicleIcon;
            //propertyIconMap[ObjectProperty.White] = icicleIcon;
        }

        // 其他特性也可以映射到相应图标
        // 树枝类特性
        if (oldVineIcon != null)
        {
            propertyIconMap[ObjectProperty.Flammable] = oldVineIcon;
            propertyIconMap[ObjectProperty.Thin] = oldVineIcon;
        }

    }

    /// <summary>
    /// 更新背包UI显示
    /// </summary>
    public void UpdateInventoryUI()
    {
        if (empowermentAbility == null) return;

        // 更新第一个槽位
        UpdateSlotUI(0, propertyIcon1);
        
        // 更新第二个槽位
        UpdateSlotUI(1, propertyIcon2);
    }

    /// <summary>
    /// 更新单个槽位的UI显示
    /// </summary>
    private void UpdateSlotUI(int slotIndex, RawImage iconUI)
    {
        if (iconUI == null) return;

        ObjectProperty property = empowermentAbility.GetProperty(slotIndex);

        if (property == ObjectProperty.None)
        {
            // 槽位为空，隐藏图标
            iconUI.gameObject.SetActive(false);
            iconUI.texture = null;
        }
        else
        {
            // 槽位有特性，显示对应图标
            Texture2D icon = GetIconForProperty(property);
            if (icon != null)
            {
                iconUI.texture = icon;
                iconUI.gameObject.SetActive(true);
                Debug.Log($"UI更新：槽位{slotIndex + 1}显示特性 [{property}] 的图标");
            }
            else
            {
                Debug.LogWarning($"找不到特性 [{property}] 对应的图标");
                iconUI.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 根据特性获取对应的图标
    /// </summary>
    private Texture2D GetIconForProperty(ObjectProperty property)
    {
        if (propertyIconMap.ContainsKey(property))
        {
            return propertyIconMap[property];
        }
        return null;
    }

    /// <summary>
    /// 手动刷新UI（用于调试）
    /// </summary>
    [ContextMenu("刷新UI显示")]
    public void RefreshUI()
    {
        UpdateInventoryUI();
    }
}


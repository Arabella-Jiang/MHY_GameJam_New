using UnityEngine;
using System.Collections.Generic;
using System;

//相当于Inventory
public class EmpowermentAbility : MonoBehaviour
{
    [Header("特性背包")]
    public ObjectProperty[] propertySlots = new ObjectProperty[2];

    // 特性变化事件 - 当背包中的特性发生变化时触发
    public event Action OnPropertyChanged;

    void Start()
    {
        // 初始化背包格为空
        for (int i = 0; i < propertySlots.Length; i++)
        {
            propertySlots[i] = ObjectProperty.None;
        }
    }


    /// 理解并存储特性（由Player调用）
    public void UnderstandProperty(InteractableObject target, int selectedPropertyIndex)
    {
        if (target == null)
        {
            Debug.LogError("理解目标为空");
            return;
        }

        List<ObjectProperty> understandableProperties = target.GetUnderstandableProperties();

        if (understandableProperties == null || selectedPropertyIndex >= understandableProperties.Count)
        {
            Debug.LogError("无效的特性选择");
            return;
        }

        ObjectProperty selectedProperty = understandableProperties[selectedPropertyIndex];
        
        // 播放赋能音效
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayEmpowerSound();
        }
        
        AddPropertyToSlot(selectedProperty);
    }

    /// <summary>
    /// 添加特性到背包格
    /// </summary>
    private void AddPropertyToSlot(ObjectProperty newProperty)
    {
        // 查找空槽位
        int emptySlot = FindEmptySlot();
        ObjectProperty replacedProperty = ObjectProperty.None;
        bool replaced = false;

        if (emptySlot != -1)
        {
            // 有空槽，直接添加
            propertySlots[emptySlot] = newProperty;
            Debug.Log($"特性 [{newProperty}] 已存入背包格 {emptySlot + 1}");
        }
        else
        {
            // 槽位已满，替换第一个格子 TODO：让玩家可以选择替换1/2背包格
            replaced = true;
            replacedProperty = propertySlots[0];
            Debug.Log($"背包已满，替换第一个特性: {propertySlots[0]} → {newProperty}");
            propertySlots[0] = newProperty;
        }

        LogCurrentInventory();
        
        // 触发特性变化事件，通知UI更新
        OnPropertyChanged?.Invoke();

        NotifyPropertyAcquired(newProperty, replaced, replacedProperty);
    }

    /// <summary>
    /// 对目标物体赋予特性
    /// </summary>
    public bool ApplyProperty(InteractableObject target, int slotIndex)
    {
        if (target == null)
        {
            Debug.LogError("赋予特性的目标为空");
            return false;
        }

        if (slotIndex < 0 || slotIndex >= propertySlots.Length)
        {
            Debug.LogError($"无效的背包格索引: {slotIndex}");
            return false;
        }

        ObjectProperty propertyToApply = propertySlots[slotIndex];

        if (propertyToApply == ObjectProperty.None)
        {
            Debug.Log($"背包格 {slotIndex + 1} 为空，无法赋予特性");
            return false;
        }

        Debug.Log($"尝试对 {target.name} 赋予特性 [{propertyToApply}]");
        bool success = target.ReceiveProperty(propertyToApply);

        if (success)
        {
            Debug.Log($"成功对 {target.name} 赋予特性 [{propertyToApply}]");
        }

        return success;
    }

    /// <summary>
    /// 查找空背包格
    /// </summary>
    private int FindEmptySlot()
    {
        for (int i = 0; i < propertySlots.Length; i++)
        {
            if (propertySlots[i] == ObjectProperty.None)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 获取指定格子的特性
    /// </summary>
    public ObjectProperty GetProperty(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < propertySlots.Length)
        {
            return propertySlots[slotIndex];
        }
        return ObjectProperty.None;
    }

    /// <summary>
    /// 输出当前背包状态
    /// </summary>
    private void LogCurrentInventory()
    {
        string inventoryInfo = "当前特性背包: ";
        for (int i = 0; i < propertySlots.Length; i++)
        {
            inventoryInfo += $"[{i + 1}]{propertySlots[i]} ";
        }
        Debug.Log(inventoryInfo);
    }

    private void NotifyPropertyAcquired(ObjectProperty newProperty, bool replaced, ObjectProperty replacedProperty)
    {
        if (newProperty == ObjectProperty.None) return;

        LevelManager manager = FindObjectOfType<LevelManager>();
        if (manager == null) return;

        string newPropertyName = GameLocalization.GetPropertyDisplayName(newProperty);
        string replacedPropertyName = GameLocalization.GetPropertyDisplayName(replacedProperty);

        string levelKey = "Global";
        string trigger = replaced ? "特性替换提示" : "特性获得提示";
        string message = replaced
            ? $"背包已满，已用 {newPropertyName} 替换 {replacedPropertyName}。"
            : $"已获得 {newPropertyName} 特性。";

        if (GameMessageCatalog.TryGetMessageText(levelKey, trigger, out string template) && !string.IsNullOrWhiteSpace(template))
        {
            try
            {
                message = replaced
                    ? string.Format(template, newPropertyName, replacedPropertyName)
                    : string.Format(template, newPropertyName);
            }
            catch (System.FormatException)
            {
                Debug.LogWarning($"GameMessageCatalog: 触发 \"{levelKey}\" / \"{trigger}\" 的格式化失败，使用默认文本。");
            }
        }

        manager.ShowMessage(message);
    }
}
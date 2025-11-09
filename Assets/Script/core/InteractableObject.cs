using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
//using Unity.VisualScripting;

public class InteractableObject : MonoBehaviour
{

    [Header("当前特性")]
    public List<ObjectProperty> currentProperties = new List<ObjectProperty>();

    [Header("可理解的特性列表")]
    public List<ObjectProperty> understandableProperties = new List<ObjectProperty>();

    [Header("特性组合效果")]
    public PropertyCombination[] propertyCombinations;

    [Header("可捡起设置")]
    public bool canBePickedUp = true;

    [System.Serializable]
    public class PropertyCombination
    {
        public List<ObjectProperty> requiredProperties;
        public string effectComponentName;         // 触发的效果
    }

    //private bool isFocus = false;

    void Start()
    {
        /* UI
        objectRenderer = GetComponent<Renderer>();
        UpdateVisualAppearance();
        */
        
        // 如果是可拾取物体，设置rigidbody为kinematic，防止地面不平导致下移
        if (canBePickedUp)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
    }

    //获取当前可理解的特性列表
    public List<ObjectProperty> GetUnderstandableProperties()
    {
        return new List<ObjectProperty>(understandableProperties);
    }

    public bool ReceiveProperty(ObjectProperty newProperty)
    {
        Debug.Log($"{name} 尝试接收特性: {newProperty}");

        // 添加新特性到物体
        if (!currentProperties.Contains(newProperty))
        {
            currentProperties.Add(newProperty);
            Debug.Log($"{name} 现在拥有特性: {string.Join(", ", currentProperties)}");

            // 检查特性组合效果
            CheckPropertyCombinations();
            return true;
        }
        else
        {
            Debug.Log($"{name} 已经拥有特性 {newProperty}");
            return false;
        }
    }

    // 检查特性组合触发效果
    private void CheckPropertyCombinations()
    {
        foreach (var combination in propertyCombinations)
        {
            if (HasAllProperties(combination.requiredProperties))
            {
                // 方法1: 尝试使用Type.GetType()获取类型
                System.Type effectType = System.Type.GetType(combination.effectComponentName);
                
                // 方法2: 如果找不到，尝试添加程序集名称
                if (effectType == null)
                {
                    effectType = System.Type.GetType(combination.effectComponentName + ", Assembly-CSharp");
                }
                
                // 方法3: 如果还找不到，遍历所有CombinationEffect组件，按名称匹配
                CombinationEffect effect = null;
                
                if (effectType != null && typeof(CombinationEffect).IsAssignableFrom(effectType))
                {
                    effect = GetComponent(effectType) as CombinationEffect;
                }
                else
                {
                    // 遍历所有CombinationEffect组件，找到名称匹配的
                    CombinationEffect[] allEffects = GetComponents<CombinationEffect>();
                    foreach (var eff in allEffects)
                    {
                        if (eff.GetType().Name == combination.effectComponentName)
                        {
                            effect = eff;
                            break;
                        }
                    }
                }
                
                if (effect != null)
                {
                    effect.TriggerEffect();
                    Debug.Log($"✅ 触发特性组合效果: {string.Join(" + ", combination.requiredProperties)} -> {combination.effectComponentName}");
                }
                else
                {
                    Debug.LogWarning($"[{name}] 找不到组合效果组件: {combination.effectComponentName}。请确保：\n" +
                                   $"1. 组件名称完全匹配类名（如 StoneLongEffect，不是 StoneLongEffect.cs）\n" +
                                   $"2. 该组件已挂载到物体上\n" +
                                   $"3. 该组件继承自 CombinationEffect");
                }
            }
        }
    }

    // 检查是否拥有所有指定特性
    private bool HasAllProperties(List<ObjectProperty> requiredProps)
    {
        foreach (var prop in requiredProps)
        {
            if (!currentProperties.Contains(prop))
                return false;
        }
        return true;
    }

    //TODO
    public void OnFocus()
    {
        // 简单的高亮效果
        // 比如：改变材质颜色、显示轮廓、显示UI提示等
        //Debug.Log($"{name} 被聚焦");
    }

    public void OnLoseFocus()
    {
        // 恢复原状
        //Debug.Log($"{name} 失去聚焦");
    }
}
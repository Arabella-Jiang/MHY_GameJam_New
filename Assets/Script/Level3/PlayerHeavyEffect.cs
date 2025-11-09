using UnityEngine;

/// <summary>
/// 玩家变重效果（玩家被赋予Heavy属性后生效）
/// 这个组件用于检测玩家是否拥有Heavy属性，并触发相关效果
/// </summary>
public class PlayerHeavyEffect : MonoBehaviour
{
    [Header("玩家组件引用")]
    public Player player;                        // 玩家组件
    public EmpowermentAbility empowermentAbility; // 玩家的属性能力组件

    [Header("状态")]
    public bool isHeavy = false;                 // 玩家是否变重

    void Start()
    {
        // 自动查找玩家组件
        if (player == null)
        {
            player = GetComponent<Player>();
        }
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }

        // 自动查找EmpowermentAbility组件
        if (empowermentAbility == null)
        {
            empowermentAbility = GetComponent<EmpowermentAbility>();
        }
        if (empowermentAbility == null && player != null)
        {
            empowermentAbility = player.empowermentAbility;
        }
    }

    void Update()
    {
        // 检测玩家是否拥有Heavy属性
        bool hasHeavy = CheckIfPlayerHasHeavy();
        
        if (hasHeavy && !isHeavy)
        {
            // 玩家刚刚变重
            OnBecomeHeavy();
        }
        else if (!hasHeavy && isHeavy)
        {
            // 玩家刚刚失去重量
            OnLoseHeavy();
        }
    }

    /// <summary>
    /// 检查玩家是否拥有Heavy属性
    /// </summary>
    private bool CheckIfPlayerHasHeavy()
    {
        if (empowermentAbility == null) return false;

        // 检查属性槽中是否有Heavy属性
        for (int i = 0; i < empowermentAbility.propertySlots.Length; i++)
        {
            if (empowermentAbility.propertySlots[i] == ObjectProperty.Heavy)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 玩家变重时的处理
    /// </summary>
    private void OnBecomeHeavy()
    {
        isHeavy = true;
        Debug.Log("✅ 玩家变重了！");
    }

    /// <summary>
    /// 玩家失去重量时的处理
    /// </summary>
    private void OnLoseHeavy()
    {
        isHeavy = false;
        Debug.Log("玩家失去重量");
    }

    /// <summary>
    /// 检查玩家当前是否变重（供其他组件调用）
    /// </summary>
    public bool IsPlayerHeavy()
    {
        return isHeavy;
    }
}


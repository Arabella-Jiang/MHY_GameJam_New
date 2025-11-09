using UnityEngine;

/// <summary>
/// 木头变细效果（路线2：玩家使用Thin特性对woodoriginal，使其变细）
/// woodoriginal变inactive，woodthin变active，玩家可以拾取woodthin
/// </summary>
public class WoodThinEffect : CombinationEffect
{
    [Header("变细设置")]
    public GameObject woodThinObject;            // 变细后的木头对象（WoodThin，应该是当前GameObject的子对象或引用）
    
    [Header("视觉效果")]
    public ParticleSystem transformEffect;       // 变形特效
    public AudioSource transformSound;           // 变形音效（可选）

    private bool effectTriggered = false;

    public override void TriggerEffect()
    {
        if (effectTriggered) return;

        effectTriggered = true;

        // 播放变形特效
        if (transformEffect != null)
        {
            transformEffect.Play();
        }

        // 播放变形音效
        if (transformSound != null)
        {
            transformSound.Play();
        }

        // woodoriginal变inactive
        gameObject.SetActive(false);

        // woodthin变active
        if (woodThinObject != null)
        {
            woodThinObject.SetActive(true);

            // 确保woodthin可以被拾取
            InteractableObject woodThinIO = woodThinObject.GetComponent<InteractableObject>();
            if (woodThinIO == null)
            {
                woodThinIO = woodThinObject.AddComponent<InteractableObject>();
            }
            woodThinIO.canBePickedUp = true;

            Debug.Log("✅ 木头变细了！现在可以拾取它。");
        }
        else
        {
            Debug.LogError("WoodThinEffect: woodThinObject未指定！请在Inspector中指定WoodThin对象。");
        }
    }
}


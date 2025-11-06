using UnityEngine;

public class StoneTableEffect : CombinationEffect
{
    [Header("通关效果")]
    public Light tabletLight; // 石碑光源
    public ParticleSystem fireEffect; // 火焰特效
    
    public override void TriggerEffect()
    {
        Debug.Log("✅ 石碑被点亮！关卡通关！");
        
        // 点亮石碑
        if (tabletLight != null)
        {
            tabletLight.enabled = true;
        }
        
        // 播放火焰特效
        if (fireEffect != null)
        {
            fireEffect.Play();
        }
        
        // 改变石碑材质，表示被点亮
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.yellow;
        }
        
        // 触发通关事件
        // GameManager.Instance.CompleteLevel();
    }
}
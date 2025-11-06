using UnityEngine;

public class StoneTableEffect : CombinationEffect
{
    [Header("通关效果")]
    public Light tabletLight; // 石碑光源
    public ParticleSystem fireEffect; // 火焰特效
    
    [Header("文字效果")]
    public Renderer wordRenderer; // 文字的Renderer组件（需要在Unity Inspector中手动指定）
    public Material litWordMaterial; // 点亮后的文字材质（可选，如果不指定则只改变颜色）
    
    public override void TriggerEffect()
    {
        Debug.Log("✅ 文字被点亮！关卡通关！");
        
        // 点亮石碑（可选，如果不需要可以注释掉）
        if (tabletLight != null)
        {
            tabletLight.enabled = true;
        }
        
        // 播放火焰特效
        if (fireEffect != null)
        {
            fireEffect.Play();
        }
        
        // 改变文字材质，表示被点亮
        if (wordRenderer != null)
        {
            if (litWordMaterial != null)
            {
                // 如果指定了新的材质，使用新材质
                wordRenderer.material = litWordMaterial;
            }
            else
            {
                // 如果没有指定新材质，只改变颜色
                wordRenderer.material.color = Color.yellow;
            }
        }
        else
        {
            // 如果没有手动指定wordRenderer，尝试在子对象中查找
            Transform wordTransform = transform.Find("wenzi");
            if (wordTransform == null)
            {
                // 如果找不到"wenzi"，尝试搜索所有子对象中的Renderer
                Renderer[] renderers = GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    if (renderer.name.Contains("wenzi") || renderer.name.Contains("word") || renderer.name.Contains("文字"))
                    {
                        wordRenderer = renderer;
                        break;
                    }
                }
            }
            else
            {
                // 在wenzi对象或其子对象中查找Renderer
                wordRenderer = wordTransform.GetComponentInChildren<Renderer>();
            }
            
            // 如果找到了文字Renderer，应用材质
            if (wordRenderer != null)
            {
                if (litWordMaterial != null)
                {
                    wordRenderer.material = litWordMaterial;
                }
                else
                {
                    wordRenderer.material.color = Color.yellow;
                }
            }
            else
            {
                Debug.LogWarning("StoneTableEffect: 找不到文字Renderer！请在Inspector中手动指定wordRenderer。");
            }
        }
        
        // 触发通关事件
        // GameManager.Instance.CompleteLevel();
    }
}
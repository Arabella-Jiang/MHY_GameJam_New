using UnityEngine;

public class BranchHardEffect : CombinationEffect
{
    public override void TriggerEffect()
    {
        InteractableObject branch = GetComponent<InteractableObject>();
        if (branch != null)
        {
            // 确保树枝拥有Flammable特性
            if (!branch.currentProperties.Contains(ObjectProperty.Flammable))
            {
                branch.currentProperties.Add(ObjectProperty.Flammable);
                Debug.Log("✅ 树枝变得坚硬且易燃！");
            }
            
            // 视觉变化：树枝颜色变深，表示更坚硬
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.4f, 0.2f, 0.1f); // 深棕色
            }
        }
    }
}
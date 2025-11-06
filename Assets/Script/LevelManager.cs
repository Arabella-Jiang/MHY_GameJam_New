using UnityEngine;

public abstract class LevelManager : MonoBehaviour
{
    [Header("玩家引用")]
    public Player player;

    void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
        
        InitializeLevel();
    }

    protected virtual void InitializeLevel()
    {
        // 关卡初始化逻辑
    }

    public abstract void HandlePlayerPickup(InteractableObject target, bool hasUnlockedEmpowerment);
    public abstract void HandlePlayerUse(InteractableObject target, int selectedSlot, bool hasUnlockedEmpowerment);
    
    public virtual void ShowMessage(string message)
    {
        Debug.Log($"UI提示: {message}");
    }
    
    public virtual void TriggerCutscene(string cutsceneName)
    {
        Debug.Log($"播放过场动画: {cutsceneName}");
    }
}
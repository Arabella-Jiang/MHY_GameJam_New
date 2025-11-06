using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [Header("分镜镜头")]
    public CinemachineVirtualCamera sunsetCamera;
    
    [Header("玩家控制")]
    public MouseLook mouseLook;
    public PlayerMovement playerMovement;

    // 保存原始状态
    private bool wasMouseLookEnabled;
    private KeyCode originalRunKey;
    private KeyCode originalJumpKey;

    void Start()
    {
        // 确保分镜镜头平时完全关闭
        if (sunsetCamera != null)
        {
            sunsetCamera.gameObject.SetActive(false);
        }
    }
    
    // 播放日落分镜
    public void PlaySunsetSequence(float duration = 5f)
    {
        // 保存原始状态
        wasMouseLookEnabled = mouseLook.enabled;
        originalRunKey = playerMovement.runKey;
        originalJumpKey = playerMovement.jumpKey;

        // 1. 禁用玩家控制
        if (mouseLook != null) mouseLook.enabled = false;
         // 通过修改按键来禁用移动输入（而不是禁用整个脚本）
        playerMovement.runKey = KeyCode.None;  // 设置为不存在的按键
        playerMovement.jumpKey = KeyCode.None;
        
        // 2. 激活分镜镜头
        if (sunsetCamera != null)
        {
            sunsetCamera.gameObject.SetActive(true);
        }
        
        Debug.Log("开始日落分镜");
        
        // 3. 定时恢复玩家控制
        Invoke("RestorePlayerControl", duration);
    }
    
    void RestorePlayerControl()
    {
        // 1. 关闭分镜镜头
        if (sunsetCamera != null)
        {
            sunsetCamera.gameObject.SetActive(false);
        }
        
        // 恢复玩家控制
        mouseLook.enabled = wasMouseLookEnabled;
        playerMovement.runKey = originalRunKey;
        playerMovement.jumpKey = originalJumpKey;
        
        Debug.Log("恢复玩家控制");
    }
}
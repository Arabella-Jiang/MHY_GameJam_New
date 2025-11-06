using UnityEngine;
using Cinemachine;


public class CameraSystemFix : MonoBehaviour
{
    void Start()
    {
        FixCameraSystem();
    }
    
    void FixCameraSystem()
    {
        // 1. 找到主相机（但不改变它的位置和层级）
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
        
        // 保存相机当前状态
        Transform cameraParent = mainCamera.transform.parent;
        Vector3 cameraLocalPosition = mainCamera.transform.localPosition;
        Quaternion cameraLocalRotation = mainCamera.transform.localRotation;
        
        Debug.Log($"相机当前状态 - 父物体: {cameraParent?.name}, 位置: {cameraLocalPosition}");

        // 2. 临时禁用 Cinemachine Brain
        CinemachineBrain brain = mainCamera.GetComponent<CinemachineBrain>();
        if (brain != null)
        {
            brain.enabled = false;
            Debug.Log("已禁用 Cinemachine Brain");
        }
        
        // 3. 确保所有 Virtual Camera 都关闭
        CinemachineVirtualCamera[] allVCams = FindObjectsOfType<CinemachineVirtualCamera>();
        foreach (var vcam in allVCams)
        {
            vcam.gameObject.SetActive(false);
            Debug.Log($"已关闭: {vcam.name}");
        }
        
        // 4. 恢复相机原始状态（确保位置不变）
        mainCamera.transform.SetParent(cameraParent);
        mainCamera.transform.localPosition = cameraLocalPosition;
        mainCamera.transform.localRotation = cameraLocalRotation;
        
        // 5. 确保 MouseLook 启用
        MouseLook mouseLook = FindObjectOfType<MouseLook>();
        if (mouseLook != null)
        {
            mouseLook.enabled = true;
            Debug.Log("已启用 MouseLook");
        }
        
        Debug.Log("相机系统修复完成！保持原有视角");
    }
}
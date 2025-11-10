using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 菜单管理器，管理StartMenu、PauseMenu、InfoMenu的显示
/// </summary>
public class MenuManager : MonoBehaviour
{
    [Header("菜单面板")]
    public GameObject startMenuPanel;           // StartMenu
    public GameObject pauseMenuPanel;           // PauseMenu
    public GameObject infoMenuPanel;            // InfoMenu

    void Start()
    {
        // 初始化：显示StartMenu，隐藏其他
        ShowStartMenu();
        
        // 动态绑定按钮事件
        BindButtonEvents();
    }
    
    /// <summary>
    /// 动态绑定所有按钮事件
    /// </summary>
    private void BindButtonEvents()
    {
        // StartMenu按钮
        if (startMenuPanel != null)
        {
            Button startButton = FindButtonInPanel(startMenuPanel, "GameStartButton");
            if (startButton != null)
            {
                startButton.onClick.RemoveAllListeners();
                startButton.onClick.AddListener(OnStartGameButton);
                Debug.Log("已绑定 GameStartButton");
            }
            
            Button quitButton = FindButtonInPanel(startMenuPanel, "ExitGameButton");
            if (quitButton != null)
            {
                quitButton.onClick.RemoveAllListeners();
                quitButton.onClick.AddListener(OnQuitButton);
                Debug.Log("已绑定 ExitGameButton");
            }
        }
        
        // PauseMenu按钮
        if (pauseMenuPanel != null)
        {
            Button resumeButton = FindButtonInPanel(pauseMenuPanel, "ResumeGameButton");
            if (resumeButton != null)
            {
                resumeButton.onClick.RemoveAllListeners();
                resumeButton.onClick.AddListener(OnResumeButton);
                Debug.Log("已绑定 ResumeGameButton");
            }
            
            Button returnButton = FindButtonInPanel(pauseMenuPanel, "ReturnToMainMenuButton");
            if (returnButton != null)
            {
                returnButton.onClick.RemoveAllListeners();
                returnButton.onClick.AddListener(OnReturnToMainMenuButton);
                Debug.Log("已绑定 ReturnToMainMenuButton");
            }
            
            Button pauseQuitButton = FindButtonInPanel(pauseMenuPanel, "ExitGameButton");
            if (pauseQuitButton != null)
            {
                pauseQuitButton.onClick.RemoveAllListeners();
                pauseQuitButton.onClick.AddListener(OnQuitButton);
                Debug.Log("已绑定 PauseMenu ExitGameButton");
            }
        }
    }
    
    /// <summary>
    /// 在Panel中查找指定名称的Button
    /// </summary>
    private Button FindButtonInPanel(GameObject panel, string buttonName)
    {
        if (panel == null) return null;

        // 先尝试常规路径（兼容旧结构）
        Transform buttonTransform = panel.transform.Find("Panel/" + buttonName);
        if (buttonTransform == null)
        {
            buttonTransform = panel.transform.Find(buttonName);
        }

        if (buttonTransform != null)
        {
            Button directButton = buttonTransform.GetComponent<Button>();
            if (directButton != null) return directButton;
        }

        // 递归扫描所有子节点（包括未激活），匹配名称
        Button[] buttons = panel.GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            if (btn != null && btn.gameObject.name == buttonName)
            {
                return btn;
            }
        }

        Debug.LogWarning($"未找到按钮: {buttonName} in {panel.name}");
        return null;
    }

    /// <summary>
    /// 显示开始菜单
    /// </summary>
    public void ShowStartMenu()
    {
        SetPanelActive(startMenuPanel, true);
        SetPanelActive(pauseMenuPanel, false);
        SetPanelActive(infoMenuPanel, false);
    }

    /// <summary>
    /// 显示信息菜单
    /// </summary>
    public void ShowInfoMenu()
    {
        SetPanelActive(startMenuPanel, false);
        SetPanelActive(pauseMenuPanel, false);
        SetPanelActive(infoMenuPanel, true);
    }
    
    /// <summary>
    /// 显示暂停菜单
    /// </summary>
    public void ShowPauseMenu()
    {
        SetPanelActive(startMenuPanel, false);
        SetPanelActive(pauseMenuPanel, true);
        SetPanelActive(infoMenuPanel, false);
    }

    void Update()
    {
        // InfoMenu显示时，点击任意键开始游戏（除了ESC）
        if (infoMenuPanel != null && infoMenuPanel.activeSelf)
        {
            // 任意键都能开始游戏，除了ESC键
            if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape))
            {
                StartGameAfterInfo();
            }
        }
    }

    /// <summary>
    /// 开始游戏按钮
    /// </summary>
    public void OnStartGameButton()
    {
        // 先显示InfoMenu（故事背景），按任意键后开始游戏
        ShowInfoMenu();
    }

    /// <summary>
    /// 退出游戏按钮
    /// </summary>
    public void OnQuitButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
    }

    /// <summary>
    /// InfoMenu显示完毕后，开始游戏
    /// </summary>
    private void StartGameAfterInfo()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
    }

    /// <summary>
    /// 继续游戏按钮（PauseMenu）
    /// </summary>
    public void OnResumeButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
            
            // 隐藏暂停菜单
            SetPanelActive(pauseMenuPanel, false);
        }
    }

    /// <summary>
    /// 返回主菜单按钮（PauseMenu）
    /// </summary>
    public void OnReturnToMainMenuButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMainMenu();
        }
    }

    /// <summary>
    /// 辅助方法：安全地设置Panel的active状态
    /// </summary>
    private void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
    }
}


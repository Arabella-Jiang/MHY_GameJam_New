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

    [Header("InfoMenu设置")]
    [Tooltip("InfoMenu最小显示时间（秒）")]
    public float infoMenuMinDisplayTime = 5f;

    private Coroutine infoMenuWaitCoroutine = null;

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
            }
            
            Button quitButton = FindButtonInPanel(startMenuPanel, "ExitGameButton");
            if (quitButton != null)
            {
                quitButton.onClick.RemoveAllListeners();
                quitButton.onClick.AddListener(OnQuitButton);
            }
        }
        
        // PauseMenu按钮在ShowPauseMenu时绑定
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
        
        // 暂停游戏，确保InfoMenu显示足够时间
        Time.timeScale = 0f;
        
        // 禁用EventSystem，防止双击等输入事件被处理
        UnityEngine.EventSystems.EventSystem eventSystem = UnityEngine.EventSystems.EventSystem.current;
        if (eventSystem != null)
        {
            eventSystem.enabled = false;
        }
        
        // 停止之前的协程（如果有）
        if (infoMenuWaitCoroutine != null)
        {
            StopCoroutine(infoMenuWaitCoroutine);
        }
        
        // 启动协程，等待5秒后允许跳过（使用WaitForSecondsRealtime，不受timeScale影响）
        infoMenuWaitCoroutine = StartCoroutine(WaitForInfoMenuDisplay());
    }
    
    /// <summary>
    /// 协程：等待InfoMenu显示足够时间
    /// </summary>
    private System.Collections.IEnumerator WaitForInfoMenuDisplay()
    {
        // 使用WaitForSecondsRealtime，不受timeScale影响
        yield return new WaitForSecondsRealtime(infoMenuMinDisplayTime);
        
        // 恢复游戏时间
        Time.timeScale = 1f;
        
        // 重新启用EventSystem，允许玩家交互
        UnityEngine.EventSystems.EventSystem eventSystem = UnityEngine.EventSystems.EventSystem.current;
        if (eventSystem != null)
        {
            eventSystem.enabled = true;
        }
    }
    
    /// <summary>
    /// 显示暂停菜单
    /// </summary>
    public void ShowPauseMenu()
    {
        SetPanelActive(startMenuPanel, false);
        SetPanelActive(pauseMenuPanel, true);
        SetPanelActive(infoMenuPanel, false);
        
        // 确保EventSystem存在（UI按钮点击需要）
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        // 重新绑定按钮事件（场景切换后可能丢失）
        BindPauseMenuButtons();
    }

    /// <summary>
    /// 绑定PauseMenu按钮事件
    /// </summary>
    private void BindPauseMenuButtons()
    {
        if (pauseMenuPanel == null) return;

        Button resumeButton = FindButtonInPanel(pauseMenuPanel, "ResumeGameButton");
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(OnResumeButton);
        }

        Button returnButton = FindButtonInPanel(pauseMenuPanel, "ReturnToMainMenuButton");
        if (returnButton != null)
        {
            returnButton.onClick.RemoveAllListeners();
            returnButton.onClick.AddListener(OnReturnToMainMenuButton);
        }

        Button pauseQuitButton = FindButtonInPanel(pauseMenuPanel, "ExitGameButton");
        if (pauseQuitButton != null)
        {
            pauseQuitButton.onClick.RemoveAllListeners();
            pauseQuitButton.onClick.AddListener(OnQuitButton);
        }
    }

    void Update()
    {
        // InfoMenu显示时，只有当Time.timeScale恢复为1（协程完成）后，才允许跳过
        if (infoMenuPanel != null && infoMenuPanel.activeSelf && Time.timeScale > 0f)
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
        // 确保游戏时间已恢复
        Time.timeScale = 1f;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
        else
        {
            Debug.LogError("[MenuManager] GameManager.Instance为null！");
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


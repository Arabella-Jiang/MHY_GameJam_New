using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 游戏管理器，处理暂停、退出等全局功能
/// 空格键：暂停/继续
/// ESC键：退出游戏（在MainMenu）或返回MainMenu（在关卡中）
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("暂停菜单")]
    public GameObject pauseMenuPanel;           // PauseMenu prefab 实例

    private bool isPaused = false;

    void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += HandleSceneLoaded;
            EnsurePauseMenuReference();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }
    }

    void Update()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 主菜单中直接退出游戏
            if (currentScene == "MainMenu")
            {
                if (isPaused)
                {
                    QuitGame();
                }
                else
                {
                    PauseGame();
                }
                return;
            }

            // 游戏中：若未暂停则打开暂停菜单；若已暂停则退出游戏
            if (isPaused)
            {
                QuitGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // 暂停时间

        EnsurePauseMenuReference();
        
        // 通过MenuManager显示暂停菜单
        MenuManager menuManager = FindObjectOfType<MenuManager>();
        if (menuManager != null)
        {
            // 确保 MenuManager 也有正确的引用
            if (pauseMenuPanel != null && menuManager.pauseMenuPanel != pauseMenuPanel)
            {
                menuManager.pauseMenuPanel = pauseMenuPanel;
            }
            
            menuManager.ShowPauseMenu();
            
            // 再次同步引用
            if (menuManager.pauseMenuPanel != null)
            {
                pauseMenuPanel = menuManager.pauseMenuPanel;
            }
        }
        else if (pauseMenuPanel != null)
        {
            // 后备方案：直接显示pauseMenuPanel并绑定按钮
            pauseMenuPanel.SetActive(true);
            BindPauseMenuButtons();
            Debug.LogWarning("GameManager: 未找到 MenuManager，直接显示并绑定 PauseMenu");
        }
        else
        {
            Debug.LogError("GameManager: 无法找到 PauseMenu 面板！");
        }
        
        // 确保EventSystem存在（UI按钮点击需要）
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        // 显示鼠标光标，让玩家可以点击按钮
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        Debug.Log("游戏已暂停");
    }

    /// <summary>
    /// 继续游戏
    /// </summary>
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // 恢复时间
        
        // MenuManager会处理面板隐藏
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        // 隐藏鼠标光标（如果游戏中不需要鼠标）
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        Debug.Log("游戏已继续");
    }

    /// <summary>
    /// 处理ESC键
    /// </summary>
    private void HandleEscape()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        
        if (currentScene == "MainMenu")
        {
            // 在主菜单，ESC退出游戏
            QuitGame();
        }
        else
        {
            // 在关卡中，ESC返回主菜单
            if (isPaused)
            {
                // 如果已经暂停，先恢复时间再返回
                Time.timeScale = 1f;
            }
            ReturnToMainMenu();
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isPaused = false;
        Time.timeScale = 1f;
        EnsurePauseMenuReference();
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }

    private void EnsurePauseMenuReference()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (pauseMenuPanel != null)
        {
            if (!pauseMenuPanel.scene.IsValid() || pauseMenuPanel.scene != currentScene)
            {
                pauseMenuPanel = null;
            }
            else
            {
                return;
            }
        }

        MenuManager menuManager = FindObjectOfType<MenuManager>();
        if (menuManager != null && menuManager.pauseMenuPanel != null)
        {
            pauseMenuPanel = menuManager.pauseMenuPanel;
            return;
        }

        GameObject found = FindPauseMenuInScene();
        if (found != null)
        {
            pauseMenuPanel = found;
        }
    }

    /// <summary>
    /// 绑定 PauseMenu 按钮事件（当没有 MenuManager 时使用）
    /// </summary>
    private void BindPauseMenuButtons()
    {
        if (pauseMenuPanel == null)
        {
            Debug.LogError("GameManager: pauseMenuPanel 为 null，无法绑定按钮");
            return;
        }

        Debug.Log($"GameManager: 开始绑定 PauseMenu 按钮，pauseMenuPanel = {pauseMenuPanel.name}");

        // 检查 EventSystem
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            Debug.LogWarning("GameManager: 未找到 EventSystem，UI 点击可能无法工作");
        }
        else
        {
            Debug.Log($"GameManager: EventSystem 存在: {eventSystem.name}");
        }

        // 查找 ResumeGameButton
        Button resumeButton = FindButtonInPanel(pauseMenuPanel, "ResumeGameButton");
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(() => {
                Debug.Log("GameManager: ResumeGameButton onClick 事件触发！");
                ResumeGame();
            });
            Debug.Log($"GameManager: 已绑定 ResumeGameButton，可交互: {resumeButton.interactable}, 激活: {resumeButton.gameObject.activeSelf}");
        }
        else
        {
            Debug.LogError($"GameManager: 未找到 ResumeGameButton");
        }

        // 查找 ReturnToMainMenuButton
        Button returnButton = FindButtonInPanel(pauseMenuPanel, "ReturnToMainMenuButton");
        if (returnButton != null)
        {
            returnButton.onClick.RemoveAllListeners();
            returnButton.onClick.AddListener(() => {
                Debug.Log("GameManager: ReturnToMainMenuButton onClick 事件触发！");
                ReturnToMainMenu();
            });
            Debug.Log("GameManager: 已绑定 ReturnToMainMenuButton");
        }

        // 查找 ExitGameButton
        Button quitButton = FindButtonInPanel(pauseMenuPanel, "ExitGameButton");
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(() => {
                Debug.Log("GameManager: ExitGameButton onClick 事件触发！");
                QuitGame();
            });
            Debug.Log("GameManager: 已绑定 ExitGameButton");
        }
    }

    /// <summary>
    /// 在 Panel 中查找指定名称的 Button
    /// </summary>
    private Button FindButtonInPanel(GameObject panel, string buttonName)
    {
        if (panel == null) return null;

        // 先尝试常规路径
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

    private GameObject FindPauseMenuInScene()
    {
        // 只使用名称查找，避免 tag 未定义导致的错误
        Scene currentScene = SceneManager.GetActiveScene();

        foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go == null) continue;
            if (go.hideFlags != HideFlags.None) continue;
            if (!go.scene.IsValid() || go.scene != currentScene) continue;

            // 只匹配名称，不检查 tag
            if (go.name == "PauseMenu")
            {
                return go;
            }
        }

        return null;
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // 确保时间恢复
        isPaused = false;
        Debug.Log("返回主菜单");
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("退出游戏");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    /// <summary>
    /// 开始游戏（从Level1开始）
    /// </summary>
    public void StartGame()
    {
        Time.timeScale = 1f;
        Debug.Log("开始游戏");
        SceneManager.LoadScene("Level1");
    }

    /// <summary>
    /// 加载指定场景
    /// </summary>
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        Debug.Log($"加载场景: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}


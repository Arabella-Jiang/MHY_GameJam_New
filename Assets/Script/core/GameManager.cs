using UnityEngine;
using UnityEngine.SceneManagement;

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
            menuManager.ShowPauseMenu();
            if (menuManager.pauseMenuPanel != null)
            {
                pauseMenuPanel = menuManager.pauseMenuPanel;
            }
        }
        else if (pauseMenuPanel != null)
        {
            // 后备方案：直接显示pauseMenuPanel
            pauseMenuPanel.SetActive(true);
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
        if (pauseMenuPanel != null) return;

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

    private GameObject FindPauseMenuInScene()
    {
        // 优先查找带Tag的对象
        GameObject found = null;
        try
        {
            found = GameObject.FindWithTag("PauseMenu");
        }
        catch (UnityException)
        {
            // Tag 未定义时忽略异常
        }

        if (found != null) return found;

        // GameObject.Find无法找到未激活对象，改用 Resources 扫描
        foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go == null) continue;
            if (go.hideFlags != HideFlags.None) continue;

            if (go.CompareTag("PauseMenu") || go.name == "PauseMenu")
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


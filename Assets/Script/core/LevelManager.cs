using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        // 查找NotificationUIManager并显示消息
        // 使用反射来避免编译时依赖（如果NotificationUIManager还未编译）
        try
        {
            System.Type notificationType = System.Type.GetType("NotificationUIManager");
            if (notificationType != null)
            {
                UnityEngine.Object[] managers = FindObjectsOfType(notificationType);
                if (managers != null && managers.Length > 0)
                {
                    System.Reflection.MethodInfo method = notificationType.GetMethod("ShowNotification");
                    if (method != null)
                    {
                        method.Invoke(managers[0], new object[] { message });
                        return;
                    }
                }
            }
        }
        catch (System.Exception)
        {
            // 如果反射失败，继续使用Debug.Log
        }
        
        // 如果找不到NotificationUIManager，回退到Debug.Log
        Debug.Log($"UI提示: {message}");
    }
    
    public virtual void TriggerCutscene(string cutsceneName)
    {
        Debug.Log($"播放过场动画: {cutsceneName}");
    }

    /// <summary>
    /// 显示结语黑屏并切换场景。
    /// </summary>
    protected void ShowConclusionAndLoad(string levelKey, string triggerKey, string nextSceneName, float holdDuration = 1.8f, string fallbackMessage = "")
    {
        ShowConclusionAndLoadStatic(levelKey, triggerKey, nextSceneName, holdDuration, fallbackMessage);
    }

    public static void ShowConclusionAndLoadStatic(string levelKey, string triggerKey, string nextSceneName, float holdDuration = 1.8f, string fallbackMessage = "")
    {
        string message = fallbackMessage ?? string.Empty;

        if (!string.IsNullOrEmpty(levelKey) && !string.IsNullOrEmpty(triggerKey))
        {
            if (GameMessageCatalog.TryGetMessageText(levelKey, triggerKey, out string catalogText) && !string.IsNullOrWhiteSpace(catalogText))
            {
                message = catalogText;
            }
        }

        TransitionOverlay.Instance.Play(message, nextSceneName, holdDuration);
    }

    private sealed class TransitionOverlay : MonoBehaviour
    {
        private static TransitionOverlay instance;
        public static TransitionOverlay Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("[SceneTransitionUI]");
                    instance = go.AddComponent<TransitionOverlay>();
                }
                return instance;
            }
        }

        [SerializeField] private float fadeDuration = 0.6f;
        [SerializeField] private float postLoadHoldDuration = 0.4f;
        [SerializeField] private int sortingOrder = 2000;

        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private TextMeshProUGUI messageText;
        private Coroutine transitionRoutine;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            BuildUI();

            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            messageText.text = string.Empty;

            SceneManager.sceneLoaded += (_, __) => TryApplyNotificationFont();
        }

        public void Play(string message, string nextSceneName, float holdDuration)
        {
            if (string.IsNullOrEmpty(nextSceneName))
            {
                Debug.LogWarning("TransitionOverlay: nextSceneName 为空，无法切换场景。");
                return;
            }

            if (transitionRoutine != null)
            {
                StopCoroutine(transitionRoutine);
            }

            transitionRoutine = StartCoroutine(TransitionRoutine(message ?? string.Empty, nextSceneName, holdDuration));
        }

        private IEnumerator TransitionRoutine(string message, string sceneName, float holdDuration)
        {
            BuildUI();
            canvasGroup.blocksRaycasts = true;
            messageText.text = message;

            yield return Fade(0f, 1f);

            if (!string.IsNullOrEmpty(message) && holdDuration > 0f)
            {
                yield return new WaitForSecondsRealtime(holdDuration);
            }

            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
            while (!async.isDone)
            {
                yield return null;
            }

            if (postLoadHoldDuration > 0f)
            {
                yield return new WaitForSecondsRealtime(postLoadHoldDuration);
            }

            yield return Fade(1f, 0f);

            canvasGroup.blocksRaycasts = false;
            messageText.text = string.Empty;
            transitionRoutine = null;
        }

        private IEnumerator Fade(float from, float to)
        {
            if (Mathf.Approximately(from, to))
            {
                canvasGroup.alpha = to;
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                canvasGroup.alpha = Mathf.Lerp(from, to, t);
                yield return null;
            }

            canvasGroup.alpha = to;
        }

        private void BuildUI()
        {
            if (canvasGroup != null) return;

            GameObject canvasGO = new GameObject("SceneTransitionCanvas");
            canvasGO.transform.SetParent(transform);

            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;

            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGO.AddComponent<GraphicRaycaster>();

            GameObject panelGO = new GameObject("Panel");
            panelGO.transform.SetParent(canvasGO.transform, false);
            RectTransform panelRect = panelGO.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image panelImage = panelGO.AddComponent<Image>();
            panelImage.color = Color.black;

            canvasGroup = panelGO.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            GameObject textGO = new GameObject("Message");
            textGO.transform.SetParent(panelGO.transform, false);
            RectTransform textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.15f, 0.25f);
            textRect.anchorMax = new Vector2(0.85f, 0.75f);

            messageText = textGO.AddComponent<TextMeshProUGUI>();
            messageText.text = string.Empty;
            messageText.alignment = TextAlignmentOptions.Center;
            messageText.fontSize = 48f;
            messageText.color = Color.white;
            messageText.enableWordWrapping = true;

            TryApplyNotificationFont();
        }

        private void TryApplyNotificationFont()
        {
            if (messageText == null) return;

            NotificationUIManager notification = FindObjectOfType<NotificationUIManager>();
            if (notification != null && notification.notificationText != null)
            {
                messageText.font = notification.notificationText.font;
            }
        }
    }
}
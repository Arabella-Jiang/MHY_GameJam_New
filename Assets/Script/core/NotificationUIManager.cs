using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 管理游戏中的通知消息显示
/// 将Debug.Log和ShowMessage的信息显示到Notification Panel中
/// </summary>
public class NotificationUIManager : MonoBehaviour
{
    [Header("UI引用")]
    [Tooltip("通知面板的TextMeshPro组件")]
    public TextMeshProUGUI notificationText;
    
    [Tooltip("通知面板的GameObject（用于控制显示/隐藏）")]
    public GameObject notificationPanel;

    [Header("字体设置")]
    [Tooltip("中文字体资源（如果为空，将使用notificationText的默认字体）")]
    public TMP_FontAsset chineseFontAsset;

    [Header("显示设置")]
    [Tooltip("消息显示持续时间（秒）")]
    public float displayDuration = 3f;
    
    [Tooltip("消息淡入淡出时间（秒）")]
    public float fadeDuration = 0.3f;
    
    [Tooltip("是否自动隐藏消息")]
    public bool autoHide = true;
    
    [Tooltip("最大消息队列长度（0表示无限制）")]
    public int maxQueueLength = 5;

    [Header("消息队列")]
    [Tooltip("是否启用消息队列（多条消息依次显示）")]
    public bool enableQueue = true;

    private Queue<string> messageQueue = new Queue<string>();
    private Coroutine currentDisplayCoroutine;
    private bool isDisplaying = false;

    void Awake()
    {
        // 确保NotificationUIManager是单例
        NotificationUIManager[] managers = FindObjectsOfType<NotificationUIManager>();
        if (managers.Length > 1)
        {
            Debug.LogWarning("发现多个NotificationUIManager实例，保留第一个，销毁其他");
            for (int i = 1; i < managers.Length; i++)
            {
                Destroy(managers[i].gameObject);
            }
        }
    }

    void Start()
    {
        // 初始化UI状态
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }
        
        if (notificationText != null)
        {
            notificationText.text = "";
            // 设置初始透明度为0
            Color color = notificationText.color;
            color.a = 0f;
            notificationText.color = color;
            
            // 如果指定了中文字体资源，应用它
            if (chineseFontAsset != null)
            {
                notificationText.font = chineseFontAsset;
                Debug.Log("NotificationUIManager: 已应用中文字体资源");
            }
            else
            {
                Debug.LogWarning("NotificationUIManager: 未指定中文字体资源，中文可能显示为方块。请参考 CHINESE_FONT_SETUP.md 创建中文字体资源。");
            }
        }
    }

    /// <summary>
    /// 显示通知消息（公共接口）
    /// </summary>
    public void ShowNotification(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        // 如果启用了队列系统
        if (enableQueue)
        {
            // 检查队列长度限制
            if (maxQueueLength > 0 && messageQueue.Count >= maxQueueLength)
            {
                // 移除最旧的消息
                messageQueue.Dequeue();
            }
            
            messageQueue.Enqueue(message);
            
            // 如果当前没有在显示消息，立即显示
            if (!isDisplaying)
            {
                ProcessNextMessage();
            }
        }
        else
        {
            // 不使用队列，直接显示（会中断当前消息）
            if (currentDisplayCoroutine != null)
            {
                StopCoroutine(currentDisplayCoroutine);
            }
            currentDisplayCoroutine = StartCoroutine(DisplayMessage(message));
        }
    }

    /// <summary>
    /// 处理队列中的下一条消息
    /// </summary>
    private void ProcessNextMessage()
    {
        if (messageQueue.Count > 0)
        {
            string message = messageQueue.Dequeue();
            currentDisplayCoroutine = StartCoroutine(DisplayMessage(message));
        }
    }

    /// <summary>
    /// 显示消息的协程
    /// </summary>
    private IEnumerator DisplayMessage(string message)
    {
        isDisplaying = true;

        // 显示面板
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(true);
        }

        // 设置文本
        if (notificationText != null)
        {
            notificationText.text = message;
        }

        // 淡入效果
        yield return StartCoroutine(FadeIn());

        // 等待显示时间
        if (autoHide)
        {
            yield return new WaitForSeconds(displayDuration);
            
            // 淡出效果
            yield return StartCoroutine(FadeOut());
        }

        // 隐藏面板
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }

        isDisplaying = false;

        // 如果使用队列，处理下一条消息
        if (enableQueue && messageQueue.Count > 0)
        {
            yield return new WaitForSeconds(0.2f); // 消息之间的间隔
            ProcessNextMessage();
        }
    }

    /// <summary>
    /// 淡入效果
    /// </summary>
    private IEnumerator FadeIn()
    {
        if (notificationText == null) yield break;

        float elapsed = 0f;
        Color color = notificationText.color;
        float startAlpha = color.a;
        float targetAlpha = 1f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            notificationText.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        notificationText.color = color;
    }

    /// <summary>
    /// 淡出效果
    /// </summary>
    private IEnumerator FadeOut()
    {
        if (notificationText == null) yield break;

        float elapsed = 0f;
        Color color = notificationText.color;
        float startAlpha = color.a;
        float targetAlpha = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            notificationText.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        notificationText.color = color;
    }

    /// <summary>
    /// 立即清除当前显示的消息
    /// </summary>
    public void ClearNotification()
    {
        if (currentDisplayCoroutine != null)
        {
            StopCoroutine(currentDisplayCoroutine);
        }
        
        messageQueue.Clear();
        isDisplaying = false;

        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }

        if (notificationText != null)
        {
            notificationText.text = "";
            Color color = notificationText.color;
            color.a = 0f;
            notificationText.color = color;
        }
    }

    /// <summary>
    /// 设置消息显示持续时间
    /// </summary>
    public void SetDisplayDuration(float duration)
    {
        displayDuration = Mathf.Max(0.1f, duration);
    }

}


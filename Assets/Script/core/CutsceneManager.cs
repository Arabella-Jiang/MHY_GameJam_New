using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 过场动画管理器，统一管理所有关卡的过场动画播放
/// </summary>
public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }

    [Header("第一关过场动画")]
    [Tooltip("玩家降落")]
    public VideoClip level1_PlayerLanding;
    
    [Tooltip("湖面结冰")]
    public VideoClip level1_WaterFreeze;
    
    [Tooltip("钻木取火")]
    public VideoClip level1_FireIgnite;
    
    [Tooltip("石碑点亮后主角发亮")]
    public VideoClip level1_LightConverge;

    [Header("第二关过场动画")]
    [Tooltip("出场动画")]
    public VideoClip level2_Entrance;
    
    [Tooltip("拿羽毛")]
    public VideoClip level2_GetFeather;
    
    [Tooltip("点亮石碑后长出花草")]
    public VideoClip level2_FlowersGrow;

    [Header("第三关过场动画")]
    [Tooltip("出场动画")]
    public VideoClip level3_Entrance;
    
    [Tooltip("雪面变成冰面")]
    public VideoClip level3_SnowToIce;
    
    [Tooltip("冰面裂开")]
    public VideoClip level3_IceBreak;
    
    [Tooltip("星空折射")]
    public VideoClip level3_StarRefraction;
    
    [Tooltip("第二个星空折射")]
    public VideoClip level3_SecondStarRefraction;
    
    [Tooltip("枯树复活")]
    public VideoClip level3_TreeRevive;
    
    [Tooltip("湖水汇入枯树")]
    public VideoClip level3_WaterFlowToTree;

    [Header("视频播放设置")]
    [Tooltip("视频播放器")]
    public VideoPlayer videoPlayer;
    
    [Tooltip("视频渲染目标（RawImage）")]
    public RawImage videoRenderTarget;
    
    [Tooltip("视频播放面板（全屏）")]
    public GameObject videoPanel;
    
    [Tooltip("是否在播放时暂停游戏")]
    public bool pauseGameDuringCutscene = true;
    
    [Tooltip("视频播放完成后是否自动隐藏面板")]
    public bool autoHideOnComplete = true;

    private bool isPlaying = false;
    private float savedTimeScale = 1f;
    private System.Action onCompleteCallback;

    void Awake()
    {
        Debug.Log("[CutsceneManager] ===== Awake 被调用 =====");
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[CutsceneManager] CutsceneManager实例已创建，Instance已设置");
            InitializeVideoPlayer();
        }
        else
        {
            Debug.Log("[CutsceneManager] 已存在实例，销毁当前GameObject");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("[CutsceneManager] ===== Start 被调用 =====");
        Debug.Log($"[CutsceneManager] Instance状态: {(Instance != null ? "存在" : "NULL")}");
        Debug.Log($"[CutsceneManager] 视频文件检查:");
        Debug.Log($"  - level1_PlayerLanding: {(level1_PlayerLanding != null ? level1_PlayerLanding.name : "NULL")}");
        Debug.Log($"  - level1_WaterFreeze: {(level1_WaterFreeze != null ? level1_WaterFreeze.name : "NULL")}");
        Debug.Log($"  - level1_FireIgnite: {(level1_FireIgnite != null ? level1_FireIgnite.name : "NULL")}");
        Debug.Log($"  - level1_LightConverge: {(level1_LightConverge != null ? level1_LightConverge.name : "NULL")}");
        Debug.Log($"  - level2_Entrance: {(level2_Entrance != null ? level2_Entrance.name : "NULL")}");
        Debug.Log($"  - level2_GetFeather: {(level2_GetFeather != null ? level2_GetFeather.name : "NULL")}");
        Debug.Log($"  - level2_FlowersGrow: {(level2_FlowersGrow != null ? level2_FlowersGrow.name : "NULL")}");
        Debug.Log($"  - level3_Entrance: {(level3_Entrance != null ? level3_Entrance.name : "NULL")}");
        Debug.Log($"  - level3_SnowToIce: {(level3_SnowToIce != null ? level3_SnowToIce.name : "NULL")}");
        Debug.Log($"  - level3_IceBreak: {(level3_IceBreak != null ? level3_IceBreak.name : "NULL")}");
        Debug.Log($"  - level3_StarRefraction: {(level3_StarRefraction != null ? level3_StarRefraction.name : "NULL")}");
        Debug.Log($"  - level3_SecondStarRefraction: {(level3_SecondStarRefraction != null ? level3_SecondStarRefraction.name : "NULL")}");
        Debug.Log($"  - level3_TreeRevive: {(level3_TreeRevive != null ? level3_TreeRevive.name : "NULL")}");
        Debug.Log($"  - level3_WaterFlowToTree: {(level3_WaterFlowToTree != null ? level3_WaterFlowToTree.name : "NULL")}");
    }

    /// <summary>
    /// 初始化视频播放器
    /// </summary>
    private void InitializeVideoPlayer()
    {
        Debug.Log("[CutsceneManager] ===== 初始化VideoPlayer =====");

        // 如果没有手动指定VideoPlayer，自动创建
        if (videoPlayer == null)
        {
            Debug.Log("[CutsceneManager] 自动创建VideoPlayer");
            GameObject videoPlayerObj = new GameObject("VideoPlayer");
            videoPlayerObj.transform.SetParent(transform);
            videoPlayer = videoPlayerObj.AddComponent<VideoPlayer>();
        }
        else
        {
            Debug.Log($"[CutsceneManager] 使用现有VideoPlayer: {videoPlayer.name}");
        }

        // 配置VideoPlayer
        videoPlayer.playOnAwake = false;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.isLooping = false;
        videoPlayer.skipOnDrop = true;
        
        // 确保使用VideoClip作为源
        videoPlayer.source = VideoSource.VideoClip;
        
        // 设置宽高比模式：NoStretching保持原始比例，避免拉伸
        videoPlayer.aspectRatio = VideoAspectRatio.NoScaling;
        
        Debug.Log($"[CutsceneManager] VideoPlayer配置完成:");
        Debug.Log($"  - playOnAwake: {videoPlayer.playOnAwake}");
        Debug.Log($"  - waitForFirstFrame: {videoPlayer.waitForFirstFrame}");
        Debug.Log($"  - isLooping: {videoPlayer.isLooping}");
        Debug.Log($"  - source: {videoPlayer.source}");
        Debug.Log($"  - aspectRatio: {videoPlayer.aspectRatio}");

        // 如果没有手动指定渲染目标，自动创建UI
        if (videoRenderTarget == null || videoPanel == null)
        {
            Debug.Log("[CutsceneManager] 自动创建视频UI");
            CreateVideoUI();
        }
        else
        {
            Debug.Log($"[CutsceneManager] 使用现有UI: videoPanel={videoPanel.name}, videoRenderTarget={videoRenderTarget.name}");
        }

        // 确保视频渲染目标设置正确
        SetupVideoRenderTarget();

        // 监听视频播放完成事件
        videoPlayer.loopPointReached += OnVideoFinished;
        Debug.Log("[CutsceneManager] 视频播放完成事件已注册");
    }

    /// <summary>
    /// 设置视频渲染目标
    /// </summary>
    private void SetupVideoRenderTarget()
    {
        if (videoPlayer == null)
        {
            Debug.LogWarning("[CutsceneManager] VideoPlayer未设置");
            return;
        }

        if (videoRenderTarget == null)
        {
            Debug.LogWarning("[CutsceneManager] VideoRenderTarget未设置");
            return;
        }

        Debug.Log("[CutsceneManager] 设置视频渲染目标...");

        // 设置VideoPlayer使用RenderTexture模式
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        Debug.Log($"[CutsceneManager] renderMode设置为: {videoPlayer.renderMode}");

        // 如果还没有设置RenderTexture，创建一个新的
        if (videoPlayer.targetTexture == null)
        {
            Debug.Log("[CutsceneManager] 创建新的RenderTexture (1920x1080)");
            RenderTexture renderTexture = new RenderTexture(1920, 1080, 0);
            videoPlayer.targetTexture = renderTexture;
        }
        else
        {
            Debug.Log($"[CutsceneManager] 使用现有RenderTexture: {videoPlayer.targetTexture.name}");
        }

        // 将RenderTexture赋值给RawImage
        videoRenderTarget.texture = videoPlayer.targetTexture;
        Debug.Log($"[CutsceneManager] RenderTexture已赋值给RawImage: {(videoRenderTarget.texture != null ? "成功" : "失败")}");
    }

    /// <summary>
    /// 创建视频播放UI
    /// </summary>
    private void CreateVideoUI()
    {
        // 创建Canvas
        GameObject canvasObj = new GameObject("CutsceneCanvas");
        canvasObj.transform.SetParent(transform);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 3000; // 确保在最上层

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // 创建全屏面板
        videoPanel = new GameObject("VideoPanel");
        videoPanel.transform.SetParent(canvasObj.transform, false);
        RectTransform panelRect = videoPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;

        Image panelImage = videoPanel.AddComponent<Image>();
        panelImage.color = Color.black;

        // 创建RawImage用于显示视频
        GameObject rawImageObj = new GameObject("VideoRawImage");
        rawImageObj.transform.SetParent(videoPanel.transform, false);
        RectTransform rawImageRect = rawImageObj.AddComponent<RectTransform>();
        rawImageRect.anchorMin = Vector2.zero;
        rawImageRect.anchorMax = Vector2.one;
        rawImageRect.sizeDelta = Vector2.zero;
        rawImageRect.anchoredPosition = Vector2.zero;

        videoRenderTarget = rawImageObj.AddComponent<RawImage>();
        videoRenderTarget.color = Color.white;

        // 初始隐藏面板
        videoPanel.SetActive(false);
    }

    /// <summary>
    /// 播放过场动画（通过名称）
    /// </summary>
    /// <param name="cutsceneName">过场动画名称</param>
    /// <param name="onComplete">播放完成回调（可选）</param>
    public void PlayCutscene(string cutsceneName, System.Action onComplete = null)
    {
        Debug.Log($"[CutsceneManager] 请求播放过场动画: '{cutsceneName}'");
        
        // 确保实例存在
        if (Instance == null)
        {
            Debug.LogWarning("[CutsceneManager] Instance为NULL，尝试创建...");
            GameObject managerObj = new GameObject("CutsceneManager");
            CutsceneManager manager = managerObj.AddComponent<CutsceneManager>();
            // 等待一帧让Awake执行
            if (CutsceneManager.Instance == null)
            {
                Debug.LogError("[CutsceneManager] 创建失败！");
                onComplete?.Invoke();
                return;
            }
        }
        
        VideoClip clip = GetVideoClipByName(cutsceneName);
        if (clip != null)
        {
            Debug.Log($"[CutsceneManager] 找到视频片段: {clip.name}");
            PlayCutscene(clip, onComplete);
        }
        else
        {
            Debug.LogWarning($"[CutsceneManager] 未找到过场动画 '{cutsceneName}'");
            Debug.Log($"[CutsceneManager] 可用的过场动画: LightConverge, FireIgnite, WaterFreeze, SpringComplete, TreeRevive, WaterFlowToTree");
            onComplete?.Invoke();
        }
    }

    /// <summary>
    /// 播放过场动画（通过VideoClip）
    /// </summary>
    /// <param name="clip">视频片段</param>
    /// <param name="onComplete">播放完成回调（可选）</param>
    public void PlayCutscene(VideoClip clip, System.Action onComplete = null)
    {
        if (clip == null)
        {
            Debug.LogWarning("[CutsceneManager] VideoClip为空，无法播放");
            onComplete?.Invoke();
            return;
        }

        if (isPlaying)
        {
            Debug.LogWarning("[CutsceneManager] 已有过场动画正在播放，跳过新的播放请求");
            return;
        }

        Debug.Log($"[CutsceneManager] 开始播放视频: {clip.name}, VideoPlayer: {(videoPlayer != null ? videoPlayer.name : "NULL")}");
        StartCoroutine(PlayCutsceneCoroutine(clip, onComplete));
    }

    /// <summary>
    /// 播放过场动画协程
    /// </summary>
    private IEnumerator PlayCutsceneCoroutine(VideoClip clip, System.Action onComplete)
    {
        Debug.Log("[CutsceneManager] ===== 开始播放过场动画协程 =====");
        isPlaying = true;
        onCompleteCallback = onComplete;

        // 确保VideoPlayer和渲染目标已正确设置
        if (videoPlayer == null)
        {
            Debug.LogError("[CutsceneManager] VideoPlayer未设置！");
            isPlaying = false;
            onComplete?.Invoke();
            yield break;
        }

        Debug.Log($"[CutsceneManager] VideoPlayer状态检查:");
        Debug.Log($"  - videoPlayer: {videoPlayer.name}");
        Debug.Log($"  - videoRenderTarget: {(videoRenderTarget != null ? videoRenderTarget.name : "NULL")}");
        Debug.Log($"  - videoPanel: {(videoPanel != null ? videoPanel.name : "NULL")}");
        Debug.Log($"  - clip: {clip.name}");

        // 确保渲染目标设置正确
        SetupVideoRenderTarget();

        // 暂停游戏（如果启用）
        if (pauseGameDuringCutscene)
        {
            savedTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            Debug.Log($"[CutsceneManager] 游戏已暂停，Time.timeScale: {Time.timeScale}");
        }

        // 先停止并清理之前的视频，避免残留画面
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }
        videoPlayer.clip = null;
        
        // 隐藏RawImage，避免显示残留画面
        if (videoRenderTarget != null)
        {
            videoRenderTarget.texture = null;
            videoRenderTarget.enabled = false;
        }

        // 显示视频面板（但RawImage暂时隐藏）
        if (videoPanel != null)
        {
            videoPanel.SetActive(true);
            Debug.Log("[CutsceneManager] 视频面板已显示");
        }
        else
        {
            Debug.LogWarning("[CutsceneManager] videoPanel未设置，视频可能无法显示");
        }

        // 设置视频
        videoPlayer.clip = clip;
        videoPlayer.source = VideoSource.VideoClip; // 确保使用VideoClip源
        Debug.Log($"[CutsceneManager] 视频片段已设置: {clip.name}");
        Debug.Log($"[CutsceneManager] VideoPlayer配置:");
        Debug.Log($"  - renderMode: {videoPlayer.renderMode}");
        Debug.Log($"  - targetTexture: {(videoPlayer.targetTexture != null ? videoPlayer.targetTexture.name : "NULL")}");
        Debug.Log($"  - source: {videoPlayer.source}");
        Debug.Log($"  - clip: {(videoPlayer.clip != null ? videoPlayer.clip.name : "NULL")}");
        Debug.Log($"  - clip length: {(videoPlayer.clip != null ? videoPlayer.clip.length.ToString() : "NULL")}");

        // 准备视频
        Debug.Log("[CutsceneManager] 开始准备视频...");
        
        // 使用prepareCompleted事件来确保准备完成
        bool prepared = false;
        VideoPlayer.EventHandler prepareCallback = (VideoPlayer vp) => { prepared = true; };
        videoPlayer.prepareCompleted += prepareCallback;
        
        videoPlayer.Prepare();

        // 等待视频准备完成（最多等待10秒，避免无限等待）
        float timeout = 10f;
        float elapsed = 0f;
        while (!prepared && !videoPlayer.isPrepared && elapsed < timeout)
        {
            elapsed += Time.unscaledDeltaTime;
            if (elapsed % 1f < 0.1f) // 每秒打印一次
            {
                Debug.Log($"[CutsceneManager] 等待视频准备中... ({elapsed:F1}s / {timeout}s), isPrepared: {videoPlayer.isPrepared}");
            }
            yield return null;
        }

        // 移除事件监听
        videoPlayer.prepareCompleted -= prepareCallback;

        if (!videoPlayer.isPrepared && !prepared)
        {
            Debug.LogError($"[CutsceneManager] 视频准备超时（{timeout}秒），无法播放");
            Debug.LogError($"[CutsceneManager] 请检查:");
            Debug.LogError($"  1. 视频文件是否存在: {clip.name}");
            Debug.LogError($"  2. VideoPlayer的source设置是否正确 (当前: {videoPlayer.source})");
            Debug.LogError($"  3. 视频文件格式是否支持");
            Debug.LogError($"  4. 视频文件是否正确导入到Unity");
            Debug.LogError($"  5. 检查视频文件的Import Settings");
            OnVideoFinished(videoPlayer);
            yield break;
        }

        Debug.Log("[CutsceneManager] 视频准备完成，开始播放");

        // 视频准备完成后，重新设置RenderTexture并启用RawImage
        if (videoRenderTarget != null && videoPlayer.targetTexture != null)
        {
            videoRenderTarget.texture = videoPlayer.targetTexture;
            videoRenderTarget.enabled = true;
            Debug.Log("[CutsceneManager] RawImage已启用，显示视频画面");
        }

        // 播放视频
        videoPlayer.Play();
        Debug.Log($"[CutsceneManager] VideoPlayer.Play() 已调用");
        Debug.Log($"[CutsceneManager] isPlaying: {videoPlayer.isPlaying}, frame: {videoPlayer.frame}");

        // 等待一小段时间确保播放开始
        yield return new WaitForSecondsRealtime(0.1f);
        
        if (!videoPlayer.isPlaying)
        {
            Debug.LogError("[CutsceneManager] 视频播放失败！");
            Debug.LogError($"  - isPlaying: {videoPlayer.isPlaying}");
            Debug.LogError($"  - isPrepared: {videoPlayer.isPrepared}");
            Debug.LogError($"  - frame: {videoPlayer.frame}");
            Debug.LogError($"  - frameCount: {videoPlayer.frameCount}");
            OnVideoFinished(videoPlayer);
            yield break;
        }

        Debug.Log("[CutsceneManager] 视频正在播放，等待完成...");

        // 等待视频播放完成（在协程中等待，不受timeScale影响）
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }

        Debug.Log("[CutsceneManager] 视频播放完成");
        // 视频播放完成
        OnVideoFinished(videoPlayer);
    }

    /// <summary>
    /// 视频播放完成回调
    /// </summary>
    private void OnVideoFinished(VideoPlayer source)
    {
        isPlaying = false;

        // 恢复游戏时间
        if (pauseGameDuringCutscene)
        {
            Time.timeScale = savedTimeScale;
        }

        // 清理视频和RenderTexture，避免残留画面
        videoPlayer.Stop();
        videoPlayer.clip = null;
        
        // 隐藏RawImage并清理texture
        if (videoRenderTarget != null)
        {
            videoRenderTarget.texture = null;
            videoRenderTarget.enabled = false;
        }

        // 隐藏视频面板
        if (autoHideOnComplete && videoPanel != null)
        {
            videoPanel.SetActive(false);
        }

        // 调用完成回调
        onCompleteCallback?.Invoke();
        onCompleteCallback = null;
    }

    /// <summary>
    /// 根据名称获取VideoClip
    /// </summary>
    private VideoClip GetVideoClipByName(string cutsceneName)
    {
        switch (cutsceneName)
        {
            // Level1
            case "PlayerLanding":
            case "玩家降落":
            case "第一关-玩家降落":
                return level1_PlayerLanding;
            
            case "WaterFreeze":
            case "湖面结冰":
            case "湖水冻结":
            case "第一关-湖面结冰":
                return level1_WaterFreeze;
            
            case "FireIgnite":
            case "钻木取火":
            case "生火":
            case "第一关-钻木取火":
                return level1_FireIgnite;
            
            case "LightConverge":
            case "石碑点亮后主角发亮":
            case "光芒汇聚到主角身上":
            case "第一关-石碑点亮后主角发亮":
                return level1_LightConverge;

            // Level2
            case "Level2Entrance":
            case "Entrance":
            case "出场动画":
            case "第二关-出场动画":
                return level2_Entrance;
            
            case "GetFeather":
            case "拿羽毛":
            case "第二关-拿羽毛":
                return level2_GetFeather;
            
            case "SpringComplete":
            case "FlowersGrow":
            case "点亮石碑后长出花草":
            case "石碑旁边长出花草":
            case "第二关-点亮石碑后长出花草":
                return level2_FlowersGrow;

            // Level3
            case "Level3Entrance":
            case "第三关-出场动画":
                return level3_Entrance;
            
            case "SnowToIce":
            case "雪面变成冰面":
            case "第三关-雪面变成冰面":
                return level3_SnowToIce;
            
            case "IceBreak":
            case "冰面裂开":
            case "第三关-冰面裂开":
                return level3_IceBreak;
            
            case "StarRefraction":
            case "星空折射":
            case "第三关-星空折射":
                return level3_StarRefraction;
            
            case "SecondStarRefraction":
            case "第二个星空折射":
            case "第三关-第二个星空折射":
                return level3_SecondStarRefraction;
            
            case "StarComplete":
            case "TreeRevive":
            case "枯树复活":
            case "第三关-枯树复活":
                return level3_TreeRevive;
            
            case "WaterFlowToTree":
            case "湖水汇入枯树":
            case "碎裂湖水流到树上":
            case "第三关-湖水汇入枯树":
                return level3_WaterFlowToTree;

            default:
                return null;
        }
    }

    /// <summary>
    /// 停止当前播放的过场动画
    /// </summary>
    public void StopCutscene()
    {
        if (isPlaying)
        {
            videoPlayer.Stop();
            OnVideoFinished(videoPlayer);
        }
    }

    /// <summary>
    /// 检查是否正在播放过场动画
    /// </summary>
    public bool IsPlaying()
    {
        return isPlaying;
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}


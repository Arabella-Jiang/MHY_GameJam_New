using UnityEngine;

/// <summary>
/// 空气墙阻挡消息组件
/// 当玩家碰到空气墙时触发block message
/// 确保空气墙的Collider不是Trigger（用于阻挡玩家）
/// </summary>
public class AirWallBlockMessage : MonoBehaviour
{
    [Header("引用")]
    public WaterHardEffect waterHardEffect; // 关联的WaterHardEffect脚本
    public string playerTag = "Player";
    
    [Header("检测设置")]
    public float detectionDistance = 2f; // 检测玩家距离
    
    private static bool globalMessageShown = false;
    private bool hasShownMessage = false;
    private GameObject player;
    private Collider playerCollider;
    private Collider selfCollider;
    private const float FacingDotThreshold = 0.4f;
    
    void Start()
    {
        // 确保Collider不是Trigger（用于阻挡玩家）
        selfCollider = GetComponent<Collider>();
        if (selfCollider != null)
        {
            selfCollider.isTrigger = false;
        }
        
        // 如果未手动指定waterHardEffect，尝试自动查找
        if (waterHardEffect == null)
        {
            waterHardEffect = FindObjectOfType<WaterHardEffect>();
        }
        
        // 如果找到了waterHardEffect，使用它的消息设置
        if (waterHardEffect != null)
        {
            playerTag = waterHardEffect.playerTag;
        }
        
        // 查找玩家
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            player = playerObj;
            playerCollider = player.GetComponent<Collider>();
            if (playerCollider == null)
            {
                playerCollider = player.GetComponentInChildren<Collider>();
            }
        }
    }
    
    void Update()
    {
        if (hasShownMessage || globalMessageShown)
        {
            return;
        }

        // 如果水已经硬化，不需要检测
        if (waterHardEffect != null && waterHardEffect.canPass)
        {
            return;
        }
        
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        if (selfCollider != null && !selfCollider.enabled)
        {
            return;
        }
        
        // 如果玩家为空，尝试重新查找
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
            {
                player = playerObj;
                playerCollider = player.GetComponent<Collider>();
                if (playerCollider == null)
                {
                    playerCollider = player.GetComponentInChildren<Collider>();
                }
            }
            return;
        }
        
        bool facing = IsPlayerFacingAirwall();

        if (facing)
        {
            if (!hasShownMessage)
            {
                GameNotification.ShowByTrigger("Level1", "水面空气墙阻挡");
                hasShownMessage = true;
                globalMessageShown = true;
            }
        }
        else
        {
            hasShownMessage = false;
        }
    }
    
    // 保留OnTriggerEnter作为备用
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !hasShownMessage)
        {
            if (waterHardEffect != null && !waterHardEffect.canPass)
            {
                if (IsPlayerFacingAirwall())
                {
                    GameNotification.ShowByTrigger("Level1", "水面空气墙阻挡");
                    hasShownMessage = true;
                    globalMessageShown = true;
                }
            }
        }
    }

    private bool IsPlayerFacingAirwall()
    {
        if (player == null) return false;

        Vector3 toAirwall = transform.position - player.transform.position;
        toAirwall.y = 0f;
        if (toAirwall.sqrMagnitude < 0.0001f)
        {
            return false;
        }

        Vector3 playerForward = player.transform.forward;
        playerForward.y = 0f;
        if (playerForward.sqrMagnitude < 0.0001f)
        {
            playerForward = Vector3.forward;
        }

        playerForward.Normalize();
        toAirwall.Normalize();

        float dot = Vector3.Dot(playerForward, toAirwall);
        return dot >= FacingDotThreshold;
    }
}

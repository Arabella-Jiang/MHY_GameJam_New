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
    
    [Header("消息设置")]
    public string blockMessage = "水流太急，试试其他办法吧";
    public string playerTag = "Player";
    
    [Header("检测设置")]
    public float detectionDistance = 2f; // 检测玩家距离
    
    private bool hasShownMessage = false;
    private GameObject player;
    private Collider playerCollider;
    
    void Start()
    {
        // 确保Collider不是Trigger（用于阻挡玩家）
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = false;
        }
        
        // 如果未手动指定waterHardEffect，尝试自动查找
        if (waterHardEffect == null)
        {
            waterHardEffect = FindObjectOfType<WaterHardEffect>();
        }
        
        // 如果找到了waterHardEffect，使用它的消息设置
        if (waterHardEffect != null)
        {
            blockMessage = waterHardEffect.blockMessage;
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
        // 如果水已经硬化，不需要检测
        if (waterHardEffect != null && waterHardEffect.canPass)
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
        
        // 计算玩家距离
        float distance = Vector3.Distance(transform.position, player.transform.position);
        
        // 如果玩家靠近且未显示消息
        if (distance <= detectionDistance && !hasShownMessage)
        {
            // 检查水是否还未硬化
            if (waterHardEffect != null && !waterHardEffect.canPass)
            {
                Debug.Log($"✅ {blockMessage}");
                hasShownMessage = true;
            }
        }
        // 如果玩家离开
        else if (distance > detectionDistance * 1.5f && hasShownMessage)
        {
            hasShownMessage = false;
        }
    }
    
    // 保留OnCollisionEnter作为备用（如果玩家有Rigidbody）
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag) && !hasShownMessage)
        {
            if (waterHardEffect != null && !waterHardEffect.canPass)
            {
                Debug.Log($"✅ {blockMessage}");
                hasShownMessage = true;
            }
        }
    }
    
    // 保留OnTriggerEnter作为备用
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !hasShownMessage)
        {
            if (waterHardEffect != null && !waterHardEffect.canPass)
            {
                Debug.Log($"✅ {blockMessage}");
                hasShownMessage = true;
            }
        }
    }
}

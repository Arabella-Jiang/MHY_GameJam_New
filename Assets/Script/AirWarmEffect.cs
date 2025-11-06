using UnityEngine;

/// <summary>
/// 空气变暖形成上升气流效果（路径4：用"暖"属性的光对林间空气使用）
/// </summary>
public class AirWarmEffect : CombinationEffect
{
    [Header("上升气流设置")]
    public float upwardForce = 15f;          // 向上的力
    public float effectRadius = 5f;           // 作用范围
    public Vector3 effectCenter;              // 效果中心（默认为物体位置）

    [Header("视觉效果")]
    public ParticleSystem warmCurrentEffect;  // 暖流特效
    public Light warmLight;                    // 暖光效果

    private bool effectTriggered = false;
    private bool isActive = false;

    void Start()
    {
        effectCenter = transform.position;
        if (effectCenter == Vector3.zero)
        {
            effectCenter = transform.position;
        }
    }

    public override void TriggerEffect()
    {
        if (effectTriggered) return;

        effectTriggered = true;
        isActive = true;

        // 启动视觉效果
        if (warmCurrentEffect != null) warmCurrentEffect.Play();
        if (warmLight != null) warmLight.enabled = true;

        Debug.Log("✅ 林间空气变暖，形成上升暖流！");
    }

    void Update()
    {
        if (!isActive) return;

        // 检测范围内的玩家并给予向上的力
        Collider[] colliders = Physics.OverlapSphere(effectCenter, effectRadius);
        foreach (var col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                ApplyUpwardForce(col.gameObject);
            }
        }
    }

    private void ApplyUpwardForce(GameObject player)
    {
        // 尝试使用Rigidbody
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.up * upwardForce, ForceMode.Force);
        }
        else
        {
            // 使用CharacterController
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                // 通过修改velocity模拟上升
                var movement = player.GetComponent<PlayerMovement>();
                if (movement != null)
                {
                    // 这里需要在PlayerMovement中添加向上移动的支持
                    // 暂时用物理推动
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // 绘制效果范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(effectCenter, effectRadius);
    }
}


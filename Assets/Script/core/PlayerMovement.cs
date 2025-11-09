using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float runSpeed = 6f;
    public float jumpHeight = 2f;
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private float gravityValue = -9.81f;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        Debug.Log("CharacterController found: " + (controller != null));
    }

    void Update()
    {
        // 地面检测
        isGrounded = controller.isGrounded;

        /*
        // 调试信息
        if (Time.frameCount % 30 == 0) // 每30帧输出一次，避免刷屏
        {
            Debug.Log($"isGrounded: {isGrounded}, Position: {transform.position}");
        }
        */

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        // 移动
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = transform.TransformDirection(move);

        float speed = Input.GetKey(runKey) ? runSpeed : walkSpeed;
        controller.Move(move * Time.deltaTime * speed);

        // 跳跃
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            //Debug.Log("Jump! Velocity: " + playerVelocity.y);
        }

        // 重力
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log($"Collided with: {hit.gameObject.name}, Normal: {hit.normal}");
    }
}
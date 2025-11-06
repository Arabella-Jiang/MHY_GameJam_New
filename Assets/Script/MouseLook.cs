using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//implement player up-down mouse rotation to up-down view
//implement player right-left mouse rotation to right-left view
public class MouseLook : MonoBehaviour
{
    [Header("鼠标灵敏度")]
    public float verticleSenstivity;
    public float horizontalSenstivity;

    [Header("Player 坐标")]
    public Transform playerBody;
    private float xRotation = 0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
    
        float mouseX = Input.GetAxis("Mouse X") * horizontalSenstivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * verticleSenstivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

    }
}

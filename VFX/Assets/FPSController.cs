using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 6f;
    public float gravity = -9.81f;
    public float sprintMultiplier = 2f;

    [Header("Mouse")]
    public float mouseSensitivity = 100f;
    public Transform cameraPivot;

    private CharacterController controller;
    private float yVelocity;
    private float xRotation = 0f;
    
   

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Look();
        Move();
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (controller.isGrounded && yVelocity < 0)
        {
            yVelocity = -2f; // mantiene pegado al suelo
        }

        // gravedad
        yVelocity += gravity * Time.deltaTime;

        float currentSpeed = speed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= sprintMultiplier;
        }


        Vector3 velocity = move * currentSpeed;
        velocity.y = yVelocity;

        controller.Move(velocity * Time.deltaTime);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
        {
            Vector3 slopeMove = Vector3.ProjectOnPlane(move, hit.normal);
            move = slopeMove;
        }
    }

 

}

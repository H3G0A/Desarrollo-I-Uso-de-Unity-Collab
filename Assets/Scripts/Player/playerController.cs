using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction walkAction;
    private InputAction runAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private CharacterController charController;
    private Camera mainCamera;
    private float xRotation;
    private Vector3 vMovement;
    private CapsuleCollider playerCollider;

    [SerializeField] private float playerGravity = 6;
    [SerializeField] private float walkSpeed = 10;
    [SerializeField] private float runSpeed = 20;
    [SerializeField] private float jumpForce = 17;
    [SerializeField] private float airJumpForce = 17;
    [SerializeField] private float mouseSensitivity;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        walkAction = playerInput.actions["Walk"];
        runAction = playerInput.actions["Run"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        charController = GetComponent<CharacterController>();
        mainCamera = GetComponentInChildren<Camera>();
        playerCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Look();
        Jump();
    }

    private void Move()
    {
        Vector3 direction = transform.right * walkAction.ReadValue<Vector2>().x + transform.forward * walkAction.ReadValue<Vector2>().y;

        if (runAction.IsPressed())
        {
            charController.Move(Time.deltaTime * runSpeed * direction);
        }
        else
        {
            charController.Move(Time.deltaTime * walkSpeed * direction);
        }
    } 

    private void Look()
    {
        float mouseX = lookAction.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookAction.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        //Solo rota la cámara
        mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //Rota el jugador
        transform.Rotate(Vector3.up * mouseX);
    }

    //Este metodo tambien le aplica la gravedad al objeto
    private void Jump()
    {
        if (isGrounded())
        {
            vMovement = Vector3.zero;
            if (jumpAction.triggered)
            {
                vMovement.y = jumpForce;
            }
            //Debug.Log("Grounded");
        }
        else
        {
            if (jumpAction.triggered)
            {
                vMovement.y = airJumpForce;
            }
            vMovement += Vector3.down * playerGravity * Time.fixedDeltaTime;
            //Debug.Log("airbone");
        }
        charController.Move(Time.deltaTime * vMovement);
        //Debug.Log(vMovement);
    }

    /////////////////////////////AUXILIARES/////////////////////////////
    private bool isGrounded()
    {
        Vector3 center = transform.TransformPoint(playerCollider.center - Vector3.up * (playerCollider.height/2 - playerCollider.radius));
        return Physics.BoxCast(center, new Vector3(playerCollider.radius, playerCollider.radius, playerCollider.radius), Vector3.down, Quaternion.identity, .34f);
    }
}

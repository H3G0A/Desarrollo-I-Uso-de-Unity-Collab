using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction walkAction;
    private InputAction runAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private CharacterController charController;
    private Camera mainCamera;
    private float xRotation;
    private float vMovement;
    private CapsuleCollider playerCollider;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private int airJumpsCounter;

    [Header("Movement")]
    [SerializeField] private float playerGravity = 6;
    [SerializeField] private float walkSpeed = 10;
    [SerializeField] private float runSpeed = 20;
    [SerializeField] private float jumpForce = 17;
    [SerializeField] private float airJumpForce = 17;
    [SerializeField] private int airJumps = 1;
    [SerializeField] private float mouseSensitivity = 13;
    [SerializeField] private float coyoteTime = .1f;
    [SerializeField] private float jumpBuffer = .07f;
    [Header("Health")]
    [SerializeField] private int maxLife = 4;
    [SerializeField] private int life;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput = this.GetComponent<PlayerInput>();
        walkAction = playerInput.actions["Walk"];
        runAction = playerInput.actions["Run"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        charController = GetComponent<CharacterController>();
        mainCamera = GetComponentInChildren<Camera>();
        playerCollider = GetComponent<CapsuleCollider>();

        coyoteTimeCounter = 0;
        jumpBufferCounter = 0;
        airJumpsCounter = 0;
        life = maxLife;

        Cursor.lockState = CursorLockMode.Locked;
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
        float mouseX = lookAction.ReadValue<Vector2>().x * mouseSensitivity * Time.fixedDeltaTime;
        float mouseY = lookAction.ReadValue<Vector2>().y * mouseSensitivity * Time.fixedDeltaTime;

        //Debug.Log("x = " + lookAction.ReadValue<Vector2>().x);
        //Debug.Log("y = " + lookAction.ReadValue<Vector2>().y);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        //Solo rota la camara
        mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //Rota el jugador
        transform.Rotate(Vector3.up * mouseX);
    }

    //Este metodo tambien le aplica la gravedad al objeto
    private void Jump()
    {
        if (isGrounded())
        {
            vMovement = 0;
            coyoteTimeCounter = coyoteTime;
            airJumpsCounter = airJumps;
            //Debug.Log("Grounded");
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            vMovement -= Time.deltaTime * playerGravity;
            //Debug.Log("airbone");
        }

        if (jumpAction.triggered)
        {
            jumpBufferCounter = jumpBuffer;   
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        /*
         * - Mientras que el tiempo entre el que el jugador salta y el objeto alcanza el suelo esta dentro del intervalo del buffer, el personaje saltara.
         * - Si el jugador salta antes de que se acabe el tiempo del Coyote Time, el personaje saltara.
         * - Coyote Time comprueba si el jugador "esta en el suelo".
         * - Jump Buffer comprueba si el jugador "ha pulsado la tecla de salto".
         */
        if (coyoteTimeCounter > 0 && jumpBufferCounter > 0)
        {
            vMovement = jumpForce;
            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;
        }
        else if (jumpAction.triggered && airJumpsCounter > 0)
        {
            vMovement = airJumpForce;
            airJumpsCounter -= 1;
        }

        charController.Move(Time.deltaTime * vMovement * Vector3.up);
        //Debug.Log(vMovement);

        
    }

    /////////////////////////////AUXILIARES/////////////////////////////
    private bool isGrounded()
    {
        Vector3 center = transform.TransformPoint(playerCollider.center - Vector3.up * (playerCollider.height/2 - playerCollider.radius));
        bool groundCheck = Physics.BoxCast(center, new Vector3(playerCollider.radius, playerCollider.radius, playerCollider.radius),
                                            Vector3.down, Quaternion.identity, .34f);
        if(groundCheck && vMovement < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void TakeDamage(int value)
    {
        if(life > 0)
        {
            life -= value;
            Debug.Log("Life: " + life + "/" + maxLife);
            if (life <= 0)
            {
                Debug.Log("Muerto");
            }
        }
        
    }
}

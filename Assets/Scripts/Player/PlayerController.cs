using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //INPUTS
    private PlayerInput playerInput;
    private InputAction walkAction;
    private InputAction runAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction slideAction;

    private CharacterController charController;
    private CapsuleCollider playerCollider;
    private Camera mainCamera;
    private float xRotation;
    private float vMovement;
    private float fallSpeed;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private int airJumpsCounter;
    private float playerHeight;
    private bool isSliding;
    private float slideCdCounter;

    [Header("Movement")]
    [SerializeField] private float playerGravity = 6;
    [SerializeField] private float walkSpeed = 10;
    [SerializeField] private float runSpeed = 15;
    [SerializeField] private float jumpForce = 17;
    [SerializeField] private float airJumpForce = 17;
    [SerializeField] private int airJumps = 1;
    [SerializeField] private float mouseSensitivity = 13;
    [SerializeField] private float coyoteTime = .1f;
    [SerializeField] private float jumpBuffer = .07f;
    [SerializeField] private float crouchHeight = 1;
    [SerializeField] private float slideSpeed = 35;
    [SerializeField] private float slideTime = .5f;
    [SerializeField] private float slideCooldown = .5f;
    [Header("Health")]
    [SerializeField] private int maxLife = 4;
    [SerializeField] private int life;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        charController = GetComponent<CharacterController>();
        mainCamera = GetComponentInChildren<Camera>();
        playerCollider = GetComponent<CapsuleCollider>();

        walkAction = playerInput.actions["Walk"];
        runAction = playerInput.actions["Run"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        slideAction = playerInput.actions["Slide"];

        coyoteTimeCounter = 0;
        jumpBufferCounter = 0;
        airJumpsCounter = 0;
        playerHeight = charController.height;
        life = maxLife;
        slideCooldown += slideTime; //Asi, slideCooldown es el valor de enfriamiento del deslizamiento DESPUES de terminar de deslizarse

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSliding)
        {
            HorizontalMovement();
            Jump();
        }
        Gravity();
        VerticalMovement();
        Slide();
        Look();
    }

    private void HorizontalMovement()
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
    private void Gravity()
    {
        if (IsGrounded() && vMovement <= 0)
        {
            vMovement = 0;
        }
        else
        {
            vMovement -= Time.deltaTime * playerGravity;
        }
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
            airJumpsCounter = airJumps;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpAction.triggered)
        {
            jumpBufferCounter = jumpBuffer;
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

    private void VerticalMovement()
    {
        charController.Move(Time.deltaTime * vMovement * Vector3.up);
    }

    private void Slide()
    {
        if (slideAction.triggered && IsGrounded() && !isSliding && slideCdCounter <= 0)
        {
            ToggleSlide();
            Invoke(nameof(ToggleSlide), slideTime); //Termina de deslizarse cuando pasa el tiempo
        }
        else if (isSliding && CanGetUp() && !slideAction.IsPressed())
        {
            CancelInvoke(nameof(ToggleSlide));
            ToggleSlide();
        }
        if(slideCdCounter > 0)
        {
            slideCdCounter -= Time.deltaTime;
        }
    }

    /////////////////////////////AUXILIARES/////////////////////////////
    private bool IsGrounded()
    {
        Vector3 lowCenter = transform.TransformPoint(playerCollider.center + Vector3.down * (playerCollider.height/2 - playerCollider.radius));
        return Physics.BoxCast(lowCenter, new Vector3(playerCollider.radius, playerCollider.radius, playerCollider.radius),
                                            Vector3.down, Quaternion.identity, .4f);
    }

    private bool CanGetUp()
    {
        Vector3 center = transform.TransformPoint(playerCollider.center + Vector3.up * (playerCollider.height / 2 - playerCollider.radius));
        return true;
        //return Physics.BoxCast(center, new Vector3(playerCollider.radius, playerCollider.radius, playerCollider.radius), 
        //                        Vector3.up, Quaternion.identity, .34f);
    }

    private void ToggleSlide()
    {
        if (!isSliding)
        {
            isSliding = true;
            slideCdCounter = slideCooldown;
            StartCoroutine(Sliding());
            Debug.Log("Slide");
        }
        else
        {
            isSliding = false;
            Debug.Log("Stand");
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

    /////////////////////////////CORRUTINAS/////////////////////////////
    private IEnumerator Sliding()
    {
        charController.height = crouchHeight;
        playerCollider.height = crouchHeight;
        Vector3 direction = this.transform.forward;
        while (isSliding) { 
            charController.Move(Time.deltaTime * slideSpeed * direction);
            yield return null;
        }
        charController.height = playerHeight;
        playerCollider.height = playerHeight;
        this.transform.position += Vector3.up * .5f;
    }
}

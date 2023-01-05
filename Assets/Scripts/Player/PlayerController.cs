using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : HealthComponent
{
    //INPUTS
    PlayerInput playerInput;
    InputAction walkAction;
    InputAction runAction;
    InputAction lookAction;
    InputAction jumpAction;
    InputAction slideAction;

    CharacterController charController;
    CapsuleCollider playerCollider;
    Camera mainCamera;
    float xRotation;
    Vector3 hMovement;
    Vector3 vMovement;
    float coyoteTimeCounter;
    float jumpBufferCounter;
    int airJumpsCounter;
    float playerHeight;
    bool isSliding;
    float slideCdCounter;
    bool isCrouching;

    public bool wallRunning;
    public bool wallJump = false;

    [SerializeField] GameObject gun;

    [Header("Movement")]
    [SerializeField]  float playerGravity = 6;
    [SerializeField]  float walkSpeed = 10;
    [SerializeField]  float runSpeed = 17;
    [SerializeField]  float jumpForce = 17;
    [SerializeField]  float airJumpForce = 17;
    [SerializeField]  int airJumps = 1;
    [SerializeField]  float mouseSensitivity = 13;
    [SerializeField]  float coyoteTime = .1f;
    [SerializeField]  float jumpBuffer = .07f;
    [SerializeField]  float crouchHeight = 1;
    [SerializeField]  float slideSpeed = 35;
    [SerializeField]  float slideTime = .5f;
    [SerializeField]  float slideCooldown = .5f;

    void Start()
    {
        SetHealth();

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
        slideCooldown += slideTime; //Asi, slideCooldown es el valor de enfriamiento del deslizamiento DESPUES de terminar de deslizarse
        isCrouching = false;

        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log(health);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCrouching)
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
        if(!wallRunning)
        {
            Vector3 direction = transform.right * walkAction.ReadValue<Vector2>().x + transform.forward * walkAction.ReadValue<Vector2>().y;

            if (runAction.IsPressed())
            {
                hMovement = runSpeed * direction;
            }
            else
            {
                hMovement = walkSpeed * direction;
            }
            charController.Move(Time.deltaTime * hMovement);
        }
        else
        {
            //if(!wallJump)
            //{
                Vector3 direction = GetComponent<WallRunning>().wallForward;
                hMovement = runSpeed * direction;
                charController.Move(Time.deltaTime * hMovement);
                Debug.Log("#Wall Hmovement = " + hMovement);
            //}
            
        }
    }
    private void Gravity()
    {
        if ((IsGrounded() && vMovement.y <= 0) || wallRunning)
        {
            vMovement = Vector3.zero;
        }
        else
        {
            Debug.Log("#Wall Applying gravity");
            vMovement += Time.deltaTime * playerGravity * Vector3.down;
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
        if(wallJump)
        {
            Debug.Log("#Wall Applying wall jump");
            vMovement = GetComponent<WallRunning>().wallJumpUpForce * Vector3.up + GetComponent<WallRunning>().wallNormal * GetComponent<WallRunning>().wallJumpSideForce;

            wallJump = false;
        }
        else
        {
            if (coyoteTimeCounter > 0 && jumpBufferCounter > 0)
            {
                vMovement = jumpForce * Vector3.up;
                jumpBufferCounter = 0;
                coyoteTimeCounter = 0;
            }
            else if (jumpAction.triggered && airJumpsCounter > 0)
            {
                vMovement = airJumpForce * Vector3.up;
                airJumpsCounter -= 1;
            }
        }
        
    }

    private void Slide()
    {
        if (slideAction.triggered && IsGrounded() && !isSliding && slideCdCounter <= 0 && walkAction.IsPressed())
        {
            ToggleSlide();
            Invoke(nameof(ToggleSlide), slideTime); //Termina de deslizarse cuando pasa el tiempo
        }
        else if (isSliding && !slideAction.IsPressed())
        {
            CancelInvoke(nameof(ToggleSlide));
            ToggleSlide();
        }
        if (slideCdCounter > 0)
        {
            slideCdCounter -= Time.deltaTime;
        }
    }

    private void VerticalMovement()
    {
        charController.Move(Time.deltaTime * vMovement);
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
        gun.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //Rota el jugador
        transform.Rotate(Vector3.up * mouseX);
    }

    /////////////////////////////AUXILIARES/////////////////////////////
    public bool IsGrounded()
    {
        Vector3 lowCenter = transform.TransformPoint(playerCollider.center + Vector3.down * (playerCollider.height/2 - playerCollider.radius));
        bool result = Physics.BoxCast(lowCenter, new Vector3(playerCollider.radius, playerCollider.radius, playerCollider.radius),
                                            Vector3.down, Quaternion.identity, .4f);
        return result;
    }

    private bool CanGetUp()
    {
        Vector3 upperCenter = transform.TransformPoint(playerCollider.center + Vector3.up * (playerCollider.height / 2 - playerCollider.radius));
        bool result = Physics.BoxCast(upperCenter, new Vector3(playerCollider.radius, playerCollider.radius, playerCollider.radius), 
                                Vector3.up, Quaternion.identity, (playerHeight - crouchHeight)/2 + .4f);
        return !result;
    }

    private void ToggleSlide()
    {
        if (!isSliding)
        {
            isSliding = true;
            isCrouching = true;
            slideCdCounter = slideCooldown;
            StartCoroutine(Sliding());
        }
        else if(CanGetUp())
        {
            isSliding = false;
            isCrouching = false;
        }
        else
        {
            isSliding = false;
            isCrouching = true;
        }
    }

    private void OnDeath()
    {
        Debug.Log("Player died");
    }

    /////////////////////////////CORRUTINAS/////////////////////////////
    private IEnumerator Sliding()
    {
        charController.height = crouchHeight;
        playerCollider.height = crouchHeight;
        Vector3 direction = transform.right * walkAction.ReadValue<Vector2>().x + transform.forward * walkAction.ReadValue<Vector2>().y;
        Vector3.Normalize(direction);
        Debug.Log(direction);
        while (isSliding) { 
            charController.Move(Time.deltaTime * slideSpeed * direction);
            yield return null;
        }
        while (isCrouching)
        {
            yield return null;
        }
        charController.height = playerHeight;
        playerCollider.height = playerHeight;
        this.transform.position += Vector3.up * .5f;
        isCrouching = false;
    }
}

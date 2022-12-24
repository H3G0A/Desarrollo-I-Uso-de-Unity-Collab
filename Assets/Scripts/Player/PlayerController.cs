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
    private Vector3 hMovement;
    private Vector3 vMovement;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private int airJumpsCounter;
    private float playerHeight;
    private bool isSliding;
    private float slideCdCounter;
    private bool isCrouching;

    [Header("Movement")]
    [SerializeField] private float playerGravity = 6;
    [SerializeField] private float walkSpeed = 10;
    [SerializeField] private float runSpeed = 17;
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
        isCrouching = false;

        Cursor.lockState = CursorLockMode.Locked;
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
    private void Gravity()
    {
        if (IsGrounded() && vMovement.y <= 0)
        {
            vMovement = Vector3.zero;
        }
        else
        {
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

    private void Slide()
    {
        if (slideAction.triggered && IsGrounded() && !isSliding && slideCdCounter <= 0)
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
        //Debug.Log(vMovement);
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

    /////////////////////////////AUXILIARES/////////////////////////////
    private bool IsGrounded()
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

    public void TakeDamage(int value)
    {
        if(life > 0)
        {
            life -= value;
            //Debug.Log("Life: " + life + "/" + maxLife);
            if (life <= 0)
            {
                //Debug.Log("Muerto");
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

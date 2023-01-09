using System.Collections;
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
    InputAction shootAction;

    //SCRIPTS
    PlayerGun playerGunScr;
    PlayerHUD playerHUDScr;
    WallRunning wallRunningScr;

    CharacterController charController;
    Camera mainCamera;
    float xRotation;
    Vector3 hMovement;
    Vector3 vMovement;
    float coyoteTimeCounter;
    float jumpBufferCounter;
    int airJumpsCounter;
    float playerHeight;
    float slideCdCounter;

    //CHECKS
    bool isSliding;
    bool isCrouching;
    [HideInInspector] public bool wallRunning;
    [HideInInspector] public bool wallJump = false;
    bool isStepSoundPlaying = false;

    [Header("Move")]
    [SerializeField]  float walkSpeed = 10;
    [SerializeField]  float runSpeed = 17;
    [SerializeField] AudioClip stepSound;
    [Header("Jump")]
    [SerializeField]  float jumpForce = 17;
    [SerializeField]  float playerGravity = 6;
    [SerializeField]  float airJumpForce = 17;
    [SerializeField]  int airJumps = 1;
    [SerializeField]  float coyoteTime = .1f;
    [SerializeField]  float jumpBuffer = .07f;
    [SerializeField] AudioClip jumpSound;
    [Header("Slide")]
    [SerializeField]  float crouchHeight = 1;
    [SerializeField]  float slideSpeed = 35;
    [SerializeField]  float slideTime = .5f;
    [SerializeField]  float slideCooldown = .5f;
    [SerializeField] AudioClip slideSound;
    [Header("Mouse")]
    [SerializeField]  float mouseSensitivity = 13;

    protected override void Start()
    {
        base.Start();    
        
        playerInput = GetComponent<PlayerInput>();
        charController = GetComponent<CharacterController>();
        mainCamera = GetComponentInChildren<Camera>();

        wallRunningScr = GetComponent<WallRunning>();
        playerGunScr = mainCamera.GetComponentInChildren<PlayerGun>();
        playerHUDScr = GameObject.FindGameObjectWithTag("HUD").GetComponent<PlayerHUD>();

        walkAction = playerInput.actions["Walk"];
        runAction = playerInput.actions["Run"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        slideAction = playerInput.actions["Slide"];
        shootAction = playerInput.actions["Shoot"];

        coyoteTimeCounter = 0;
        jumpBufferCounter = 0;
        airJumpsCounter = 0;
        playerHeight = charController.height;
        slideCooldown += slideTime; //Asi, slideCooldown es el valor de enfriamiento del deslizamiento DESPUES de terminar de deslizarse
        isCrouching = false;
        audioSource.clip = stepSound;

        Cursor.lockState = CursorLockMode.Locked;
    }

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
        Shoot();
    }

    //////////////////////MOVEMENT
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

            if(IsGrounded() && hMovement != Vector3.zero && !isStepSoundPlaying)
            {
                StartCoroutine(StepSound());
            }
            else
            {
                StopCoroutine(StepSound());
            }

            charController.Move(Time.deltaTime * hMovement);
        }
        else
        {
            Vector3 direction = GetComponent<WallRunning>().wallForward;
            hMovement = runSpeed * direction;
            charController.Move(Time.deltaTime * hMovement);
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
            vMovement = wallRunningScr.wallJumpUpForce * Vector3.up + wallRunningScr.wallNormal * wallRunningScr.wallJumpSideForce;

            wallJump = false;
        }
        else
        {
            if (coyoteTimeCounter > 0 && jumpBufferCounter > 0) //Aqui es donde salta
            {
                audioSource.PlayOneShot(jumpSound);
                vMovement = jumpForce * Vector3.up;
                jumpBufferCounter = 0;
                coyoteTimeCounter = 0;
            }
            else if (jumpAction.triggered && airJumpsCounter > 0) //Salto en el aire
            {
                audioSource.PlayOneShot(jumpSound);
                vMovement = airJumpForce * Vector3.up;
                airJumpsCounter -= 1;
                jumpBufferCounter = 0;
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

        //Rota el jugador
        transform.Rotate(Vector3.up * mouseX);
    }

    /////////////////////////COMBAT
    private void Shoot()
    {
        if (shootAction.triggered)
        {
            playerGunScr.Shoot();
        }
    }

    public override void TakeDamage(int value) //la keyword new hace que puedas sobrescribir la funcion del padre
    {
        base.TakeDamage(value); //con base. llamamos a funciones en el padre
        playerHUDScr.SetHealth(currentHealth);
    }

    /////////////////////////////AUXILIARES/////////////////////////////
    public bool IsGrounded()
    {
        Vector3 lowCenter = transform.TransformPoint(charController.center + Vector3.down * (charController.height * .5f - charController.radius));
        Vector3 halfExtents = new Vector3(charController.radius * transform.localScale.x, charController.radius * transform.localScale.y,
                                        charController.radius * transform.localScale.z);

        bool result = Physics.BoxCast(lowCenter, halfExtents, Vector3.down, Quaternion.identity, .1f, -1, QueryTriggerInteraction.Ignore);
        return result;
    }

    

    private bool CanGetUp()
    {
        Vector3 upperCenter = transform.TransformPoint(charController.center + Vector3.up * (charController.height * .5f - charController.radius));
        Vector3 halfExtents = new Vector3(charController.radius * transform.localScale.x, charController.radius * transform.localScale.y,
                                       charController.radius * transform.localScale.z);

        bool result = Physics.BoxCast(upperCenter, halfExtents, Vector3.up, Quaternion.identity, (playerHeight - crouchHeight) * .5f + .4f);
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
        audioSource.PlayOneShot(deathSound);
        Debug.Log("Player died");
    }

    /////////////////////////////CORRUTINAS/////////////////////////////
    private IEnumerator Sliding()
    {
        charController.height = crouchHeight;

        Vector3 direction = transform.right * walkAction.ReadValue<Vector2>().x + transform.forward * walkAction.ReadValue<Vector2>().y;
        Vector3.Normalize(direction);
        //Debug.Log(direction);

        while (isSliding) 
        {
            charController.Move(Time.deltaTime * slideSpeed * direction);
            yield return null;

            if (!audioSource.isPlaying) 
            { 
                audioSource.clip = slideSound;
                audioSource.Play();
            }
        }

        audioSource.Stop();

        while (isCrouching)
        {
            yield return null;
        }

        charController.height = playerHeight;
        this.transform.position += Vector3.up * .5f;
        isCrouching = false;
    }

    private IEnumerator StepSound()
    {
        isStepSoundPlaying = true;
        float speed = hMovement.magnitude;

        audioSource.PlayOneShot(stepSound);
        yield return new WaitForSeconds(1/(speed * .15f));

        isStepSoundPlaying = false;
    }
}

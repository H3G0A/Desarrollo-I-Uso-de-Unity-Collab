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
    private CharacterController charController;
    private Camera mainCamera;
    private float xRotation;

    //Modelo?
    [SerializeField] private int maxLife = 4;
    [SerializeField] private int life;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    [SerializeField] private float mouseSensitivity;

    // Start is called before the first frame update
    void Start()
    {
        life = maxLife;
        playerInput = this.GetComponent<PlayerInput>();
        walkAction = playerInput.actions["Walk"];
        runAction = playerInput.actions["Run"];
        lookAction = playerInput.actions["Look"];
        this.charController = this.GetComponent<CharacterController>();
        this.mainCamera = this.GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Look();

    }

    private void Move()
    {
        Vector3 direction = this.transform.right * walkAction.ReadValue<Vector2>().x + this.transform.forward * walkAction.ReadValue<Vector2>().y;

        if (runAction.IsPressed())
        {
            //Debug.Log("trigger");
            this.charController.Move(Time.deltaTime * runSpeed * direction);
        }
        else
        {
            //Debug.Log("No");
            this.charController.Move(Time.deltaTime * walkSpeed * direction);
        }
    } 

    private void Look()
    {
        float mouseX = lookAction.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookAction.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;

        Debug.Log("x = " + lookAction.ReadValue<Vector2>().x);
        Debug.Log("y = " + lookAction.ReadValue<Vector2>().y);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        //Solo rota la cámara
        mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //Rota el jugador
        this.transform.Rotate(Vector3.up * mouseX);
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

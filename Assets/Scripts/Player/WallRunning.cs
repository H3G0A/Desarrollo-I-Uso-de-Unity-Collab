using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallRunning : MonoBehaviour
{
    [Header("WallRunning")]
    [SerializeField] LayerMask whatIsWall;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float wallRunForce;
    [SerializeField] public float wallJumpUpForce;
    [SerializeField] public float wallJumpSideForce;

    [Header("Inputs")]
    PlayerInput playerInput;
    InputAction jumpAction;

    [Header("Detection")]
    [SerializeField] float wallCheckDistance;
    [SerializeField] float minJumpHeight;
    RaycastHit leftWallHit;
    RaycastHit rightWallHit;
    bool wallLeft;
    bool wallRight;
    public Vector3 wallForward;
    public Vector3 wallNormal;

    [Header("Exiting")]
    [SerializeField] float exitWallTime;
    bool exitingWall;
    float exitWallTimer;

    [Header("References")]
    public Transform orientation;
    PlayerController pc;
    Rigidbody rb;


    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        jumpAction = playerInput.actions["Jump"];

        rb = GetComponent<Rigidbody>();
        pc = GetComponent<PlayerController>();
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    private void StateMachine()
    {
        //State 1 - WallRunning
        if((wallLeft || wallRight) && !pc.IsGrounded() && !exitingWall)
        {
            if(!pc.wallRunning)
            {
                StartWallRun();
            }

            //wall jump
            if(jumpAction.triggered)
            {
                Debug.Log("#Wall Salta!");
                WallJump();
            }
        }
        //State 2 - Exiting
        else if(exitingWall)
        {
            if(pc.wallRunning)
            {
                StopWallRun();
            }

            if(exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }

            if(exitWallTimer <= 0)
            {
                exitingWall = false;
            }
        }

        //State 3 - None
        else
        {
            if(pc.wallRunning)
            {
                StopWallRun();
            }
        }
    }

    private void StartWallRun()
    {
        Debug.Log("#Wall Start wall run");
        pc.wallRunning = true;
    }

    private void WallRunningMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.y);

        wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }
    }

    private void StopWallRun()
    {
        pc.wallRunning = false;
    }

    private void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;
        pc.wallJump = true;
        pc.wallRunning = false;

        wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if(pc.wallRunning)
        {
            WallRunningMovement();
        }
    }
}

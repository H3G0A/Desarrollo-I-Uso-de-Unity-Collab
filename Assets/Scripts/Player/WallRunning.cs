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
    [SerializeField] AudioClip stepWallSound;

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
    [HideInInspector] public Vector3 wallForward;
    [HideInInspector] public Vector3 wallNormal;

    [Header("Exiting")]
    [SerializeField] float exitWallTime;
    bool exitingWall;
    float exitWallTimer;

    [Header("References")]
    public Transform orientation;
    PlayerController pc;


    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        jumpAction = playerInput.actions["Jump"];

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

    

    private void WallRunningMovement()
    {
        wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }
    }


    private void StartWallRun()
    {
        Debug.Log("#Sound Starting wall Run");
        pc.wallRunning = true;
        StartCoroutine(WallSound());
    }

    private void StopWallRun()
    {
        Debug.Log("#Sound Stopping wall Run");
        pc.wallRunning = false;
        StopCoroutine(WallSound());
    }

    IEnumerator WallSound()
    {
        float speed = pc.HMovement.magnitude;
        while (pc.wallRunning)
        {
            pc.AudioSourceRef.PlayOneShot(stepWallSound);
            
            yield return new WaitForSeconds(1 / (speed * .2f));
        }
    }

    private void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;
        pc.wallJump = true;
        StopWallRun();

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

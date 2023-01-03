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
    [SerializeField] float maxWallRunTime;
    [SerializeField] float wallRunTime;

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

    [Header("References")]
    public Transform orientation;
    PlayerController pc;
    Rigidbody rb;

    // Start is called before the first frame update
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

    private bool AboveGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        //State 1 - WallRunning
        if((wallLeft || wallRight) && !pc.IsGrounded())// && AboveGround())
        {
            if(!pc.wallRunning)
            {
                StartWallRun();
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

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
    }

    private void StopWallRun()
    {
        pc.wallRunning = false;
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

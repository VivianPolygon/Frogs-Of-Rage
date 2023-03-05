using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{

    [Header("Detection")]
    [SerializeField]
    private  float wallCheckDistance;
    [SerializeField]
    private float minJumpHieght;
    [SerializeField]
    private float wallJumpForce;
    [SerializeField]
    private float wallJumpSideForce;

    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exiting")]
    private bool exitingWall;
    [SerializeField]
    private float exitWallTime;
    private float exitWallTimer;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;

    [Header("References")]
    private InputManager inputManager;
    private PlayerController playerController;
    private CharacterController controller;


    private void Start()
    {
        inputManager = InputManager.Instance;
        playerController = GetComponent<PlayerController>();
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
        if (playerController.wallRunning)
            WallRunningMovement();
    }

    private void FixedUpdate()
    {
        if(controller.isGrounded)
            playerController.useGravity = true;
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position + Vector3.up, transform.right, out rightWallHit, wallCheckDistance);
        wallLeft = Physics.Raycast(transform.position + Vector3.up , -transform.right, out leftWallHit, wallCheckDistance);

        Debug.DrawRay(transform.position + Vector3.up, transform.right, Color.red);
        Debug.DrawRay(transform.position + Vector3.up, -transform.right, Color.blue);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHieght);
    }

    private void StateMachine()
    {
        //Get input ref
        horizontalInput = inputManager.GetMovement().x;
        verticalInput = inputManager.GetMovement().y;

        //Wallrunning
        if ((wallLeft || wallRight)  && AboveGround() && !exitingWall)
        {
            if (!playerController.wallRunning)
                StartWallRun();

            if (inputManager.GetJump())
                WallJump();
        }
        else if(exitingWall)
        {
            if(playerController.wallRunning)
                StopWallRun();
            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;
            if (exitWallTimer <= 0)
                exitingWall = false;
        }
        //None
        else
        {
            if (playerController.wallRunning)
                StopWallRun();
        }
    }

    private void StartWallRun()
    {
        playerController.wallRunning = true;
    }
    private void WallRunningMovement()
    {
        playerController.useGravity = false;
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((transform.forward - wallForward).magnitude > (transform.forward - -wallForward).magnitude)
            wallForward = -wallForward;


        //Push to wall force
        if(!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
            controller.Move(-wallNormal * Time.deltaTime * 100);


    }
    private void StopWallRun()
    {
        playerController.wallRunning = false;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * wallJumpForce + wallNormal * wallJumpSideForce;

        playerController.wallVelocity -= forceToApply;

    }
    private void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Debug.Log(wallNormal);
        Debug.Log(wallRight);
        Debug.Log(rightWallHit.normal);
        Debug.Log(leftWallHit.normal);

        Vector3 forceToApply = transform.up * wallJumpForce + wallNormal * wallJumpSideForce;

        playerController.wallVelocity = new Vector3(playerController.wallVelocity.x, 0, playerController.wallVelocity.z);
        playerController.wallVelocity += forceToApply;
        Debug.Log(forceToApply);

    }
}

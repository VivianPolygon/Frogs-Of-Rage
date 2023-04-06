using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{

    [Header("Detection")]
    [SerializeField, Tooltip("How close does the player have to be to the wall")]
    private  float wallCheckDistance;
    [SerializeField, Tooltip("Minimum hieght off the floor before the player can stick to a wall")]
    private float minJumpHieght;
    [SerializeField, Tooltip("Force the player jumps up when on a wall")]
    private float wallJumpForce;
    [SerializeField, Tooltip("Force the player jumps away from the wall")]
    private float wallJumpSideForce;
    [SerializeField, Tooltip("The wall gravity counter force on the player")]
    private float wallGravityCounterForce = -3f;
    private bool useGravity = true;

    [SerializeField]
    private LayerMask wallRunLayers;

    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [HideInInspector]
    public bool onWall;

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
    //private CharacterController controller;
    private Rigidbody rb;


    private void Start()
    {
        inputManager = InputManager.Instance;
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>(); 
        //controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
        if (playerController.wallRunning)
            WallRunningMovement();
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position + Vector3.up, transform.right, out rightWallHit, wallCheckDistance, wallRunLayers, QueryTriggerInteraction.Ignore);
        wallLeft = Physics.Raycast(transform.position + Vector3.up , -transform.right, out leftWallHit, wallCheckDistance, wallRunLayers, QueryTriggerInteraction.Ignore);

        playerController.playerAnimator.SetBool("OnLeftWall", wallLeft && playerController.wallRunning);
        playerController.playerAnimator.SetBool("OnRightWall", wallRight && playerController.wallRunning);

        Debug.DrawRay(transform.position + Vector3.up, transform.right, Color.red);
        Debug.DrawRay(transform.position + Vector3.up, -transform.right, Color.blue);

        if (wallLeft || wallRight)
            onWall = true;
        else
            onWall = false;
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position + (Vector3.up / 2), Vector3.down, minJumpHieght);
    }

    private void StateMachine()
    {
        //Get input ref
        horizontalInput = inputManager.GetMovement().x;
        verticalInput = inputManager.GetMovement().y;

        //Wallrunning
        if ((wallLeft || wallRight)  && AboveGround() && !exitingWall && playerController.curStamina > 0)
        {
            if (!playerController.wallRunning)
                StartWallRun();

            if (inputManager.GetJump())
                WallJump();
        }
        //Exiting
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

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

    }
    private void WallRunningMovement()
    {
        rb.useGravity = useGravity;
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((transform.forward - wallForward).magnitude > (transform.forward - -wallForward).magnitude)
            wallForward = -wallForward;


        //Push to wall force
        if(!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
            //controller.Move(-wallNormal * Time.deltaTime * 100);
            rb.AddForce(-wallNormal * 200 * Time.deltaTime);

        //Weaken gravity on wall
        if (useGravity)
            rb.AddForce(transform.up * wallGravityCounterForce, ForceMode.Force);
        
    }
    private void StopWallRun()
    {
        playerController.wallRunning = false;
        rb.useGravity = true;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * wallJumpForce + wallNormal * wallJumpSideForce;

    }
    private void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * wallJumpForce + wallNormal * wallJumpSideForce;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

    }
}

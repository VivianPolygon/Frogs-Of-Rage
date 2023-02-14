using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerController : MonoBehaviour
{
    #region Variables in Inspector
    [Header("Movement Variables")]
    [Space(10)]
    public float staminaMax = 100f;
    [Space(5)]
    [SerializeField, Tooltip("The walk speed of the player.")]
    private float walkSpeed = 5.0f;
    [SerializeField, Tooltip("The walk speed of the player.")]
    private float sprintSpeed = 7.0f;
    [Space(5)]
    [SerializeField, Tooltip("The jump force the player has while still.")]
    private float jumpForce = 2.0f;
    [SerializeField, Tooltip("The jump force the player has while moving.")]
    private float movingJumpForce = 1.0f;
    [SerializeField, Tooltip("The gravity force on the player")]
    private float gravityValue = -9.81f;

    [Space(5)]
    [SerializeField]
    private float rotationSpeed = 4f;

    #endregion

    #region Private Variables
    private float airTime;
    private bool inAir;
    private float curStamina;
    private float staminaTimer;
    private InputManager inputManager;
    private Transform mainCamTransform;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float curSpeed;
    #endregion

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        mainCamTransform = Camera.main.transform;

        curSpeed = walkSpeed;
        curStamina = staminaMax;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        #region Movemnt
        HandleMove();
        HandleSprint();
        HandleAirTime();
        #endregion
        //Debug.Log(AirTime); 
    }

    #region Movement
    //value is true if increasing stamina and false if decreasing
    private void HandleStamina(bool value)
    {
        if (!value)
            curStamina -= Time.deltaTime;
        else
            curStamina += Time.deltaTime;

        curStamina = Mathf.Clamp(curStamina, 0, staminaMax);
    }

    //Controls player movement (WASD)
    private void HandleMove()
    {
        groundedPlayer = controller.isGrounded;

        //Ensure player's Y velocity is 0 if grounded
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        //Gets input from input manager
        Vector2 movement = inputManager.GetMovement();
        //Turns input into Vector3
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        //Utilizes camera to move in the forwward direction
        move = mainCamTransform.forward * move.z + mainCamTransform.right * move.x;
        //Helps ensure the character controller doesn't "flicker" the grounded check
        move.y = 0;
        move.Normalize();
        //Moves the actual character controller
        controller.Move(move * Time.deltaTime * curSpeed);

        //Jump
        if (inputManager.GetJump() && groundedPlayer)
        {
            //Player is moving
            if (movement == Vector2.zero)
                playerVelocity.y += Mathf.Sqrt(jumpForce * -3.0f * gravityValue);
            //Player is standing still
            else if (movement != Vector2.zero)
                playerVelocity.y += Mathf.Sqrt(movingJumpForce * 2 * -3.0f * gravityValue);
            //Debug.Log(movement);
        }


        //Adds gravity
        playerVelocity.y += gravityValue * Time.deltaTime;
        //Moves the character controller for gravity
        controller.Move(playerVelocity * Time.deltaTime);

        if (movement != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + mainCamTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }

        //Keep player facing the same direction as the camera
        //transform.rotation = Quaternion.Euler(0, mainCamTransform.rotation.eulerAngles.y, 0);
    }

    private void HandleSprint()
    {
        if (inputManager.GetSprint())
        {
            curSpeed = sprintSpeed;
            HandleStamina(false);
        }
        else
        {
            HandleStamina(true);
            curSpeed = walkSpeed;
        }
    }

    private void HandleAirTime()
    {
        if (!groundedPlayer)
        {
            airTime += Time.deltaTime;
        }
        else
            airTime = 0;
    }

    #endregion

    //Return the time player is in the air
    public float AirTime
    {
        get { return airTime; }
    }

    public delegate void OnCollectible();
    public static event OnCollectible onCollectable;
}

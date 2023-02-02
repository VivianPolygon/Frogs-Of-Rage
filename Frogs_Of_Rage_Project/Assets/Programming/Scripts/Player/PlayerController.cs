using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Variables")]
    [Space(10)]
    [SerializeField, Tooltip("The walk speed of the player.")]
    [Range(1, 10)] private float walkSpeed = 5.0f;
    [SerializeField, Tooltip("The jump force the player has.")]
    private float jumpHeight = 1.0f;
    [SerializeField, Tooltip("The gravity force on the player")]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float rotationSpeed = 4f;

    private InputManager inputManager;
    private Transform mainCamTransform;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float curSpeed;


    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        mainCamTransform = Camera.main.transform;

        curSpeed = walkSpeed;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMove();
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

        //Adds gravity
        playerVelocity.y += gravityValue * Time.deltaTime;
        //Moves the character controller for gravity
        controller.Move(playerVelocity * Time.deltaTime);

        if(movement != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + mainCamTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }

        //Keep player facing the same direction as the camera
        //transform.rotation = Quaternion.Euler(0, mainCamTransform.rotation.eulerAngles.y, 0);
    }
}

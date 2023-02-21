using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using System;

public class PlayerController : MonoBehaviour
{
    #region Variables in Inspector
    [Space(10)]
    [SerializeField]
    private float healthMax = 100f;
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

    [Space(5)]
    [SerializeField]
    private ManageSlider staminaGauge;
    [SerializeField]
    private ManageSlider healthGauge;

    #endregion

    #region Private Variables
    private float airTime;
    private bool inAir = false;
    private float staminaTimer;
    private InputManager inputManager;
    private Transform mainCamTransform;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float curSpeed;
    private Vector3 fallPos;
    private float fallTime;

    [HideInInspector]
    public float curHealth;
    [HideInInspector]
    public float curStamina;
    #endregion

    #region Events
    public delegate void OnCollectible();
    public static event OnCollectible onCollectable;


    #endregion

    #region OnEnable/OnDisable
    private void OnEnable()
    {
        #region Gauges
        ManageSlider.SetStaminaMax += SetStaminaMax;
        ManageSlider.SetStaminaValue += SetStaminaValue;
        ManageSlider.SetHealthMax += SetHealthMax;
        ManageSlider.SetHealthValue += SetHealthValue;
        #endregion
        Hazard.OnDamage += ReduceHealth;
        EventManager.OnPlayerFall += TestFall;


    }
    private void OnDisable()
    {
        #region Gauges
        ManageSlider.SetStaminaMax -= SetStaminaMax;
        ManageSlider.SetStaminaValue -= SetStaminaValue;
        ManageSlider.SetHealthMax -= SetHealthMax;
        ManageSlider.SetHealthValue -= SetHealthValue;
        #endregion
        Hazard.OnDamage -= ReduceHealth;
        EventManager.OnPlayerFall -= TestFall;

    }
    #endregion

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        mainCamTransform = Camera.main.transform;

        curSpeed = walkSpeed;
        curStamina = staminaMax;
        curHealth = healthMax;

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
            curStamina -= Time.deltaTime * 5;
        else
            curStamina += Time.deltaTime * 5;

        curStamina = Mathf.Clamp(curStamina, 0, staminaMax);
        SetStaminaValue();
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
            inAir= true;
        }
        else
            inAir = false;

        if(inAir)
        {
            airTime += Time.deltaTime;
            if(groundedPlayer)
                inAir = false;
        }
        else if(!inAir && airTime != 0)
        {
            TestFall(transform.position, airTime);
            airTime = 0; 
        }

    }

    private void TestFall(Vector3 fallpos, float time)
    {
        Debug.Log(fallpos + "  " + time);
    }

    #endregion

    #region UI

    private void SetStaminaMax()
    {
        if (staminaGauge != null)
            staminaGauge.SetMaxValue(staminaMax);
    }
    private void SetStaminaValue()
    {
        if (staminaGauge != null)
            staminaGauge.SetValue(curStamina);
    }
    private void SetHealthMax()
    {
        if (healthGauge != null)
            healthGauge.SetMaxValue(healthMax);
    }
    private void SetHealthValue()
    {
        if (healthGauge != null)
            healthGauge.SetValue(curHealth);
    }

    #endregion

    #region Health
    //This is set up for when we impliment playerfeedback when taking damage
    private void ReduceHealth()
    {
        Debug.Log("Player lost health and is now at " + curHealth);
    }



    #endregion

    //Return the time player is in the air
    public float AirTime
    {
        get { return airTime; }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Exit")
        {
            Debug.Log("You exited");
        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum MovementState
{
    Walking,
    Sprinting,
    WallRunning,
    Jumping
};

public class PlayerController : MonoBehaviour
{
    #region Variables in Inspector
    #region Movement
    [Space(10)]
    public  float curHealthMax = 100f;
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
    private float standingJumpForce = 2.0f;
    [SerializeField, Tooltip("The jump force the player has while moving.")]
    private float movingJumpForce = 1.0f;
    [SerializeField, Tooltip("The gravity force on the player")]
    private float gravityValue = -9.81f;
    [SerializeField, Tooltip("The gravity force on the player when on a slope")]
    private float slopeForce = -100f;
    [SerializeField, Tooltip("The detection distance from bottom of player down if they are on a slope")]
    private float slopeDetectionDistance = 0.2f;
    [SerializeField, Tooltip("The wall gravity force on the player")]
    private float wallGravity = -3f;


    #endregion

    #region Collectables
    [Header("Collectable Bonus")]
    [Space(5)]
    [SerializeField, Tooltip("How much speed is increased per spider collected")]
    private float speedModifier = 0.5f;
    [SerializeField, Tooltip("Max speed player can have")]
    private float maxSpeed = 15f;
    [Space(5)]

    [SerializeField, Tooltip("How much stamina is increased per ants collected")]
    private float staminaModifier = 0.5f;
    [SerializeField, Tooltip("Max stamina player can have")]
    private float maxStamina = 200f;
    [Space(5)]

    [SerializeField, Tooltip("How much total health is increased per spider collected")]
    private float healthModifier = 0.5f;
    [SerializeField, Tooltip("Max health player can have")]
    private float clampedMaxHealth = 200f;
    [Space(5)]

    [SerializeField, Tooltip("How much total jump force is increased per grasshopper collected")]
    private float jumpModifier = 0.5f;
    [SerializeField, Tooltip("Max standing jump force player can have")]
    private float maxStandingJumpForce = 10f;
    [SerializeField, Tooltip("Max moving jump force player can have")]
    private float maxMovingJumpForce = 7f;
    #endregion


    [Space(10)]
    [SerializeField]
    private float rotationSpeed = 4f;

    [Space(5)]
    [SerializeField]
    private ManageSlider staminaGauge;
    [SerializeField]
    private ManageSlider healthGauge;

    [Space(5)]
    public PlayerData playerData;
    
    #endregion

    #region Private Variables
    private float airTime;
    private bool inAir = false;
    private float staminaTimer;
    private InputManager inputManager;
    private Transform mainCamTransform;
    //private CharacterController controller;
    private Rigidbody rb;
    private bool groundedPlayer;
    private float curSpeed;
    private GameManager gameManager;
    private bool isMoving = false;
    private float baseHealth;
    private float baseStamina;
    private float baseStandingJumpForce;
    private float baseMovingJumpForce;
    public Vector3 wallVelocity;

    [HideInInspector]
    public Vector3 playerVelocity;
    [HideInInspector]
    public MovementState state;
    [HideInInspector]
    public bool walking;
    [HideInInspector]
    public bool sprinting;
    public bool wallRunning;
    [HideInInspector]
    public bool jumping;
    [HideInInspector]
    public bool useGravity = true;

    [HideInInspector]
    public float curHealth;
    [HideInInspector]
    public float curStamina;
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
        GameManager.OnPlayerDeath += Respawn;
        VacuumNavigation.onPlayerHit += VacuumInstaKill;

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
        GameManager.OnPlayerDeath -= Respawn;
        VacuumNavigation.onPlayerHit -= VacuumInstaKill;

    }
    #endregion

    private void Start()
    {
        //controller = gameObject.GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        inputManager = InputManager.Instance;
        mainCamTransform = Camera.main.transform;
        gameManager = GameManager.Instance;
        curSpeed = walkSpeed;
        curStamina = staminaMax;
        curHealth = curHealthMax;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Get base variables for collectables
        baseHealth = curHealthMax;
        baseStamina = staminaMax;
        baseStandingJumpForce = standingJumpForce;
        baseMovingJumpForce = movingJumpForce;

        gameManager.lastCheckpointPos = transform.position;
        //DontDestroyOnLoad(gameObject);


    }

    private void Update()
    {

        #region Movemnt
        HandleNormalMove();
        HandleSprint();
        HandleAirTime();
        curSpeed = IncreaseMaxSpeed();

        //HandleWallRun();
        
        #endregion
        HandlePauseMenu();
        OnPlayerCanvas?.Invoke(new PlayerCanvasEventArgs(GameManager.Instance.gameTimer, GameManager.Instance));
    }

    private void FixedUpdate()
    {
        
    }

    #region Movement

    private void StateHandler()
    {
        if(wallRunning)
        {
            state = MovementState.WallRunning;
            
        }
        if(walking)
        {
            state = MovementState.Walking;
        }
        else if(sprinting)
        {
            state = MovementState.Sprinting;
        }
        if(jumping)
        {
            state = MovementState.Jumping;
        }
    }

    #region Collectable Increase
    //Increases max stamina on collectables
    public void IncreaseMaxStamina()
    {
        float newStaminaMax;

        newStaminaMax = baseStamina + (playerData.AntCount * staminaModifier);

        newStaminaMax = Mathf.Clamp(newStaminaMax, staminaMax, maxStamina);

        staminaMax = newStaminaMax;
        curStamina = curStamina + (playerData.AntCount * staminaModifier);
    }

    //Increases jump force on collectables
    public void IncreaseJumpForce()
    {
        float newStandingJumpForce;
        float newMovingJumpForce;

        //Increase new jump forces
        newStandingJumpForce = baseStandingJumpForce + (playerData.GrasshopperCount * jumpModifier);
        newMovingJumpForce = baseMovingJumpForce + (playerData.GrasshopperCount * jumpModifier);

        //Clamp the new jump forces with min as base and max as max
        newStandingJumpForce = Mathf.Clamp(newStandingJumpForce, baseStandingJumpForce, maxStandingJumpForce);
        newMovingJumpForce = Mathf.Clamp(newMovingJumpForce, baseMovingJumpForce, maxMovingJumpForce);

        standingJumpForce = newStandingJumpForce;
        movingJumpForce = newMovingJumpForce;
    }


    //Changes speed for amount of spiders
    private float IncreaseMaxSpeed()
    {
        float newSpeed;

        newSpeed = curSpeed + (playerData.SpiderCount * speedModifier);

        newSpeed = Mathf.Clamp(newSpeed, curSpeed, maxSpeed);

        return newSpeed;
    }
    #endregion

    //value is true if increasing stamina and false if decreasing
    private void HandleStamina(bool value)
    {
        if (!value && isMoving)
            curStamina -= Time.deltaTime * 5;
        else
            curStamina += Time.deltaTime * 5;

        curStamina = Mathf.Clamp(curStamina, 0, staminaMax);
        SetStaminaValue();
    }

    //Controls player movement (WASD)
    private void HandleNormalMove()
    {


        //groundedPlayer = controller.isGrounded;

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
        //controller.Move(move * Time.deltaTime * curSpeed);
        rb.AddForce(move * Time.deltaTime * curSpeed);


        if (move != Vector3.zero)
            isMoving = true;
        else
            isMoving = false;

        //Jump
        if (inputManager.GetJump() && groundedPlayer)
        {
            jumping = true;
            //Player is standing still
            if (movement == Vector2.zero)
                playerVelocity.y += Mathf.Sqrt(standingJumpForce * -3.0f * gravityValue);
            //Player is moving
            else if (movement != Vector2.zero)
                playerVelocity.y += Mathf.Sqrt(movingJumpForce * 2f * -3.0f * gravityValue);
        }
        

        //Adjusts gravity so the player doesnt skip down slopes
        if (move != Vector3.zero && OnSlope())
            playerVelocity.y += slopeForce * Time.deltaTime;
        else
            //Adds gravity
            playerVelocity.y += gravityValue * Time.deltaTime;

        //Moves the character controller for gravity
        //if (useGravity)
        //{
        //    controller.Move(playerVelocity * Time.deltaTime);
        //    wallVelocity.y = wallGravity;

        //}
        //else
        //{
        //    controller.Move(wallVelocity * Time.deltaTime);
        //}


        //Rotates player to face direction based on input
        if (movement != Vector2.zero && state !=MovementState.WallRunning)
        {
            walking = true;
            float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + mainCamTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
        if (movement == Vector2.zero)
            walking = false;

        //Keep player facing the same direction as the camera
        //transform.rotation = Quaternion.Euler(0, mainCamTransform.rotation.eulerAngles.y, 0);

    }

    

    private void HandleSprint()
    {
        if (inputManager.GetSprint())
        {
            sprinting = true;
            curSpeed = sprintSpeed;
            HandleStamina(false);
        }
        else
        {
            sprinting = false;
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
            PlayerFell(transform.position, airTime);
            jumping = false;
            airTime = 0; 
        }

    }

    private void PlayerFell(Vector3 fallpos, float time)
    {
        //Debug.Log(fallpos + "  " + time);
        OnPlayerFall?.Invoke(new PlayerFallEventArgs(fallpos, time));
    }

    //Returns true if on a slope
    private bool OnSlope()
    {
        if (jumping)
            return false;

        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, slopeDetectionDistance))
            if (hit.normal != Vector3.up)
                return true;
        return false;
        
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
            healthGauge.SetMaxValue(curHealthMax);
    }
    private void SetHealthValue()
    {
        if (healthGauge != null)
            healthGauge.SetValue(curHealth);
    }

    private void HandlePauseMenu()
    {
        //Toggle paused bool
        if (InputManager.Instance.GetPause())
        {
            UIManager.Instance.isPaused = !UIManager.Instance.isPaused;
            OnPlayerPause?.Invoke(new PlayerPauseEventArgs(playerData));
        }
        if(!UIManager.Instance.isPaused) 
        {
            UIManager.Instance.pauseCanvas.gameObject.SetActive(false);
            Time.timeScale = 1.0f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }


    #endregion

    #region Health
    //This is set up for when we impliment playerfeedback when taking damage
    private void ReduceHealth()
    {
        //Debug.Log("Player lost health and is now at " + curHealth);
    }

    //Changes speed for amount of spiders
    public void IncreaseMaxHealth()
    {
        float newHealthMax;

        newHealthMax = baseHealth + (playerData.FlyCount * healthModifier);

        newHealthMax = Mathf.Clamp(newHealthMax, curHealth, clampedMaxHealth);

        curHealthMax = newHealthMax;
        curHealth = curHealth + (playerData.FlyCount * healthModifier);
    }
    #endregion

    #region Respawn

    private void Respawn(PlayerDeathEventArgs e)
    {
        //controller.enabled= false;
        transform.position = e.respawnPos;
        //controller.enabled = true;

    }

    private void VacuumInstaKill()
    {
        Respawn(new PlayerDeathEventArgs(gameManager.lastCheckpointPos));
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Exit")
        {
            //Invoke on player win event
            OnPlayerWin?.Invoke(new PlayerWinEventArgs(gameManager.gameTimer, playerData));
            Debug.Log("You exited");
        }
    }

    #region Events

    //public event EventHandler OnPlayerFall;

    public delegate void PlayerFallEvent(PlayerFallEventArgs e);
    public static PlayerFallEvent OnPlayerFall;

    public delegate void PlayerWinEvent(PlayerWinEventArgs e);
    public static PlayerWinEvent OnPlayerWin;

    public delegate void PlayerPauseEvent(PlayerPauseEventArgs e);
    public static PlayerPauseEvent OnPlayerPause;

    public delegate void PlayerCanvasEvent(PlayerCanvasEventArgs e);
    public static PlayerCanvasEvent OnPlayerCanvas;

    #endregion


}

#region Player Fall Event
[System.Serializable]
public class PlayerFallEvent : UnityEvent<PlayerFallEventArgs> { }
public class PlayerFallEventArgs
{
    public Vector3 fallPos;
    public float time;

   
    public PlayerFallEventArgs(Vector3 fallPos, float time)
    {
        this.fallPos = fallPos;
        this.time = time;
    }
}
#endregion

#region Player Win Event
[System.Serializable]
public class PlayerWinEvent : UnityEvent<PlayerWinEventArgs> { }
public class PlayerWinEventArgs
{
    public GameTimer gameTimer;
    public PlayerData playerData;

    public PlayerWinEventArgs(GameTimer gameTimer, PlayerData playerData)
    {
        this.gameTimer = gameTimer;
        this.playerData = playerData;
    }
}
#endregion

#region Player Canvas Event
[System.Serializable]
public class PlayerCanvasEvent : UnityEvent<PlayerCanvasEventArgs> { }
public class PlayerCanvasEventArgs
{
    public GameTimer gameTimer;
    public GameManager gameManager;

    public PlayerCanvasEventArgs(GameTimer gameTimer, GameManager gameManager)
    {
        this.gameTimer = gameTimer;
        this.gameManager = gameManager;
    }
}
#endregion

#region Player Pause Event
[System.Serializable]
public class PlayerPauseEvent : UnityEvent<PlayerPauseEventArgs> { }
public class PlayerPauseEventArgs
{
    public PlayerData playerData;

    public PlayerPauseEventArgs(PlayerData playerData)
    {
        this.playerData = playerData;
    }
}


#endregion



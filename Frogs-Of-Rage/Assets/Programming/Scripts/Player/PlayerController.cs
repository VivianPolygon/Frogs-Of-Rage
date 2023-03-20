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
    public float curStaminaMax = 100f;
    [Space(5)]
    [SerializeField, Tooltip("The walk speed of the player.")]
    private float walkSpeed = 5.0f;
    [SerializeField, Tooltip("The walk speed of the player.")]
    private float sprintSpeed = 7.0f;
    [SerializeField, Tooltip("The drag force on the player when grounded.")]
    private float groundDrag = 2.0f;
    [SerializeField, Tooltip("Multiplies speed when in air")]
    private float airMultiplier = 2.0f;
    [Space(5)]
    [SerializeField, Tooltip("The jump force the player has.")]
    private float jumpForce = 2.0f;
   
    [SerializeField]
    private float coyoteTime = 0.2f;
    [SerializeField]
    private float airDrag = 5f;
    [SerializeField]
    private float airGravity = -3f;
    [SerializeField]
    private float groundGravity = -1f;

    [SerializeField, Tooltip("The detection distance from bottom of player down if they are on a slope"), HideInInspector]
    private float slopeDetectionDistance = 0.2f;

    [SerializeField]
    private float groundCheckDistance = 0.05f;
    [SerializeField]
    private float jumpCooldown = 0.25f;



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
    private Rigidbody rb;
    private float curSpeed;
    private GameManager gameManager;
    public bool isMoving = false;
    private Vector2 movement;

    private float baseHealth;
    private float baseStamina;
    private float baseJumpForce;

    public bool canJump = true;

    private float coyoteTimeCounter;
   

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
        rb = GetComponent<Rigidbody>();
        inputManager = InputManager.Instance;
        mainCamTransform = Camera.main.transform;
        gameManager = GameManager.Instance;
        curSpeed = walkSpeed;
        curStamina = curStaminaMax;
        curHealth = curHealthMax;

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        //Get base variables for collectables
        baseHealth = curHealthMax;
        baseStamina = curStaminaMax;
        baseJumpForce = jumpForce;

        gameManager.lastCheckpointPos = transform.position;

    }

    private void Update()
    {

        #region Movemnt
        HandleDrag();
        SpeedControl();
        HandleSprint();
        HandleAirTime();
        curSpeed = IncreaseMaxSpeed();
        CoyoteTime();
        HandleJump();
        StateHandler();
        HandleStamina();
        #endregion
        HandlePauseMenu();
        if(!GroundedPlayer())
            rb.velocity += Vector3.up * Physics.gravity.y * airGravity * Time.deltaTime;

        OnPlayerCanvas?.Invoke(new PlayerCanvasEventArgs(GameManager.Instance.gameTimer, GameManager.Instance));
    }

    private void FixedUpdate()
    {
        HandleNormalMove();
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

        newStaminaMax = Mathf.Clamp(newStaminaMax, curStaminaMax, maxStamina);

        curStaminaMax = newStaminaMax;
        curStamina = curStamina + (playerData.AntCount * staminaModifier);
    }

    //Increases jump force on collectables
    public void IncreaseJumpForce()
    {
        float newStandingJumpForce;

        //Increase new jump forces
        newStandingJumpForce = baseJumpForce + (playerData.GrasshopperCount * jumpModifier);

        //Clamp the new jump forces with min as base and max as max
        newStandingJumpForce = Mathf.Clamp(newStandingJumpForce, baseJumpForce, maxStandingJumpForce);

        jumpForce = newStandingJumpForce;
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
    public void ChangeStamina(bool value)
    {
        if (!value && (isMoving || wallRunning))
            curStamina -= Time.deltaTime * 5;
        else
            curStamina += Time.deltaTime * 5;

        curStamina = Mathf.Clamp(curStamina, 0, curStaminaMax);
        SetStaminaValue();
    }
    private void HandleStamina()
    {
        if (sprinting || wallRunning)
            ChangeStamina(false);
        else
            ChangeStamina(true);
    }
    public bool GroundedPlayer()
    {
        return Physics.Raycast(transform.position + (Vector3.up / 2), Vector3.down, 0.5f + groundCheckDistance, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);
    }
    private void HandleDrag()
    {
        if (GroundedPlayer())
            rb.drag = groundDrag;
        else
            rb.drag = airDrag;
    }
    //Controls player movement (WASD)
    private void HandleNormalMove()
    {
        //Gets input from input manager
        movement = inputManager.GetMovement();
        //Turns input into Vector3
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        //Utilizes camera to move in the forwward direction
        move = mainCamTransform.forward * move.z + mainCamTransform.right * move.x;

        move.y = 0f;
        move.Normalize();
        //Moves the actual player
        if(GroundedPlayer())
            rb.AddForce(AdjustVelocityForSlope(move) * curSpeed * 10, ForceMode.Force);
        else if(!GroundedPlayer())
            rb.AddForce(move * curSpeed  * airMultiplier, ForceMode.Force);

        if (move != Vector3.zero)
            isMoving = true;
        else
            isMoving = false;

        //rb.AddForce(transform.up * groundGravity, ForceMode.Force);

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
    }
    private void HandleJump()
    {
        //movement= Vector2.zero;
        //Jump
        if (inputManager.GetJump() && coyoteTimeCounter > 0f && canJump)
        {
            canJump = false;
            
            //Reset velocity
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            jumping = true;
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);


            //Add air gravity
            //rb.AddForce(transform.up * airGravity, ForceMode.Force);

            coyoteTimeCounter = 0f;

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    private void ResetJump()
    {
        canJump = true;
    }

    private void CoyoteTime()
    {
        if (GroundedPlayer() && canJump)
        {
            coyoteTimeCounter = coyoteTime;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        }
        else
            coyoteTimeCounter -= Time.deltaTime;
    }
    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //limit velocity if needed
        if (flatVelocity.magnitude > curSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * curSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }
    private void HandleSprint()
    {
        if (inputManager.GetSprint())
        {
            sprinting = true;
            curSpeed = sprintSpeed;
        }
        else 
        {
            sprinting = false;
            curSpeed = walkSpeed;

        }
    }


    private void HandleAirTime()
    {
        if (!GroundedPlayer())
        {
            inAir= true;
        }
        else
            inAir = false;

        if(inAir)
        {
            airTime += Time.deltaTime;
            if(GroundedPlayer())
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
    private Vector3 AdjustVelocityForSlope(Vector3 velocity)
    {
        Ray ray = new Ray(transform.position + (Vector3.up /2), Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 0.5f + slopeDetectionDistance))
        {
            Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Vector3 adjustedVelocity = slopeRotation * velocity;

            if (adjustedVelocity.y < 0)
            {
                return adjustedVelocity;
            }
        }
        
        return velocity;
    }

    //Returns true if on a slope
    private bool OnSlope()
    {
        if (jumping)
            return false;

        if(Physics.Raycast(transform.position + (Vector3.up / 2), Vector3.down, out RaycastHit hit, slopeDetectionDistance))
            if (hit.normal != Vector3.up)
                return true;
        return false;
        
    }
    #endregion

    #region UI

    private void SetStaminaMax()
    {
        if (staminaGauge != null)
            staminaGauge.SetMaxValue(curStaminaMax);
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
            OnPlayerPause?.Invoke(new PlayerPauseEventArgs(playerData));
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
        transform.position = e.respawnPos;
        curHealth = curHealthMax;
        curStamina = curStaminaMax;
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



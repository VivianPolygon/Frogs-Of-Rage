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
    public float curStaminaMax = 100f;
    [Header("Movement Variables")]
    [Space(10)]
    [Space(5)]
    [Tooltip("The walk speed of the player.")]
    public float walkSpeed = 5.0f;
    [Tooltip("The walk speed of the player.")]
    public float sprintSpeed = 7.0f;
    [SerializeField, Tooltip("The deceleration on the player when grounded and no longer moving.")]
    private float groundDrag = 2.0f;
    [SerializeField, Tooltip("Multiplies speed when in air")]
    private float airMultiplier = 2.0f;
    [Header("Jump Variables")]
    [Space(10)]
    [Tooltip("The jump force the player has.")]
    public float jumpHeight = 10.0f;
    [SerializeField, Tooltip("The gravity scale on the player.")]
    private float gravityScale = 10.0f;
    [SerializeField, Tooltip("The falling gravity scale on the player.")]
    private float fallingGravityScale = 30.0f;
    [SerializeField]
    private float coyoteTime = 0.2f;
    [Space(10)]

    [SerializeField, Tooltip("The detection distance from bottom of player down if they are on a slope")]
    private float slopeDetectionDistance = 0.2f;
    [SerializeField, Tooltip("Minimum slope angle (any slope with less of an angle than this will be treated as flat ground)")]
    private float minSlopeAngle = 20f;
    [SerializeField, Range(0,1)] private float sidewaysSlopeReducer = 0.05f;

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

    [SerializeField, Tooltip("How much total jump height is increased per grasshopper collected")]
    private float jumpModifier = 0.5f;
    [SerializeField, Tooltip("Max jump height player can have")]
    private float maxJumpHeight = 10f;
    #endregion

    [Space(10)]
    public bool secondaryObjectiveComplete = false;

    [Space(10)]
    [SerializeField]
    private float rotationSpeed = 4f;

    [Space(5)]
    [SerializeField]
    private StaminaGauge staminaGauge;
    [SerializeField]
    private HealthGauge healthGauge;

    [Space(5)]
    public PlayerData playerData;

    [Space(10)]
    [Header("Health & Lives")]
    [SerializeField]
    private float healthPoolIncrement = 2f;
    [SerializeField]
    private float healthPoolWaitTime = 1.5f;
    [Space(5)]
    public int maxLives = 3;
    [HideInInspector]
    public int curLives = 3;



    


    #endregion

    #region Private Variables
    private float airTime;
    private bool inAir = false;
    private float staminaTimer;
    private InputManager inputManager;
    private Transform mainCamTransform;
    public Rigidbody rb;
    [HideInInspector]
    public float curSpeed;
    private GameManager gameManager;
    public bool isMoving = false;
    private Vector2 movement;
    [HideInInspector]
    public Vector3 moveDirection;
    private Vector3 slopeMoveDirection;
    private RaycastHit slopeHit;
    public bool onslope;

    private float baseHealth;
    private float baseStamina;
    private float baseJumpHeight;

    public bool canJump = true;

    private float coyoteTimeCounter;
    private float currentGravityScale;

    public Animator playerAnimator;
    [HideInInspector]
    public PlayerPath currentPath;

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
    [HideInInspector]
    public bool isDead = false;
    private Coroutine healthPool;
    #endregion
   
    #region OnEnable/OnDisable
    private void OnEnable()
    {
        #region Gauges
        StaminaGauge.SetStaminaMax += SetStaminaMax;
        StaminaGauge.SetStaminaValue += SetStaminaValue;
        HealthGauge.SetHealthMax += SetHealthMax;
        HealthGauge.SetHealthValue += SetHealthValue;
        #endregion
        Hazard.OnDamage += ReduceHealth;
        GameManager.OnPlayerDeath += Respawn;
        VacuumNavigation.onPlayerHit += VacuumInstaKill;

    }
    private void OnDisable()
    {
        #region Gauges
        StaminaGauge.SetStaminaMax -= SetStaminaMax;
        StaminaGauge.SetStaminaValue -= SetStaminaValue;
        HealthGauge.SetHealthMax -= SetHealthMax;
        HealthGauge.SetHealthValue -= SetHealthValue;
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
        curLives = maxLives;

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        currentGravityScale = gravityScale;

        //Get base variables for collectables
        baseHealth = curHealthMax;
        baseStamina = curStaminaMax;
        baseJumpHeight = jumpHeight;

        gameManager.lastCheckpointPos = transform.position;

    }

    private void Update()
    {

        #region Movemnt
        HandleDrag();
        //SpeedControl();
        HandleSprint();
        HandleAirTime();
        curSpeed = IncreaseMaxSpeed();
        CoyoteTime();
        HandleJump();
        StateHandler();
        HandleStamina();
        #endregion
        HandlePauseMenu();

        //rb.useGravity = !OnSlope();
        onslope = OnSlope();
        OnPlayerCanvas?.Invoke(new PlayerCanvasEventArgs(GameManager.Instance.gameTimer, GameManager.Instance));
    }

    private void FixedUpdate()
    {
        SpeedControl();
        HandleNormalMove();
        HandleGravity();
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
    public void IncreaseJumpHeight()
    {
        float newStandingJumpHeight;

        //Increase new jump forces
        newStandingJumpHeight = baseJumpHeight + (playerData.GrasshopperCount * jumpModifier);

        //Clamp the new jump forces with min as base and max as max
        newStandingJumpHeight = Mathf.Clamp(newStandingJumpHeight, baseJumpHeight, maxJumpHeight);

        jumpHeight = newStandingJumpHeight;
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
        //return Physics.Raycast(transform.position + (Vector3.up / 2), Vector3.down, 0.5f + groundCheckDistance, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);
        return Physics.CheckSphere(transform.position, groundCheckDistance, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);
    }
    private void HandleDrag()
    {
        if (movement == Vector2.zero && !jumping)
            rb.velocity = rb.velocity / groundDrag;
    }

    //Controls player movement (WASD)
    private void HandleNormalMove()
    {
        OnSlope();

        //Gets input from input manager
        movement = inputManager.GetMovement();
        //Turns input into Vector3
        moveDirection = new Vector3(movement.x, 0, movement.y);
        
        //Utilizes camera to move in the forward direction
        moveDirection = mainCamTransform.forward * moveDirection.z + mainCamTransform.right * moveDirection.x;

        //Debug.DrawRay(transform.position, slopeHit.normal * 10, Color.blue);
        //Get slope move direction
        //slopeMoveDirection = Vector3.Project(moveDirection, slopeHit.normal);
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);


        //Moves the actual player
        if (GroundedPlayer() && !OnSlope())
        {
            Debug.Log("Using normal move");
            rb.AddForce(moveDirection.normalized * curSpeed * 10, ForceMode.Force);
        }
        else if (GroundedPlayer() && OnSlope())
        {
            Debug.Log("Using slope move");
            if (slopeMoveDirection.normalized.x != 0)
            {
                Debug.Log("Reducing sideways move");
                slopeMoveDirection.x *= sidewaysSlopeReducer;
            }
            rb.AddForce(slopeMoveDirection.normalized * curSpeed * 10, ForceMode.Force);

            
            Debug.DrawRay(transform.position, slopeMoveDirection,Color.yellow);
        }
        else if (!GroundedPlayer() && !OnSlope())
        {
            Debug.Log("Using not grounded move");

            rb.AddForce(moveDirection.normalized * curSpeed * airMultiplier, ForceMode.Force);
        }

        //Debug.DrawRay(transform.position, slopeMoveDirection.normalized, Color.yellow);

        playerAnimator.SetFloat("Speed", rb.velocity.magnitude);
        playerAnimator.SetFloat("VerticalSpeed", rb.velocity.y);

        if (moveDirection != Vector3.zero)
            isMoving = true;
        else
            isMoving = false;


        //Rotates player to face direction based on input
        if (movement != Vector2.zero && state != MovementState.WallRunning)
        {
            walking = true;
            float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + mainCamTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
        if (movement == Vector2.zero)
            walking = false;
    }
    private void HandleGravity()
    {
        if(!GetComponent<WallRunning>().onWall && !OnSlope())
            rb.AddForce(Physics.gravity * (currentGravityScale - 1) * rb.mass);
    }
    private void HandleJump()
    {
        //movement= Vector2.zero;
        //Jump
        if (inputManager.GetJump() && coyoteTimeCounter > 0f && canJump)
        {
            canJump = false;
            playerAnimator.SetTrigger("Jump");
            //Reset velocity
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.drag = 0f;
            jumping = true;

            float jumpForce = Mathf.Sqrt(jumpHeight * (Physics.gravity.y * gravityScale) * - 2) * rb.mass;

            rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);



            coyoteTimeCounter = 0f;
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        if (rb.velocity.y >= 0)
            currentGravityScale = gravityScale;
        else if (rb.velocity.y < 0)
            currentGravityScale = fallingGravityScale;
    }
    private void ResetJump()
    {
        canJump = true;
        playerAnimator.ResetTrigger("Jump");

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
        if (OnSlope())
        {
            if (rb.velocity.magnitude > curSpeed)
                rb.velocity = rb.velocity.normalized * curSpeed;
        }
        else
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            //limit velocity if needed
            if (flatVelocity.magnitude > curSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * curSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
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
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, slopeDetectionDistance))
        {
            //Get rotation of objects normal
            Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            //Change velovity to align with objects normal
            Vector3 adjustedVelocity = slopeRotation * velocity;
            Debug.DrawRay(transform.position, adjustedVelocity, Color.yellow);

            if (OnSlope())
            {
                Debug.Log("Adjusted velocity");
                return adjustedVelocity;
            }
        }       
        Debug.Log("Did not adjust velocity");
        return velocity;
    }

    //Returns true if on a slope
    private bool OnSlope()
    {
        if (jumping)
            return false;

        //if (Physics.CheckSphere(transform.position, slopeDetectionDistance, ~LayerMask.GetMask("Player")))
        if(Physics.Raycast(transform.position, -transform.up, out slopeHit, slopeDetectionDistance, ~LayerMask.GetMask("Player")))
        {
            if (Vector3.Angle(slopeHit.normal, Vector3.up) >= minSlopeAngle)
                return true;
        }

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
            staminaGauge.SetValue(curStamina * staminaGauge.sliderMaxPercent);
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
        if(curHealth > 0 && !isDead)
            gameObject.GetComponentInChildren<PlayerSoundEffects>().PlayDamageAudio();
        
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

    private IEnumerator HealthPool()
    {
        while (true)
        {
            yield return new WaitForSeconds(healthPoolWaitTime / 2);
            if (curHealth  < curHealthMax)
            {
                curHealth += healthPoolIncrement;
                curHealth = Mathf.Clamp(curHealth, 0, curHealthMax);
            }

            else
                break;
            yield return new WaitForSeconds(healthPoolWaitTime / 2);
        }
    }

    private void IncreaseLives()
    {
        curLives++;
        UIManager.Instance.HandleUILives();
    }

    private bool HandleLives(PlayerGameOverEventArgs e)
    {
        curLives--;
        Debug.Log("Reduced lives");
        if (curLives > 0)
            return true;

        else 
            return false;
    }

    
    #endregion

    #region Respawn

    private void Respawn(PlayerDeathEventArgs e)
    {
        if (e.loseLife)
        {
            
            if (!HandleLives(new PlayerGameOverEventArgs(playerData)))
            {
                Debug.Log("Ran game over");
                OnGameOver?.Invoke(new PlayerGameOverEventArgs(playerData));
            }
            GetComponent<RagdollManager>().ToggleRagdoll();

        }
        transform.position = e.respawnPos;

        curHealth = curHealthMax;
        curStamina = curStaminaMax;
        isDead= false;
        //UIManager.Instance.deathCanvas.gameObject.GetComponent<Animator>().SetTrigger("FadeIn");
    }

    public void VacuumInstaKill()
    {
        curHealth = 0f;
        Debug.Log("Set players health to " + curHealth);
        //Respawn(new PlayerDeathEventArgs(gameManager.lastCheckpointPos));
    }
    #endregion

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Exit")
        {
            //Invoke on player win event
            OnPlayerWin?.Invoke(new PlayerWinEventArgs(gameManager.gameTimer, playerData, currentPath));
            Debug.Log("You exited");
        }
        if(other.tag == "HealthPool")
        {
            if(healthPool == null)
                healthPool = StartCoroutine(HealthPool());
        }
        if (other.tag == "ExtraLife")
        {
            IncreaseLives();
            Destroy(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "HealthPool")
        {
            if (healthPool != null)
            {
                StopCoroutine(healthPool);
                healthPool = null;
            }
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

    public delegate void PlayerGameOverEvent(PlayerGameOverEventArgs e);
    public static PlayerGameOverEvent OnGameOver;
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
    public PlayerPath playerPath;

    public PlayerWinEventArgs(GameTimer gameTimer, PlayerData playerData, PlayerPath path)
    {
        this.playerPath = path;
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

#region Player Game Over Event
[System.Serializable]
public class PlayerGameOverEvent : UnityEvent<PlayerGameOverEventArgs> { }
public class PlayerGameOverEventArgs
{
    public PlayerData playerData;

    public PlayerGameOverEventArgs(PlayerData playerData)
    {
        this.playerData = playerData;
    }
}
#endregion


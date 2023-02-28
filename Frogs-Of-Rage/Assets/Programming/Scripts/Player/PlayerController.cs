using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    #region Variables in Inspector
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
    private float jumpForce = 2.0f;
    [SerializeField, Tooltip("The jump force the player has while moving.")]
    private float movingJumpForce = 1.0f;
    [SerializeField, Tooltip("The gravity force on the player")]
    private float gravityValue = -9.81f;
    [SerializeField, Tooltip("The gravity force on the player when on a slope")]
    private float slopeForce = -100f;
    [SerializeField, Tooltip("The detection distance from bottom of player down if they are on a slope")]
    private float slopeDetectionDistance = 0.2f;

    [Header("Collectable Bonus")]
    [Space(5)]
    [SerializeField, Tooltip("How much speed is increased per spider collected")]
    private float speedModifier = 0.5f;
    [SerializeField, Tooltip("Max speed player can have")]
    private float maxSpeed = 15f;
    [SerializeField, Tooltip("How much total health is increased per fly collected")]
    private float healthModifier = 0.5f;
    [SerializeField, Tooltip("Max health player can have")]
    private float clampedMaxHealth = 200f;

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
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float curSpeed;
    private GameManager gameManager;
    private bool isJumping = false;
    private bool isMoving = false;


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
        controller = gameObject.GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        mainCamTransform = Camera.main.transform;
        gameManager = GameManager.Instance;
        curSpeed = walkSpeed;
        curStamina = staminaMax;
        curHealth = curHealthMax;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        gameManager.lastCheckpointPos = transform.position;
        //DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {

        #region Movemnt
        HandleMove();
        HandleSprint();
        HandleAirTime();
        curSpeed = HandleSpeed();
        
        #endregion
        HandlePauseMenu();
        OnPlayerCanvas?.Invoke(new PlayerCanvasEventArgs(GameManager.Instance.gameTimer, GameManager.Instance));
    }

    #region Movement

    //Changes speed for amount of spiders
    private float HandleSpeed()
    {
        float newSpeed;

        newSpeed = curSpeed + (playerData.SpiderCount * speedModifier);

        newSpeed = Mathf.Clamp(newSpeed, curSpeed, maxSpeed);

        return newSpeed;
    }

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

        if(move != Vector3.zero)
            isMoving= true;
        else
            isMoving = false;

        //Jump
        if (inputManager.GetJump() && groundedPlayer)
        {
            isJumping = true;
            //Player is moving
            if (movement == Vector2.zero)
                playerVelocity.y += Mathf.Sqrt(jumpForce * -3.0f * gravityValue);
            //Player is standing still
            else if (movement != Vector2.zero)
                playerVelocity.y += Mathf.Sqrt(movingJumpForce * 2 * -3.0f * gravityValue);
            //Debug.Log(movement);
        }

        if (move != Vector3.zero && OnSlope())
            playerVelocity.y += slopeForce * Time.deltaTime;
        else
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
            PlayerFell(transform.position, airTime);
            isJumping = false;
            airTime = 0; 
        }

    }

    private void PlayerFell(Vector3 fallpos, float time)
    {
        //Debug.Log(fallpos + "  " + time);
        OnPlayerFall?.Invoke(new PlayerFallEventArgs(fallpos, time));
    }

    private bool OnSlope()
    {
        if (isJumping)
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
    public void HandleHealth()
    {
        float newHealthMax;

        newHealthMax = curHealthMax + (playerData.FlyCount * healthModifier);

        newHealthMax = Mathf.Clamp(newHealthMax, curHealth, clampedMaxHealth);

        curHealthMax = newHealthMax;
        curHealth = curHealth + (playerData.FlyCount * healthModifier);
    }
    #endregion

    #region Respawn

    private void Respawn(PlayerDeathEventArgs e)
    {
        controller.enabled= false;
        transform.position = e.respawnPos;
        controller.enabled = true;

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



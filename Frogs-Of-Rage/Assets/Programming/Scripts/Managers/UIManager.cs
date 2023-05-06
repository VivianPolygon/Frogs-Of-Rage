using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Cinemachine;

public enum CanvasState
{
    Start,
    Player,
    Paused,
    Win,
    Credits,
    Options,
    Death,
    GameOver,
    HatMenu
}

public class UIManager : Singleton<UIManager>
{
    public CanvasState state;


    private InputManager inputManager;
    private Animator panelAnimator;
    [Header("Start Canvas")]
    [Space(10)]
    public Canvas startCanvas;


    [Header("Player Canvas")]
    [Space(10)]
    public Canvas playerCanvas;
    public Image collectedImage;
    public Text collectedCount;
    public GameObject collectablePanel;
    [HideInInspector]
    public CollectableData collectedData;
    public GameObject checkpointIcon;
    public Text inGameTimer;
    public GameObject UILife;



    [Header("Pause Canvas")]
    [Space(10)]
    public Canvas pauseCanvas;
    [HideInInspector]
    public Image flyImage;
    public Text flyCount;
    public Image antImage;
    public Text antCount;
    public Image grasshopperImage;
    public Text grasshopperCount;
    public Image spiderImage;
    public Text spiderCount;

    [Header("You Win Canvas")]
    [Space(10)]
    public Canvas youWinCanvas;
    public Text timer;
    public Image flyImageYouWin;
    public Text flyCountYouWin;
    public Image antImageYouWin;
    public Text antCountYouWin;
    public Image grasshopperImageYouWin;
    public Text grasshopperCountYouWin;
    public Image spiderImageYouWin;
    public Text spiderCountYouWin;
    public Leaderboard leaderboard;

    [Header("Credits Canvas")]
    [Space(10)]
    public Canvas creditsCanvas;

    [Header("Options Canvas")]
    [Space(10)]
    public Canvas optionsCanvas;
    public SettingsManager settingsManager;

    [Header("Death Canvas")]
    [Space(10)]
    public Canvas deathCanvas;

    [Header("Game Over Canvas")]
    [Space(10)]
    public Canvas gameOverCanvas;

    [Header("Hat Menu Canvas")]
    [Space(10)]
    public Canvas hatMenuCanvas;



    private List<Canvas> canvasList = new List<Canvas>();
    private List<bool> boolCanvasList = new List<bool>();

    private float finalGameTime;
    public bool isPaused = false;
    private bool isStartState = false;
    private bool isPlayerState = false;
    private bool isPausedState = false;
    private bool isWinState = false;
    private bool isCreditsState = false;
    private bool isOptionsState = false;
    private bool isDeathState = false;
    private bool isGameOverState = false;
    private bool isHatMenuCanvas = false;


    private void OnEnable()
    {
        Collectable.OnCollectable += CollectCollectable;
        PlayerController.OnPlayerWin += HandleWinScreen;
        PlayerController.OnPlayerPause += DisplayPauseScreen;
        PlayerController.OnPlayerCanvas += DisplayTimer;
        //PlayerController.OnGameOver += DisplayGameOver;
    }
    private void OnDisable()
    {
        Collectable.OnCollectable -= CollectCollectable;
        PlayerController.OnPlayerWin -= HandleWinScreen;
        PlayerController.OnPlayerPause -= DisplayPauseScreen;
        PlayerController.OnPlayerCanvas -= DisplayTimer;        
        //PlayerController.OnGameOver -= DisplayGameOver;
    }

    private void Start()
    {
        panelAnimator = collectablePanel.GetComponent<Animator>();
        inputManager = InputManager.Instance;

        //Add canvases to list in order
        canvasList.Add(startCanvas);
        canvasList.Add(playerCanvas);
        canvasList.Add(pauseCanvas);
        canvasList.Add(youWinCanvas);
        canvasList.Add(creditsCanvas);
        canvasList.Add(optionsCanvas);
        canvasList.Add(deathCanvas);
        canvasList.Add(gameOverCanvas);
        canvasList.Add(hatMenuCanvas);

        boolCanvasList.Add(isStartState);
        boolCanvasList.Add(isPlayerState);
        boolCanvasList.Add(isPausedState);
        boolCanvasList.Add(isWinState);
        boolCanvasList.Add(isCreditsState);
        boolCanvasList.Add(isOptionsState);
        boolCanvasList.Add(isDeathState);
        boolCanvasList.Add(isGameOverState);
        boolCanvasList.Add(isHatMenuCanvas);

        if(leaderboard)
        {
            leaderboard.InitilizeLeaderboard();
        }
        if(settingsManager)
        {
            settingsManager.IntilizeSettings(); //loads settings initialy
        }
    }

    private void Update()
    {
        HandleCanvasState();
    }

    public void HandleCanvasState()
    {
        CanvasState tempState = state;

        switch (state)
        {
            case CanvasState.Start:
                TurnOnCanvasIndex(0);
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                inputManager.playerControls.Player.Disable();

                break;
            case CanvasState.Player:
                TurnOnCanvasIndex(1);
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                inputManager.playerControls.Player.Enable();

                break;
            case CanvasState.Paused:
                TurnOnCanvasIndex(2);
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                inputManager.playerControls.Player.Disable();

                break;
            case CanvasState.Win:
                TurnOnCanvasIndex(3);
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                inputManager.playerControls.Player.Disable();

                break;
            case CanvasState.Credits:
                TurnOnCanvasIndex(4);
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                inputManager.playerControls.Player.Disable();

                break;
            case CanvasState.Options:
                TurnOnCanvasIndex(5);
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                inputManager.playerControls.Player.Disable();

                break;
            case CanvasState.Death:
                TurnOnCanvasIndex(6);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                inputManager.playerControls.Player.Disable();

                break;
            case CanvasState.GameOver:
                TurnOnCanvasIndex(7);
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                inputManager.playerControls.Player.Disable();

                break;
            case CanvasState.HatMenu:
                TurnOnCanvasIndex(8);
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                inputManager.playerControls.Player.Disable();

                break;
            default:
                break;
        }    
    }

    public void ChangeState(CanvasState _state)
    {
        state = _state;
    }
   
    private void TurnOnCanvasIndex(int turnOn)
    {
        //Turn off all canvases except the index passed in 'turnOn'
        for (int i = 0; i < canvasList.Count; i++)
        {
            if(i != turnOn)
                canvasList[i].gameObject.SetActive(false);
            else
                canvasList[turnOn].gameObject.SetActive(true);
        }

        //Switch all bools to false except the indec passed in 'turnOn'
        for (int i = 0; i < boolCanvasList.Count; i++)
        {
            if (i != turnOn)
                boolCanvasList[i] = false;
            else
                boolCanvasList[turnOn] = true;
        }
    }

    #region Buttons
    public void StartGame()
    {
        state = CanvasState.Player;
    }
    public void Credits()
    {
        state = CanvasState.Credits;
    }
    public void Forum()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSdVrndT0r_g_4O77PF6T1I3WF9q52PxMi9R8X7_9kJoW4alag/viewform?usp=sf_link");
    }
    public void Continue()
    {
        if(state == CanvasState.Paused)
            ToggleLivesOnPause();
        state = CanvasState.Player;
        if (isPaused)
            isPaused = !isPaused;
    }
    public void SaveGame()
    {
        Debug.Log("Saving game");

    }
    public void Options()
    {
        state = CanvasState.Options;
    }
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quitting game");
    }
    public void Menu()
    {
        state = CanvasState.Start;
        GameManager.Instance.ResetGame();
    }

    public void Paused()
    {
        state = CanvasState.Paused;

    }

    public void HatMenu()
    {
        state = CanvasState.HatMenu;
        ToggleLivesOnPause();

    }
    #endregion

    //private void DisplayGameOver(PlayerGameOverEventArgs e)
    //{
    //    state = CanvasState.GameOver;
    //}
    public void HandleUILives()
    {
        UILife.GetComponentInChildren<Canvas>().gameObject.GetComponentInChildren<Text>().text = GameManager.Instance.playerController.curLives.ToString();

        UILife.GetComponentInChildren<Animator>().SetTrigger("ShowLives");

        //if (GameManager.Instance.playerController.curLives == 0)
        //    state = CanvasState.GameOver;
        //else
            state = CanvasState.Player;

    }

    public void ToggleLivesOnPause()
    {
        UILife.GetComponentInChildren<Canvas>().gameObject.GetComponentInChildren<Text>().text = GameManager.Instance.playerController.curLives.ToString();
        UILife.GetComponentInChildren<Animator>().SetTrigger("Paused");
    }

    private void DisplayCollectedItem(CollectableData collectableData)
    {
        
        collectablePanel.SetActive(true);
        collectedImage.sprite = collectableData.Image;
        collectableData.Count++;
        collectedCount.text = collectableData.Count.ToString();

        panelAnimator.SetTrigger("Collect");
       
    }
    
    public void CollectCollectable(OnCollectableEventArgs e)
    {
        DisplayCollectedItem(e.collectableData);

        #region Check Type And Add To PlayerData
        Collectable type = e.gameObject.GetComponent<Collectable>();
        FlyCollectable fly = type as FlyCollectable;
        AntCollectable ant = type as AntCollectable;
        SpiderCollectable spider = type as SpiderCollectable;
        GrasshopperCollectable grasshopper = type as GrasshopperCollectable;

        if (type == fly)
        {
            e.playerData.FlyCount++;
            e.playerController.IncreaseMaxHealth();
        }
        else if (type == ant)
        {
            e.playerData.AntCount++;
            e.playerController.IncreaseMaxStamina();
        }
        else if (type == spider)
            e.playerData.SpiderCount++;
        else if (type == grasshopper)
        {
            e.playerData.GrasshopperCount++;
            e.playerController.IncreaseJumpHeight();
        }
        #endregion


    }

    private void DisplayPauseScreen(PlayerPauseEventArgs e)
    {
        //isPaused = !isPaused;
        Debug.Log(isPaused);
        if (isPaused && state != CanvasState.Start && state != CanvasState.GameOver)
        {
            //Change canvas state
            state = CanvasState.Paused;

            //Display UI info
            //Count
            flyCount.text = e.playerData.FlyCount.ToString() + "/" + GameManager.Instance.fliesInScene;
            antCount.text = e.playerData.AntCount.ToString() + "/" + GameManager.Instance.antsInScene;
            grasshopperCount.text = e.playerData.GrasshopperCount.ToString() + "/" + GameManager.Instance.grasshoppersInScene;
            spiderCount.text = e.playerData.SpiderCount.ToString() + "/" + GameManager.Instance.spidersInScene;

            ToggleLivesOnPause();
            //Images
            //flyImage.sprite = e.playerData.FlyImage;
            //antImage.sprite = e.playerData.AntImage;
            //grasshopperImage.sprite = e.playerData.GrasshopperImage;
            //spiderImage.sprite = e.playerData.SpiderImage;
        }
        else
        {
            state = CanvasState.Player;
        }
    }

    private void HandleWinScreen(PlayerWinEventArgs e)
    {
        state = CanvasState.Win;
        isWinState = true;
        if(isWinState)
        {
            //Change canvas state
            inputManager.playerControls.Disable();
            

            //Display UI info
            //Count
            flyCountYouWin.text = e.playerData.FlyCount.ToString() + "/" + GameManager.Instance.fliesInScene;
            antCountYouWin.text = e.playerData.AntCount.ToString() + "/" + GameManager.Instance.antsInScene;
            grasshopperCountYouWin.text = e.playerData.GrasshopperCount.ToString() + "/" + GameManager.Instance.grasshoppersInScene;
            spiderCountYouWin.text = e.playerData.SpiderCount.ToString() + "/" + GameManager.Instance.spidersInScene;
                //Images
            flyImageYouWin.sprite = e.playerData.FlyImage;
            antImageYouWin.sprite = e.playerData.AntImage;
            grasshopperImageYouWin.sprite = e.playerData.GrasshopperImage;
            spiderImageYouWin.sprite = e.playerData.SpiderImage;
                //Displays the game timers current time
            timer.text = "Your Time: " + string.Format("{0:00}:{1:00}:{2:000}", e.gameTimer.minutes, e.gameTimer.seconds, e.gameTimer.milliseconds);

            //Stores the total time it took for use later?
            finalGameTime = e.gameTimer.totalTime;

            //Do camera stuff here for the fade out and showcase level
        }
    }

    private void DisplayTimer(PlayerCanvasEventArgs e)
    {
        inGameTimer.text = string.Format("{0:00}:{01:00}:{2:000}",  e.gameTimer.minutes, e.gameTimer.seconds, e.gameTimer.milliseconds);
    }

}

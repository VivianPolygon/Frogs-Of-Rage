using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    private InputManager inputManager;
    private Animator panelAnimator;

    [Header("Player Canvas")]
    [Space(10)]
    public Image collectedImage;
    public Text collectedCount;
    public GameObject collectablePanel;
    [HideInInspector]
    public CollectableData collectedData;
    public GameObject checkpointIcon;
    public Text inGameTimer;

    [Header("Pause Canvas")]
    [Space(10)]
    public Canvas pauseCanvas;
    [HideInInspector]
    public bool isPaused = false;
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
    private bool isWin = false;
    public Image flyImageYouWin;
    public Text flyCountYouWin;
    public Image antImageYouWin;
    public Text antCountYouWin;
    public Image grasshopperImageYouWin;
    public Text grasshopperCountYouWin;
    public Image spiderImageYouWin;
    public Text spiderCountYouWin;

    private float finalGameTime;



    private void OnEnable()
    {
        Collectable.OnCollectable += CollectCollectable;
        PlayerController.OnPlayerWin += HandleWinScreen;
        PlayerController.OnPlayerPause += DisplayPauseScreen;
        PlayerController.OnPlayerCanvas += DisplayTimer;
    }
    private void OnDisable()
    {
        Collectable.OnCollectable -= CollectCollectable;
        PlayerController.OnPlayerWin -= HandleWinScreen;
        PlayerController.OnPlayerPause -= DisplayPauseScreen;
        PlayerController.OnPlayerCanvas -= DisplayTimer;
    }

    private void Start()
    {
        panelAnimator = collectablePanel.GetComponent<Animator>();
        inputManager = InputManager.Instance;
    }


    #region Buttons
    public void Continue()
    {
        isPaused = false;
    }
    public void SaveGame()
    {
        Debug.Log("Saving game");

    }
    public void Options()
    {
        Debug.Log("Options menu");

    }
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quitting game");
    }
    public void Menu()
    {
        //For now this will just reload scene 
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

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

        if(type == fly)
            e.playerData.FlyCount++;
        else if(type == ant) 
            e.playerData.AntCount++;
        else if(type == spider)
            e.playerData.SpiderCount++;
        else if(type == grasshopper)
            e.playerData.GrasshopperCount++;
        #endregion


    }

    
    private void DisplayPauseScreen(PlayerPauseEventArgs e)
    {
        //Activate win canvas
        pauseCanvas.gameObject.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //Display UI info
        //Count
        flyCount.text = e.playerData.FlyCount.ToString();
        antCount.text = e.playerData.AntCount.ToString();
        grasshopperCount.text = e.playerData.GrasshopperCount.ToString();
        spiderCount.text = e.playerData.SpiderCount.ToString();
        //Images
        flyImage.sprite = e.playerData.FlyImage;
        antImage.sprite = e.playerData.AntImage;
        grasshopperImage.sprite = e.playerData.GrasshopperImage;
        spiderImage.sprite = e.playerData.SpiderImage;
    }

    private void HandleWinScreen(PlayerWinEventArgs e)
    {
        isWin = !isWin;
        if(isWin)
        {
            //Activate win canvas
            youWinCanvas.gameObject.SetActive(true);
            inputManager.playerControls.Disable();
            isPaused = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

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
            timer.text = "Your Time: " + string.Format("{0:00}:{0:00}:{1:00}:{2:000}",e.gameTimer.hours, e.gameTimer.minutes, e.gameTimer.seconds, e.gameTimer.milliseconds);

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

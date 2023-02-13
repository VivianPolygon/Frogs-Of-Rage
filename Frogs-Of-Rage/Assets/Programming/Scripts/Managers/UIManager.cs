using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

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

    [Header("Pause Canvas")]
    [Space(10)]
    public Canvas pauseCanvas;
    private bool isPaused = false;
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
    private bool isWin = false;
    public Image flyImageYouWin;
    public Text flyCountYouWin;
    public Image antImageYouWin;
    public Text antCountYouWin;
    public Image grasshopperImageYouWin;
    public Text grasshopperCountYouWin;
    public Image spiderImageYouWin;
    public Text spiderCountYouWin;



    private void OnEnable()
    {
        FlyCollectable.OnFlyCollected += CollectCollectable;
    }
    private void OnDisable()
    {
        FlyCollectable.OnFlyCollected -= CollectCollectable;
    }

    private void Start()
    {
        panelAnimator = collectablePanel.GetComponent<Animator>();
        inputManager = InputManager.Instance;
    }

    private void Update()
    {
        HandlePauseMenu();
    }
    private void DisplayCollectedItem(CollectableData collectableData)
    {
        
        collectablePanel.SetActive(true);
        collectedImage.sprite = collectableData.Image;
        collectableData.Count++;
        collectedCount.text = collectableData.Count.ToString();

        panelAnimator.SetTrigger("Collect");
       
    }
    
    public void CollectCollectable()
    {
        DisplayCollectedItem(collectedData);
    }

    private void HandlePauseMenu()
    {
        //Toggle paused bool
        if(InputManager.Instance.GetPause())
            isPaused = !isPaused;
        if(isPaused)
        {
            pauseCanvas.gameObject.SetActive(true);
        }
        else if(!isPaused)
        {
            pauseCanvas.gameObject.SetActive(false);
        }
    }
}

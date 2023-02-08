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

    [Space(10)]
    [Header("Player Canvas")]
    public Image collectedImage;
    public Text collectedCount;
    public GameObject collectablePanel;
    public CollectableData collectedData;

    [Space(10)]
    [Header("Pause Canvas")]
    public Canvas pauseCanvas;
    private bool isPaused = false;
    public Image flyImage;
    public float flyCount;

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

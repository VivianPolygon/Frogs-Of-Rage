using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIManager : Singleton<UIManager>
{
    public Image collectedImage;
    public Text collectedCount;
    public GameObject collectablePanel;
    public CollectableData collectedData;

    private Animator panelAnimator;

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
}

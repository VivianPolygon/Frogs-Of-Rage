using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitIndicator : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerController playerController;
    public float scaleModifier = 2f;

    private Vector3 initialScale;

    private void Start()
    {
        gameManager = GameManager.Instance;
        playerController = gameManager.playerController;
        initialScale = transform.localScale;
    }

    private void Update()
    {
        Billboard();
        AdjustSize();
    }

    private void AdjustSize()
    {
        float distance = Vector3.Distance(transform.position, gameManager.playerController.transform.position);
        transform.localScale = initialScale + Vector3.one * distance * scaleModifier / 10;        
    }
    private void Billboard()
    {
        transform.forward = gameManager.mainCam.transform.forward;
    }
}

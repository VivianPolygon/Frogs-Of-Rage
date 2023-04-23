using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayerBox : MonoBehaviour
{
    protected GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        gameManager.playerController.curHealth -= 1000;
    }
}
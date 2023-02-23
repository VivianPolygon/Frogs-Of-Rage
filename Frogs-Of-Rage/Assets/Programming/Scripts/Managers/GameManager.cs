using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{

    public PlayerController playerController;
    public Camera mainCam;
    public Vector3 lastCheckpointPos;
    public GameTimer gameTimer;


    public delegate void PlayerDeath(PlayerDeathEventArgs e);
    public static event PlayerDeath OnPlayerDeath;


    public override void Awake()
    {
        base.Awake();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        mainCam = Camera.main;
        gameTimer = GetComponent<GameTimer>();
    }

    public void Update()
    {
        ManageRespawn();
    }

    private void ManageRespawn()
    {
        if (playerController.curHealth > 0)
            return;
        else if (playerController.curHealth <= 0)
        {
            playerController.curHealth = playerController.healthMax;
            OnPlayerDeath?.Invoke(new PlayerDeathEventArgs(lastCheckpointPos));

        }
    }
}
#region Player Death Event
[System.Serializable]
public class PlayerDeathEvent : UnityEvent<PlayerDeathEventArgs> { }
public class PlayerDeathEventArgs
{
    public Vector3 respawnPos;


    public PlayerDeathEventArgs(Vector3 respawnPos)
    {
        this.respawnPos = respawnPos;
    }
}
#endregion
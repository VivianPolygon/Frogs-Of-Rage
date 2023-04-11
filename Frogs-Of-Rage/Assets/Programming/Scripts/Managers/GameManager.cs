using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    
    [HideInInspector]
    public PlayerController playerController;
    [HideInInspector]
    public Camera mainCam;
    [HideInInspector]
    public Vector3 lastCheckpointPos;
    [HideInInspector]
    public Vector3 startPos;
    [HideInInspector]
    public GameTimer gameTimer;
    [HideInInspector]
    public CinemachineVirtualCamera playerCam;

    [HideInInspector]
    public float fliesInScene;
    [HideInInspector]
    public float antsInScene;
    [HideInInspector]
    public float grasshoppersInScene;
    [HideInInspector]
    public float spidersInScene;

    public List<CollectableData> collectableData = new List<CollectableData>();


    public delegate void PlayerDeath(PlayerDeathEventArgs e);
    public static event PlayerDeath OnPlayerDeath;


    public override void Awake()
    {
        base.Awake();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        mainCam = Camera.main;
        DontDestroyOnLoad(mainCam);
        gameTimer = GetComponent<GameTimer>();
        playerCam = GameObject.Find("3RD Person Cam").GetComponent<CinemachineVirtualCamera>();
        startPos = playerController.transform.position;

       
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
            playerController.curHealth = playerController.curHealthMax;
            StartCoroutine(WaitForFadeScreen(lastCheckpointPos));
        }
    }

    public IEnumerator WaitForFadeScreen(Vector3 spawnPos)
    {
        UIManager.Instance.ChangeState(CanvasState.Death);
        UIManager.Instance.GetComponent<Animator>().SetTrigger("FadeIn");
        yield return new WaitForSeconds(3.2f);
        OnPlayerDeath?.Invoke(new PlayerDeathEventArgs(spawnPos));

    }


    public void ResetCollectables()
    {
        if (playerController != null)
        {
            playerController.playerData.FlyCount = 0;
            playerController.playerData.AntCount = 0;
            playerController.playerData.SpiderCount = 0;
            playerController.playerData.GrasshopperCount = 0;

            foreach (CollectableData collectable in collectableData)
            {
                collectable.count = 0;
            }
        }
        else
            Debug.Log("Need to be in play mode to use this button");
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
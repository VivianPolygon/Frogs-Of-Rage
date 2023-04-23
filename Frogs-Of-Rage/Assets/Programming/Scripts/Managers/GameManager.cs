using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;
using UnityEngine.SceneManagement;

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
            playerController.GetComponentInChildren<PlayerSoundEffects>().PlayDeathAudio();
            playerController.curHealth = playerController.curHealthMax;
            StartCoroutine(WaitForFadeScreen(lastCheckpointPos, true));
        }
    }

    /// <summary>
    /// Changes UI state to "Death" and runs the Frog Face UI animation. 
    /// Then invokes the OnPlayerDeath event which moves the player to "spawnPos" and if "isDead" is true, 
    /// removes a players life and turns on the ragdoll
    /// </summary>
    /// <param name="spawnPos"></param>
    /// <param name="isDead"></param>
    /// <returns></returns>
    public IEnumerator WaitForFadeScreen(Vector3 spawnPos, bool isDead)
    {
        UIManager.Instance.ChangeState(CanvasState.Death);
        UIManager.Instance.GetComponent<Animator>().SetTrigger("FadeIn");
        yield return new WaitForSeconds(3.2f);

        OnPlayerDeath?.Invoke(new PlayerDeathEventArgs(spawnPos, isDead));
        
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

    public void ResetGame()
    {
        SceneManager.LoadScene(0);
        //ResetCollectables();
        //OnPlayerDeath?.Invoke(new PlayerDeathEventArgs(startPos));
        playerController.curLives = playerController.maxLives;
        playerController.curHealth = playerController.curHealthMax;
    }
}
#region Player Death Event
[System.Serializable]
public class PlayerDeathEvent : UnityEvent<PlayerDeathEventArgs> { }
public class PlayerDeathEventArgs
{
    public Vector3 respawnPos;
    public bool loseLife;

    public PlayerDeathEventArgs(Vector3 respawnPos, bool loseLife)
    {
        this.respawnPos = respawnPos;
        this.loseLife = loseLife;   
    }
}
#endregion
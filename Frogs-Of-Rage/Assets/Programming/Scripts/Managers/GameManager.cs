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

    //public static event Action<Vector3, float> onPlayerFall;
    //public static void InvokePlayerFall(Vector3 fallPos, float fallTime) { onPlayerFall.Invoke(fallPos, fallTime); }

 

    public override void Awake()
    {
        base.Awake();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        mainCam = Camera.main;
    }

}

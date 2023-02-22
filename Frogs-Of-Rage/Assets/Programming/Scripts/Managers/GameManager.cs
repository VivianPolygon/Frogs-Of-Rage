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

    public override void Awake()
    {
        base.Awake();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        mainCam = Camera.main;
    }
}

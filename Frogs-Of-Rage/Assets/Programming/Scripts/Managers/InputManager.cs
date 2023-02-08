using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    //private static InputManager _instance;
    //public static InputManager Instance { get { return _instance; } }

    [HideInInspector]
    public PlayerControls playerControls;

    public override void Awake()
    {
        //if (_instance != null && _instance != this)
        //{
        //    Destroy(this.gameObject);
        //}
        //else
        //{
        //    _instance = this;
        //}

        base.Awake();

        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }


    //Gets movement input
    public Vector2 GetMovement()
    {
        return playerControls.Player.Movement.ReadValue<Vector2>();
    }

    //Gets mouse look input
    public Vector2 GetMouseLook()
    {
        return playerControls.Player.Look.ReadValue<Vector2>();
    }
    
    //Gets pause input
    public bool GetPause()
    {
        return playerControls.Player.Pause.triggered;
    }
}

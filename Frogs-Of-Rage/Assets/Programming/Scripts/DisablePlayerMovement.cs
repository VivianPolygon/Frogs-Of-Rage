using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DisablePlayerMovement : MonoBehaviour
{
    private InputManager inputManager;

    private void Start()
    {
        inputManager = InputManager.Instance;
    }

    public void DisablePlayerInput()
    {
        inputManager.playerControls.Disable();
    }

    public void EnablePlayerInput()
    {
        inputManager.playerControls.Enable();
    }



}

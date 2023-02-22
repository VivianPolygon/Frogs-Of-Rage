using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//client state for the vacuum state pattern. controls the state logic
[RequireComponent(typeof(VacuumNavigation))] // holds navigation functions
public class VacuumNavigationController : MonoBehaviour
{
    private VacuumNavigation _vacuumNavigation; // navigation script holding AI states and sight check information

    //state change logic goes here

    private void Awake()
    {
        _vacuumNavigation = GetComponent<VacuumNavigation>();
    }

    private void Start()
    {
        _vacuumNavigation.Roam();
    }

    private void PlayerSeen()
    {
        _vacuumNavigation.Chase();
    }

    private void PlayerLost()
    {
        _vacuumNavigation.Roam();
    }

    private void OnEnable()
    {
        _vacuumNavigation.onPlayerSeen += PlayerSeen;
        _vacuumNavigation.onPlayerLost += PlayerLost;

        PlayerController.OnPlayerFall += TestPrint;
    }
    private void OnDisable()
    {
        _vacuumNavigation.onPlayerSeen -= PlayerSeen;
        _vacuumNavigation.onPlayerLost -= PlayerLost;

        PlayerController.OnPlayerFall -= TestPrint;
    }

    private void TestPrint(PlayerFallEventArgs eventArguments)
    {
        Debug.Log("Position: " + eventArguments.fallPos + "Fall Time: " + eventArguments.time);
    }
}

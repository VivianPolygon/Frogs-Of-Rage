using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//client state for the vacuum state pattern. controls the state logic
[RequireComponent(typeof(VacuumNavigation))] // holds navigation functions
public class VacuumNavigationController : MonoBehaviour
{
    private VacuumNavigation _vacuumNavigation;

    //state change logic goes here

    private void Awake()
    {
        _vacuumNavigation = GetComponent<VacuumNavigation>();
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
    }
    private void OnDisable()
    {
        _vacuumNavigation.onPlayerSeen -= PlayerSeen;
        _vacuumNavigation.onPlayerLost -= PlayerLost;
    }


}

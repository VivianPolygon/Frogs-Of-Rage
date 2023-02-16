using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//client state for the vacuum state pattern. controls the state logic
[RequireComponent(typeof(VacuumNavigation))] // holds navigation functions
public class VacuumNavigationController : MonoBehaviour
{
    private VacuumNavigation _vacuumNavigation;

    //state change logic goes here

    private void Start()
    {
        _vacuumNavigation = GetComponent<VacuumNavigation>();

        _vacuumNavigation.Roam();
    }
}

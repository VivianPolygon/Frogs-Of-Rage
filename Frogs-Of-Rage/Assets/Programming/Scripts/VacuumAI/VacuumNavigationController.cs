using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//client state for the vacuum state pattern. controls the state logic
[RequireComponent(typeof(VacuumNavigation))] // holds navigation functions
public class VacuumNavigationController : MonoBehaviour
{
    private VacuumNavigation _vacuumNavigation; // navigation script holding AI states and sight check information

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

        PlayerController.OnPlayerFall += ListenForPlayerFall;
    }
    private void OnDisable()
    {
        _vacuumNavigation.onPlayerSeen -= PlayerSeen;
        _vacuumNavigation.onPlayerLost -= PlayerLost;

        PlayerController.OnPlayerFall -= ListenForPlayerFall;
    }

    public void ListenForPlayerFall(PlayerFallEventArgs eventArguments)
    {
        VacuumHearingResult hearingResult = _vacuumNavigation.CheckSoundInRange(eventArguments.fallPos, eventArguments.time);

        if (hearingResult == VacuumHearingResult.PlayerNotHeard)
        {
            return;
        }

        if(hearingResult == VacuumHearingResult.PlayerHeardTooHigh)
        {
            _vacuumNavigation.Closeroam(eventArguments.fallPos);
        }
        else
        {
            _vacuumNavigation.Detection(eventArguments.fallPos);
        }
    }
}

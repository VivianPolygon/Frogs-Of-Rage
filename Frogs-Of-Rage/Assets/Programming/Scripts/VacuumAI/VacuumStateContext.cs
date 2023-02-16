using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumStateContext : MonoBehaviour
{
    public IVacuumState CurrentState
    {
        get;
        set;
    }

    private readonly VacuumNavigation _vacuumNavigation;

    public VacuumStateContext(VacuumNavigation vacuumNavigation)
    {
        _vacuumNavigation = vacuumNavigation;
    }

    public void TransitionStates()
    {
        CurrentState.HandleAiState(_vacuumNavigation);
    }

    public void TransitionStates(IVacuumState state)
    {
        CurrentState = state;
        CurrentState.HandleAiState(_vacuumNavigation);
    }

}

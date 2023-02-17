using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumStateContext
{
    public IVacuumState CurrentState // current state
    {
        get;
        set;
    }

    private readonly VacuumNavigation _vacuumNavigation;

    public VacuumStateContext(VacuumNavigation vacuumNavigation) // sets up the vacuumNavigationScript
    {
        _vacuumNavigation = vacuumNavigation;
    }

    public void TransitionStates() // changes the state to whatever is currently chosen, cancels current coroutine
    {
        CurrentState.HandleAiState(_vacuumNavigation);
    }

    public void TransitionStates(IVacuumState state) // changes the state to whatever script is inputed as long as it incorporates the IVacuumState Interface, cancels current coroutine
    {  
        CurrentState = state;

        CurrentState.ReceiveNavigationData(_vacuumNavigation); // gets data
        CurrentState.HandleAiState(_vacuumNavigation); // switches states
    }

}

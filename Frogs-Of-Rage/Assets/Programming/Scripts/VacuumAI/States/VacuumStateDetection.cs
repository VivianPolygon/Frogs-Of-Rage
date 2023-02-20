using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VacuumStateDetection : MonoBehaviour, IVacuumState //interface needed for each state
{
    private VacuumNavigation _vacuumNavigation; //refrence of vacuumNavigation
    private VacuumAnimation _vacuumAnimation; //refrence of vacuumAnimation

    private VacuumNavigation.GeneralData _generalData;
    private VacuumNavigation.DetectionData _detectionData;

    private List<Vector3> _roamingPossibleTargets;
    private NavMeshHit _navHit;


    public void HandleAiState(VacuumNavigation vacuumNavigation)
    {
        if (!_vacuumNavigation) _vacuumNavigation = vacuumNavigation; //null checks incoming vacuum navigation to make sure it exsists, sets is properly
        _vacuumAnimation = _vacuumNavigation.VacuumAnimation;

        //preform Ai Actions here

        _roamingPossibleTargets = new List<Vector3>();

    }

    public void ReceiveNavigationData(VacuumNavigation vacuumNavigation)
    {
        if (!_vacuumNavigation) _vacuumNavigation = vacuumNavigation; //null checks incoming vacuum navigation to make sure it exsists, sets is properly

        _generalData = new VacuumNavigation.GeneralData();
        _generalData = _vacuumNavigation.GetGeneralData();

        _detectionData = new VacuumNavigation.DetectionData();
        _detectionData = _vacuumNavigation.GetDetectionData();
    }
}
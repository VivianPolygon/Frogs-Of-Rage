using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VacuumStateRoaming : MonoBehaviour, IVacuumState //interface needed for each state
{
    private VacuumNavigation _vacuumNavigation; //instance of vacuumNavigation

    private VacuumNavigation.GeneralData _generalData;
    private VacuumNavigation.RoamingData _roamingData;
    
    private List<Vector3> _roamingPossibleTargets;
    private NavMeshHit _navHit;


    public void HandleAiState(VacuumNavigation vacuumNavigation)
    {
        if (!_vacuumNavigation) _vacuumNavigation = vacuumNavigation; //null checks incoming vacuum navigation to make sure it exsists, sets is properly

        //preform Ai Actions here

        _roamingPossibleTargets = new List<Vector3>();
        RoamingState();

    }
    #region "Roaming Behavior"

    public void ReceiveNavigationData(VacuumNavigation vacuumNavigation)
    {
        if (!_vacuumNavigation) _vacuumNavigation = vacuumNavigation; //null checks incoming vacuum navigation to make sure it exsists, sets is properly

        _generalData = new VacuumNavigation.GeneralData();
        _generalData = _vacuumNavigation.GetGeneralData();

        _roamingData = new VacuumNavigation.RoamingData();
        _roamingData = _vacuumNavigation.GetRoamingData();
    }

    private void RoamingState()
    {
        Vector3 targetRoamPosition = Vector3.zero;

        _roamingPossibleTargets.Clear();

        for (int i = 0; i < _roamingData.roamingScanCap; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * _roamingData.maxRoamingPointRange;
            randomPoint.y = transform.position.y; //sets the Y to the vacumm Y, assumes floor is perfectly flat. will need to add raycasts to find Y if floors end up being not flat
            if (NavMesh.SamplePosition(randomPoint, out _navHit, _roamingData.roamingPointScanRange, NavMesh.AllAreas))
            {
                _roamingPossibleTargets.Add(_navHit.position);
                break;
            }
        }
        //No viable point was found, stalls
        if (_roamingPossibleTargets.Count <= 0)
        {
            if (_vacuumNavigation.vacuumStateActionCoroutine == null)
            {
                _vacuumNavigation.vacuumStateActionCoroutine = StartCoroutine(Roaming(transform.position));
            }
        }
        else
        //points were found, finds farthest point and moves to it.
        {
            _roamingPossibleTargets.Sort(SortPointsByDistance);
            targetRoamPosition = _roamingPossibleTargets[0];

            if (_vacuumNavigation.vacuumStateActionCoroutine == null)
            {
                _vacuumNavigation.vacuumStateActionCoroutine = StartCoroutine(Roaming(targetRoamPosition));
            }
        }
    }

    private IEnumerator Roaming(Vector3 roamingPoint)
    {
        _vacuumNavigation.VacuumAgent.SetDestination(roamingPoint);
        _vacuumNavigation.VacuumAgent.speed = _generalData.baseRotationPhaseForwardSpeed * _roamingData.roamingSpeedFactor;

        //rotation phase
        for (float t = 0; t < _roamingData.rotationPhaseTimeCap; t += Time.deltaTime)
        {
            
            if (Vector3.Dot(Vector3.Normalize(roamingPoint - transform.position), transform.forward) > _generalData.turnAngleThreshold) //checks that the vacuum is looking in the proper cone of vision before continuing
            {
                break;
            }

            yield return null;
        }

        //movement phase
        _vacuumNavigation.VacuumAgent.speed = _generalData.baseSpeed *  _roamingData.roamingSpeedFactor;

        for (float t = 0; t < _roamingData.forceNewRoamingPointTime; t += Time.deltaTime)
        {
            if (Vector3.Distance(transform.position, roamingPoint) < _roamingData.distanceForNewPoint)
            {
                break;
            }
            yield return null;
        }
        _vacuumNavigation.vacuumStateActionCoroutine = null;
        RoamingState();
    }

    private int SortPointsByDistance(Vector3 a, Vector3 b)
    {
        if (Vector3.Distance(a, transform.position) > Vector3.Distance(b, transform.position))
        {
            return 1;
        }
        else if (Vector3.Distance(a, transform.position) < Vector3.Distance(b, transform.position))
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    #endregion

}

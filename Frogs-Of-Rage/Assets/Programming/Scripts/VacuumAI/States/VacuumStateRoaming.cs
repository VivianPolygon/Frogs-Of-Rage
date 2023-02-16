using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumStateRoaming : MonoBehaviour, IVacuumState //interface needed for each state
{
    private VacuumNavigation _vacuumNavigation; //instance of vacuumNavigation

    private VacuumNavigation.RoamingData _roamingData;

    public void HandleAiState(VacuumNavigation vacuumNavigation)
    {
        if (!_vacuumNavigation) _vacuumNavigation = vacuumNavigation; //null checks incoming vacuum navigation to make sure it exsists, sets is properly

        //preform Ai Actions here




    }
    #region "Roaming Behavior"

    public void ReceiveNavigationData(VacuumNavigation vacuumNavigation)
    {
        if (!_vacuumNavigation) _vacuumNavigation = vacuumNavigation; //null checks incoming vacuum navigation to make sure it exsists, sets is properly

        _roamingData = new VacuumNavigation.RoamingData();
        _roamingData = _vacuumNavigation.GetRoamingData();
    }


    /*
    private void RoamingState()
    {
        Vector3 targetRoamPosition = Vector3.zero;

        _roamingPossibleTargets.Clear();

        NavMeshHit navHit;

        for (int i = 0; i < _roamingScanCap; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * _maxRoamingPointRange;
            randomPoint.y = transform.position.y; //sets the Y to the vacumm Y, assumes floor is perfectly flat. will need to add raycasts to find Y if floors end up being not flat
            if (NavMesh.SamplePosition(randomPoint, out navHit, _roamingPointScanRange, NavMesh.AllAreas))
            {
                _roamingPossibleTargets.Add(navHit.position);
                break;
            }
        }
        //No viable point was found, stalls
        if (_roamingPossibleTargets.Count <= 0)
        {
            if (_actionCoroutine == null)
            {
                _actionCoroutine = StartCoroutine(Roaming(transform.position));
            }
        }
        else
        //points were found, finds farthest point and moves to it.
        {
            _roamingPossibleTargets.Sort(SortPointsByDistance);
            targetRoamPosition = _roamingPossibleTargets[0];

            if (_actionCoroutine == null)
            {
                _actionCoroutine = StartCoroutine(Roaming(targetRoamPosition));
            }
        }
    }

    private IEnumerator Roaming(Vector3 roamingPoint)
    {
        _vacuumNavigation.VacuumeAgent.SetDestination(roamingPoint);
        _vacuumNavigation.VacuumeAgent.speed = _roamingTurningForwardSpeed;

        //rotation phase
        for (float t = 0; t < _rotationPhaseTimeCap; t += Time.deltaTime)
        {
            if (Vector3.Dot(Vector3.Normalize(roamingPoint - transform.position), transform.forward) > _roamingTurnAngleThreshold) //checks that the vacuum is looking in the proper cone of vision before continuing
            {
                break;
            }

            yield return null;
        }

        //movement phase
        _vacuumNavigation.VacuumeAgent.speed = _moveSpeed;

        for (float t = 0; t < _forceNewRoamingPointTime; t += Time.deltaTime)
        {
            if (Vector3.Distance(transform.position, roamingPoint) < _distanceForNewPoint)
            {
                break;
            }
            yield return null;
        }
        _actionCoroutine = null;
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
        */
    #endregion

}

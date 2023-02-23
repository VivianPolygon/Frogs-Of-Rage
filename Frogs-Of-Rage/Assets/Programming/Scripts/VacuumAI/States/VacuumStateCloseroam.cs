using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VacuumStateCloseroam : MonoBehaviour, IVacuumState //interface needed for each state
{
    private VacuumNavigation _vacuumNavigation; //refrence of vacuumNavigation
    private VacuumAnimation _vacuumAnimation; //refrence of vacuumAnimation

    private VacuumNavigation.GeneralData _generalData;
    private VacuumNavigation.RoamingData _roamingData;
    private VacuumNavigation.DetectionData _detectionData;
    


    public void HandleAiState(VacuumNavigation vacuumNavigation)
    {
        if (!_vacuumNavigation) _vacuumNavigation = vacuumNavigation; //null checks incoming vacuum navigation to make sure it exsists, sets is properly
        _vacuumAnimation = _vacuumNavigation.VacuumAnimation;

        //preform Ai Actions here
        CloseroamState();
    }
    #region "Roaming Behavior"

    public void ReceiveNavigationData(VacuumNavigation vacuumNavigation)
    {
        if (!_vacuumNavigation) _vacuumNavigation = vacuumNavigation; //null checks incoming vacuum navigation to make sure it exsists, sets is properly

        _generalData = new VacuumNavigation.GeneralData();
        _generalData = _vacuumNavigation.GetGeneralData();

        _roamingData = new VacuumNavigation.RoamingData();
        _roamingData = _vacuumNavigation.GetRoamingData();

        _detectionData = new VacuumNavigation.DetectionData();
        _detectionData = _vacuumNavigation.GetDetectionData();
    }

    private void CloseroamState()
    {
        if (_vacuumNavigation.vacuumStateActionCoroutine == null)
        {
            _vacuumNavigation.vacuumStateActionCoroutine = _vacuumNavigation.StartCoroutine(Closeroam(_detectionData.detectionPoint));
        }
    }

    private IEnumerator Closeroam(Vector3 roamingPoint)
    {
        _vacuumNavigation.VacuumAgent.SetDestination(roamingPoint);
        _vacuumNavigation.VacuumAgent.speed = _generalData.baseRotationPhaseForwardSpeed * _roamingData.roamingSpeedFactor;
        _vacuumNavigation.VacuumAgent.angularSpeed = _generalData.baseTurnSpeed * _roamingData.roamingSpeedFactor;

        _vacuumAnimation.UpdateWheelAnimationSpeed(_roamingData.roamingSpeedFactor); // updates the animation speeds for the wheels
        _vacuumAnimation.UpdateEyeAnimation(VacuumAnimation.EyeAnimationState.Stop);

        Vector3 caculatedAngle = (roamingPoint - transform.position).normalized;
        //rotation phase
        for (float t = 0; t < _roamingData.rotationPhaseTimeCap; t += Time.deltaTime)
        {
            //wheel animation
            if (Mathf.Sign(Vector3.SignedAngle(caculatedAngle, transform.forward, transform.up)) > 0)
            {
                //Left
                _vacuumAnimation.UpdateWheelAnimationDirection(VacuumAnimation.WheelRotationDirection.Left);
            }
            else
            {
                //Right
                _vacuumAnimation.UpdateWheelAnimationDirection(VacuumAnimation.WheelRotationDirection.Right);

            }

            if (Vector3.Dot(Vector3.Normalize(roamingPoint - transform.position), transform.forward) > _generalData.turnAngleThreshold) //checks that the vacuum is looking in the proper cone of vision before continuing
            {
                break;
            }

            yield return null;
        }

        _vacuumAnimation.UpdateWheelAnimationDirection(VacuumAnimation.WheelRotationDirection.Straight);

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
        _vacuumNavigation.Roam();
    }


    #endregion

}

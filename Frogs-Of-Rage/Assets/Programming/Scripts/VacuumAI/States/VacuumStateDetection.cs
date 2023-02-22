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


    public void HandleAiState(VacuumNavigation vacuumNavigation)
    {
        if (!_vacuumNavigation) _vacuumNavigation = vacuumNavigation; //null checks incoming vacuum navigation to make sure it exsists, sets is properly
        _vacuumAnimation = _vacuumNavigation.VacuumAnimation;

        //preform Ai Actions here

        DetectionState();
    }

    public void ReceiveNavigationData(VacuumNavigation vacuumNavigation)
    {
        if (!_vacuumNavigation) _vacuumNavigation = vacuumNavigation; //null checks incoming vacuum navigation to make sure it exsists, sets is properly

        _generalData = new VacuumNavigation.GeneralData();
        _generalData = _vacuumNavigation.GetGeneralData();

        _detectionData = new VacuumNavigation.DetectionData();
        _detectionData = _vacuumNavigation.GetDetectionData();
    }

    private void DetectionState()
    {
        if (_vacuumNavigation.vacuumStateActionCoroutine == null)
        {
            _vacuumNavigation.vacuumStateActionCoroutine = _vacuumNavigation.StartCoroutine(Detection(_detectionData.detectionPoint));
        }
    }

    private IEnumerator Detection(Vector3 targetPoint)
    {
        float t;
        _vacuumNavigation.VacuumAgent.SetDestination(_detectionData.detectionPoint);
        NavMeshAgent agent = _vacuumNavigation.VacuumAgent;

        agent.speed = (_generalData.baseRotationPhaseForwardSpeed * _detectionData.detectionSpeedFactor);
        agent.angularSpeed = (_generalData.baseTurnSpeed * _detectionData.detectionSpeedFactor);

        _vacuumAnimation.UpdateWheelAnimationSpeed(_detectionData.detectionSpeedFactor);
        _vacuumAnimation.UpdateEyeAnimation(VacuumAnimation.EyeAnimationState.Stop);

        Vector3 caculatedAngle = (targetPoint - transform.position).normalized;
        for (t = 0; t < _vacuumNavigation.VacuumRotationPhaseCap; t += Time.deltaTime) // rotation phase
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

            if (Vector3.Dot(Vector3.Normalize(targetPoint - transform.position), transform.forward) > _generalData.turnAngleThreshold) //checks that the vacuum is looking in the proper cone of vision before continuing
            {
                break;
            }

            yield return null;
        }

        _vacuumAnimation.UpdateWheelAnimationDirection(VacuumAnimation.WheelRotationDirection.Straight);

        t = 0;
        agent.speed = (_generalData.baseSpeed * _detectionData.detectionSpeedFactor); //goes to point
        while(Vector3.Distance(transform.position, targetPoint) > _vacuumNavigation.VacuumPointReachedDistance)
        {
            t += Time.deltaTime;

            if (t > _vacuumNavigation.VacuumPointBailOutTime)
                _vacuumNavigation.Roam();
            yield return null;
        }

        _vacuumAnimation.UpdateWheelAnimationSpeed(0);

        agent.speed = (_generalData.baseRotationPhaseForwardSpeed * _detectionData.detectionSpeedFactor);
        agent.angularSpeed = 0;
        agent.SetDestination(transform.position + transform.forward);
        for (t = 0; t < 1; t += Time.deltaTime) //stalls for a second
        {
            yield return null;
        }

        _vacuumAnimation.UpdateWheelAnimationDirection(VacuumAnimation.WheelRotationDirection.Right);
        _vacuumAnimation.UpdateWheelAnimationSpeed(_detectionData.turboSpinSpeed * _detectionData.detectionSpeedFactor);

        agent.angularSpeed = (_generalData.baseTurnSpeed * _detectionData.detectionSpeedFactor) * _detectionData.turboSpinSpeed;
        for (t = 0; t < _detectionData.spinScanTime; t += Time.deltaTime) // spins around
        {
            agent.SetDestination(transform.position + transform.right);

            yield return null;
        }
        //exits to roaming (player wasen't found)
        _vacuumNavigation.Roam();
    }
}
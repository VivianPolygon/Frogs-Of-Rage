using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VacuumStateChasing : MonoBehaviour, IVacuumState //interface needed for each state
{
    private VacuumNavigation _vacuumNavigation; //refrence of vacuumNavigation
    private VacuumAnimation _vacuumAnimation; //refrence of vacuumAnimation

    private VacuumNavigation.GeneralData _generalData;
    private VacuumNavigation.ChasingData _chasingData;
    


    public void HandleAiState(VacuumNavigation vacuumNavigation)
    {
        if (!_vacuumNavigation) _vacuumNavigation = vacuumNavigation; //null checks incoming vacuum navigation to make sure it exsists, sets is properly
        _vacuumAnimation = _vacuumNavigation.VacuumAnimation;

        //preform Ai Actions here
        _vacuumAnimation.AnimateHeadRaise(_chasingData.headRaiseTime);
        ChasingState();

    }
    public void ReceiveNavigationData(VacuumNavigation vacuumNavigation)
    {
        if (!_vacuumNavigation) _vacuumNavigation = vacuumNavigation; //null checks incoming vacuum navigation to make sure it exsists, sets is properly

        _generalData = new VacuumNavigation.GeneralData();
        _generalData = _vacuumNavigation.GetGeneralData();

        _chasingData = new VacuumNavigation.ChasingData();
        _chasingData = _vacuumNavigation.GetChasingData();
    }

    #region "Chasing Behavior"

    private void ChasingState()
    {
        if (_vacuumNavigation.vacuumStateActionCoroutine != null)
        {
            _vacuumNavigation.StopCoroutine(_vacuumNavigation.vacuumStateActionCoroutine);
        }
        _vacuumNavigation.vacuumStateActionCoroutine = _vacuumNavigation.StartCoroutine(ChasePlayer());
    }

    private IEnumerator ChasePlayer()
    {
        Vector3 PlayerPosition = Vector3.zero;
        _vacuumNavigation.VacuumAgent.angularSpeed = _generalData.baseTurnSpeed * _chasingData.chasingSpeedFactor;
        _vacuumNavigation.VacuumAgent.speed = _generalData.baseSpeed * _chasingData.chasingSpeedFactor;

        //makes sure player transform isint null, stores its position incase it goes null mid loop. returns to roaming if null
        if (_vacuumNavigation.PlayerTransform == null)
        {
            yield break;
        }
        else
        {
            PlayerPosition = _vacuumNavigation.PlayerTransform.position;
        }

        if(_chasingData.attackDistance > Vector3.Distance(transform.position, PlayerPosition))
        {
            _vacuumAnimation.AnimateHeadDrop(_chasingData.headDropAttackTime);

            for (float t = 0; t < _chasingData.headDropAttackTime; t += Time.deltaTime)
            {
                yield return null;
            }

            _vacuumNavigation.VacuumAgent.velocity = Vector3.zero;
            _vacuumAnimation.AnimateHeadRaise(_chasingData.attackMissHeadRaiseTime);

            for (float t = 0; t < _chasingData.attackMissHeadRaiseTime; t += Time.deltaTime)
            {
                yield return null;
            }
        }

        //rapid rotation phase
        _vacuumNavigation.VacuumAgent.SetDestination(PlayerPosition);
        //_vacuumNavigation.VacuumAgent.speed = _generalData.baseRotationPhaseForwardSpeed * _chasingData.chasingSpeedFactor;


        /*
        for (float t = 0; t < _chasingData.maxChaseRotationPhaseLength; t += Time.deltaTime)
        {
            if (Vector3.Dot(Vector3.Normalize(PlayerPosition - transform.position), transform.forward) > _generalData.turnAngleThreshold) //checks that the vacuum is looking in the proper cone of vision before continuing
            {
                break;
            }
            yield return null;
        }
        */

        for (float t = 0; t < _chasingData.chasingUpdatePositionRate; t += Time.deltaTime)
        {
            yield return null;
        }
        _vacuumNavigation.vacuumStateActionCoroutine = null;
        ChasingState();
    }


    #endregion

}

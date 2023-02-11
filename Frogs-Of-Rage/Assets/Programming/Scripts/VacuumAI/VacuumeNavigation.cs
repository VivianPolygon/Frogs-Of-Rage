using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class VacuumeNavigation : MonoBehaviour
{
    //internaly used instance for the vacuum navmesh agent
    private NavMeshAgent _vacuumAgent;

    //varriables used to set up the nav mesh agent
    [Header("Vacuum Speed Variables")]
    [SerializeField] [Tooltip("The Speed the Vacuum Moves Forward")] private float _moveSpeed;
    [SerializeField] [Tooltip("The Speed the Vacuum Turns")] private float _turnSpeed;
    [SerializeField] [Tooltip("The Rate the Vacuum Accelerates")] private float _acceleration;

    [Header("Vacuum Navigation Variables, Roaming")]
    [SerializeField] [Tooltip("Distance Scanned when Choosing Point While Roaming")] private float _maxRoamingPointRange;
    [SerializeField] [Tooltip("Distance from a navmesh a scan can give to allow navigation")] private float _roamingPointScanRange;
    [SerializeField] [Tooltip("Number of Times Scanned to Find a point. If scans are unsucsessful, Vacuume Stays Still Untill Next Scan")] private float _roamingScanCap;
    [SerializeField] [Tooltip("Amount of Time Before The Vacume is forced a new point if it hasent reached its target point. prevents getting stuck")] private float _forceNewRoamingPointTime;
    [SerializeField] [Tooltip("Amount of World Units away From the Target the Vacume needs to Be away from the target point before being able to find a new point")] private float _distanceForNewPoint;
    [SerializeField] [Tooltip("Forward Movespeed of the Vacuum while turning, Lower than normal")] [Range(0.25f, 3)] private float _roamingTurningForwardSpeed;
    [SerializeField] [Tooltip("Time Limit on the Rotation Phase")] [Range(0.1f, 3f)] private float _rotationPhaseTimeCap;
    [SerializeField] [Tooltip("Angle the Vacuum Turns too before it can move forward. The closer to one, the more diretly pointed towards the point the vacuum is")] [Range(-1, 0.99f)] float _roamingTurnAngleThreshold;
    //Internal Roaming Variables
    private List<Vector3> _roamingPossibleTargets;


    //Coroutines ussed to control States that have repeated behaviors with time delays
    private Coroutine _roamCoroutine;


    //property for acsessing version elsewhere, used here too, to prevent null issues.
    public NavMeshAgent VacuumeAgent
    {
        get
        {
            if (_vacuumAgent == null)
            {
                InitilizeNavmeshAgent();
            }

            return _vacuumAgent;
        }
    }


    private void Awake()
    {
        InitilizeNavmeshAgent();

        _roamCoroutine = null;

        _roamingPossibleTargets = new List<Vector3>();

        RoamingChooseRandomPoint();
    }



    //initilizes the navmesh agent, creating it, or updating the current one with the vacume movment data
    private void InitilizeNavmeshAgent()
    {
        NavMeshAgent newAgent = new NavMeshAgent();

        //either adds, or uses a current nav mesh agent on the vacuum
        if (gameObject.TryGetComponent(out NavMeshAgent agent))
        {
            _vacuumAgent = gameObject.GetComponent<NavMeshAgent>();
        }
        else
        {
            _vacuumAgent = gameObject.AddComponent<NavMeshAgent>();
        }

        _vacuumAgent.speed = _moveSpeed;
        _vacuumAgent.angularSpeed = _turnSpeed;
        _vacuumAgent.acceleration = _acceleration;

        //adjustments that are assumed
        _vacuumAgent.autoBraking = false;
        _vacuumAgent.stoppingDistance = 0;
    }

    #region "Roaming Behavior"

    private void RoamingChooseRandomPoint()
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
        if(_roamingPossibleTargets.Count <= 0)
        {
            if (_roamCoroutine == null)
            {
                _roamCoroutine = StartCoroutine(Roaming(transform.position));
            }
        }
        //points were found, finds farthest point and moves to it.
        {
            _roamingPossibleTargets.Sort(SortPointsByDistance);
            targetRoamPosition = _roamingPossibleTargets[0];

            if(_roamCoroutine == null)
            {
                _roamCoroutine = StartCoroutine(Roaming(targetRoamPosition));
            }
        }
    }

    private IEnumerator Roaming(Vector3 roamingPoint)
    {
        VacuumeAgent.SetDestination(roamingPoint);
        VacuumeAgent.speed = _roamingTurningForwardSpeed;

        //rotation phase
        for (float t = 0; t < _rotationPhaseTimeCap; t += Time.deltaTime)
        {
            if(Vector3.Dot(Vector3.Normalize(roamingPoint - transform.position), transform.forward) > _roamingTurnAngleThreshold) //checks that the vacuum is looking in the proper cone of vision before continuing
            {
                break;
            }

            yield return null;
        }

        //movement phase
        VacuumeAgent.speed = _moveSpeed;

        for (float t = 0; t < _forceNewRoamingPointTime; t += Time.deltaTime)
        {
            if(Vector3.Distance(transform.position, roamingPoint) < _distanceForNewPoint)
            {
                break;
            }
            yield return null;
        }
        _roamCoroutine = null;
        RoamingChooseRandomPoint();
    }

    private int SortPointsByDistance(Vector3 a, Vector3 b)
    {
        if(Vector3.Distance(a, transform.position) > Vector3.Distance(b, transform.position))
        {
            return 1;
        }
        else if(Vector3.Distance(a, transform.position) < Vector3.Distance(b, transform.position))
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

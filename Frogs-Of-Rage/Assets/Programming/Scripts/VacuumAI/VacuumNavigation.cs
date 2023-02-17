using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class VacuumNavigation : MonoBehaviour
{
    //enum used for the state machine
    private enum VacuumAiState : short
    {
        Roaming,
        Detecting,
        Chasing,
        Circiling
    }
    private VacuumAiState _activeAIState;



    //internaly used instance for the vacuum navmesh agent
    private NavMeshAgent _vacuumAgent;

    //varriables used to set up the nav mesh agent
    [Header("General Vacuum Variables")]
    [SerializeField] [Tooltip("The Speed the Vacuum Moves Forward")] private float _baseSpeed;
    [SerializeField] [Tooltip("The Speed the Vacuum Turns")] private float _baseTurnSpeed;
    [SerializeField] [Tooltip("The Rate the Vacuum Accelerates")] private float _baseAcceleration;
    [SerializeField] [Tooltip("Small value for forward speed while the vacuum is rotating")] [Range(0.25f, 1f)] private float _baseRotationPhaseForwardSpeed;
    [SerializeField] [Tooltip("Angle threshold expresed as a dot product needed before entering movment phase")] [Range(-1f, 0.95f)] private float _turnAngleThreshold;

    #region "Player Detection Variables"
    //variables coresponding to the vacuums FOV for the player
    [Header("Vaccum FOV Variables")]
    [SerializeField] [Tooltip("Vacuums field of view in degrees")] [Range(0, 360)] private float _fieldOfView;
    [SerializeField] [Tooltip("Vaccums distance of view in world units")] private float _distanceOfView;
    [Space(5)]
    [SerializeField] [Tooltip("Layer Mask used to see the player through an overlap sphere")] private LayerMask _playerLayer;
    [Space(10)]
    [SerializeField] [Tooltip("Amout of sight checks preformed per second. more = more acurate, but less performant")] [Range(1, 10)] private int _sightChecksPerSecond;
    private WaitForSeconds _sightTimeDelay;
    [SerializeField] [Tooltip("Amount of worldspace units above or below the vacuum for the vacuum to cirle instead of chase the player")] [Min(0)] private float _circleHeightOffset;
    public float CircleHeightOffset { get { return _circleHeightOffset; } } //used in editor for visualization

    //caculated angle Property between 1 and neg 1, for use in comparing angle to dot product
    private float _caculatedAngleRange;

    private List<Collider> _playerColliderList;
    private Transform _playerTransform;

    //public properties for the editor script
    public float FieldOfView { get { return _fieldOfView; } }
    public float DistanceOfView { get { return _distanceOfView; } }
    public Transform PlayerTransform { get { return _playerTransform; } }
    private List<Collider> PlayerColliderList //used to automaticaly null check
    {
        get
        {
            if (_playerColliderList == null)
            {
                _playerColliderList = new List<Collider>();
            }

            return _playerColliderList;
        }
        set
        {
            _playerColliderList = new List<Collider>(value);
        }
    }

    //sight coroutine, runs asyncrously to other coroutines, as any state can lead into a sight related phase
    private Coroutine _sightCheckCoroutine;
    #endregion

    [Header("Roaming Variables")]
    [Space(15)]
    [SerializeField] [Tooltip("Multiplier that modifies the base speed, acceleration, turning speed, and turning forward speed from the general data while in this state")] private float _roamingSpeedFactor;
    [SerializeField] [Tooltip("Distance Scanned when Choosing Point While Roaming")] private float _maxRoamingPointRange;
    [SerializeField] [Tooltip("Distance from a navmesh a scan can give to allow navigation")] private float _roamingPointScanRange;
    [SerializeField] [Tooltip("Number of Times Scanned to Find a point. If scans are unsucsessful, Vacuume Stays Still Untill Next Scan")] private int _roamingScanCap;
    [SerializeField] [Tooltip("Amount of Time Before The Vacume is forced a new point if it hasent reached its target point. prevents getting stuck")] private float _forceNewRoamingPointTime;
    [SerializeField] [Tooltip("Amount of World Units away From the Target the Vacume needs to Be away from the target point before being able to find a new point")] private float _distanceForNewPoint;
    [SerializeField] [Tooltip("Time Limit on the Rotation Phase")] [Range(0.1f, 3f)] private float _rotationPhaseTimeCap;

    [Header("Vacuum Navigation Variables, Chasing")]
    [Space(20)]
    [SerializeField] [Tooltip("Speed at which the vacuum moves towards the player")] private float _chasingMoveSpeed;
    [SerializeField] [Tooltip("Speed at which the vacuum rotates before moving while chasing the player")] private float _chasingRotationSpeed;
    [SerializeField] [Tooltip("Time in seconds before the vacuum updates where it is moving to to get the player.")] [Range(0.2f, 2f)] private float _chasingUpdatePositionRate;
    [SerializeField] [Tooltip("Maximum Time in second the vacuum can spend rotating in place until it begins moving towards the player in the chase phase")] [Range(0.1f, 0.5f)] private float _maxChaseRotationPhaseLength;

    [Header("Vacuum Navigation Variables, Circle")]
    [SerializeField] [Tooltip("Range the raycast looking for ground under the player shoots")] private float _groundDetectionRange;
    [SerializeField] [Tooltip("Height offset for sloped ground raycast")] private float _groudDetectionVerticalOffset;
    //for raycast
    private Ray _groundDetectionRaycast;
    private RaycastHit _groundDetectionHit;

    [SerializeField] [Tooltip("Layermask used to detect low ground the player is on")] private LayerMask _terrainLayerMask;
    public float GroundDetectionRange { get { return _groundDetectionRange; } }
    public float GroudDetectionVerticalOffset { get { return _groudDetectionVerticalOffset; } }

    //property for acsessing version elsewhere, used here too, to prevent null issues.
    public NavMeshAgent VacuumAgent
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
    #region "Data Structs"

    //general data pulled in by all states
    public struct GeneralData
    {
        public float baseSpeed;
        public float baseTurnSpeed;
        public float baseAcceleration;

        public float baseRotationPhaseForwardSpeed;
        public float turnAngleThreshold;
    }
    public GeneralData GetGeneralData()
    {
        GeneralData retreivedData = new GeneralData();

        retreivedData.baseSpeed = _baseSpeed;
        retreivedData.baseTurnSpeed = _baseTurnSpeed;
        retreivedData.baseAcceleration = _baseAcceleration;

        retreivedData.baseRotationPhaseForwardSpeed = _baseRotationPhaseForwardSpeed;
        retreivedData.turnAngleThreshold = _turnAngleThreshold;

        return retreivedData;
    }

    //data pulled in by the roaming state
    public struct RoamingData
    {
        public float roamingSpeedFactor;

        public float maxRoamingPointRange;
        public float roamingPointScanRange;
        public int roamingScanCap;

        public float forceNewRoamingPointTime;
        public float distanceForNewPoint;
        public float rotationPhaseTimeCap;
    }
    public RoamingData GetRoamingData()
    {
        RoamingData retreivedData = new RoamingData();

        retreivedData.roamingSpeedFactor = _roamingSpeedFactor;

        retreivedData.maxRoamingPointRange = _maxRoamingPointRange;
        retreivedData.roamingPointScanRange = _roamingPointScanRange;
        retreivedData.roamingScanCap = _roamingScanCap;

        retreivedData.forceNewRoamingPointTime = _forceNewRoamingPointTime;
        retreivedData.distanceForNewPoint = _distanceForNewPoint;
        retreivedData.rotationPhaseTimeCap = _rotationPhaseTimeCap;

        return retreivedData;
    }

    #endregion

    //for state pattern

    //states
    private IVacuumState _roamingState;
    //state context, used for transitioning states
    private VacuumStateContext _vacuumStateContext;

    //coroutine used to store and execute all state's actions over time (excludes sight checks)
    public Coroutine vacuumStateActionCoroutine;

    private void Awake()
    {
        InitilizeNavmeshAgent();

        _vacuumStateContext = new VacuumStateContext(this);  
        _roamingState = gameObject.AddComponent<VacuumStateRoaming>();

    }

    private void ResetActionCoroutine() // stops and empties the action coroutine in preperation for a state change
    {
        if(vacuumStateActionCoroutine != null)
        {
            StopCoroutine(vacuumStateActionCoroutine);
        }
        vacuumStateActionCoroutine = null;
    }

    public void Roam()
    {
        ResetActionCoroutine();

        _vacuumStateContext.TransitionStates(_roamingState);
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

        _vacuumAgent.speed = _baseSpeed;
        _vacuumAgent.angularSpeed = _baseTurnSpeed;
        _vacuumAgent.acceleration = _baseAcceleration;

        //adjustments that are assumed
        _vacuumAgent.autoBraking = false;
        _vacuumAgent.stoppingDistance = 0;
    }

    #region "State Change"


    #endregion


    /*

    #region "Player Detection"

    //public for editor script
    public Vector3 DirectionFromAngle(float angle, bool horizontalAngle)
    {
        if(horizontalAngle)
        {
            angle += transform.eulerAngles.y;
            return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
        }    
        else
        {
            angle += transform.eulerAngles.x;
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
            direction = Quaternion.AngleAxis(transform.rotation.eulerAngles.y - 90, transform.up) * direction;

            return direction;
        }
    }

    public Transform CheckPlayerInSight()
    {
        PlayerColliderList.Clear();
        PlayerColliderList = Physics.OverlapSphere(transform.position, DistanceOfView, _playerLayer).ToList();
        if(PlayerColliderList.Count > 0)
        {
            Transform playerTransform = PlayerColliderList[0].gameObject.transform;
            if(Vector3.Angle(transform.forward, (playerTransform.position - transform.position).normalized) < FieldOfView / 2)
            {
                return playerTransform;
            }
            return null;
        }
        else
        {
            return null;
        }
    }

    private IEnumerator LookForPlayer()
    {
        Transform playerTransform;
        while (true)
        {
            playerTransform = null;
            playerTransform = CheckPlayerInSight();
            if (playerTransform != null)
            {
                _playerTransform = playerTransform; // sets refrence for elsewhere
                //if (Mathf.Abs(transform.position.y - playerTransform.position.y) < CircleHeightOffset) 


            }
            yield return _sightTimeDelay;
        }
    }

    #endregion


    #region "Chasing Behavior"

    private void ChasingState()
    {
        if(_actionCoroutine != null)
        {
            StopCoroutine(_actionCoroutine);
        }
        _actionCoroutine = StartCoroutine(ChasePlayer());
    }

    private IEnumerator ChasePlayer()
    {
        Vector3 PlayerPosition = Vector3.zero;
        //makes sure player transform isint null, stores its position incase it goes null mid loop. returns to roaming if null
        if(_playerTransform == null)
        {
            yield break;
        }
        else
        {
            PlayerPosition = _playerTransform.position;
        }

        //rapid rotation phase
        VacuumAgent.SetDestination(PlayerPosition);
        VacuumAgent.speed = _roamingTurningForwardSpeed;
        VacuumAgent.angularSpeed = _chasingRotationSpeed;

        for (float t = 0; t < _maxChaseRotationPhaseLength; t += Time.deltaTime)
        {
            if (Vector3.Dot(Vector3.Normalize(PlayerPosition - transform.position), transform.forward) > _roamingTurnAngleThreshold) //checks that the vacuum is looking in the proper cone of vision before continuing
            {
                break;
            }
            yield return null;
        }
        //rushdown phase
        VacuumAgent.speed = _chasingMoveSpeed;
        for (float t = 0; t < _chasingUpdatePositionRate; t += Time.deltaTime)
        {
            yield return null;
        }
        _actionCoroutine = null;
        ChasingState();
    }

    #endregion

    #region "Circiling State"

    private void CircilingState()
    {
        Vector3 RayOrgin = transform.position;
        RayOrgin.y += _groudDetectionVerticalOffset;
        _groundDetectionRaycast.origin = transform.position;
        _groundDetectionRaycast.direction = transform.forward;

        if(Physics.Raycast(_groundDetectionRaycast, _groundDetectionRange, _terrainLayerMask))
        {

        }
        

  
    }

    private IEnumerator CircleChase()
    {
        return null;
    }

    private IEnumerator Circle()
    {
        return null;
    }

    private IEnumerator Pace()
    {
        return null;
    }

    private IEnumerator Leave()
    {
        return null;
    }


    #endregion
    */
}

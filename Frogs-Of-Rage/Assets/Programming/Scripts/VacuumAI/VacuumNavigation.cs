using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;

[RequireComponent(typeof(VacuumAnimation))]
public class VacuumNavigation : MonoBehaviour
{
    static event Action onPlayerHit;
    public static void InvokeOnPlayerHit() { onPlayerHit?.Invoke(); }

    public VacuumAnimation VacuumAnimation // animation Script responsible for animating the model
    {
        get;
        private set;
    }

    //internaly used instance for the vacuum navmesh agent
    private NavMeshAgent _vacuumAgent;
    [SerializeField] [Tooltip("Box Collider on the neck that is used for detecting if the vacuum hit the player")] private BoxCollider _damageCollider;

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

    //transform of the player, set through vision checks, is often null. 
    public Transform PlayerTransform
    {
        get;
        private set;
    }

    //public properties for the editor script
    public float FieldOfView { get { return _fieldOfView; } }
    public float DistanceOfView { get { return _distanceOfView; } }

    private List<Collider> _playerColliderList;
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

    //Navmesh variables
    #region "Navmesh Agent Variables
    [SerializeField] [Tooltip("Radius of the Nav mesh agent. Should Encompass the whole mesh")] private float _navMeshAgentRadius;
    //for editor
    public float NavMeshAgentRadius { get { return _navMeshAgentRadius; } }

    #endregion

    //general movement variables for the vacuum and the general struct
    #region "General Phase Variables"
    [Header("General Vacuum Variables")]
    [SerializeField] [Tooltip("The Speed the Vacuum Moves Forward")] private float _baseSpeed;
    [SerializeField] [Tooltip("The Speed the Vacuum Turns")] [Range(40, 120)] private float _baseTurnSpeed;
    [SerializeField] [Tooltip("The Rate the Vacuum Accelerates")] [Range(60, 120)] private float _baseAcceleration;
    [SerializeField] [Tooltip("Small value for forward speed while the vacuum is rotating")] [Range(0.25f, 1f)] private float _baseRotationPhaseForwardSpeed;
    [SerializeField] [Tooltip("Angle threshold expresed as a dot product needed before entering movment phase")] [Range(-1f, 0.95f)] private float _turnAngleThreshold;
    #endregion
    //variables used for the roaming state and struct
    #region "Roaming Phase Variables"
    [Header("Roaming Variables")]
    [Space(15)]
    [SerializeField] [Tooltip("Multiplier that modifies the base speed, acceleration, turning speed, and turning forward speed from the general data while in this state")] [Range(1, 2)] private float _roamingSpeedFactor;
    [SerializeField] [Tooltip("Distance Scanned when Choosing Point While Roaming")] private float _maxRoamingPointRange;
    [SerializeField] [Tooltip("Distance from a navmesh a scan can give to allow navigation")] private float _roamingPointScanRange;
    [SerializeField] [Tooltip("Number of Times Scanned to Find a point. If scans are unsucsessful, Vacuume Stays Still Untill Next Scan")] private int _roamingScanCap;
    [SerializeField] [Tooltip("Amount of Time Before The Vacuum is forced a new point if it hasent reached its target point. prevents getting stuck")] [Range (4f, 12f)]private float _forceNewRoamingPointTime;
    [SerializeField] [Tooltip("Amount of World Units away From the Target the Vacuum needs to Be away from the target point before being able to find a new point")] [Range (0.5f, 5f)] private float _distanceForNewPoint;
    [SerializeField] [Tooltip("Time Limit on the Rotation Phase")] [Range(0.1f, 3f)] private float _rotationPhaseTimeCap;

    //property for scan range, bail time and point range, and rotation phase cap used elsewhere too, but mostly relevant for roaming 
    public float VacuumNavMeshScanRange { get { return _roamingPointScanRange; } }
    public float VacuumPointBailOutTime{ get { return _forceNewRoamingPointTime; } }
    public float VacuumPointReachedDistance { get { return _distanceForNewPoint; } }
    public float VacuumRotationPhaseCap { get { return _rotationPhaseTimeCap; } }

    #endregion
    //variables used for the chasing state and struct
    #region "Chasing Phase Variables"
    [Header("Chasing Variables")]
    [Space(20)]
    [SerializeField] [Tooltip("Multiplier applied to the base speeds at which the vacuum moves during the chase speed.")] [Range(1, 2)] private float _chasingSpeedFactor;
    [SerializeField] [Tooltip("Time in seconds before the vacuum updates where it is moving to to get the player.")] [Range(0.1f, 1f)] private float _chasingUpdatePositionRate;
    [SerializeField] [Tooltip("Maximum Time in second the vacuum can spend rotating in place until it begins moving towards the player in the chase phase")] [Range(0.1f, 0.5f)] private float _maxChaseRotationPhaseLength;

    //head movment times
    [SerializeField] [Tooltip("time for the head to raise to max angle after being seen")] private float _headRaiseTime;
    [SerializeField] [Tooltip("time for the head to raise up again if the vacuum missed its attack")] private float _attackMissHeadRaiseTime;
    [SerializeField] [Tooltip("time the vacuum takes to slam its head into the ground")] private float _headDropAttackTime;

    [SerializeField] [Tooltip("distance from the player needs to be before attacking")] private float _attackDistance;
    // for Editor scrit
    public float AttackDistance { get { return _attackDistance; } }
    #endregion
    //variables used for the detection state and struct
    #region "Detection Phase Variables"
    [SerializeField] [Tooltip("Speed mult for the detection phase")] private float _detectionSpeedFactor;

    [SerializeField] [Tooltip("1 Second = 1 world unit Multiplied by this variable to determine if the sound made from the player landing is within detection range")] [Min(0)] private float _hearingSensitivity;
    [SerializeField] [Tooltip("Height Diffrence between the vacuum and the player to activate the detection phase")] [Min(0)] private float _detectionHeightRange;
    [SerializeField] [Tooltip("Speed bonus for the vacuum during its final spin in the detection phase")] [Range(2,5)] private float _turboSpinSpeed;
    //properties of the above two for the editor script
    public float SoundVolumeAmplifier { get { return _hearingSensitivity; } }
    public float DetectionHeightRange { get { return _detectionHeightRange; } }

    [SerializeField] [Tooltip("the vacuum spins in place scanning for the player when they get to the point where the player was heard, which corresponds to the duration of that spin")] private float _spinScanTime;

    private Vector3 _detectionPoint;

    #endregion

    //events for seeing and losing sight of the player
    public event Action onPlayerSeen;
    public event Action onPlayerLost;

    public void OnPlayerSeen() { onPlayerSeen?.Invoke(); }
    public void OnPlayerLost() { onPlayerLost?.Invoke(); }

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

    public struct ChasingData
    {
        public float chasingSpeedFactor;
        public float chasingUpdatePositionRate;
        public float maxChaseRotationPhaseLength;

        public float headRaiseTime; //time for the head to raise to max angle after being seen
        public float attackMissHeadRaiseTime; //time for the head to raise up again if the vacuum missed its attack
        public float headDropAttackTime; //time the vacuum takes to slam its head into the ground

        public float attackDistance;
    }
    public ChasingData GetChasingData()
    {
        ChasingData retreivedData = new ChasingData();

        retreivedData.chasingSpeedFactor = _chasingSpeedFactor;
        retreivedData.chasingUpdatePositionRate = _chasingUpdatePositionRate;
        retreivedData.maxChaseRotationPhaseLength = _maxChaseRotationPhaseLength;

        retreivedData.headRaiseTime = _headRaiseTime;
        retreivedData.attackMissHeadRaiseTime = _attackMissHeadRaiseTime;
        retreivedData.headDropAttackTime = _headDropAttackTime;

        retreivedData.attackDistance = _attackDistance;

        return retreivedData;
    }


    public struct DetectionData
    {
        public float detectionSpeedFactor;
        public float hearingSensitivity;
        public float spinScanTime;
        public float turboSpinSpeed;
        public Vector3 detectionPoint;
    }
    public DetectionData GetDetectionData()
    {
        DetectionData retreivedData = new DetectionData();

        retreivedData.hearingSensitivity = _hearingSensitivity;
        retreivedData.spinScanTime = _spinScanTime;
        retreivedData.detectionPoint = _detectionPoint;
        retreivedData.detectionSpeedFactor = _detectionSpeedFactor;
        retreivedData.turboSpinSpeed = _turboSpinSpeed;

        return retreivedData;
    }
    #endregion

    //for state pattern
    //states
    private IVacuumState _roamingState, _chasingState, _detectionState, _closeroam;
    //state context, used for transitioning states
    private VacuumStateContext _vacuumStateContext;

    //coroutine used to store and execute all state's actions over time (excludes sight checks)
    public Coroutine vacuumStateActionCoroutine;

    private void Awake()
    {
        InitilizeNavmeshAgent();
        EnableSight();

        VacuumAnimation = GetComponent<VacuumAnimation>();

        _vacuumStateContext = new VacuumStateContext(this);  
        _roamingState = gameObject.AddComponent<VacuumStateRoaming>();
        _chasingState = gameObject.AddComponent<VacuumStateChasing>();
        _detectionState = gameObject.AddComponent<VacuumStateDetection>();
        _closeroam = gameObject.AddComponent<VacuumStateCloseroam>();
    }

    private void ResetActionCoroutine() // stops and empties the action coroutine in preperation for a state change
    {
        if(vacuumStateActionCoroutine != null)
        {
            StopCoroutine(vacuumStateActionCoroutine);
            vacuumStateActionCoroutine = null;
        }
    }
    public void Roam()
    {
        ResetActionCoroutine();
        _vacuumStateContext.TransitionStates(_roamingState);
    }

    public void Chase()
    {
        ResetActionCoroutine();
        _vacuumStateContext.TransitionStates(_chasingState);
    }

    public void Detection(Vector3 detectionPoint)
    {
        _detectionPoint = detectionPoint;

        ResetActionCoroutine();
        _vacuumStateContext.TransitionStates(_detectionState);
    }

    public void Closeroam(Vector3 closeRoamPoint)
    {
        _detectionPoint = closeRoamPoint;

        ResetActionCoroutine();
        _vacuumStateContext.TransitionStates(_closeroam);
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
        _vacuumAgent.radius = _navMeshAgentRadius;

        //adjustments that are assumed
        _vacuumAgent.autoBraking = false;
        _vacuumAgent.stoppingDistance = 0;
    }

    #region "Player Vision Check"

    //public for editor script
    public Vector3 DirectionFromAngle(float angle, bool horizontalAngle)
    {
        if (horizontalAngle)
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
        if (PlayerColliderList.Count > 0)
        {
            Transform playerTransform = PlayerColliderList[0].gameObject.transform;
            if (Vector3.Angle(transform.forward, (playerTransform.position - transform.position).normalized) < FieldOfView / 2)
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
        bool playerInSights = false;

        while (true)
        {
            playerTransform = null;
            playerTransform = CheckPlayerInSight();
            if (playerTransform != null && !playerInSights)
            {
                playerInSights = true;
                PlayerTransform = playerTransform;
                OnPlayerSeen(); // invokes event for the player being seen
            }
            else if (playerTransform == null && playerInSights)
            {
                playerInSights = false;
                OnPlayerLost();
            }
            yield return _sightTimeDelay;
        }
    }

    public void EnableSight()
    {
        if(_sightCheckCoroutine == null)
        {
            _sightCheckCoroutine = StartCoroutine(LookForPlayer());
        }
    }
    public void DisableSight()
    {
        if(_sightCheckCoroutine != null)
        {
            StopCoroutine(_sightCheckCoroutine);
        }
        _sightCheckCoroutine = null;
    }

    public VacuumHearingResult CheckSoundInRange(Vector3 fallPosition, float fallTime)
    {
        Vector3 flattendPlayerPosition = fallPosition;
        flattendPlayerPosition.y = transform.position.y; // takes the Y out for distance math

        int returnValue = 0;

        if (!NavMesh.SamplePosition(flattendPlayerPosition, out NavMeshHit _navHit, VacuumNavMeshScanRange, NavMesh.AllAreas))
        {
            return (VacuumHearingResult)returnValue; // returns out if a point couldnt be moved to
        }

        if (Vector3.Distance(flattendPlayerPosition, transform.position) <= (fallTime * _hearingSensitivity)) // distance check
        {
            //point found
            returnValue++;
        }
        if (Mathf.Abs(fallPosition.y - transform.position.y) > _detectionHeightRange) // height check
        {
            returnValue *= 2;
        }

        return (VacuumHearingResult)returnValue;
    }

    #endregion

}

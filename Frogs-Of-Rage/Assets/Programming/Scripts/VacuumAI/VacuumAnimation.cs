using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumAnimation : MonoBehaviour
{
    public enum WheelRotationDirection
    {
        Left,
        Right,
        Straight
    }

    public enum EyeAnimationState
    {
        Swirl,
        Stop
    }

    [Header("Components To Animate")]
    [SerializeField] private Transform _vacuumNeckJoint;
    [SerializeField] private Transform _vacuumLeftEyeJoint;
    [SerializeField] private Transform _vacuumRightEyeJoint;
    [Space(10)]
    [SerializeField] private Transform _vacuumLeftWheelJoint;
    [SerializeField] private Transform _vacuumRightWheelJoint;
    [SerializeField] private Transform _vacuumBackWheelJoint;
    [Space(10)]
    [SerializeField] private Transform _spinnerJoint;
    //internal variables
    private Vector3 _wheelRotation;

    //for head animation
    [SerializeField][Range(20, 80)] private float _neckRotationRange;
    private Quaternion _neckInitialRotation;
    private Quaternion _neckMaxRotation;

    private Coroutine _animateHeadCoroutine;

    //for spinner animation
    [SerializeField] private float spinnerSpeed;
    private Vector3 spinnerRotation;

    private WheelRotationDirection _wheelAnimationDirection;
    private float _wheelRotationSpeed = 30;
    [SerializeField] [Tooltip("Base spin speed of the wheels in degrees per second")] private float _wheelRotationBaseSpeed;

    private EyeAnimationState _eyeState;
    [SerializeField] [Tooltip("Base spin speed of the Eye Twirls in degrees per second")]  private float _eyeAnimSpeed = 20;

    private bool NullCheckFaceComponents()
    {
        if(_vacuumNeckJoint == null || _vacuumLeftEyeJoint == null || _vacuumRightEyeJoint == null )
        {
            return false;
        }
        return true;
    }
    private bool NullCheckWheelComponents()
    {
        if(_vacuumBackWheelJoint == null || _vacuumLeftWheelJoint == null || _vacuumRightWheelJoint == null)
        {
            return false;
        }
        return true;
    }

    //preforms null checks
    private void Start()
    {
        if(!NullCheckWheelComponents())
        {
            Debug.LogWarning("Please Assign the Vacuum's wheel Joints to the Vacuum Animation script in the inspector for animations to work properly. Game Object Name: " + name);
            enabled = false;
        }
        if(!NullCheckFaceComponents())
        {
            Debug.LogWarning("Please Assign the Vacuum's Neck and Eye Joints to the Vacuum Animation script in the inspector for animations to work properly. Game Object Name: " + name);
            enabled = false;
        }
        if(_spinnerJoint == null)
        {
            Debug.LogWarning("Please Assign the Vacuum's Spinner Joint to the Vacuum Animation script in the inspector for animations to work properly. Game Object Name: " + name);
            enabled = false;
        }
        _wheelRotation = Vector3.zero;

        InitilizeNeckRotations();
    }

    private void Update()
    {
        ChangeSpinnerSpeed(spinnerSpeed); // updates spinner speed
        RotateSpinner(); // spins spinner

        AnimateWheels(); // spins wheels
        AnimateEyes(); // spins eyes
    }

    #region "Wheel Animation"
    private void AnimateWheels(float leftWheelRotation, float rightWheelRotation, float backWheelRotation)
    {
        _wheelRotation.z = leftWheelRotation;
        _vacuumLeftWheelJoint.Rotate(_wheelRotation);
        _wheelRotation.z = rightWheelRotation;
        _vacuumRightWheelJoint.Rotate(_wheelRotation);
        _wheelRotation.z = backWheelRotation;
        _vacuumBackWheelJoint.Rotate(_wheelRotation);
    }
    private void SpinWheelsLeft(float speed)
    {
        AnimateWheels(-speed, speed, 0);
    }
    private void SpinWheelsRight(float speed)
    {
        AnimateWheels(speed, -speed, 0);
    }
    private void SpinWheelsStraight(float speed)
    {
        AnimateWheels(speed, speed, speed);
    }

    public void UpdateWheelAnimationDirection(WheelRotationDirection newDirection)
    {
        _wheelAnimationDirection = newDirection;
    }
    public void UpdateWheelAnimationSpeed(float newSpeed)
    {
        _wheelRotationSpeed = newSpeed * _wheelRotationBaseSpeed * Time.deltaTime;
    }

    private void AnimateWheels()
    {
        switch (_wheelAnimationDirection)
        {
            case WheelRotationDirection.Left:
                SpinWheelsLeft(_wheelRotationSpeed);
                break;
            case WheelRotationDirection.Right:
                SpinWheelsRight(_wheelRotationSpeed);
                break;
            case WheelRotationDirection.Straight:
                SpinWheelsStraight(_wheelRotationSpeed);
                break;
            default:
                break;
        }
    }
    #endregion

    public void AnimateHead(float lerpT, Quaternion startingRotation, Quaternion endingRotation)
    {
        _vacuumNeckJoint.localRotation =  Quaternion.Lerp(startingRotation, endingRotation, lerpT);
    }

    private void InitilizeNeckRotations()
    {
        _neckInitialRotation = _vacuumNeckJoint.localRotation;
        _vacuumNeckJoint.Rotate(0, 0, -_neckRotationRange, Space.Self);
        _neckMaxRotation = _vacuumNeckJoint.localRotation;
        _vacuumNeckJoint.localRotation = _neckInitialRotation;
    }


    //ran once to drop the head, either if the vacuum has stopped chasing the player, or if the vacuum is attacking the player
    public void AnimateHeadDrop(float dropSpeed)
    {
        if(_animateHeadCoroutine != null)
        {
            StopCoroutine(_animateHeadCoroutine);
            _animateHeadCoroutine = null;
        }
        _animateHeadCoroutine = StartCoroutine(DropHead(dropSpeed));
    }

    public void AnimateHeadRaise(float raiseSpeed)
    {
        if (_animateHeadCoroutine != null)
        {
            StopCoroutine(_animateHeadCoroutine);
            _animateHeadCoroutine = null;
        }
        _animateHeadCoroutine = StartCoroutine(RaiseHead(raiseSpeed));
    }

    //coroutine that handles the head drop
    private IEnumerator DropHead(float dropSpeed)
    {
        Quaternion initialRotation = _vacuumNeckJoint.localRotation;
        for (float t = 0; t < dropSpeed; t += Time.deltaTime)
        {
            _vacuumNeckJoint.localRotation = Quaternion.Lerp(initialRotation, _neckInitialRotation, t / dropSpeed);
            yield return null;
        }
    }

    //coroutine that handles head raise
    private IEnumerator RaiseHead(float raiseSpeed)
    {
        Quaternion initialRotation = _vacuumNeckJoint.localRotation;
        for (float t = 0; t < raiseSpeed; t += Time.deltaTime)
        {
            _vacuumNeckJoint.localRotation = Quaternion.Lerp(initialRotation, _neckMaxRotation, t / raiseSpeed);
            yield return null;
        }
    }

    public void ChangeSpinnerSpeed(float newSpeed)
    {
        spinnerRotation.z = newSpeed;
    }
    private void RotateSpinner()
    {
        _spinnerJoint.Rotate(spinnerRotation * Time.deltaTime, Space.Self);
    }

    private void AnimateEyes()
    {
        switch (_eyeState)
        {
            case EyeAnimationState.Swirl:
                TwirlEyes(_eyeAnimSpeed);
                break;
            case EyeAnimationState.Stop:
                break;
            default:
                break;
        }
    }
    private void TwirlEyes(float speed)
    {
        _vacuumLeftEyeJoint.transform.Rotate(0, speed * Time.deltaTime, 0);
        _vacuumRightEyeJoint.transform.Rotate(0, -speed * Time.deltaTime, 0);
    }
    public void UpdateEyeAnimation(EyeAnimationState newEyeAnimState)
    {
        _eyeState = newEyeAnimState;
    }
}

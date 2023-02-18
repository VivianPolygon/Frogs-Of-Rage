using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumAnimation : MonoBehaviour
{
    [Header("Components To Animate")]
    [SerializeField] private Transform _vacuumNeckJoint;
    [SerializeField] private Transform _vacuumLeftEyeJoint;
    [SerializeField] private Transform _vacuumRightEyeJoint;
    [Space(10)]
    [SerializeField] private Transform _vacuumLeftWheelJoint;
    [SerializeField] private Transform _vacuumRightWheelJoint;
    [SerializeField] private Transform _vacuumBackWheelJoint;

    //internal variables
    private Vector3 _wheelRotation;

    [Range(0,1)]public float testNeck;

    [SerializeField][Range(20, 80)] private float _neckRotationRange;
    private Quaternion _neckInitialRotation;
    private Quaternion _neckMaxRotation;

    private void Update()
    {
        AnimateHead(testNeck);
    }

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

        _wheelRotation = Vector3.zero;

        InitilizeNeckRotations();
    }

    public void AnimateWheels(float leftWheelRotation, float rightWheelRotation, float backWheelRotation)
    {
        _wheelRotation.z = leftWheelRotation;
        _vacuumLeftWheelJoint.Rotate(_wheelRotation);
        _wheelRotation.z = rightWheelRotation;
        _vacuumRightWheelJoint.Rotate(_wheelRotation);
        _wheelRotation.z = backWheelRotation;
        _vacuumBackWheelJoint.Rotate(_wheelRotation);
    }

    public void AnimateHead(float lerpT)
    {
        _vacuumNeckJoint.localRotation = Quaternion.Lerp(_neckInitialRotation, _neckMaxRotation, lerpT);
    }

    private void InitilizeNeckRotations()
    {
        _neckInitialRotation = _vacuumNeckJoint.localRotation;
        _vacuumNeckJoint.Rotate(0, 0, -_neckRotationRange, Space.Self);
        _neckMaxRotation = _vacuumNeckJoint.localRotation;
        _vacuumNeckJoint.rotation = _neckInitialRotation;
    }
}

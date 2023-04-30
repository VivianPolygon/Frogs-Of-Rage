using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    private Rigidbody playerRB;
    private CapsuleCollider playerCollider;
    private Animator playerAnimator;

    public List<Collider> colliders = new List<Collider>();
    public List<Rigidbody> rigidbodies = new List<Rigidbody>();


    public bool isRagdoll = false;



    private void OnEnable()
    {
        playerRB = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        playerAnimator = GetComponentInChildren<Animator>();
        SetCollidersActive(isRagdoll);
        SetRigidbodyIsKinematic(isRagdoll);
    }

    public void ToggleRagdoll()
    {
        isRagdoll = !isRagdoll;
        playerAnimator.enabled = !isRagdoll;
        SetRigidbodyIsKinematic(isRagdoll);
        SetCollidersActive(isRagdoll);
    }

    private void SetCollidersActive(bool enabled)
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = enabled;
        }        
    }

    private void SetRigidbodyIsKinematic(bool kinematic)
    {
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = !kinematic;
        }
    }
}

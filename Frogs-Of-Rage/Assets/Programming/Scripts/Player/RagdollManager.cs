using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    public Rigidbody playerRB;
    public CapsuleCollider playerCollider;

    public List<Component> ragdoll = new List<Component>();


    private void SetCollidersActive(bool enabled)
    {
        foreach (Collider collider in ragdoll)
        {
            collider.enabled = enabled;
        }
    }

    private void SetRigidbodyIsKinematic(bool kinematic)
    {
        foreach (Rigidbody rigidbody in ragdoll)
        {
            rigidbody.isKinematic = kinematic;
        }

    }
}

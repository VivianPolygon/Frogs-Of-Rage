using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public float zOffset = 0.2f;
    public float yOffset = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            transform.parent = other.gameObject.transform;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            transform.localPosition = Vector3.zero;
            Vector3 temp = transform.localPosition;
            temp.z += zOffset;
            temp.y += yOffset;
            transform.localPosition = temp;
        }
    }
}

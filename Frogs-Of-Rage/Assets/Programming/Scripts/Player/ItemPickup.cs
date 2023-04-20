using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public float xOffset = 1.0f;
    public float yOffset = 1.0f;
    public float zOffset = 0.2f;

    public Vector3 rotationOffset = Vector3.zero;

    private HatSpin hatSpinScript;

    private void Start()
    {
        hatSpinScript = GetComponent<HatSpin>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            transform.parent = other.gameObject.transform.GetChild(0).GetChild(4).GetChild(0).Find("SpineIK_target");
            transform.localRotation = Quaternion.Euler(Vector3.zero + rotationOffset);
            transform.localPosition = Vector3.zero;
            Vector3 temp = transform.localPosition;
            temp.z += zOffset;
            temp.y += yOffset;
            temp.x += xOffset;
            transform.localPosition = temp;

            GameManager.Instance.playerController.secondaryObjectiveComplete = true;
            transform.Find("Icon").gameObject.SetActive(false);

            // Disable HatSpin script if attached and not null
            if (hatSpinScript != null)
            {
                hatSpinScript.enabled = false;
            }

            // GetComponent<Collider>().enabled = false;
        }
    }
}

using UnityEngine;
using System.Collections;

public class HazardousObject : MonoBehaviour
{
    public GameObject firePrefab; // Assign the fire prefab in the inspector
    private GameObject fireInstance; // Store the instance of the fire prefab

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Instantiate the fire prefab and parent it to the player
            fireInstance = Instantiate(firePrefab, other.transform.position, Quaternion.identity, other.transform);

            // Activate the fire effect
            fireInstance.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Deactivate the fire effect when the player leaves the collider
            fireInstance.SetActive(false);
        }
    }
}

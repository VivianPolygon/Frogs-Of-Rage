using UnityEngine;

public class Transporter : MonoBehaviour
{
    public Transform destination; // destination object to transport player to
    public GameObject objectToDisable; // game object to disable after teleporting the player

    private void OnTriggerEnter(Collider other)
    {
        // check if the colliding object is the player
        if (other.gameObject.name == "Player" &&
            other.gameObject.CompareTag("Player") &&
            other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // check if a destination is assigned
            if (destination != null)
            {
                // transport the player to the destination object
                other.gameObject.transform.position = destination.position;

                // disable the game object if it's assigned
                if (objectToDisable != null)
                {
                    objectToDisable.SetActive(false);
                }
            }
            else
            {
                Debug.LogWarning("Transporter: No destination assigned!");
            }
        }
    }
}

using UnityEngine;

public class Transporter : MonoBehaviour
{
    public Transform destination; // destination object to transport player to

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
            }
            else
            {
                //Debug.LogWarning("Transporter: No destination assigned!");
            }
        }
    }
}

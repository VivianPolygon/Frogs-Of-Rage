using System.Collections;
using UnityEngine;

public class Transporter : MonoBehaviour
{
    public Transform destination;

    private void Start()
    {
        if (destination == null)
        {
            Debug.LogWarning("Transporter: No destination assigned!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Check if the destination is assigned
            if (destination != null)
            {
                // Wait for the fade screen coroutine to finish before teleporting the player
                StartCoroutine(WaitForFadeAndTeleport(other.gameObject));
            }
            else
            {
                // Warn the developer that the destination is not assigned
                Debug.LogWarning("Transporter: No destination assigned!");
            }
        }
    }

    private IEnumerator WaitForFadeAndTeleport(GameObject player)
    {
        // Wait for the fade screen coroutine to finish
        yield return StartCoroutine(GameManager.Instance.WaitForFadeScreen(destination.position, false));

        // Teleport the player to the destination
        player.transform.position = destination.position;
    }
}

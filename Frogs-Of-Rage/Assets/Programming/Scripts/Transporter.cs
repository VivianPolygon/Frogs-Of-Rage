using UnityEngine;

public class Transporter : MonoBehaviour
{
    public Transform destination;

    public PickExit pickExit;

    private void Start()
    {
        pickExit = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PickExit>();
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
                other.gameObject.transform.position = destination.position;

                // Enable the chosen exit canvas element
                pickExit.ShowExitCanvas();
            }
            else
            {
                // Warn the developer that the destination is not assigned
                Debug.LogWarning("Transporter: No destination assigned!");
            }

            // Disable this game object after teleporting the player
            gameObject.SetActive(false);
        }
    }
}

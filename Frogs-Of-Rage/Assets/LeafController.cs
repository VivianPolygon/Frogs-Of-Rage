using UnityEngine;

public class LeafController : MonoBehaviour
{
    private bool playerColliding;
    private Quaternion originalRotation;
    public float rotationSpeed = 10f;

    private void Start()
    {
        // Store the original rotation of the leaf
        originalRotation = transform.rotation;
    }

    private void Update()
    {
        // If the player is colliding with the leaf's trigger, rotate the leaf downwards on the x-axis
        if (playerColliding)
        {
            transform.rotation *= Quaternion.Euler(-rotationSpeed * Time.deltaTime, 0f, 0f);
        }
        // Otherwise, rotate the leaf back to its original rotation
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has collided with the leaf's trigger
        if (other.CompareTag("Player"))
        {
            playerColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player has left the leaf's trigger
        if (other.CompareTag("Player"))
        {
            playerColliding = false;
        }
    }
}

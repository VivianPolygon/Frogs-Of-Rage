using UnityEngine;

public class BookDrop : MonoBehaviour
{
    // The animator component to play the animation
    public Animator animator;

    // Whether the animation has already been played
    private bool hasPlayedAnimation = false;

    // Whether the player is in range to interact with the object
    private bool playerInRange = false;

    // Called when a collider enters the trigger
    void OnTriggerEnter(Collider other)
    {
        // Check if the collider has the tag "Player" (you can change this to whatever you need)
        if (other.CompareTag("Player"))
        {
            // Display a prompt to press E to play the animation
            Debug.Log("Press E to play the animation.");

            // Set the playerInRange flag to true
            playerInRange = true;
        }
    }

    // Called when a collider exits the trigger
    void OnTriggerExit(Collider other)
    {
        // Check if the collider has the tag "Player"
        if (other.CompareTag("Player"))
        {
            // Set the playerInRange flag to false
            playerInRange = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is in range and has pressed the E key
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Check if the animation has already been played
            if (!hasPlayedAnimation)
            {
                // Set the isPushed parameter to true
                animator.SetBool("isPushed", true);

                // Play the animation
                animator.Play("BookDropAnim");

                // Set the hasPlayedAnimation flag to true
                hasPlayedAnimation = true;
            }
        }
    }
}

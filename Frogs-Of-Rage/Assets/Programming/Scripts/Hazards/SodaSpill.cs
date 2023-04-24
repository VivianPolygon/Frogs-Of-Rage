using UnityEngine;

public class SodaSpill : MonoBehaviour
{
    public float slowFactor = 2f;
    private int spillCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController != null)
            {
                spillCount++;

                if (spillCount == 1)
                {
                    playerController.walkSpeed /= slowFactor;
                    playerController.sprintSpeed /= slowFactor;
                    playerController.canJump = false;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController != null)
            {
                spillCount--;

                if (spillCount == 0)
                {
                    playerController.walkSpeed *= slowFactor;
                    playerController.sprintSpeed *= slowFactor;
                    playerController.canJump = true;
                }
            }
        }
    }
}

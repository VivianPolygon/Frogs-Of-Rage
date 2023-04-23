using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SodaSpill : MonoBehaviour
{
    public float slowFactor = 0.5f;
    private PlayerController playerController;
    private float originalWalkSpeed;
    private float originalSprintSpeed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerController>();

            if (playerController != null)
            {
                // Get the original walkSpeed and sprintSpeed using reflection
                FieldInfo walkSpeedField = typeof(PlayerController).GetField("walkSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo sprintSpeedField = typeof(PlayerController).GetField("sprintSpeed", BindingFlags.NonPublic | BindingFlags.Instance);

                originalWalkSpeed = (float)walkSpeedField.GetValue(playerController);
                originalSprintSpeed = (float)sprintSpeedField.GetValue(playerController);

                // Slow down the player
                walkSpeedField.SetValue(playerController, originalWalkSpeed * slowFactor);
                sprintSpeedField.SetValue(playerController, originalSprintSpeed * slowFactor);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerController != null)
            {
                // Restore the original walkSpeed and sprintSpeed using reflection
                FieldInfo walkSpeedField = typeof(PlayerController).GetField("walkSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo sprintSpeedField = typeof(PlayerController).GetField("sprintSpeed", BindingFlags.NonPublic | BindingFlags.Instance);

                walkSpeedField.SetValue(playerController, originalWalkSpeed);
                sprintSpeedField.SetValue(playerController, originalSprintSpeed);
            }
        }
    }
}

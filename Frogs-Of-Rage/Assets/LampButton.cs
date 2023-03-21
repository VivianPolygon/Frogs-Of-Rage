using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampButton : MonoBehaviour
{
    public Animator animator;
    public Light light;

    private bool isPressed = false;

    public float emissionIntensity = 6.0f;
    public float pressedEmissionIntensity = -6.0f;
    private Color originalColor;
    private Color pressedColor;
    private Material material;

    private void Start()
    {
        // Store the original and pressed colors of the material
        originalColor = GetComponent<Renderer>().material.color;
        pressedColor = new Color(0f, 0f, 0f);

        // Get the material from an object named "LampLightBulb"
        Transform lightBulbTransform = transform.Find("LampLightBulb");
        if (lightBulbTransform != null)
        {
            Renderer lightBulbRenderer = lightBulbTransform.GetComponent<Renderer>();
            if (lightBulbRenderer != null)
            {
                material = lightBulbRenderer.material;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            animator.SetBool("isPressed", true);

            if (light.enabled)
            {
                light.enabled = false;
                isPressed = false;

                // Restore the original color of the material
                GetComponent<Renderer>().material.color = pressedColor;

                // Set the emission value to 0
                material.SetColor("_EmissionColor", Color.black * pressedEmissionIntensity);
            }
            else
            {
                light.enabled = true;
                isPressed = true;

                // Change the color of the material to pressed color
                GetComponent<Renderer>().material.color = originalColor;

                // Set the emission value to the desired intensity
                material.SetColor("_EmissionColor", Color.white * emissionIntensity);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            animator.SetBool("isPressed", false);
        }
    }
}

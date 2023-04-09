using System;
using UnityEngine;

public class LevelEventManager : MonoBehaviour
{
    // Singleton instance for easy access
    public static LevelEventManager Instance;

    // Delegate for the event handler
    public delegate void AnimationTriggerHandler(string animationName);

    // Event to subscribe to
    public event AnimationTriggerHandler onAnimationTrigger;

    private void Awake()
    {
        // Ensure only one instance of LevelEventManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to invoke the event
    public void TriggerAnimation(string animationName)
    {
        onAnimationTrigger?.Invoke(animationName);
    }
}

using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    public Animator rocketAnimator;
    public string triggerAnimationName = "Pump";
    public string rocketAnimationName = "RocketLaunchAnim";

    private void Start()
    {
        // Subscribe to the event
        LevelEventManager.Instance.onAnimationTrigger += OnAnimationTrigger;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event
        LevelEventManager.Instance.onAnimationTrigger -= OnAnimationTrigger;
    }

    private void OnAnimationTrigger(string triggeredAnimationName)
    {
        // Check if the triggered animation matches the desired animation name
        if (triggeredAnimationName == triggerAnimationName)
        {
            // Play the rocket animation
            rocketAnimator.Play(rocketAnimationName);
        }
    }
}

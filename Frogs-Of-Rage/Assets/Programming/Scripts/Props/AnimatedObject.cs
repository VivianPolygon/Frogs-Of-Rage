using UnityEngine;

public class AnimatedObject : MonoBehaviour
{
    public Animator animator;
    public string animationName;

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
        if (triggeredAnimationName == animationName)
        {
            // Play the animation
            animator.Play(animationName);
        }
    }
}

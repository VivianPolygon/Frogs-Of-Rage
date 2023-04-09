using System.Collections;
using UnityEngine;

public class CupSpill : MonoBehaviour
{
    public Animator animator;
    public string spillAnimationName = "cupSpill";
    public Collider targetCollider; // The collider to be enabled after the animation

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
        if (triggeredAnimationName == spillAnimationName)
        {
            // Play the cup spill animation
            animator.Play(spillAnimationName);
            StartCoroutine(EnableColliderAfterAnimation());
        }
    }

    private IEnumerator EnableColliderAfterAnimation()
    {
        // Get the animation clip duration
        float animationDuration = 0f;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == spillAnimationName)
            {
                animationDuration = ac.animationClips[i].length;
                break;
            }
        }

        // Wait for the animation to finish
        yield return new WaitForSeconds(animationDuration);

        // Enable the target collider
        targetCollider.enabled = true;
    }
}

using UnityEngine;

public class PlayAnimationOnTrigger : MonoBehaviour
{
    public Animator animator;
    public GameObject colliderObject;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == colliderObject)
        {
            animator.SetBool("bookDropped", true);
            animator.Play("Pump");

            // Trigger the onAnimationTrigger event
            LevelEventManager.Instance.TriggerAnimation("Pump");
        }
    }
}

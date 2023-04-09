using UnityEngine;

public class RocketCollision : MonoBehaviour
{
    public GameObject targetObject;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == targetObject)
        {
            // Trigger the onAnimationTrigger event
            LevelEventManager.Instance.TriggerAnimation("cupSpill");
        }
    }
}

using UnityEngine;

public class EnableOnTriggerEnterWithTag : MonoBehaviour
{
    public PosterController posterController;
    [SerializeField] private string targetTag = "YourTargetTag";
    [SerializeField] private GameObject objectToEnable;
    [SerializeField] private float detectionRadius = 1f;
    public bool hasTriggered = false;

    private void Start()
    {
        if (posterController == null)
        {
            Debug.LogError("PosterController is not assigned!");
        }
        hasTriggered = false;
    }

    private void Update()
    {
        if (!hasTriggered)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);

            foreach (Collider hitCollider in hitColliders)
            {
                Debug.Log("Detected: " + hitCollider.gameObject.name);

                if (hitCollider.CompareTag(targetTag))
                {
                    objectToEnable.SetActive(true);
                    hasTriggered = true;
                    if (posterController != null)
                    {
                        posterController.HasTriggered = true;
                    }
                    else
                    {
                        Debug.LogError("PosterController is not assigned!");
                    }
                }
            }
        }
    }
}

using UnityEngine;

public class EnableOnTriggerEnterWithTag : MonoBehaviour
{
    public PosterController posterController;
    [SerializeField] private string targetTag = "YourTargetTag";
    [SerializeField] private GameObject objectToEnable;
    public bool hasTriggered = false;

    private void Start()
    {
        if (posterController == null)
        {
            Debug.LogError("PosterController is not assigned!");
        }
        hasTriggered = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered by: " + other.gameObject.name);

        if (other.CompareTag(targetTag) && !hasTriggered)
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

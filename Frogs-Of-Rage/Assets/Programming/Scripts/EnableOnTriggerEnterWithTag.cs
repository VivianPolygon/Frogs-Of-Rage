using UnityEngine;

public class EnableOnTriggerEnterWithTag : MonoBehaviour
{
    [SerializeField] private string targetTag = "YourTargetTag";
    [SerializeField] private GameObject objectToEnable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            objectToEnable.SetActive(true);
        }
    }
}

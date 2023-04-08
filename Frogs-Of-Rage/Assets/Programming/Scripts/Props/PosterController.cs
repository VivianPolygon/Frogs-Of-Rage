using UnityEngine;
using System.Collections;

public class PosterController : MonoBehaviour
{
    public EnableOnTriggerEnterWithTag triggeredObjectScript;
    public Animator posterAnimation;
    public float swapDelay = 5f;
    public GameObject newPrefab;

    [SerializeField] private bool hasTriggered = false;

    public bool HasTriggered
    {
        get { return hasTriggered; }
        set
        {
            hasTriggered = value;
            if (hasTriggered)
            {
                posterAnimation.enabled = true;
                posterAnimation.SetBool("isBurning", true);
                StartCoroutine(SwapPrefabs());
                RenderSettings.fog = true;
            }
        }
    }

    private void Start()
    {
        if (triggeredObjectScript == null)
        {
            Debug.LogError("TriggeredObjectScript is not assigned!");
        }

        if (posterAnimation == null)
        {
            Debug.LogError("PosterAnimation is not assigned!");
        }
        else
        {
            posterAnimation.enabled = false; // Disable the Animator component at start
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggeredObjectScript != null && triggeredObjectScript.hasTriggered)
        {
            // Trigger the poster animation
            HasTriggered = true;

            //StartCoroutine(SwapPrefabs());
        }
    }

    private IEnumerator SwapPrefabs()
    {
        yield return new WaitForSeconds(swapDelay);
        Instantiate(newPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}

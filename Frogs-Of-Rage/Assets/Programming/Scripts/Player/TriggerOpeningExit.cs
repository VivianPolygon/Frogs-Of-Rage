using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOpeningExit : MonoBehaviour
{
    public float waitTimeBeforeAnimation = 3f;
    public AudioClip launchSFX, fuseSFX;
    public GameObject sparksVFX;
    public GameObject explosionVFX;

    private AudioSource audioSource;
    private bool activated = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sparksVFX.SetActive(false);
        explosionVFX.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!activated)
        {
            if (other.gameObject.CompareTag("Player") && GameManager.Instance.playerController.secondaryObjectiveComplete)
            {
                StartCoroutine(TriggerObjectiveAnimation(other));
                activated = true;
            }
        }
    }

    private IEnumerator TriggerObjectiveAnimation(Collider other)
    {
        sparksVFX.SetActive(true);
        audioSource.PlayOneShot(fuseSFX);
        yield return new WaitForSeconds(waitTimeBeforeAnimation);

        GameObject.FindGameObjectWithTag("Exit").GetComponent<Collider>().enabled = true;
        GetComponent<Animator>().SetTrigger("ObjectiveComplete");

        yield return new WaitForSeconds(.3f);
        audioSource.PlayOneShot(launchSFX);
    }
}

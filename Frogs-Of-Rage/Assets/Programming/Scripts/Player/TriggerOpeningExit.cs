using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOpeningExit : MonoBehaviour
{
    public float waitTimeBeforeAnimation = 3f;
    public GameObject sparksVFX;
    public GameObject explosionVFX;

    private void Start()
    {
        sparksVFX.SetActive(false);
        explosionVFX.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && GameManager.Instance.playerController.secondaryObjectiveComplete)
            StartCoroutine(TriggerObjectiveAnimation(other));
    }

    private IEnumerator TriggerObjectiveAnimation(Collider other)
    {
        sparksVFX.SetActive(true);
        yield return new WaitForSeconds(waitTimeBeforeAnimation);

        GameObject.FindGameObjectWithTag("Exit").GetComponent<Collider>().enabled = true;
        GetComponent<Animator>().SetTrigger("ObjectiveComplete");

        yield return new WaitForSeconds(.3f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOpeningExit : MonoBehaviour
{
    public float waitTimeBeforeAnimation = 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && GameManager.Instance.playerController.secondaryObjectiveComplete)
            StartCoroutine(TriggerObjectiveAnimation(other));
    }

    private IEnumerator TriggerObjectiveAnimation(Collider other)
    {
        yield return new WaitForSeconds(waitTimeBeforeAnimation);

        GameObject.FindGameObjectWithTag("Exit").GetComponent<Collider>().enabled = true;
        GetComponent<Animator>().SetTrigger("ObjectiveComplete");

        yield return new WaitForSeconds(.3f);
    }
}

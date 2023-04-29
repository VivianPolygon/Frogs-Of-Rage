using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOpeningExit : MonoBehaviour
{
    public float waitTimeBeforeAnimation = 3f;
    public GameObject BottleRocketSparks;
    public GameObject BottleRocketExplosion;

    private void Start()
    {
        BottleRocketSparks.SetActive(false);
        BottleRocketExplosion.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && GameManager.Instance.playerController.secondaryObjectiveComplete)
            StartCoroutine(TriggerObjectiveAnimation(other));
    }

    private IEnumerator TriggerObjectiveAnimation(Collider other)
    {
        BottleRocketSparks.SetActive(true);
        yield return new WaitForSeconds(waitTimeBeforeAnimation);

        GameObject.FindGameObjectWithTag("Exit").GetComponent<Collider>().enabled = true;
        GetComponent<Animator>().SetTrigger("ObjectiveComplete");

        yield return new WaitForSeconds(1f);
        BottleRocketExplosion.SetActive(true);
    }
}

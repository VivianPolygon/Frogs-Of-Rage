using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOpeningExit : MonoBehaviour
{
    public float waitTimeBeforeAnimation = 3f;
    public GameObject BottleRocketSparks;
    public GameObject BottleRocketExplosion;
    public SwitchObjects switchObjects;
    public AudioSource audioSource1; // Add the first audio source
    public AudioSource audioSource2; // Add the second audio source
    public GameObject LighterPickup;

    private void Start()
    {
        BottleRocketSparks.SetActive(false);
        BottleRocketExplosion.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && GameManager.Instance.playerController.secondaryObjectiveComplete)
        {
            StartCoroutine(TriggerObjectiveAnimation(other));
            LighterPickup.SetActive(false);
        }
    }

    private IEnumerator TriggerObjectiveAnimation(Collider other)
    {
        BottleRocketSparks.SetActive(true);
        yield return new WaitForSeconds(waitTimeBeforeAnimation);
        audioSource1.Play(); // Play the first audio source

        GameObject.FindGameObjectWithTag("Exit").GetComponent<Collider>().enabled = true;
        GetComponent<Animator>().SetTrigger("ObjectiveComplete");

        yield return new WaitForSeconds(1f);
        BottleRocketExplosion.SetActive(true);
        audioSource2.Play(); // Play the second audio source
        switchObjects.Switch();
        RenderSettings.fog = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ventogic : MonoBehaviour
{
    public AudioClip explosionSFX;
    //public GameObject explosionVFX;
    //public Vector3 offset;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name == "BottleRocket")
        {
            audioSource.PlayOneShot(explosionSFX);
            //GameObject vfx = Instantiate(explosionVFX);
            //vfx.transform.position = offset;
            //vfx.SetActive(true);
            //transform.GetChild(0).gameObject.SetActive(true);
            //transform.GetChild(1).gameObject.SetActive(false);
            //RenderSettings.fog = true;
        }
    }
}

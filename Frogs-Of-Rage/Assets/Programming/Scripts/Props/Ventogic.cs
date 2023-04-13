using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ventogic : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.name == "BottleRocket")
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
            RenderSettings.fog = true;
        }
    }
}

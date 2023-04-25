using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatPickup : MonoBehaviour
{
    [SerializeField] private int _hatPickupID;
    public int HatPickupID
    {
        get { return _hatPickupID; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(PlayerHatManager.Instance != null)
            {
                PlayerHatManager.Instance.Inventory.UnlockHat(_hatPickupID);
                gameObject.SetActive(false);
            }
        }
    }
}

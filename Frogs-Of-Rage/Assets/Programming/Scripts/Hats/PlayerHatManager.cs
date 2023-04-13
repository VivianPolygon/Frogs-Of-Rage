using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHatManager : MonoBehaviour
{
    public PlayerHatsInventory _inventory = new PlayerHatsInventory();

    private void Awake()
    {
        _inventory.LoadHatInventory();
    }

    public void UnlockHat(int hatID)
    {
        _inventory.UnlockHat(hatID);
        _inventory.SaveHatInventory();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCollectable : MonoBehaviour, ICollectable
{
    public CollectableData collectableData;

    public static event Action OnFlyCollected;


    private void Update()
    {
        Rotate();
    }


    public void Rotate()
    {
        transform.Rotate(0, 1, 0, Space.World);
    }
    public void Collect()
    {
        UIManager.Instance.collectedData = collectableData;
        Destroy(gameObject);
        OnFlyCollected?.Invoke();
    }

}

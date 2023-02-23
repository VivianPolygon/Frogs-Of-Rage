using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrasshopperCollectable : Collectable, ICollectable
{
    public void Start()
    {
        GameManager.Instance.grasshoppersInScene++;


    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCollectable : Collectable, ICollectable
{
    public void Start()
    {
        GameManager.Instance.fliesInScene++;
    }
}

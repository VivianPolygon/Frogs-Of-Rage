using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntCollectable : Collectable, ICollectable
{
    public void Start()
    {
        GameManager.Instance.antsInScene++;

    }
}

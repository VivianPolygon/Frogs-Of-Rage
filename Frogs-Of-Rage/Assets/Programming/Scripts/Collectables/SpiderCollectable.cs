using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderCollectable : Collectable, ICollectable
{
    public void Start()
    {
        GameManager.Instance.spidersInScene++;
    }


}

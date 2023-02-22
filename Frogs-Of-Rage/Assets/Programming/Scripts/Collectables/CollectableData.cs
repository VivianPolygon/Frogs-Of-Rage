using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Collectable Data", menuName = "CollectableData", order = 52)]

public class CollectableData : ScriptableObject
{
    public int count;
    public Sprite image;
    public Material material;
   
    public int Count
    {
        get { return count; }
        set { count = value; }
    }
    public Sprite Image
    {
        get { return image; }
    }
    public Material Material
    {
        get { return material; }
    }
    
}

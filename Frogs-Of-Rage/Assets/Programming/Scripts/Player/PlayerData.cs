using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "PlayerData", order = 51)]
public class PlayerData : ScriptableObject
{
    public int flyCount;
    public Sprite flyImage;
    public int antCount;
    public Sprite antImage;
    public int grasshopperCount;
    public Sprite grasshopperImage;
    public int spiderCount;
    public Sprite spiderImage;



    public int FlyCount
    {
        get { return flyCount; }
        set { flyCount = value; }
    }
    public Sprite FlyImage
    {
        get { return flyImage; }
    }
    public int AntCount
    {
        get { return antCount; }
        set { antCount = value; }
    }
    public Sprite AntImage
    {
        get { return antImage; }
    }
    public int GrasshopperCount
    {
        get { return grasshopperCount; }
        set
        {
            grasshopperCount = value;
        }
    }
    public Sprite GrasshopperImage
    {
        get { return grasshopperImage; }
    }
    public int SpiderCount
    {
        get { return spiderCount; }
        set
        {
            spiderCount = value;
        }
    }
    public Sprite SpiderImage
    {
        get { return spiderImage; }
    }
}

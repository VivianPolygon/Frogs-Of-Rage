using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.Events;

public class Collectable : MonoBehaviour, ICollectable
{
    public CollectableData collectableData;

    private Texture2D texture;

    public delegate void OnCollectableEvent(OnCollectableEventArgs e);
    public static event OnCollectableEvent OnCollectable;


    //private void Awake()
    //{
    //    texture = new Texture2D((int)collectableData.Image.rect.width, (int)collectableData.Image.rect.height);

    //    //collectableData.Material.SetTexture("_BaseMap",);
        
    //    var pixels = collectableData.Image.texture.GetPixels((int)collectableData.Image.textureRect.x,
    //        (int)collectableData.Image.rect.y,
    //        (int)collectableData.Image.rect.width,
    //        (int)collectableData.Image.rect.height);

    //    texture.SetPixels(pixels);
    //    texture.Apply();

    //    collectableData.Material.SetTexture("_BaseMap", texture);


    //}

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
        OnCollectable?.Invoke(new OnCollectableEventArgs(collectableData, GameManager.Instance.playerController.playerData, gameObject, GameManager.Instance.playerController));
        Destroy(gameObject);
    }
}


#region Collectable Event
[System.Serializable]
public class OnCollectableEvent : UnityEvent<OnCollectableEventArgs> { }

public class OnCollectableEventArgs
{
    public CollectableData collectableData;
    public PlayerData playerData;
    public GameObject gameObject;
    public PlayerController playerController;

    public OnCollectableEventArgs(CollectableData collectableData, PlayerData playerData, GameObject gameObject, PlayerController playerController)
    {
        this.collectableData = collectableData;
        this.playerData = playerData;
        this.gameObject = gameObject;
        this.playerController = playerController;
    }
}


#endregion
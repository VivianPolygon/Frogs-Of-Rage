using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleRocketExplosion : MonoBehaviour
{
    public float explosionFlashDuration = 0.1f;
    public float explosionLifetime = 3.0f;
    public Color[] explosionColors = new Color[2];

    private Light light;

    private void Awake()
    {
        light = GetComponent<Light>();
    }

    private void Start()
    {
        Invoke("DisableLight", explosionFlashDuration);
        Invoke("Despawn", explosionLifetime);
        InvokeRepeating("ChangeColor", 1f, 0.2f);
    }

    private void DisableLight()
    {
        light.enabled = false;
        CancelInvoke("ChangeColor");
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }

    private void ChangeColor()
    {
        if(Random.Range(0f, 1f) > 0.5f)
            light.color = explosionColors[0];
        else
            light.color = explosionColors[1];
    }
}

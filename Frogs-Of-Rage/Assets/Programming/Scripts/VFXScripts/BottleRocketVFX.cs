using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleRocketVFX : MonoBehaviour
{
    [SerializeField]
    public float speed = .01f;

    private void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;    
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class VacuumKillBox : MonoBehaviour
{
    BoxCollider _collider;
    [SerializeField] private LayerMask _playerLayer;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((_playerLayer.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            VacuumNavigation.InvokeOnPlayerHit();
        }
    }

}

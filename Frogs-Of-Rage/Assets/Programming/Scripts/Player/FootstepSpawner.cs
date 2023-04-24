using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FootLocation
{
    Right,
    Left
}
public class FootstepSpawner : MonoBehaviour
{
    [SerializeField] private float destroyTime = 10f;
    [SerializeField] private GameObject rFootPrefab, lFootPrefab;
    [SerializeField] private Transform rightFoot, leftFoot;

    public void SpawnFootIcon(FootLocation location)
    {
        switch (location)
        {
            case FootLocation.Left:
                SpawnFoot(leftFoot, lFootPrefab);
                break;
            case FootLocation.Right:
                SpawnFoot(rightFoot, rFootPrefab);
                break;
        }
    }

    private void SpawnFoot(Transform foot, GameObject prefab)
    {
        if(Physics.Raycast(foot.position, -foot.transform.up ,out RaycastHit hit, 1, ~LayerMask.GetMask("Player")))
        {
            GameObject gO = Instantiate(prefab, hit.point + Vector3.up / 100, Quaternion.FromToRotation(foot.transform.up, hit.normal));
            Destroy(gO, destroyTime);
        }
    }
}

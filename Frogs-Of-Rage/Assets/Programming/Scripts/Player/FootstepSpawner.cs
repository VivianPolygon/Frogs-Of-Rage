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
    [SerializeField] private float destroyTime = 5f;
    [SerializeField] private GameObject footPrefab;
    [SerializeField] private Transform rightFoot, leftFoot;
    private FootLocation footLocation = FootLocation.Right;
    public void SpawnFootIcon(FootLocation location)
    {
        switch (location)
        {
            case FootLocation.Left:
                SpawnFoot(leftFoot, location);
                break;
            case FootLocation.Right:
                SpawnFoot(rightFoot, location);
                break;
        }
    }

    private void SpawnFoot(Transform foot, FootLocation location)
    {
        bool isRightFoot = location == FootLocation.Right;

        if (Physics.Raycast(foot.position, isRightFoot ? foot.transform.up : -foot.transform.up, out RaycastHit hit, 1, ~LayerMask.GetMask("Player")))
        {
            GameObject gO = Instantiate(footPrefab, hit.point + foot.transform.up / 70, Quaternion.FromToRotation(isRightFoot ? foot.transform.up : -foot.transform.up, hit.normal));
            Quaternion rotation = Quaternion.LookRotation(isRightFoot ? -foot.transform.right : foot.transform.right, isRightFoot ? -foot.transform.up : foot.transform.up);
            //rotation.x = hit.normal.x;
            gO.transform.rotation = rotation;
            Destroy(gO, destroyTime);
        }
    }
}

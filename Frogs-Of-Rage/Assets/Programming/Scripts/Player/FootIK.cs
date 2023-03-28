using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootIK : MonoBehaviour
{
    [SerializeField]
    private Transform body;
    [SerializeField]
    private float footSpacing;
    [SerializeField]
    private float stepDistance = 0.5f;
    [SerializeField]
    private float stepHeight = 0.5f;
    [SerializeField]
    private float stepLength = 4;
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private FootIK otherFoot = default;

    private float lerp;
    public Vector3 floorToAnkle;
    public Vector3 normalModifier;
    private Vector3 currentPosition, newPosition, oldPosition;
    private Vector3 oldNormal, currentNormal, newNormal;

    private void Awake()
    {
        //newPosition = new Vector3(0, 0, stepDistance);
        footSpacing = transform.position.x;
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = transform.up;
        lerp = 1;
    }
    // Update is called once per frame
    void Update()
    {
        FindNextFootPos();
    }

    //private void PlaceFeetOnGround()
    //{
    //    Ray ray = new Ray(body.position + (body.right * footSpacing), Vector3.down);
    //    if (Physics.Raycast(ray, out RaycastHit hit2, 10, LayerMask.GetMask("Ground")))
    //    {
    //        transform.position = hit2.point + new Vector3(0, floorToAnkle, 0);
    //    }
    //}

    //public bool isDown()
    //{
    //    if (oldPosition == newPosition)
    //        return true;
    //    else
    //        return false;
    //}
    //public bool OtherFootIsDown()
    //{
    //    if (otherFoot.isDown())
    //        return true;
    //    else
    //        return false;
    //}
    private void FindNextFootPos()
    {
        transform.position = currentPosition;
        transform.up = currentNormal;

        Ray ray = new Ray(body.position + (body.right * footSpacing), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10, LayerMask.GetMask("Ground")))
        {
            if (Vector3.Distance(newPosition, hit.point) > stepDistance && !otherFoot.IsMoving() && lerp >= 1)
            {
                lerp = 0;
                int direction = body.InverseTransformPoint(hit.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                newPosition = hit.point + (body.forward * stepLength * direction) + floorToAnkle;
                //newPosition = hit.point + new Vector3(0, floorToAnkle, 0);
            }
        }
        if (lerp < 1)
        {
            Vector3 footPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            footPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currentPosition = footPosition;
            currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
            lerp += Time.deltaTime * speed;
        }
        else
        {
            oldPosition = newPosition;
            oldNormal = newNormal;
        }

    }

    public bool IsMoving()
    {
        return lerp < 1;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.1f);
    }
}

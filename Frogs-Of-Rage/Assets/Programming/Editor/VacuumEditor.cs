using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (VacuumNavigation))]
public class VacuumEditor : Editor
{
    private void OnSceneGUI()
    {
        VacuumNavigation vNav = (VacuumNavigation)target;

        //draws the circle showing view range
        Handles.color = Color.white;
        Handles.DrawWireArc(vNav.transform.position, vNav.transform.up, vNav.transform.forward, 360, vNav.DistanceOfView);
        //draws lines that indicate cone of vision
        Handles.color = Color.blue;
        Vector3 viewAngleA = vNav.DirectionFromAngle(-vNav.FieldOfView / 2, true);
        Vector3 viewAngleB = vNav.DirectionFromAngle(vNav.FieldOfView / 2, true);
        //draws horizontal lines
        Handles.DrawLine(vNav.transform.position, vNav.transform.position + viewAngleA * vNav.DistanceOfView);
        Handles.DrawLine(vNav.transform.position, vNav.transform.position + viewAngleB * vNav.DistanceOfView);
        //draws vertical lines
        viewAngleA = vNav.DirectionFromAngle(-vNav.FieldOfView / 2, false);
        viewAngleB = vNav.DirectionFromAngle(vNav.FieldOfView / 2, false);
        Handles.DrawLine(vNav.transform.position, vNav.transform.position + viewAngleA * vNav.DistanceOfView);
        Handles.DrawLine(vNav.transform.position, vNav.transform.position + viewAngleB * vNav.DistanceOfView);



        //for Player Detection Demoing
        if (vNav.CheckPlayerInSight() != null)
        {
            Transform PlayerPos = vNav.CheckPlayerInSight();

            Handles.color = Color.green;
            Handles.DrawWireArc(PlayerPos.position, PlayerPos.up, PlayerPos.forward, 360, (PlayerPos.localScale.x + PlayerPos.localScale.y + PlayerPos.localScale.z) / 3);
        }


    }
}

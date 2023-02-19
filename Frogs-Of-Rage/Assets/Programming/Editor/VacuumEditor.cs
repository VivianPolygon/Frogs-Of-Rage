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
        Handles.Label(vNav.transform.position + -vNav.transform.forward * vNav.DistanceOfView, "View Range");

        //draws navmesh agent radius
        Handles.color = Color.green;
        Handles.DrawWireArc(vNav.transform.position, vNav.transform.up, vNav.transform.forward, 360, vNav.NavMeshAgentRadius * vNav.transform.localScale.x);
        Handles.Label(vNav.transform.position + -vNav.transform.forward * vNav.NavMeshAgentRadius * vNav.transform.localScale.x, "Navmesh Agent Radius");


        //draws lines that indicate cone of vision
        Handles.color = Color.blue;
        Vector3 viewAngleA = vNav.DirectionFromAngle(-vNav.FieldOfView / 2, true);
        Vector3 viewAngleB = vNav.DirectionFromAngle(vNav.FieldOfView / 2, true);
        //draws horizontal lines
        Handles.DrawLine(vNav.transform.position, vNav.transform.position + viewAngleA * vNav.DistanceOfView);
        Handles.Label((vNav.transform.position + viewAngleA * vNav.DistanceOfView), ("Left View Bound"));
        Handles.DrawLine(vNav.transform.position, vNav.transform.position + viewAngleB * vNav.DistanceOfView);
        Handles.Label((vNav.transform.position + viewAngleB * vNav.DistanceOfView), ("Right View Bound"));
        //draws vertical lines
        viewAngleA = vNav.DirectionFromAngle(-vNav.FieldOfView / 2, false);
        viewAngleB = vNav.DirectionFromAngle(vNav.FieldOfView / 2, false);

        Handles.DrawLine(vNav.transform.position, vNav.transform.position + viewAngleA * vNav.DistanceOfView);
        Handles.Label((vNav.transform.position + viewAngleA * vNav.DistanceOfView), ("Bottom View Bound"));
        Handles.DrawLine(vNav.transform.position, vNav.transform.position + viewAngleB * vNav.DistanceOfView);
        Handles.Label((vNav.transform.position + viewAngleB * vNav.DistanceOfView), ("Top View Bound"));



        //for Player Detection visualization
        if (vNav.CheckPlayerInSight() != null)
        {
            Transform PlayerPos = vNav.CheckPlayerInSight();

            Handles.color = Color.green;
            Handles.Label(PlayerPos.position, "Player Seen");
            Handles.DrawSolidDisc(PlayerPos.position, PlayerPos.up, (PlayerPos.localScale.x + PlayerPos.localScale.y + PlayerPos.localScale.z) / 3);
        }

        //for Attack Range Display
        Handles.color = Color.red;
        Handles.Label(vNav.transform.position + (vNav.transform.forward * vNav.AttackDistance), ("Chasing Attack Distance"));
        Handles.DrawLine(vNav.transform.position, vNav.transform.position + (vNav.transform.forward * vNav.AttackDistance));

    }
}

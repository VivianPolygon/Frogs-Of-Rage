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


        //draws navmesh agent radius
        Handles.color = Color.green;
        Handles.DrawWireArc(vNav.transform.position, vNav.transform.up, vNav.transform.forward, 360, vNav.NavMeshAgentRadius * vNav.transform.localScale.x);
        Handles.Label(vNav.transform.position + -vNav.transform.forward * vNav.NavMeshAgentRadius * vNav.transform.localScale.x, "Navmesh Agent Radius");


        //draws lines that indicate cone of vision
        Handles.color = Color.blue;
        Vector3 viewAngleA = vNav.DirectionFromAngle(-vNav.FieldOfView / 2, true);
        Vector3 viewAngleB = vNav.DirectionFromAngle(vNav.FieldOfView / 2, true);
        Vector3 heightOffset = (vNav.transform.up * vNav.HeightOfView);
        //draws horizontal lines
        Handles.DrawLine(vNav.transform.position + heightOffset, vNav.transform.position - heightOffset);

        Handles.DrawLine(vNav.transform.position + heightOffset, vNav.transform.position + heightOffset + viewAngleA * vNav.DistanceOfView);
        Handles.DrawLine(vNav.transform.position - heightOffset, vNav.transform.position - heightOffset + viewAngleA * vNav.DistanceOfView);
        Handles.Label((vNav.transform.position + viewAngleA * vNav.DistanceOfView), ("Left View Bound"));

        Handles.DrawLine(vNav.transform.position + heightOffset, vNav.transform.position + heightOffset + viewAngleB * vNav.DistanceOfView);
        Handles.DrawLine(vNav.transform.position - heightOffset, vNav.transform.position - heightOffset + viewAngleB * vNav.DistanceOfView);
        Handles.Label((vNav.transform.position + viewAngleB * vNav.DistanceOfView), ("Right View Bound"));

        Handles.Label(vNav.transform.position + vNav.transform.forward * vNav.DistanceOfView, "View Distance");
        //draws height lines
        Vector3 viewAngleAPoint = vNav.transform.position + viewAngleA * vNav.DistanceOfView;
        Vector3 viewAngleBPoint = vNav.transform.position + viewAngleB * vNav.DistanceOfView;
        Handles.DrawLine(viewAngleAPoint + heightOffset, viewAngleAPoint - heightOffset);
        Handles.DrawLine(viewAngleBPoint + heightOffset, viewAngleBPoint - heightOffset);

        //draws height lines across
        Handles.DrawWireArc(vNav.transform.position + heightOffset, vNav.transform.up, viewAngleA, vNav.FieldOfView, vNav.DistanceOfView);
        Handles.DrawWireArc(vNav.transform.position - heightOffset, vNav.transform.up, viewAngleA, vNav.FieldOfView, vNav.DistanceOfView);



        //for Player Hearing Visuilization
        {
            Handles.color = Color.yellow;

            Handles.DrawWireArc(vNav.transform.position - (vNav.transform.up * vNav.DetectionHeightRange), vNav.transform.up, vNav.transform.right, 30, vNav.SoundVolumeAmplifier * 0.5f);
            Handles.DrawWireArc(vNav.transform.position + (vNav.transform.up * vNav.DetectionHeightRange), vNav.transform.up, vNav.transform.right, 30, vNav.SoundVolumeAmplifier * 0.5f);
            Handles.DrawLine(vNav.transform.position + (vNav.transform.right * (vNav.SoundVolumeAmplifier * 0.5f)) + (vNav.transform.up * vNav.DetectionHeightRange), (vNav.transform.position + (vNav.transform.right * (vNav.SoundVolumeAmplifier * 0.5f)) - (vNav.transform.up * vNav.DetectionHeightRange)));
            Handles.Label(vNav.transform.position + vNav.transform.right * (vNav.SoundVolumeAmplifier * 0.5f), "0.5 second Fall Heard");

            Handles.DrawWireArc(vNav.transform.position - (vNav.transform.up * vNav.DetectionHeightRange), vNav.transform.up, vNav.transform.right, 35, vNav.SoundVolumeAmplifier);
            Handles.DrawWireArc(vNav.transform.position + (vNav.transform.up * vNav.DetectionHeightRange), vNav.transform.up, vNav.transform.right, 35, vNav.SoundVolumeAmplifier);
            Handles.DrawLine(vNav.transform.position + (vNav.transform.right * (vNav.SoundVolumeAmplifier * 1)) + (vNav.transform.up * vNav.DetectionHeightRange), (vNav.transform.position + (vNav.transform.right * (vNav.SoundVolumeAmplifier * 1)) - (vNav.transform.up * vNav.DetectionHeightRange)));
            Handles.Label(vNav.transform.position + vNav.transform.right * vNav.SoundVolumeAmplifier, "1 second Fall Heard");

            Handles.DrawWireArc(vNav.transform.position - (vNav.transform.up * vNav.DetectionHeightRange), vNav.transform.up, vNav.transform.right, 40, vNav.SoundVolumeAmplifier * 1.5f);
            Handles.DrawWireArc(vNav.transform.position + (vNav.transform.up * vNav.DetectionHeightRange), vNav.transform.up, vNav.transform.right, 40, vNav.SoundVolumeAmplifier * 1.5f);
            Handles.DrawLine(vNav.transform.position + (vNav.transform.right * (vNav.SoundVolumeAmplifier * 1.5f)) + (vNav.transform.up * vNav.DetectionHeightRange), (vNav.transform.position + (vNav.transform.right * (vNav.SoundVolumeAmplifier * 1.5f)) - (vNav.transform.up * vNav.DetectionHeightRange)));
            Handles.Label(vNav.transform.position + vNav.transform.right * vNav.SoundVolumeAmplifier * 1.5f, "1.5 second Fall Heard");


            Handles.DrawWireArc(vNav.transform.position - (vNav.transform.up * vNav.DetectionHeightRange), vNav.transform.up, vNav.transform.right, 45, vNav.SoundVolumeAmplifier * 2);
            Handles.DrawWireArc(vNav.transform.position + (vNav.transform.up * vNav.DetectionHeightRange), vNav.transform.up, vNav.transform.right, 45, vNav.SoundVolumeAmplifier * 2);
            Handles.DrawLine(vNav.transform.position + (vNav.transform.right * (vNav.SoundVolumeAmplifier * 2)) + (vNav.transform.up * vNav.DetectionHeightRange), (vNav.transform.position + (vNav.transform.right * (vNav.SoundVolumeAmplifier * 2)) - (vNav.transform.up * vNav.DetectionHeightRange)));
            Handles.Label(vNav.transform.position + vNav.transform.right * (vNav.SoundVolumeAmplifier * 2), "2 second Fall Heard");
        }


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

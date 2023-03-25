using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(EmberVFX))]
public class EmberVFXEditor : Editor
{
    private void OnSceneGUI()
    {
        EmberVFX embers = (EmberVFX)target;

        Handles.color = Color.red;
        Handles.DrawSolidDisc(embers.transform.position + embers.EmitterOffset, embers.transform.up, 1);
    }
}

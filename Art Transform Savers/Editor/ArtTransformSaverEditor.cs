using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ArtTransformSaver))]

public class ArtTransformSaverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ArtTransformSaver transformSaverScript = (ArtTransformSaver)target;

        if (GUILayout.Button("Save Selected Details"))
        {
            transformSaverScript.SaveTransformDetails();
        }

        if (GUILayout.Button("Apply Selected Details"))
        {
            transformSaverScript.ApplyTransformDetails();
        }
    }
}

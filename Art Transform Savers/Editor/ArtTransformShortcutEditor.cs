using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ArtTransformShortcut))]
public class ArtTransformShortcutEditor : Editor
{
    //This script is a script placed at the big locations game object which contains all frames.
    //If any gameobject (art) has a transform saver, it will use its functions.

    //This is a global button and will affect all sprites. Only used in editor.

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ArtTransformShortcut transformSaverScript = (ArtTransformShortcut)target;

        if (GUILayout.Button("Retrieve All Artsaver and Parallax Objects From Children"))
        {
            transformSaverScript.RetrieveAllTransformSavers();
        }

        EditorGUILayout.Space(25);

        EditorGUILayout.HelpBox("Before modifying the scene's arts, please disable all parallax objects with the button below. This is to prevent any accidental movement of the sprites during editing.", MessageType.Warning);

        if (GUILayout.Button("Enable All Parallax Objects"))
        {
            transformSaverScript.EnableAllParallax();
        }

        if (GUILayout.Button("Disable All Parallax Objects"))
        {
            transformSaverScript.DisableAllParallax();
        }


        EditorGUILayout.Space(25);



        EditorGUILayout.Space(25);

        EditorGUILayout.HelpBox("This will save all positions. Make sure everything is where it needs to be!", MessageType.Warning);

        if (GUILayout.Button("Save All Art Positions"))
        {
            transformSaverScript.SaveAll();
        }

        EditorGUILayout.Space(25);



        EditorGUILayout.HelpBox("This will reset all positions to their saved place. Make sure everything is saved correctly!", MessageType.Warning);


        if (GUILayout.Button("Reset All Art Positions To Saved"))
        {
            transformSaverScript.ResetAllToSaved();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ParallaxFacCalc))]

public class ParallaxFacCalcEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("It is recommended to only use factor calculation if your sprite number is more than 4. If you have less than that, then it's better to set them up manually", MessageType.Warning);
        
        DrawDefaultInspector();

        ParallaxFacCalc factorCalculation = (ParallaxFacCalc)target;

        if (GUILayout.Button("Perform Calculations"))
        {
            factorCalculation.PerformFactorCalculations();
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("Hint: Background elements should not be moving in the opposite direction. Foreground elements should be instead. If you set this button for background sorting group, it will falsify moving in the opposite direction for you.", MessageType.Info);
    }
}


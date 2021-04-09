using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class ArtTransformSaver : MonoBehaviour
{
    //Simple script to keep the sprites from moving around during editing from the parallax effect.

    //It is added to a gameobject and its button is then pressed to save any of its transform parameters.

    //On awake, it will reset the object's transform to the saved one.

    //Say goodbye to messy sprite positions.

    public Transform thisTransform;
    public Vector3 ConstantPos;
    public Quaternion ConstantRot;
    public Vector3 ConstantScale;
	
	public void SaveTransformDetails()
    {
        thisTransform = GetComponent<Transform>();
        ConstantPos = thisTransform.localPosition;
        ConstantRot = thisTransform.localRotation;
        ConstantScale = thisTransform.localScale;
    }

    public void ApplyTransformDetails()
    {
        if (thisTransform == null)
        {
            Debug.LogError("No transform component was set.");
        }

        thisTransform.localRotation = ConstantRot;
        thisTransform.localScale = ConstantScale;
        thisTransform.localPosition = ConstantPos;
    }

    private void OnApplicationQuit()
    {
        ApplyTransformDetails();
    }
}

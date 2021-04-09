using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;

public class ArtTransformShortcut : MonoBehaviour
{
    //This script is a script placed at the big locations game object which contains all frames.
    //If any gameobject (art) has a transform saver, it will use its functions.

    //This is a global button and will affect all sprites. Only to be used in editor.

    public ArtTransformSaver[] AllArtSavers;
    public ParallaxElement[] AllParallaxElements;
    public bool EnableAllParallaxElementsOnStart = true;

    private void Awake()
    {
        if (EnableAllParallaxElementsOnStart)
        {
            EnableAllParallax();
        }
    }

    public void RetrieveAllTransformSavers()
    {
        AllArtSavers = GetComponentsInChildren<ArtTransformSaver>();
        AllParallaxElements = GetComponentsInChildren<ParallaxElement>();

        if(AllArtSavers == null || AllParallaxElements == null)
        {
            Debug.LogError("No artsavers were found in children. Make sure this script is placed at the top most gameobject.");
        }
        if (AllArtSavers != null || AllParallaxElements != null)
        {
            Debug.Log("Artsavers found or updated, and assigned successfully.");
        }
        
    }

    public void DisableAllParallax()
    {
        for (int i = 0; i < AllParallaxElements.Length; i++)
        {
            AllParallaxElements[i].enabled = false;
        }
        Debug.Log("All Parallax Elements Disabled.");
    }

    public void EnableAllParallax()
    {
        for (int i = 0; i < AllParallaxElements.Length; i++)
        {
            AllParallaxElements[i].enabled = true;
        }
        Debug.Log("All Parallax Elements Enabled.");
    }



    public void SaveAll()
    {
        for (int i = 0; i < AllArtSavers.Length; i++)
        {
            AllArtSavers[i].SaveTransformDetails();
        }
        Debug.Log("Transforms Saved Successfully");
    }

    public void ResetAllToSaved()
    {
        for (int i = 0; i < AllArtSavers.Length; i++)
        {
            AllArtSavers[i].ApplyTransformDetails();
        }
        Debug.Log("Transforms Reset Successfully");
    }
}

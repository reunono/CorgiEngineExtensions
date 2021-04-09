using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxFacCalc : MonoBehaviour
{
    //This script is placed at the father game object which contains the sorting group. Which would either be the background or foreground gameobject.

    //It aims to set up parallax factors automatically based on the distance of each image in the hierarchy using the calculation method present in ParallaxO.cs.

    ParallaxO[] ParallaxElements;
    [Header("Experimental:")]
    public bool Foreground;
    public void PerformFactorCalculations()

    {
        ParallaxElements = GetComponentsInChildren<ParallaxO>();
        if (ParallaxElements.Length == 0)
        {
            Debug.LogError("No parallax elements under this gameobject! Are you sure you placed the calculator in the correct place?");
            return;
        }
        for (int i = 0; i < ParallaxElements.Length; i++)
        {
            if (Foreground)
            {
                ParallaxElements[i].CalculateAndSetParallaxFactors(ParallaxElements.Length, true);
            }
            else
            {
                ParallaxElements[i].CalculateAndSetParallaxFactors(ParallaxElements.Length, false);
            }
        }
        Debug.Log("Calculations of factors have been made and set to each parallax object!");
    }
}

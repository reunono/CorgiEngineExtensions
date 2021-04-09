using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;

public class ParallaxO : ParallaxElement
{
    //Functions for the parallax element calculations.
    int Index;
    float MultFactor;

    public void CalculateAndSetParallaxFactors(int NumberOfSiblings, bool ForeGround) //A calculation method which will take into account different factors and give this object its appropriate parallax factors. It is called by a factor calculation script which fills its number of siblings.
    {
        //We first calculate the probable speed factor based on distance. Foreground calculation is still experimental and not always correct.
        float SpeedFactor;
        if (ForeGround)
        {
            Index = transform.GetSiblingIndex();
            MultFactor = 1f / NumberOfSiblings;
            SpeedFactor = Mathf.Abs(MultFactor * Index);
            // Then we set the speed factor for the horizontal speed as is, but reduce it by the multfactor for vertical speed.
            HorizontalSpeed = SpeedFactor;
            VerticalSpeed = SpeedFactor - MultFactor;

            //For the closest gameobject in foreground terms, the vertical factor needs to be zero as it might break and become a large number on calculation

            if (Index == 0) //
            {
                VerticalSpeed = 0f;
            }
        }
        else
        {
            MoveInOppositeDirection = false; //Since we are making calculations for background elements, they need to be moving with the camera.
            Index = transform.GetSiblingIndex();
            MultFactor = 1f / NumberOfSiblings;
            SpeedFactor = Mathf.Abs(MultFactor * Index - 1);
            // Then we set the speed factor for the horizontal speed as is, but reduce it by the multfactor for vertical speed.
            HorizontalSpeed = Mathf.Abs(SpeedFactor);
            VerticalSpeed = Mathf.Abs(SpeedFactor - MultFactor);
            //For the closest gameobject, the vertical factor needs to be zero as it might break and become a large number on calculation.
            if (Index == NumberOfSiblings - 1)
            {
                VerticalSpeed = 0f;
            }
        }
    }
}

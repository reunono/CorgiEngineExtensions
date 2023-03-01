using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemManagerWithRotation : InputSystemManagerEventsBased
{
    public Vector2 CurrentDirection { get; protected set; }
  
    private Vector2 _previousDirection;
    public void OnRotationalMovement(InputValue value)
    {
        CurrentDirection = value.Get<Vector2>();
        
    }
    
    protected override void BindButton(InputValue inputValue, MMInput.IMButton imButton)
    {
        if (imButton != null)
        {
            base.BindButton(inputValue, imButton);
        }
    }

    protected override void Update()
    {
        
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using UnityEngine.UIElements;

/// <summary>
/// Used to measure how a character rotates their analog stick - ideal for operating gears, pulleys, and wheels.
/// Currently only implemented for use with New Input System
/// </summary>
    public class CharacterRotationalActivation : CharacterButtonActivation
    {
        //true if character has started rotating their input device.
        [SerializeField] private float totalRotation = 0f;
        
        public bool IsRotating { get; set; }
        public RotationActivated RotationActivatedZone { get; set; }
        public bool InRotationActivatedZone { get; set; }
        private Vector2 _previousDirection;
        private bool _previouslyActivated;
        
        new InputSystemManagerWithRotation _inputManager => (InputSystemManagerWithRotation)(base._inputManager);
        public override string HelpBoxText()
        {
            return "This component allows your character to generate a rotational interaction from Gamepad/Mouse.";
        }

        protected override void Initialization()
        {
            base.Initialization();
            IsRotating = false;
            _previouslyActivated = false;
        }

        protected override void HandleInput()
        {

            if (InRotationActivatedZone && (RotationActivatedZone != null) && _inputManager.CurrentDirection.sqrMagnitude > 0)
            {
                if (_previouslyActivated)
                {
                    RotationActivation();
                }
                else
                {
                    _previouslyActivated = true;
                    _previousDirection = _inputManager.CurrentDirection;
                }
            }
            else
            {
                _previouslyActivated = false;
            }
            
        }

        protected virtual void RotationActivation()
        {
            if ((InRotationActivatedZone
                && RotationActivatedZone != null)
                && (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
                )
            {
                if (_inputManager.CurrentDirection.sqrMagnitude > 0)
                {
                    MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.ButtonActivation);
                    
                    
                    if (!_previouslyActivated)
                    {
                        _previouslyActivated = true;
                        _previousDirection = _inputManager.CurrentDirection;
                        return;
                    }

                    float currentAngleDelta = Quaternion
                        .FromToRotation(_inputManager.CurrentDirection, _previousDirection).eulerAngles.z;
                    if (currentAngleDelta > 180)
                    {
                        currentAngleDelta -= 360;
                    }

                    totalRotation += currentAngleDelta;
                    _previousDirection = _inputManager.CurrentDirection;
                    RotationActivatedZone.TriggerRotationAction(this.gameObject, currentAngleDelta);
                }
                else
                {
                    _previouslyActivated = false;
                }
            }
        }
        
        




    }

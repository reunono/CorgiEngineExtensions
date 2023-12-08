using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using UnityEngine;

public class CharacterMultiButtonActivation : CharacterButtonActivation
{
    private readonly List<ButtonActivated> _buttonActivated = new();
    private void OnTriggerEnter2D(Collider2D other) => _buttonActivated.AddRange(other.GetComponents<ButtonActivated>());
    private void OnTriggerExit2D(Collider2D other)
    {
        foreach (var buttonActivated in other.GetComponents<ButtonActivated>())
            _buttonActivated.Remove(buttonActivated);
    }

    protected override void HandleInput()
    {
        if (!AbilityAuthorized) return;
        for (var i=_buttonActivated.Count-1; i >= 0; i--)
        {
            InButtonActivatedZone = true;
            ButtonActivatedZone = _buttonActivated[i];
            var buttonPressed = false;
            switch (ButtonActivatedZone.InputType)
            {
                case ButtonActivated.InputTypes.Default:
                    buttonPressed = _inputManager.InteractButton.State.CurrentState == MMInput.ButtonStates.ButtonDown;
                    break;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
					case ButtonActivated.InputTypes.Button:
					case ButtonActivated.InputTypes.Key:
						buttonPressed = ButtonActivatedZone.InputActionPerformed;
						break;
#else
                case ButtonActivated.InputTypes.Button:
                    buttonPressed = Input.GetButtonDown(_character.PlayerID + "_" + ButtonActivatedZone.InputButton);
                    break;
                case ButtonActivated.InputTypes.Key:
                    buttonPressed = Input.GetKeyDown(ButtonActivatedZone.InputKey);
                    break;
#endif
            }

            if (buttonPressed) ButtonActivation();
        }
    }
}

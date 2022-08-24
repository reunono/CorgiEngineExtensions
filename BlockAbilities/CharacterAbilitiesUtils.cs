using System;
using MoreMountains.CorgiEngine;

public static class CharacterAbilitiesUtils
{
    public static void PermitAbilities(Character character, CharacterAbilities[] abilities, bool permit = true)
    {
        foreach (var ability in abilities)
            switch (ability)
            {
                case CharacterAbilities.CharacterRun:
                    character.FindAbility<CharacterRun>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterDash:
                    character.FindAbility<CharacterDash>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterJump:
                    character.FindAbility<CharacterJump>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterMovement:
                    character.FindAbility<CharacterHorizontalMovement>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterSlopeOrientation:
                    character.FindAbility<CharacterSlopeOrientation>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterCrouch:
                    character.FindAbility<CharacterCrouch>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterSwap:
                    character.FindAbility<CharacterSwap>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterSwitchModel:
                    character.FindAbility<CharacterSwitchModel>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterButtonActivation:
                    character.FindAbility<CharacterButtonActivation>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterTimeControl:
                    character.FindAbility<CharacterTimeControl>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterInventory:
                    character.FindAbility<CharacterInventory>()?.PermitAbility(permit);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
    }
}

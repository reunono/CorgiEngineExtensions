using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
 
public class WeaponDisable : CharacterAbility {
 
    public override string HelpBoxText() { return "This component handles the disabling of weapons during both wall hanging and ladder climbing.  This is to create a more visually pleasing situation and should be supplimented in the animator with animations that include the weapon in the position the designer would like it to be in while clibming or wall hanging"; }
 
    private GameObject weapon;
    private GameObject weapon2;
    private GameObject weaponSpecial;
    private CharacterHandleWeapon weaponScript;
    private CharacterHandleSecondaryWeapon secondaryWeaponScript;
    private CharacterHandleSpecialWeapon specialWeaponScript;
 
    public bool secondaryWeapon;
    public bool specialWeapon;
 
    [HideInInspector]
    public bool usingSwitch;
 
    void Awake ()
    {
 
        weapon = transform.Find("WeaponAttachement").gameObject;
 
 
        if(secondaryWeapon)
        {
            weapon2 = transform.Find("WeaponAttachement2").gameObject;
        }
     
 
        if(specialWeapon)
        {
            weaponSpecial = transform.Find("WeaponAttachementSpecial").gameObject;
        }
 
        weaponScript = GetComponent<CharacterHandleWeapon>();
        secondaryWeaponScript = GetComponent<CharacterHandleSecondaryWeapon>();
        specialWeaponScript = GetComponent<CharacterHandleSpecialWeapon>();
 
        usingSwitch = false;
    }
 
    void Update ()
    {
        if(_movement.CurrentState == CharacterStates.MovementStates.LadderClimbing || _movement.CurrentState == CharacterStates.MovementStates.WallClinging || usingSwitch)
        {
            DisableNormalWeapons();
            DisableSpecialWeapons();
        }
        else
        {
            EnableNormalWeapons();
            EnableSpecialWeapons();
        }
    }
 
    public void EnableNormalWeapons ()
    {
        weapon.SetActive(true);
        weaponScript.enabled = true;
 
        if(secondaryWeapon && secondaryWeaponScript != null)
        {
            weapon2.SetActive(true);
            secondaryWeaponScript.enabled = true;
 
        }
    }
 
    public void EnableSpecialWeapons ()
    {
        if(specialWeapon && specialWeaponScript != null)
        {
            weaponSpecial.SetActive(true);
            specialWeaponScript.enabled = true;
        }
    }
 
    public void DisableNormalWeapons ()
    {
        weapon.SetActive(false);
        weaponScript.enabled = false;
 
        if(secondaryWeapon && secondaryWeaponScript != null)
        {
            weapon2.SetActive(false);
            secondaryWeaponScript.enabled = false;
        }
    }
 
    public void DisableSpecialWeapons ()
    {
        if(specialWeapon && specialWeaponScript != null)
        {
            weaponSpecial.SetActive(false);
            specialWeaponScript.enabled = false;
        }
    }
}
 
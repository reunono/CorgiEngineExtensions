using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.Events;

namespace MoreMountains.CorgiEngine
{	
	/// This script extends Key Operated Zone, you can now set any amount of item to be used, only conditions are: they have to be the same ID and non stackable.
	/// intended to use with Corgi Engine 5.3+
	/// V2.1 / Muppo (2018)

	public enum inventoryToLook { Main, Weapon, Hotbar }

	public class KeyAmountOperatedZone : KeyOperatedZone
	{
		[MMInformation("Does this need more than one key? Set the ammount here.\n" +
			"<b>Only keys of same ID</b> will work with this.",MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]

		/// How many keys are needed to open
		public int NumberOfKeys;
		/// In which inventory should we look for
		public inventoryToLook _inventoryToLook = inventoryToLook.Main;


		public override void TriggerButtonAction(GameObject instigator)
		{
			if (!CheckNumberOfUses())
				{ return; }

			if (_collidingObject == null)
				{ return; }

			if (RequiresKey)
			{
				CharacterInventory characterInventory = _collidingObject.gameObject.MMGetComponentNoAlloc<CharacterInventory> ();
				if (characterInventory == null)
					{ return; }	

				_keyList.Clear ();
				/// Search in the selected inventory
				if(_inventoryToLook == inventoryToLook.Main)
					_keyList = characterInventory.MainInventory.InventoryContains(KeyID);

				if(_inventoryToLook == inventoryToLook.Weapon)
					_keyList = characterInventory.WeaponInventory.InventoryContains(KeyID);

				if(_inventoryToLook == inventoryToLook.Hotbar)
					_keyList = characterInventory.HotbarInventory.InventoryContains(KeyID);

				// Check the key amount
				if (_keyList.Count == 0) {
					if (_buttonPromptAnimator != null) {
						_buttonPromptAnimator.SetTrigger("Error");
					}
                    return;
				}

				/// Check if we have one or more keys
				if (_keyList.Count >= 1)
				{
					// Check if the ammount is less than required. If so, do nothing
					if (_keyList.Count < NumberOfKeys)
					{
						if (_buttonPromptAnimator != null) {
							_buttonPromptAnimator.SetTrigger("Error");
						}
						return;
					}
					
					// But if the ammount is equal or higher than required ...
					if (_keyList.Count >= NumberOfKeys)
					{
						// We count how many times we need to use that kind of key ...
						for (int k = 1; k < NumberOfKeys + 1; k++)
						{
							// And repeat the method to use them from inventory as many times as necessary
							UseRequiredKeys();
						}
					}
				}
				else {
					base.TriggerButtonAction(instigator);
					UseRequiredKeys();
				}
			}

			TriggerKeyAction ();
			ActivateZone ();
		}

		/// Uses the required keys.
		protected virtual void UseRequiredKeys()
		{
			CharacterInventory characterInventory = _collidingObject.gameObject.MMGetComponentNoAlloc<CharacterInventory> ();
			if(_inventoryToLook == inventoryToLook.Main) {
				characterInventory.MainInventory.UseItem(KeyID); }
		
			if(_inventoryToLook == inventoryToLook.Weapon) {
				characterInventory.WeaponInventory.UseItem(KeyID); }

			if(_inventoryToLook == inventoryToLook.Hotbar) {
				characterInventory.HotbarInventory.UseItem(KeyID); }
		}

		/// Disables the key required
		public virtual void DisableKeyRequired()
		{
			if(RequiresKey)
				RequiresKey = false;
		}
	}
}

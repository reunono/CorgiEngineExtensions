using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.Events;

namespace MoreMountains.CorgiEngine
{	
	/// This script extends Key Operated Zone, you can now set any amount of item to be used, no matter if stackable or not, only condition is they have to be the same ID
	/// intended to use with Corgi Engine 4.2+
	/// V2.0 / Muppo (2018)

	public enum inventoryToLook { Main, Weapon, Hotbar }

	public class KeyAmountOperatedZone : KeyOperatedZone
	{
		[Information("Does this need more than one key? Set the ammount here.\n" +
			"<b>Only keys of same ID</b> will work with this.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]

		/// How many keys are needed to open
		public int NumberOfKeys = 1;
		/// In which inventory should we look for
		public inventoryToLook _inventoryToLook = inventoryToLook.Main;


		public override void TriggerButtonAction()
		{
			if (!CheckNumberOfUses()) { return; }

			if (_collidingObject == null) { return; }

			if (RequiresKey)
			{
				CharacterInventory characterInventory = _collidingObject.gameObject.GetComponentNoAlloc<CharacterInventory> ();
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

				if (_keyList.Count == 0)
				{
					return;
				}
				else
				{
					base.TriggerButtonAction ();			
					UseRequiredKeys();
				}
			}
			TriggerKeyAction ();
			ZoneActivated ();
		}

		/// Uses the required keys.
		protected virtual void UseRequiredKeys()
		{
			CharacterInventory characterInventory = _collidingObject.gameObject.GetComponentNoAlloc<CharacterInventory> ();

			if(_inventoryToLook == inventoryToLook.Main)
			{
				for (int k = 1; k < NumberOfKeys; k++)
				{ characterInventory.MainInventory.UseItem(KeyID); }
			}

			if(_inventoryToLook == inventoryToLook.Weapon)
			{
				for (int k = 1; k < NumberOfKeys; k++)
				{ characterInventory.WeaponInventory.UseItem(KeyID); }
			}

			if(_inventoryToLook == inventoryToLook.Hotbar)
			{
				for (int k = 1; k < NumberOfKeys; k++)
				{ characterInventory.HotbarInventory.UseItem(KeyID); }
			}
		}

		/// Disables the key required
		public virtual void DisableKeyRequired()
		{
			if(RequiresKey)
				RequiresKey = false;
		}
	}
}

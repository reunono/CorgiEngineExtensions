using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.Events;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// This script is based on Key Operated Zone and it's intended to use with Corgi Engine 4.2+
	/// This is free to use, no credit required.
	/// Muppo [ muppotronic@gmail.com ]

	/// Key Operated Zone Extended allows you to set an ammount of needed keys to open whatever you add this component,
	/// the only limitation is keys must not be stackable and have to be the same ID, that's it: one key per slot on inventory.

	/// Add this component to a collider 2D and you'll be able to have it perform an action when 
	/// a character equipped with the specified amount of keys enters it.
	/// </summary>
	public class KeyOperatedZoneExtended : KeyOperatedZone
	{
		[Information("Does this need more than one key? Set the ammount here.\n" +
			"<b>Only not stackable keys and same ID</b> will work with this.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// How many keys are needed to open
		public int NumberOfKeys = 1;


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
				_keyList = characterInventory.MainInventory.InventoryContains (KeyID);


				if (_keyList.Count == 0)
				{ return; }

				/// Check if we have one or more keys
				if (_keyList.Count >= 1)
				{
					// Check if the ammount is less than required. If so, do nothing
					if (_keyList.Count < NumberOfKeys)
					{ return; }
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
				else
				{
					base.TriggerButtonAction ();
					characterInventory.MainInventory.UseItem(KeyID);
				}
			}
			TriggerKeyAction ();
			ZoneActivated ();
		}
			
		protected virtual void UseRequiredKeys()
		{
			CharacterInventory characterInventory = _collidingObject.gameObject.GetComponentNoAlloc<CharacterInventory> ();
			characterInventory.MainInventory.UseItem(KeyID);
		}
	}
}

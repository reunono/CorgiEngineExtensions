using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	

	[RequireComponent(typeof(Collider2D))]

	/// <summary>
	/// This script allow you to create requesting items dialogues
	/// Intended to use with Corgi Engine 5.1+
	/// v2.0 / Muppo (2018)
	/// </summary>

	public class DialogueZoneItems : DialogueZone 
	{	
		/// There are several options for conditional dialogue, if none of these is active, dialogue works as usual.
		/// Final Dialogue will only happens when character fulfilled the action: receive an item or deliver it to other character.
		/// You can make the character give an item, request an item, keep it if requested or just wanted Player to have it and enable or disable objects,
		/// all this options let you make little quest which can improve variety on your game.

		[Space (10)]
		[Header("Item Options")]

		[Information("Does this character give any item? If this is checked and the item set, character will give it when the dialogue ends.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		public bool giveItem = false;
		public Transform ItemToGive;
		protected bool itemGiven = false;

		[Space (5)]
		[Information("Does this character request an item? If so don't forget to set the item ID and make it usable or the dialogue will never go forward.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		public bool needItem = false;
		public string itemID;
		protected bool itemReceived = false;

		[Space (5)]
		[Information("Does this character wants the item? If you want the character to keep it, check this. If you rather like Player to keep it, don't check this." +
			"\nIf you want this item to be special (not usable by player), check it and it will be stored on a hidden inventory.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		public bool deliverItem;
		public bool deliverSpecialItem;

		[Space (10)]
		[Information("Set the last dialogue when the request is finished. If you want some action to be done when Player gives the item, set it here.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		[Multiline]
		public string[] FinalDialogue;

		[Space (5)]
		public UnityEvent EndOfFinalDialogueAction;
		

		protected List<int> _itemList;
		protected List<int> _itemListSpecial;
		protected Collider2D _collidingObject;


		/// <summary>
		/// On Start we initialize our object
		protected virtual void Start()
		{
			_itemList = new List<int>();
			_itemListSpecial = new List<int>();
			if (ItemToGive != null)
			{
				ItemToGive.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// On enter we store our colliding object
		protected override void OnTriggerEnter2D(Collider2D collider)
		{
			_collidingObject = collider;
			base.OnTriggerEnter2D (collider);
		}	

		/// <summary>
		/// Plays the next dialogue in the queue
		protected override IEnumerator PlayNextDialogue()
		{	
			if (_dialogueBox == null) 
			{
				yield break;
			}

			// Check if the item is already given, if so we disable it
			if (itemGiven)
			{
				ItemToGive = null;
				giveItem = false;
			}

			// If a item is needed we call the dedicated coroutine
			if (needItem)
			{
				StartCoroutine(NeedItem());
			}

			if (_currentIndex != 0)
			{
				_dialogueBox.FadeOut(FadeDuration);	
				yield return _transitionTimeWFS;
			}	

			// if we've reached the last dialogue line, we exit
			if (_currentIndex >= Dialogue.Length)
			{
				if (giveItem)
				{
					if (ItemToGive != null)
					{
						ItemToGive.gameObject.SetActive(true);
						itemGiven = true;
					}
				}

				_currentIndex = 0;
				Destroy(_dialogueBox.gameObject);
				_collider.enabled = false;
				_activated = true;

				if (!CanMoveWhileTalking)
				{
					LevelManager.Instance.UnFreezeCharacters();
				}

				if ((_characterButtonActivation != null))
				{				
					_characterButtonActivation.InButtonActivatedZone = false;
					_characterButtonActivation.ButtonActivatedZone = null;
				}

				if (ActivableMoreThanOnce)
				{
					_activable = false;
					_playing = false;
					StartCoroutine(Reactivate());
				}
				else
				{
					gameObject.SetActive(false);
				}
				yield break;

			}

			if (_dialogueBox.DialogueText != null)
			{
				_dialogueBox.FadeIn(FadeDuration);
				_dialogueBox.DialogueText.text = Dialogue[_currentIndex];
			}

			_currentIndex++;

			if (!ButtonHandled)
			{
				StartCoroutine(AutoNextDialogue());
			}
		}

		/// <summary>
		/// Search in the Player inventory for the item
		protected virtual IEnumerator NeedItem()
		{
			// Check if the requested item is in any of the Player's inventories
			CharacterInventory characterInventory = _collidingObject.gameObject.GetComponentNoAlloc<CharacterInventory> ();
			_itemList.Clear();
			_itemListSpecial.Clear();
			_itemList = characterInventory.MainInventory.InventoryContains (itemID);
			_itemListSpecial = characterInventory.HotbarInventory.InventoryContains (itemID);

			// If it is on the main inventory...
			if (_itemList.Count >= 1)
			{
				// ...and the character wants it, it's used.
				if (deliverItem && !deliverSpecialItem)
				{
					characterInventory.MainInventory.UseItem(itemID);
				}

				// If there's some action to be done, we do it now.
				if (EndOfFinalDialogueAction != null)
				{
					EndOfFinalDialogueAction.Invoke();
				}

				itemReceived = true;
				needItem = false;
				Dialogue = FinalDialogue;
			}

			// If it is on the hidden inventory...
			if (_itemListSpecial.Count >= 1)
			{
				// ...and the character wants it, it's used.
				if (deliverSpecialItem)
				{
					characterInventory.HotbarInventory.UseItem(itemID);
				}

				// If there's some action to be done, we do it now.
				if (EndOfFinalDialogueAction != null)
				{
					EndOfFinalDialogueAction.Invoke();
				}

				itemReceived = true;
				needItem = false;
				Dialogue = FinalDialogue;
			}
			yield break;
		}
	}
}
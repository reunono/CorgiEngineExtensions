using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	

	[RequireComponent(typeof(Collider2D))]

	/// <summary>
	/// This script is based on Dialogue Zone and Key Operated Zone scripts and it's intended to use with Corgi Engine 4.2+
	/// This is free to use, no credit required. However if you improve it, sharing will be appreciated.
	/// Modifications by: Muppo [ muppotronic@gmail.com ]

	/// Dialogue Zone Items allows you to have item giving and request from dialogues.
	/// Add this class to an empty component, add a boxcollider2d, set it to "is trigger",
	/// then customize the dialogue zone and options through the inspector.
	/// </summary>

	public class DialogueZoneItems : ButtonActivated 
	{	
		[Header("Dialogue Look")]
		public Color TextBackgroundColor=Color.black;
		public Color TextColor=Color.white;
		public bool ArrowVisible=true;
		public Font TextFont;
		public int TextSize = 20;
		public TextAnchor Alignment = TextAnchor.MiddleCenter;

		[Header("Dialogue Speed (in seconds)")]
		public float FadeDuration=0.2f;
		public float TransitionTime=0.2f;

		[Header("Dialogue Position")]
		public float DistanceFromTop=0;

		[Header("Player Movement")]
		public bool CanMoveWhileTalking = true;
		[Header("Press button to go from one message to the next ?")]
		public bool ButtonHandled=true;
		[Header("Only if the dialogue is not button handled :")]
		[Range (1, 100)]
		public float MessageDuration=3f;

		[Header("Activations")]
		public bool ActivableMoreThanOnce=true;
		[Range (1, 100)]
		public float InactiveTime=2f;

		[Space (10)]

		/// There are several options for conditional dialogue, if none of these is active, dialogue works as usual.
		/// Final Dialogue will only happens when character fulfilled the action: receive an item or deliver it to other character.
		/// You can make the character give an item, request an item, keep it if requested or just wanted Player to have it and enable or disable objects,
		/// all this options let you make little quest wich can improve variety on your game.


		[Header("Dialogues Options")]

		[Information("Does this character give any item? If this is checked and the item set, character will give it when the dialogue ends. Item must be inactive in hierarchy in order to work the right way, otherwise it well be available from start.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		public bool giveItem = false;
		public Transform ItemToGive;
		protected bool itemGiven = false;

		[Space (5)]
		[Information("Does this character request an item? If so don't forget to set the item ID and make it usable or the dialogue will never go forward.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		public bool needItem = false;
		public string itemID;
		protected bool itemReceived = false;

		[Space (5)]
		[Information("Does this character wants the item? If you want the character to keep it, check this. If you rather like Player to keep it, don't check this.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		public bool useItem;

		[Space (5)]
		[Information("If you want to make anything active / inactive when Player gets the item, use this and set whatever you want to be toggled.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		
		public bool toggle = false;
		public Transform ObjectToToggle;

		protected List<int> _itemList;
		protected Collider2D _collidingObject;

		[Space (10)]

		[Multiline]
		public string[] Dialogue;

		[Space (10)]

		[Multiline]
		public string[] FinalDialogue;

		/// private variables
		protected DialogueBox _dialogueBox;
		protected bool _activated=false;
		protected bool _playing=false;
		protected int _currentIndex;
		protected bool _activable=true;
		protected WaitForSeconds _transitionTimeWFS;
		protected WaitForSeconds _messageDurationWFS;
		protected WaitForSeconds _inactiveTimeWFS;

		/// <summary>
		/// On Start we initialize our object
		/// </summary>
		protected virtual void Start()
		{
			_itemList = new List<int> ();
		}

		/// <summary>
		/// On enter we store our colliding object
		/// </summary>
		/// <param name="collider">Something colliding with the water.</param>
		protected override void OnTriggerEnter2D(Collider2D collider)
		{
			_collidingObject = collider;
			base.OnTriggerEnter2D (collider);
		}	

		/// <summary>
		/// Determines whether this instance can show button prompt.
		/// </summary>
		/// <returns><c>true</c> if this instance can show prompt; otherwise, <c>false</c>.</returns>
		public override bool CanShowPrompt()
		{
			if ( (_buttonA==null) && _activable && !_playing )
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Initializes the dialogue zone
		/// </summary>
		protected override void OnEnable () 
		{		
			base.OnEnable();
			_currentIndex=0;
			_transitionTimeWFS = new WaitForSeconds (TransitionTime);
			_messageDurationWFS = new WaitForSeconds (MessageDuration);
			_inactiveTimeWFS = new WaitForSeconds (InactiveTime);
		}

		/// <summary>
		/// When the button is pressed we start the dialogue
		/// </summary>
		public override void TriggerButtonAction()
		{
			if (!CheckNumberOfUses())
			{
				return;
			}
			if (_playing && !ButtonHandled)
			{
				return;
			}
			base.TriggerButtonAction ();
			StartDialogue ();
			ZoneActivated ();
		}

		/// <summary>
		/// When triggered, either by button press or simply entering the zone, starts the dialogue
		/// </summary>
		public virtual void StartDialogue()
		{
			if (_buttonA!=null)
				Destroy(_buttonA);

			if (_collider==null)
				return;	

			if (_activated && !ActivableMoreThanOnce)
				return;

			if (!_activable)
				return;

			if (!CanMoveWhileTalking)
			{
				LevelManager.Instance.FreezeCharacters();
				if (ShouldUpdateState)
				{
					_characterButtonActivation.GetComponent<Character>().MovementState.ChangeState(CharacterStates.MovementStates.Idle);
				}
			}

			if (!_playing)
			{	
				GameObject dialogueObject = (GameObject)Instantiate(Resources.Load("GUI/DialogueBox"));
				_dialogueBox = dialogueObject.GetComponent<DialogueBox>();		
				_dialogueBox.transform.position=new Vector2(_collider.bounds.center.x,_collider.bounds.max.y+DistanceFromTop); 
				_dialogueBox.ChangeColor(TextBackgroundColor,TextColor);
				_dialogueBox.ButtonActive(ButtonHandled);

				if (TextFont != null)
				{
					_dialogueBox.DialogueText.font = TextFont;
				}
				if (TextSize != 0)
				{
					_dialogueBox.DialogueText.fontSize = TextSize;
				}
				_dialogueBox.DialogueText.alignment = Alignment;

				if (!ArrowVisible)
				{
					_dialogueBox.HideArrow();			
				}

				_playing=true;
			}
			StartCoroutine(PlayNextDialogue());
		}

		/// <summary>
		/// Plays the next dialogue in the queue
		/// </summary>
		protected virtual IEnumerator PlayNextDialogue()
		{	
			if (_dialogueBox == null) 
			{
				yield break;
			}

			// Check if the item is already given, if so we change the dialogue
			if (giveItem && itemGiven)
			{
				if (toggle)
				{
					ObjectToToggle.gameObject.SetActive(!toggle);
				}
				giveItem = false;
				Dialogue = FinalDialogue;
			}

			// If a item is needed we call the dedicated coroutine
			if (needItem)
			{
				StartCoroutine(NeedItem());
			}

			if (_currentIndex!=0)
			{
				_dialogueBox.FadeOut(FadeDuration);	
				yield return _transitionTimeWFS;
			}	

			// if we've reached the last dialogue line, we exit
			if (_currentIndex>=Dialogue.Length)
			{
				if (giveItem)
				{
					if (ItemToGive != null)
					{
						ItemToGive.gameObject.SetActive(true);
						itemGiven=true;
					}
				}

				_currentIndex=0;
				Destroy(_dialogueBox.gameObject);
				_collider.enabled=false;
				_activated=true;

				if (!CanMoveWhileTalking)
				{
					LevelManager.Instance.UnFreezeCharacters();
				}

				if ((_characterButtonActivation!=null))
				{				
					_characterButtonActivation.InButtonActivatedZone=false;
					_characterButtonActivation.ButtonActivatedZone=null;
				}

				if (ActivableMoreThanOnce)
				{
					_activable=false;
					_playing=false;
					StartCoroutine(Reactivate());
				}
				else
				{
					gameObject.SetActive(false);
				}
				yield break;

			}

			if (_dialogueBox.DialogueText!=null)
			{
				_dialogueBox.FadeIn(FadeDuration);
				_dialogueBox.DialogueText.text=Dialogue[_currentIndex];
			}

			_currentIndex++;

			if (!ButtonHandled)
			{
				StartCoroutine(AutoNextDialogue());
			}
		}

		/// <summary>
		/// Automatically goes to the next dialogue line
		/// </summary>
		/// <returns>The next dialogue.</returns>
		protected virtual IEnumerator AutoNextDialogue()
		{
			yield return _messageDurationWFS;
			StartCoroutine(PlayNextDialogue());
		}

		/// <summary>
		/// Reactivate the dialogue zone
		/// </summary>
		protected virtual IEnumerator Reactivate()
		{
			yield return _inactiveTimeWFS;
			_collider.enabled=true;
			_activable=true;
			_playing=false;
			_currentIndex=0;

			if (AlwaysShowPrompt)
			{
				ShowPrompt();
			}
		}

		/// <summary>
		/// Search in the Player inventory for the item
		/// </summary>
		protected virtual IEnumerator NeedItem()
		{
			// Check if the requested item is in Player's inventory
			CharacterInventory characterInventory = _collidingObject.gameObject.GetComponentNoAlloc<CharacterInventory> ();
			_itemList.Clear ();
			_itemList = characterInventory.MainInventory.InventoryContains (itemID);
			// If it is...
			if (_itemList.Count >= 1)
			{
				// ...and the character wants it, it's used.
				if (useItem)
				{
					characterInventory.MainInventory.UseItem(itemID);

				}
				// If we have something to toggle, we do it now.	
				if (toggle)
				{
					ObjectToToggle.gameObject.SetActive(!toggle);
				}

				itemReceived = true;
				needItem = false;
				Dialogue = FinalDialogue;
			}
			yield break;
		}
	}
}
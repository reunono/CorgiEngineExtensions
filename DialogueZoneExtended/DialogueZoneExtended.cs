using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	

	public class DialogueZoneExtended : DialogueZone {


		/// <summary>
		///***This script free to use and no credit is required. 
		///***This is intended to be used with More Mountain's Corgi Engine 4.2+
		///***Extension Written by: Keith Henderson. Any questions can be sent to keith.donaldh@gmail.com3
		///***This script requires that you also use the AIWalkExtended.cs on your AI character to replace AIWalk.cs
		/// This will allow your AI to patrol and then stop to speak to the player, and then continue on a regular patrol.
		/// You can still use the regular walk on sight if you so choose to.
		/// Dialogue Zone colliders can be set up in front of the AI and any dialogue or prompts will still display relative to the AI sprite and not the dialogue zone collider itself.
		/// </summary>

		//Private Stuff
		private bool flipCheck;
		private bool Patrol;
		private Vector3 spriteCollider;
		private Vector2 _direction;


		public virtual void Awake()
		{
			//We check to see if character is facing right.
			flipCheck = gameObject.GetComponentInParent<Character> ().IsFacingRight;

			//If the initial direction is not right we invert the collider offset so it flips properly
			if(!flipCheck){
				gameObject.GetComponent<BoxCollider2D> ().offset = gameObject.GetComponent<BoxCollider2D> ().offset * -1;
			}

			//We set the direction
			_direction = gameObject.transform.localScale;

			//We check to see if AI walk behaviour is set to Patrol or not
			if (gameObject.GetComponentInParent<AIWalkExtended> ().WalkBehaviour == AIWalkExtended.WalkBehaviours.Patrol)
				Patrol = true;
			else
				Patrol = false;
				
		}

		//Every frame we check to see if the AI has changed direction and flip if needed.
		public virtual void Update()
		{

		//We check every frame to see which way the character is facing
		flipCheck = gameObject.GetComponentInParent<Character> ().IsFacingRight;

		//We flip the dialogue collider based on which direction the AI is walking
		if (!flipCheck) {
				_direction.x = (-1);
				gameObject.transform.localScale = (_direction);
			}
			else
				{
				_direction.x = (1);
				gameObject.transform.localScale = (_direction);
			}

		}

		/// <summary>
		/// Shows the button A prompt.
		/// </summary>
		/// Override for showprompt to show it relative to parent sprite.
		public override void ShowPrompt()
		{
			if (_promptHiddenForever)
			{
				return;
			}
			// we add a blinking A prompt to the top of the zone
			_buttonA = (GameObject)Instantiate(Resources.Load("GUI/ButtonA"));	

			//We get the transform of the parent sprite to ensure the prompt is above the sprite
			_buttonA.transform.position = gameObject.GetComponentInParent<BoxCollider2D> ().transform.position + PromptRelativePosition; 
			_buttonA.transform.parent = transform;
			_buttonA.GetComponent<SpriteRenderer>().material.color=new Color(1f,1f,1f,0f);
			StartCoroutine(MMFade.FadeSprite(_buttonA.GetComponent<SpriteRenderer>(),0.2f,new Color(1f,1f,1f,1f)));	
		}

		public override void StartDialogue()
		{
			// if the button A prompt is displayed, we hide it
			if (_buttonA!=null)
				Destroy(_buttonA);

			// if the dialogue zone has no box collider, we do nothing and exit
			if (_collider==null)
				return;	

			// if the zone has already been activated and can't be activated more than once.
			if (_activated && !ActivableMoreThanOnce)
				return;

			// if the zone is not activable, we do nothing and exit
			if (!_activable)
				return;
			
			//If the AI is set to patrol then we stop it when dialogue is activated
			if(Patrol)
				gameObject.GetComponentInParent<AIWalkExtended> ().WalkBehaviour = AIWalkExtended.WalkBehaviours.MoveOnSight;

			// if the player can't move while talking, we notify the game manager
			if (!CanMoveWhileTalking)
			{
				LevelManager.Instance.FreezeCharacters();
				if (ShouldUpdateState)
				{
					_characterButtonActivation.GetComponent<Character>().MovementState.ChangeState(CharacterStates.MovementStates.Idle);
				}
			}

			// if it's not already playing, we'll initialize the dialogue box
			if (!_playing)
			{	
				// we instantiate the dialogue box
				GameObject dialogueObject = (GameObject)Instantiate(Resources.Load("GUI/DialogueBox"));

				_dialogueBox = dialogueObject.GetComponent<DialogueBox>();	

				// we set the dialogue box position relative to the parent sprite
				spriteCollider = gameObject.GetComponentInParent<BoxCollider2D> ().transform.position;
				_dialogueBox.transform.position=new Vector2(spriteCollider.x,spriteCollider.y+DistanceFromTop); 

				// we set the color's and background's colors
				_dialogueBox.ChangeColor(TextBackgroundColor,TextColor);
				// if it's a button handled dialogue, we turn the A prompt on
				_dialogueBox.ButtonActive(ButtonHandled);
				// if font settings have been specified, we set them
				if (TextFont != null)
				{
					_dialogueBox.DialogueText.font = TextFont;
				}
				if (TextSize != 0)
				{
					_dialogueBox.DialogueText.fontSize = TextSize;
				}
				_dialogueBox.DialogueText.alignment = Alignment;

				// if we don't want to show the arrow, we tell that to the dialogue box
				if (!ArrowVisible)
				{
					_dialogueBox.HideArrow();			
				}

				// the dialogue is now playing
				_playing=true;
			}
			// we start the next dialogue
			StartCoroutine(PlayNextDialogue());
		}

		/// <summary>
		/// Plays the next dialogue in the queue
		/// </summary>
		protected override IEnumerator PlayNextDialogue()
		{		
			// we check that the dialogue box still exists
			if (_dialogueBox == null) 
			{
				yield break;
			}
			// if this is not the first message
			if (_currentIndex!=0)
			{
				// we turn the message off
				_dialogueBox.FadeOut(FadeDuration);	
				// we wait for the specified transition time before playing the next dialogue
				yield return _transitionTimeWFS;
			}	

			// if we've reached the last dialogue line, we exit
			if (_currentIndex>=Dialogue.Length)
			{
				_currentIndex=0;
				Destroy(_dialogueBox.gameObject);
				_collider.enabled=false;
				// we set activated to true as the dialogue zone has now been turned on		
				_activated=true;



				//End of dialogue, start walking again if AI Walk Behaviour is set to patrol.
				if (Patrol) {
					gameObject.GetComponentInParent<AIWalkExtended> ().WalkBehaviour = AIWalkExtended.WalkBehaviours.Patrol;
					gameObject.GetComponentInParent<AIWalkExtended> ().Reset ();
				}



				// we let the player move again
				if (!CanMoveWhileTalking)
				{
					LevelManager.Instance.UnFreezeCharacters();
				}
				if ((_characterButtonActivation!=null))
				{				
					_characterButtonActivation.InButtonActivatedZone=false;
					_characterButtonActivation.ButtonActivatedZone=null;
				}
				// we turn the zone inactive for a while
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

			// we check that the dialogue box still exists
			if (_dialogueBox.DialogueText!=null)
			{
				// every dialogue box starts with it fading in
				_dialogueBox.FadeIn(FadeDuration);
				// then we set the box's text with the current dialogue
				_dialogueBox.DialogueText.text=Dialogue[_currentIndex];
			}

			_currentIndex++;

			// if the zone is not button handled, we start a coroutine to autoplay the next dialogue
			if (!ButtonHandled)
			{
				StartCoroutine(AutoNextDialogue());
			}
		}

}
}

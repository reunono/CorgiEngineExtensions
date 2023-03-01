using System;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

/// <summary>
/// Only works with InputSystem!
/// This component is activated by rotation input from the player, via CharacterRotationalActivation component.
/// It can be easily used as a slider or any incremental-type input, since it just receives a float as activation (i.e. how much the player has done).
/// It is mostly a duplicate of ButtonActivated.
/// </summary>
	[MMRequiresConstantRepaint]
    [RequireComponent(typeof(Collider2D))]
    public class RotationActivated : MMMonoBehaviour
    {
	    public enum RotationType { Clockwise, CounterClockWise, BiDirectional}
	    public float TotalRotation { get => _totalRotation; protected set => _totalRotation = value; }
	    
	    [MMInspectorGroup("Activation", true, 10)]
	    [Tooltip("Does this control spin in one direction or both")]
	     
	    [SerializeField]
	    public float _totalRotation;
	    public RotationType rotationType;

	    /// if this is false, the zone won't be activable 
	    [Tooltip("if this is false, the zone won't be activable ")]
	    public bool Activable = true;
	    [Tooltip("if true, the zone will activate whether the button is pressed or not")]
	    public bool AutoActivation = false;
	    [Tooltip("if this is true, enter won't be retriggered if another object enters, and exit will only be triggered when the last object exits")]
	    public bool OnlyOneActivationAtOnce;
	    /// if true, this zone will be auto activated but will still allow button interaction
	    [Tooltip("if true, this zone will be auto activated but will still allow button interaction")]
	    [MMCondition("AutoActivation", true)]
	    public bool AutoActivationAndButtonInteraction = false;
	    /// a layermask with all the layers that can interact with this specific button activated zone
	    [Tooltip("a layermask with all the layers that can interact with this specific button activated zone")]
	    public LayerMask TargetLayerMask = ~0;

	    
        [MMInspectorGroup("Animation", true, 14)]

		/// an (absolutely optional) animation parameter that can be triggered on the character when activating the zone
		[Tooltip("an (absolutely optional) animation parameter that can be triggered on the character when activating the zone")]
		public string AnimationTriggerParameterName;

		[MMInspectorGroup("Visual Prompt", true, 15)]
		[MMInformation("You can have this zone show a visual prompt to indicate to the player that it's interactable.", MoreMountains.Tools.MMInformationAttribute.InformationType.Info, false)]

		///TODO edit the prompt UI for RotationActivated device

		[Tooltip("if this is true, a prompt will be shown if setup properly")]
		public bool UseVisualPrompt = true;
		/// the gameobject to instantiate to present the prompt
		[MMCondition("UseVisualPrompt", true)]
		[Tooltip("the gameobject to instantiate to present the prompt")]
		public ButtonPrompt ButtonPromptPrefab;
		/// the text to display in the button prompt
		[MMCondition("UseVisualPrompt", true)]
		[Tooltip("the text to display in the button prompt")]
		public string ButtonPromptText = "A";
		/// the text to display in the button prompt
		[MMCondition("UseVisualPrompt", true)]
		[Tooltip("the text to display in the button prompt")]
		public Color ButtonPromptColor = MMColors.LawnGreen;
		/// the color for the prompt's text
		[MMCondition("UseVisualPrompt", true)]
		[Tooltip("the color for the prompt's text")]
		public Color ButtonPromptTextColor = MMColors.White;
		/// If true, the "buttonA" prompt will always be shown, whether the player is in the zone or not.
		[MMCondition("UseVisualPrompt", true)]
		[Tooltip("If true, the 'buttonA' prompt will always be shown, whether the player is in the zone or not.")]
		public bool AlwaysShowPrompt = true;
		/// If true, the "buttonA" prompt will be shown when a player is colliding with the zone
		[MMCondition("UseVisualPrompt", true)]
		[Tooltip("If true, the 'buttonA' prompt will be shown when a player is colliding with the zone")]
		public bool ShowPromptWhenColliding = true;
		/// If true, the prompt will hide after use
		[MMCondition("UseVisualPrompt", true)]
		[Tooltip("If true, the prompt will hide after use")]
		public bool HidePromptAfterUse = false;
		/// the position of the actual buttonA prompt relative to the object's center
		[MMCondition("UseVisualPrompt", true)]
		[Tooltip("the position of the actual buttonA prompt relative to the object's center")]
		public Vector3 PromptRelativePosition = Vector3.zero;

		[MMInspectorGroup("Feedbacks", true, 16)]

		/// a feedback to play when the zone gets activated
		[Tooltip("a feedback to play when the zone gets activated")]
		public MMFeedbacks ActivationFeedback;
		/// a feedback to play when the zone tries to get activated but can't
		[Tooltip("a feedback to play when the zone tries to get activated but can't")]
		public MMFeedbacks DeniedFeedback;
		/// a feedback to play when the zone gets entered
		[Tooltip("a feedback to play when the zone gets entered")]
		public MMFeedbacks EnterFeedback;
		/// a feedback to play when the zone gets exited
		[Tooltip("a feedback to play when the zone gets exited")]
		public MMFeedbacks ExitFeedback;

		[MMInspectorGroup("Actions", true, 17)]

		/// an action to trigger when this gets activated
		[Tooltip("an action to trigger when this gets activated")]
		public UnityEvent<float, float> OnActivation;
		/// an action to trigger when exiting this zone
		[Tooltip("an action to trigger when exiting this zone")]
		public UnityEvent OnExit;
		/// an action to trigger when staying in the zone
		[Tooltip("an action to trigger when staying in the zone")]
		public UnityEvent OnStay;

		
		protected Animator _rotationPromptAnimator;
		//TODO: replace ButtonPrompt class with appropriate rotational class.
		protected ButtonPrompt _rotationPrompt;
		protected bool _promptHiddenForever = false;
		protected float _lastActivationTimestamp;
		protected Collider2D _rotationActivatedZoneCollider;
		
		
		protected CharacterRotationalActivation _characterRotationalActivation;
		protected Character _currentCharacter;

		protected List<GameObject> _collidingObjects;
		protected List<GameObject> _stayingGameObjects;
		protected List<Collider2D> _enteredColliders;


		protected virtual void OnEnable()
		{
			Initialization();
		}

		public virtual void Initialization()
		{
			_collidingObjects = new List<GameObject>();
			_enteredColliders = new List<Collider2D>();
			_stayingGameObjects = new List<GameObject>();
			_rotationActivatedZoneCollider = this.gameObject.GetComponent<Collider2D>();

			if (AlwaysShowPrompt)
			{
				ShowPrompt();
			}

			ActivationFeedback?.Initialization(this.gameObject);
			DeniedFeedback?.Initialization(this.gameObject);
			EnterFeedback?.Initialization(this.gameObject);
			ExitFeedback?.Initialization(this.gameObject);
			
			
		}
		
		/// <summary>
		/// Makes the zone activable
		/// </summary>
		public virtual void MakeActivable()
		{
			Activable = true;
		}

		/// <summary>
		/// Makes the zone unactivable
		/// </summary>
		public virtual void MakeUnactivable()
		{
			Activable = false;
		}
		
		/// <summary>
		/// Makes the zone activable if it wasn't, unactivable if it was activable.
		/// </summary>
		public virtual void ToggleActivable()
		{
			Activable = !Activable;
		}
		
	
		/// <summary>
		/// When the input button is pressed, we check whether or not the zone can be activated, and if yes, trigger ZoneActivated
		/// </summary>
		public virtual void TriggerRotationAction(GameObject instigator, float angle)
		{
			if (!Activable)
			{
				PromptError();
				return;
			}

			_stayingGameObjects.Add(instigator);
			ActivateZone(angle);
		}

		
		/// <summary>
		/// On exit, we reset our staying bool and invoke our OnExit event
		/// </summary>
		/// <param name="collider"></param>
		public virtual void TriggerExitAction(GameObject collider)
		{
			_stayingGameObjects.Remove(collider);
			if (OnExit != null)
			{
				OnExit.Invoke();
			}
		}
		
		
		/// <summary>
		/// Activates the zone
		/// </summary>
		protected virtual void ActivateZone(float angle)
		{
			float angleDirectional = angle;
			switch (rotationType)
			{
				case(RotationType.Clockwise):
					angleDirectional = Mathf.Clamp(angle,0,180f);
					break;
				case(RotationType.CounterClockWise):
					angleDirectional = Mathf.Clamp(angle, -180f, 0);
					break;
				default:
					break;
			}

			TotalRotation += angleDirectional;
			
			if (OnActivation != null)
			{
				OnActivation.Invoke(angleDirectional, TotalRotation);
			}
			_lastActivationTimestamp = Time.time;
			if (HidePromptAfterUse)
			{
				_promptHiddenForever = true;
				HidePrompt();
			}

			ActivationFeedback?.PlayFeedbacks(this.transform.position);
		}

		/// <summary>
		/// Triggers an error 
		/// </summary>
		public virtual void PromptError()
		{
			if (_rotationPromptAnimator != null)
			{
				_rotationPromptAnimator.SetTrigger("Error");
			}
			DeniedFeedback?.PlayFeedbacks(this.transform.position);
		}
		
		/// <summary>
		/// Shows the button A prompt.
		/// </summary>
		public virtual void ShowPrompt()
		{            
			if (!UseVisualPrompt || _promptHiddenForever || (ButtonPromptPrefab == null))
			{
				return;
			}

			// we add a blinking A prompt to the top of the zone
			if (_rotationPrompt == null)
			{
				_rotationPrompt = (ButtonPrompt)Instantiate(ButtonPromptPrefab);
				_rotationPrompt.Initialization();
				_rotationPromptAnimator = _rotationPrompt.gameObject.MMGetComponentNoAlloc<Animator>();
			}

			if (_rotationActivatedZoneCollider != null)
			{
				_rotationPrompt.transform.position = _rotationActivatedZoneCollider.bounds.center + PromptRelativePosition;
			}
			_rotationPrompt.transform.parent = transform;
			_rotationPrompt.SetText(ButtonPromptText);
			_rotationPrompt.SetBackgroundColor(ButtonPromptColor);
			_rotationPrompt.SetTextColor(ButtonPromptTextColor);
			_rotationPrompt.Show();
		}
		
		/// <summary>
		/// Hides the button A prompt.
		/// </summary>
		public virtual void HidePrompt()
		{
			if ((_rotationPrompt != null) && (_rotationPrompt.isActiveAndEnabled))
			{
				_rotationPrompt.Hide();
			}            
		}
		
		/// <summary>
		/// Enables the button activated zone
		/// </summary>
		public virtual void DisableZone()
		{
			Activable = false;
			_rotationActivatedZoneCollider.enabled = false;
			if (_characterRotationalActivation != null)
			{
				_characterRotationalActivation.InRotationActivatedZone = false;
				_characterRotationalActivation.RotationActivatedZone = null;
			}
		}
		
		/// <summary>
		/// Handles enter collision with 2D triggers
		/// </summary>
		/// <param name="collidingObject">Colliding object.</param>
		protected virtual void OnTriggerEnter2D(Collider2D collidingObject)
		{
			_enteredColliders.Add(collidingObject);
			TriggerEnter(collidingObject.gameObject);
		}
		
		
		
		/// <summary>
		/// On stay we invoke our stay event if needed, and perform a trigger enter check if it hasn't been done already
		/// </summary>
		/// <param name="collidingObject"></param>
		protected virtual void OnTriggerStay2D(Collider2D collidingObject)
		{
			if (!_enteredColliders.Contains(collidingObject))
			{
				return;
			}

			bool staying = _stayingGameObjects.Contains(collidingObject.gameObject);

			if (staying && (OnStay != null))
			{
				OnStay.Invoke();
			}
        
			if (staying)
			{
				return;
			}
			TriggerEnter(collidingObject.gameObject);
		}
		
		
		/// <summary>
		/// Handles enter collision with 2D triggers
		/// </summary>
		/// <param name="collidingObject">Colliding object.</param>
		protected virtual void OnTriggerExit2D(Collider2D collidingObject)
		{
			_enteredColliders.Remove(collidingObject);
			TriggerExit(collidingObject.gameObject);
		}
		
		
		/// <summary>
		/// Triggered when something collides with the button activated zone
		/// </summary>
		/// <param name="collider">Something colliding with the water.</param>
		protected virtual void TriggerEnter(GameObject collider)
		{    
           
			// at this point the object is colliding and authorized, we add it to our list
			_collidingObjects.Add(collider.gameObject);
			if (!TestForLastObject(collider))
			{
				return;
			}

			_currentCharacter = collider.gameObject.MMGetComponentNoAlloc<Character>();


			_characterRotationalActivation = _currentCharacter?.FindAbility<CharacterRotationalActivation>();
			if (_characterRotationalActivation != null)
			{
				_characterRotationalActivation.InRotationActivatedZone = true;
				_characterRotationalActivation.RotationActivatedZone = this;
				_characterRotationalActivation.InButtonAutoActivatedZone = AutoActivation;
				_characterRotationalActivation.SetTriggerParameter();
			}
			EnterFeedback?.PlayFeedbacks(this.transform.position);


			// if we're not already showing the prompt and if the zone can be activated, we show it
			if (ShowPromptWhenColliding)
			{
				ShowPrompt();
			}
		}
		
		
		/// <summary>
		/// Triggered when something exits the activation zone
		/// </summary>
		/// <param name="collider">Something colliding with the activation zone.</param>
		protected virtual void TriggerExit(GameObject collider)
		{

			_collidingObjects.Remove(collider.gameObject);
			if (!TestForLastObject(collider))
			{
				return;
			}

			_currentCharacter = null;

			_characterRotationalActivation = collider.gameObject.MMGetComponentNoAlloc<Character>()?.FindAbility<CharacterRotationalActivation>();
			if (_characterRotationalActivation != null)
			{
				_characterRotationalActivation.InRotationActivatedZone = false;
				_characterRotationalActivation.RotationActivatedZone= null;
				_characterRotationalActivation.InButtonAutoActivatedZone = false;
				_characterRotationalActivation.InJumpPreventingZone = false;
			}
		

			ExitFeedback?.PlayFeedbacks(this.transform.position);

			if ((_rotationPrompt != null) && !AlwaysShowPrompt)
			{
				HidePrompt();
			}

			TriggerExitAction(collider);
		}
		
		
		/// <summary>
		/// Tests if the object exiting our zone is the last remaining one
		/// </summary>
		/// <param name="collider"></param>
		/// <returns></returns>
		protected virtual bool TestForLastObject(GameObject collider)
		{
			if (OnlyOneActivationAtOnce)
			{
				if (_collidingObjects.Count > 0)
				{
					bool lastObject = true;
					foreach (GameObject obj in _collidingObjects)
					{
						if ((obj != null) && (obj != collider))
						{
							lastObject = false;
						}
					}
					return lastObject;
				}
			}
			return true;
		}
    }
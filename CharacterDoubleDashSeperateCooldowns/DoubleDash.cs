using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;

{	
	/// <summary>
	/// Add this class to a character and it'll be able to perform a horizontal dash
	/// Animator parameters : Dashing
	/// </summary>
	[AddComponentMenu("Character/Abilities/DogBox Character Dash")] 
	public class DoubleDash : MoreMountains.CorgiEngine.CharacterAbility
	{		
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component allows your character to dash. Here you can define the distance the dash should cover, " +
		                                              "how much force to apply during the dash (which impacts its duration), whether forces should be reset on dash exit (otherwise inertia will apply)." +
		                                              "Then you can define how to pick the dash's direction, whether or not the character should be automatically flipped to match the dash's direction, and " +
		                                              "whether or not you want to correct the trajectory to prevent grounded characters to not dash if the input was slightly wrong." +
		                                              "And finally you can tweak the cooldown between the end of a dash and the start of the next one."; }

		[Header("Dash")]

		/// the distance this dash should cover
		[Tooltip("the distance this dash should cover")]
		public float DashDistance = 3f;
		/// the force of the dash
		[Tooltip("the force of the dash")]
		public float DashForce = 40f;
		/// if this is true, forces will be reset on dash exit (killing inertia)
		[Tooltip("if this is true, forces will be reset on dash exit (killing inertia)")]
		public bool ResetForcesOnExit = false;
		/// if this is true, position will be forced on exit to match an exact distance
		[Tooltip("if this is true, position will be forced on exit to match an exact distance")]
		public bool ForceExactDistance = false;

		[Header("Direction")]

		/// the dash's aim properties
		[Tooltip("the dash's aim properties")]
		public MMAim Aim;
		/// the minimum amount of input required to apply a direction to the dash
		[Tooltip("the minimum amount of input required to apply a direction to the dash")]
		public float MinimumInputThreshold = 0.1f;
		/// if this is true, the character will flip when dashing and facing the dash's opposite direction
		[Tooltip("if this is true, the character will flip when dashing and facing the dash's opposite direction")]
		public bool FlipCharacterIfNeeded = true;
		/// if this is true, will prevent the character from dashing into the ground when already grounded
		[Tooltip("if this is true, will prevent the character from dashing into the ground when already grounded")]
		public bool AutoCorrectTrajectory = true;

		public enum SuccessiveDashResetMethods { Grounded, Time }

		[Header("Cooldown")]
		/// the duration of the cooldown after a dash
		[Tooltip("the cooldown in between two dashes - so you cannot virtually perform a giga dash")]
		public float DashCooldown = 1f;
		[Tooltip("the cooldown time for each dash to be ready again")]
		/// cooldown time for each dash
		public float DashResetTime = 4f;
		[Header("Uses")]
		/// whether the character can perform a double dash. false if not allowed
		public bool DoubleDash = true;

		[Header("Damage")] 
		/// if this is true, this character won't receive any damage while a dash is in progress
		[Tooltip("if this is true, this character won't receive any damage while a dash is in progress")]
		public bool InvincibleWhileDashing = false; 
		[Tooltip("duration of the invincibility so the character wont get damaged immediately after ending the das")]
		public float InvincibilityWhileDashingDuration = 1f;

		/// Dogbox Dash with two different cooldowns 

		[Header("Feedbacks")]
		/// tweening the ui elements and playing ui sounds
		[Tooltip("Feedback Components for the Non-Characterfeedbacks - UI, etc.")]

		[SerializeField]
		private MMF_Player DashOneStarted;
		[SerializeField]
		private MMF_Player DashOneReady;

		[SerializeField]
		private MMF_Player DashTwoStarted;
		[SerializeField]
		private MMF_Player DashTwoReady;

		protected bool _dashOneReady = true;

		protected bool _dashTwoReady = true;

		protected float _cooldownTimeStamp = 0;
		protected float _startTime ;
		protected Vector2 _initialPosition ;
		protected Vector2 _dashDirection;
		protected float _distanceTraveled = 0f;
		protected bool _shouldKeepDashing = true;
		protected float _slopeAngleSave = 0f;
		protected bool _dashEndedNaturally = true;
		protected IEnumerator _dashCoroutine;
		protected CharacterDive _characterDive;
		protected float _lastDashAt = 0f;
		protected float _lastDashOneAt = 0f;
		protected float _lastDashTwoAt = 0f;
		protected float _averageDistancePerFrame;
		protected int _startFrame;

		// animation parameters
		protected const string _dashingAnimationParameterName = "Dashing";
		protected int _dashingAnimationParameter;

		/// <summary>
		/// Initializes our aim instance
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			Aim.Initialization();
			_characterDive = _character?.FindAbility<CharacterDive>();
		}

		/// <summary>
		/// At the start of each cycle, we check if we're pressing the dash button. If we
		/// </summary>
		protected override void HandleInput()
		{
			if (_inputManager.DashButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
			{
				StartDash();
			}
		}

		/// <summary>
		/// The second of the 3 passes you can have in your ability. Think of it as Update()
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();

			// If the character is dashing, we cancel the gravity
			if (_movement.CurrentState == CharacterStates.MovementStates.Dashing) 
			{
				_controller.GravityActive(false);
			}

			// we reset our slope tolerance if dash didn't end naturally
			if ((!_dashEndedNaturally) && (_movement.CurrentState != CharacterStates.MovementStates.Dashing))
			{
				_dashEndedNaturally = true;
				_controller.Parameters.MaximumSlopeAngle = _slopeAngleSave;
			}

			HandleAmountOfDashesLeft();
		}

		/// <summary>
		/// Causes the character to dash or dive (depending on the vertical movement at the start of the dash)
		/// </summary>
		public virtual void StartDash()
		{
			if (!DashAuthorized())
			{
				return; 
			}

			if (!DashConditions())
			{
				return;
			}

			if (_dashOneReady)
			{
				InitiateDashOne();
				DashOneStarted.PlayFeedbacks();
			}

			else if (DoubleDash)
			{
				InitiateDashTwo();
			}
		}




		/// <summary>
		/// This method evaluates the internal conditions for a dash (cooldown between dashes, amount of dashes left) and returns true if a dash can be performed, false otherwise
		/// </summary>
		/// <returns></returns>
		public virtual bool DashConditions()
		{			
			if (DoubleDash)
			{
				Debug.Log("Double Dash is set to "+DoubleDash+" so we can double dash");
			}

			// if we don't have dashes left, we prevent dash
			if (_dashOneReady == false & _dashTwoReady == false)
			{
				return false;
			}

			if (_dashOneReady == true & _dashTwoReady == false)
			{
				return true;
			}

			if (_dashTwoReady == true & _dashOneReady == false & DoubleDash)
			{
				return true;
			}

			else 
			{
				return true;
			}
	
		}

		/// <summary>
		/// Checks if conditions are met to reset the amount of dashes left
		/// </summary>
		protected virtual void HandleAmountOfDashesLeft()
		{
			/// We only check the conditions if the Cooldown after a dash has been reset 
			if (Time.time - _lastDashAt > DashCooldown) 
			{
				if (_dashOneReady == false)
				{
					if (Time.time - _lastDashOneAt > DashResetTime)
					{
						_dashOneReady = true;
						DashOneReady.PlayFeedbacks();
					}
				}

				if (_dashTwoReady == false)
				{
					if (Time.time - _lastDashTwoAt > DashResetTime)
					{
						_dashTwoReady = true;
						DashTwoReady.PlayFeedbacks();
					}
				}
			}
			else
				return;
		}
		
		/// <summary>
		/// This method evaluates the external conditions (state, other abilities) for a dash, and returns true if a dash can be performed, false otherwise
		/// </summary>
		/// <returns></returns>
		public virtual bool DashAuthorized()
		{
			// if the Dash action is enabled in the permissions, we continue, if not we do nothing
			if (!AbilityAuthorized
			    || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
			    || (_movement.CurrentState == CharacterStates.MovementStates.LedgeHanging)
			    || (_movement.CurrentState == CharacterStates.MovementStates.Gripping))
				return false;

			// If the user presses the dash button and is not aiming down
			if (_characterDive != null)
			{
				if ((_characterDive.AbilityAuthorized) && (_characterDive.enabled) && (_inputManager != null))
				{
					if (_verticalInput < -_inputManager.Threshold.y)
					{
						return false;
					}
				}
			}

			return true;
		}
        
		/// <summary>
		/// initializes all parameters prior to a dash and triggers the pre dash feedbacks
		/// </summary>
		public virtual void InitiateDashOne()
		{
			// we set its dashing state to true
			_movement.ChangeState(CharacterStates.MovementStates.Dashing);

			// we start our sounds
			PlayAbilityStartFeedbacks();
			MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.Dash, MMCharacterEvent.Moments.Start);

			// we initialize our various counters and checks
			_startTime = Time.time;
			_startFrame = Time.frameCount;
			_dashEndedNaturally = false;
			_initialPosition = _characterTransform.position;
			_distanceTraveled = 0;
			_shouldKeepDashing = true;
			_cooldownTimeStamp = Time.time + DashCooldown;
			_lastDashOneAt = Time.time;
			_dashOneReady = false;
			_lastDashAt = Time.time;

			if (InvincibleWhileDashing)
			{
				_health.DamageDisabled();
			}

			// we prevent our character from going through slopes
			_slopeAngleSave = _controller.Parameters.MaximumSlopeAngle;
			_controller.Parameters.MaximumSlopeAngle = 0;
			_controller.SlowFall(0f);

			ComputeDashDirection();
			CheckFlipCharacter();

			// we launch the boost corountine with the right parameters
			_dashCoroutine = Dash();
			StartCoroutine(_dashCoroutine);
		}


		public virtual void InitiateDashTwo()
		{
			// we set its dashing state to true
			_movement.ChangeState(CharacterStates.MovementStates.Dashing);
			// we start our sounds
			PlayAbilityStartFeedbacks();
			MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.Dash, MMCharacterEvent.Moments.Start);

			// we initialize our various counters and checks
			_startTime = Time.time;
			_startFrame = Time.frameCount;
			_dashEndedNaturally = false;
			_initialPosition = _characterTransform.position;
			_distanceTraveled = 0;
			_shouldKeepDashing = true;
			_cooldownTimeStamp = Time.time + DashCooldown;
			_lastDashTwoAt = Time.time;
			_dashTwoReady = false;
			_lastDashAt = Time.time;


			if (InvincibleWhileDashing)
			{
				_health.DamageDisabled();
			}

			// we prevent our character from going through slopes
			_slopeAngleSave = _controller.Parameters.MaximumSlopeAngle;
			_controller.Parameters.MaximumSlopeAngle = 0;
			_controller.SlowFall(0f);

			ComputeDashDirection();
			CheckFlipCharacter();

			// we launch the boost corountine with the right parameters
			_dashCoroutine = Dash();
			StartCoroutine(_dashCoroutine);
		}
		/// <summary>
		/// Computes the dash direction based on the selected options
		/// </summary>
		protected virtual void ComputeDashDirection()
		{
			// we compute our direction
			if (_character.LinkedInputManager != null)
			{
				Aim.PrimaryMovement = _character.LinkedInputManager.PrimaryMovement;
				Aim.SecondaryMovement = _character.LinkedInputManager.SecondaryMovement;
			}
            
			Aim.CurrentPosition = _characterTransform.position;
			_dashDirection = Aim.GetCurrentAim();

			CheckAutoCorrectTrajectory();
            
			if (_dashDirection.magnitude < MinimumInputThreshold)
			{
				_dashDirection = _character.IsFacingRight ? Vector2.right : Vector2.left;
			}
			else
			{
				_dashDirection = _dashDirection.normalized;
			}
		}

		/// <summary>
		/// Prevents the character from dashing into the ground when already grounded and if AutoCorrectTrajectory is checked
		/// </summary>
		protected virtual void CheckAutoCorrectTrajectory()
		{
			if (AutoCorrectTrajectory && _controller.State.IsCollidingBelow && (_dashDirection.y < 0f))
			{
				_dashDirection.y = 0f;
			}
		}

		/// <summary>
		/// Checks whether or not a character flip is required, and flips the character if needed
		/// </summary>
		protected virtual void CheckFlipCharacter()
		{
			// we flip the character if needed
			if (FlipCharacterIfNeeded && (Mathf.Abs(_dashDirection.x) > 0.05f))
			{
				if (_character.IsFacingRight != (_dashDirection.x > 0f))
				{
					_character.Flip();
				}
			}
		}




		/// <summary>
		/// Coroutine used to move the player in a direction over time
		/// </summary>
		protected virtual IEnumerator Dash()
		{
			// if the character is not in a position where it can move freely, we do nothing.
			if ( !AbilityAuthorized
			     || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
			{
				yield break;
			}            

			// we keep dashing until we've reached our target distance or until we get interrupted
			while (_distanceTraveled < DashDistance 
			       && _shouldKeepDashing 
			       && !_controller.State.TouchingLevelBounds
			       && TestForExactDistance()
			       && _movement.CurrentState == CharacterStates.MovementStates.Dashing)
			{
				_distanceTraveled = Vector3.Distance(_initialPosition,_characterTransform.position);

				// if we collide with something on our left or right (wall, slope), we stop dashing, otherwise we apply horizontal force
				if ( (_controller.State.IsCollidingLeft && _dashDirection.x < 0f)
				     || (_controller.State.IsCollidingRight && _dashDirection.x > 0f)
				     || (_controller.State.IsCollidingAbove && _dashDirection.y > 0f)
				     || (_controller.State.IsCollidingBelow && _dashDirection.y < 0f)
					 || (_health.PostDamageInvulnerable))
				{
					_shouldKeepDashing = false;
					_controller.SetForce (Vector2.zero);
				}
				else
				{
					_controller.GravityActive(false);
					_controller.SetForce(_dashDirection * DashForce);
				}
				yield return null;
			}

			StopDash();				
		}

		/// <summary>
		/// Checks (if needed) if we've exceeded our distance, and positions the character at the exact final position
		/// </summary>
		/// <returns></returns>
		protected virtual bool TestForExactDistance()
		{
			if (!ForceExactDistance)
			{
				return true;
			}
			
			int framesSinceStart = Time.frameCount - _startFrame;
			_averageDistancePerFrame = _distanceTraveled / framesSinceStart;
			
			if (DashDistance - _distanceTraveled < _averageDistancePerFrame)
			{
				_characterTransform.position = _initialPosition + (_dashDirection * DashDistance);
				return false;
			}
			
			
			return true;
		}

		/// Method which will be invoked when the dash ends so its possible to set a fixed time for the invincibility duration
		public void StopDashInvincibility()
		{	
			_health.DamageEnabled();
		}

		/// <summary>
		/// Stops the dash coroutine and resets all necessary parts of the character
		/// </summary>
		public virtual void StopDash()
		{
			if (_dashCoroutine != null)
			{
				StopCoroutine(_dashCoroutine);    
			}

			// once our dash is complete, we reset our various states
			_controller.DefaultParameters.MaximumSlopeAngle = _slopeAngleSave;
			_controller.Parameters.MaximumSlopeAngle = _slopeAngleSave;
			_controller.GravityActive(true);
			_dashEndedNaturally = true;

			// we reset our forces
			if (ResetForcesOnExit)
			{
				_controller.SetForce(Vector2.zero);
			}			
			



			if (InvincibleWhileDashing)
			{		
				Invoke("StopDashInvincibility", InvincibilityWhileDashingDuration);		

			}
            
			// we play our exit sound
			StopStartFeedbacks();
			MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.Dash, MMCharacterEvent.Moments.End);
			PlayAbilityStopFeedbacks();

			// once the boost is complete, if we were dashing, we make it stop and start the dash cooldown
			if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
			{
				if (_controller.State.IsGrounded)
				{
					_movement.ChangeState(CharacterStates.MovementStates.Idle);
				}
				else
				{
					_movement.RestorePreviousState();
				}                
			}
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter(_dashingAnimationParameterName, AnimatorControllerParameterType.Bool, out _dashingAnimationParameter);
		}

		/// <summary>
		/// At the end of the cycle, we update our animator's Dashing state 
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _dashingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Dashing), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
		}

		/// <summary>
		/// On reset ability, we cancel all the changes made
		/// </summary>
		public override void ResetAbility()
		{
			base.ResetAbility();
			if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
			{
				StopDash();	
			}

			if (_animator != null)
			{
				MMAnimatorExtensions.UpdateAnimatorBool(_animator, _dashingAnimationParameter, false, _character._animatorParameters, _character.PerformAnimatorSanityChecks);	
			}
		}
	}
}
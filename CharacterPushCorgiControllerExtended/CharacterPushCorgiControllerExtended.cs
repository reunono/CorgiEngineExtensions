using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	/// Add this class to a Character and it'll be able to push and/or pull CorgiController equipped objects around.
    /// Animator parameters : Pushing (bool), Pulling (bool)
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Push Corgi Controller Extended")]
	public class CharacterPushCorgiControllerExtended : CharacterAbility
	{
		public override string HelpBoxText() { return "This component allows your character to push blocks. This is not a mandatory component, it will just override CorgiController push settings, and allow you to have a dedicated push animation."; }

        /// if this is true, the user will have to press the Push button to push or pull, otherwise it's automatic on contact
        [Tooltip("if this is true, the user will have to press the Push button to push or pull, otherwise it's automatic on contact")]
        public bool ButtonBased = false;
        /// If this is set to true, the Character will be able to push blocks
        [Tooltip("If this is set to true, the Character will be able to push blocks")]
        public bool CanPush = true;
        /// If this is set to true, the Character will be able to pull blocks. Note that this requires ButtonBased to be true.
        [Tooltip("If this is set to true, the Character will be able to pull blocks. Note that this requires ButtonBased to be true.")]
        public bool CanPull = true;
        /// if this is true, the Character will only be able to push objects while grounded
        [Tooltip("if this is true, the Character will only be able to push objects while grounded")]
        public bool PushWhenGroundedOnly = true;
        /// the length of the raycast used to detect if we're colliding with a pushable object. Increase this if your animation is flickering.
        [Tooltip("the length of the raycast used to detect if we're colliding with a pushable object. Increase this if your animation is flickering.")]
        public float DetectionRaycastLength = 0.2f;
        /// Raycast vertical postion. 1 = top, 0 = middle, -1 bottom
        [Tooltip("Raycast vertical postion. 1 = top, 0 = middle, -1 bottom"),Range(-1.0f,1.0f)]
        public float RaycastVerticalPostion = 0.0f;
        /// the minimum horizontal speed below which we don't consider the character pushing anymore
        [Tooltip("the minimum horizontal speed below which we don't consider the character pushing anymore")]
        public float MinimumPushSpeed = 0.05f;

		protected bool _collidingWithPushable = false;
		protected Vector3 _raycastDirection;
		protected Vector3 _raycastOrigin;
        protected Pushable _pushedObject;
        protected float _movementMultiplierStorage;
        protected bool _pulling = false;
        protected CharacterRun _characterRun;

        // animation parameters
        protected const string _pushingAnimationParameterName = "Pushing";
        protected const string _pullingAnimationParameterName = "Pulling";
        protected int _pushingAnimationParameter;
        protected int _pullingAnimationParameter;

        /// <summary>
        /// On Start(), we initialize our various flags
        /// </summary>
        protected override void Initialization()
		{
			base.Initialization();
            _characterRun = this.gameObject.GetComponent<CharacterRun>();
		}

		/// <summary>
		/// Every frame we override parameters if needed and cast a ray to see if we're actually pushing anything
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();

			if (!CanPush || !AbilityPermitted)
			{
				return;
			}

            CheckForPushEnd();

            // if we're button based we only proceed if the push button is being pressed
            if (ButtonBased
                && (_character.CharacterType == Character.CharacterTypes.Player)
                && (_inputManager.PushButton.State.CurrentState != MMInput.ButtonStates.ButtonPressed))
            {
                return;
            }

			// we set our flag to false
			_collidingWithPushable = false;

			// we cast a ray in front of us to see if we're colliding with a pushable object
			_raycastDirection = _character.IsFacingRight ? transform.right : -transform.right;
            
            // Added offset for Raycast vertical position
            Vector3 raycastVerticalOffset = Vector3.zero;
            raycastVerticalOffset.y = (_controller.Height()/2 * RaycastVerticalPostion);

			_raycastOrigin = _controller.ColliderCenterPosition + _raycastDirection * (_controller.Width()/2) + raycastVerticalOffset;

			// we cast our ray to see if we're hitting something
			RaycastHit2D hit = MMDebug.RayCast (_raycastOrigin,_raycastDirection,DetectionRaycastLength,_controller.PlatformMask,Color.green,_controller.Parameters.DrawRaycastsGizmos);
			if (hit)
			{
				if (hit.collider.gameObject.MMGetComponentNoAlloc<Pushable>() != null)
                {
                    _collidingWithPushable = true;
				}
            }

            // if we're colliding with a pushable and are in the right conditions, we start pushing
            if (_controller.State.IsGrounded
                && _collidingWithPushable
                && Mathf.Abs(_controller.ExternalForce.x) >= MinimumPushSpeed
                && _movement.CurrentState != CharacterStates.MovementStates.Pushing
                && _movement.CurrentState != CharacterStates.MovementStates.Jumping)
			{
                if (_movement.CurrentState == CharacterStates.MovementStates.Running)
                {
                    if (_characterRun != null)
                    {
                        _characterRun.RunStop();
                    }
                }
                PlayAbilityStartFeedbacks ();
                _movement.ChangeState (CharacterStates.MovementStates.Pushing);
            }

            if (hit && (_movement.CurrentState == CharacterStates.MovementStates.Pushing) && (_pushedObject == null))
            {
                _pushedObject = hit.collider.gameObject.MMGetComponentNoAlloc<Pushable>();
                _pushedObject.Attach(_controller);
                _character.CanFlip = false;
                _movementMultiplierStorage = _characterHorizontalMovement.PushSpeedMultiplier;
                _characterHorizontalMovement.PushSpeedMultiplier = _pushedObject.PushSpeed;
            }

            if (((_controller.Speed.x > MinimumPushSpeed)
                && (_movement.CurrentState == CharacterStates.MovementStates.Pushing)
                && (_pushedObject.transform.position.x < this.transform.position.x))
                ||
                ((_controller.Speed.x < -MinimumPushSpeed)
                && (_movement.CurrentState == CharacterStates.MovementStates.Pushing)
                && (_pushedObject.transform.position.x > this.transform.position.x)))
            {
                if (!CanPull)
                {
                    StopPushing();
                }
                else
                {
                    _pulling = true;
                }
            }
            else
            {
                _pulling = false;
            }
        }

        /// <summary>
        /// Checks whether we should stop pushing and change state
        /// </summary>
        protected virtual void CheckForPushEnd()
        {
            if ((_pushedObject != null) && (_character.CharacterType == Character.CharacterTypes.Player) && _inputManager.PushButton.State.CurrentState != MMInput.ButtonStates.ButtonPressed)
            {
                StopPushing();
            }

            if (!_collidingWithPushable && (_movement.CurrentState == CharacterStates.MovementStates.Pushing))
            {
                StopPushing();
            }

            if (((_pushedObject == null) && _movement.CurrentState == CharacterStates.MovementStates.Pushing)
                || ((_pushedObject != null) && Mathf.Abs(_controller.Speed.x) <= MinimumPushSpeed && _movement.CurrentState == CharacterStates.MovementStates.Pushing))
            {
                // we reset the state
                _movement.ChangeState(CharacterStates.MovementStates.Idle);

                PlayAbilityStopFeedbacks();
                StopStartFeedbacks();
            }

            if ((_movement.CurrentState != CharacterStates.MovementStates.Pushing) && _startFeedbackIsPlaying)
            {
                PlayAbilityStopFeedbacks();
                StopStartFeedbacks();
            }
        }

        /// <summary>
        /// Stops the character from pushing or pulling
        /// </summary>
        protected virtual void StopPushing()
        {
            if (_pushedObject == null)
            {
                return;
            }
            _pushedObject.Detach(_controller);
            _pushedObject = null;
            _character.CanFlip = true;
            _characterHorizontalMovement.PushSpeedMultiplier = _movementMultiplierStorage;
            _pulling = false;
        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_pushingAnimationParameterName, AnimatorControllerParameterType.Bool, out _pushingAnimationParameter);
            RegisterAnimatorParameter(_pullingAnimationParameterName, AnimatorControllerParameterType.Bool, out _pullingAnimationParameter);
        }

		/// <summary>
		/// Sends the current state of the push and pull states to the character's animator
		/// </summary>
		public override void UpdateAnimator()
        {
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _pushingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Pushing), _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _pullingAnimationParameter, _pulling, _character._animatorParameters);
        }
	}
}

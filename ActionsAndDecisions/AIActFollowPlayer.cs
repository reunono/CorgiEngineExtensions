using UnityEngine;
using MoreMountains.Tools;
using Unity.Collections;

namespace MoreMountains.CorgiEngine
{
	// This action emulates basic AI Follow behaviour, but works with AI Brain from Corgi Engine 7.0+
	// Muppo (2018-2021)
	
	public class AIActFollowPlayer : AIAction
	{
		[MMInformation("Set distances for different stats, as in legacy AI Follow script."+
			"Optionally you can make your AI to flee from player instead following them.",MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]
	    
		[Header("Settings")]
		public bool flee;
		public float RunDistance = 10f;
		public float WalkDistance = 5f;
		public float StopDistance = 1f;
		public float JetpackDistance = 0.2f;


		public bool AgentFollowPlayer;
		[ReadOnly] public Transform _target;
		protected float _speed;
		protected float _direction;
		protected CorgiController _controller;
		protected Character thisCharacter;
		protected CharacterHorizontalMovement _characterHorizontalMovement;
		protected CharacterRun _characterRun;
		protected CharacterJump _characterJump;
		protected CharacterJetpack _jetpack;

	    ///
	    protected override void Initialization()
	    {
		    if (LevelManager.Instance.Players.Count == 0)
		    {
			    Debug.LogError("No player found!");
			    return;
		    }

		    if (_controller == null)
		    {
			    _controller = this.gameObject.GetComponent<CorgiController>();
			    thisCharacter = this.gameObject.GetComponent<Character>();
		    }
		    else
		    {
			    _controller = this.gameObject.GetComponentInParent<CorgiController>();
			    thisCharacter = this.gameObject.GetComponentInParent<Character>();
		    }
		    
		    _characterHorizontalMovement = thisCharacter?.FindAbility<CharacterHorizontalMovement>();
		    _characterRun = thisCharacter?.FindAbility<CharacterRun>();
		    _characterJump = thisCharacter?.FindAbility<CharacterJump>();
		    _jetpack = thisCharacter?.FindAbility<CharacterJetpack>();

		    AgentFollowPlayer = false;
		    thisCharacter.MovementState.ChangeState (CharacterStates.MovementStates.Idle);
	    }

	    ///
		public override void PerformAction()
        {
			AssignTarget();
	        FollowPlayer();
	     }

		protected virtual void FollowPlayer() 
		{
			if ((thisCharacter == null) || (_controller == null))
			{
				return;
			}

			if ((thisCharacter.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
			    || (thisCharacter.ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen))
			{
				return;
			}

			Character _targetChara = _target.GetComponent<Character>();
			if (_targetChara.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
			{
				AgentFollowPlayer = false;
				UnassignTarget();
			}

			float distance = Mathf.Abs(_target.position.x - transform.position.x);
					
			if(!flee)
			{
				_direction = _target.position.x > transform.position.x ? 1f : -1f;
			}
			else
			{
				_direction = _target.position.x > transform.position.x ? -1f : 1f;
			}


			if (_characterRun != null && _characterRun.AbilityInitialized)
			{
				if (distance > RunDistance)
				{
					_speed = 1;
					_characterRun.RunStart();
				}
				else
				{
					_characterRun.RunStop();
				}
			}
			
			if (distance < RunDistance && distance > WalkDistance)
			{
				_speed = 1; // walk
			}
			
			if (distance < WalkDistance && distance > StopDistance)
			{
				_speed = distance / WalkDistance; // walk slowly
			}
			
			if (distance < StopDistance)
			{
				_speed = 0f; // stop
			}
			
			_characterHorizontalMovement.SetHorizontalMove(_speed*_direction);

			if ((_characterJump != null) && (AgentFollowPlayer))
			{
				if (_controller.State.IsCollidingRight || _controller.State.IsCollidingLeft)
				{
					_characterJump.JumpStart();
				}
			}

			if (_jetpack != null && _jetpack.AbilityInitialized)
			{
				if (_target.position.y > transform.position.y + JetpackDistance)
				{
					_jetpack.JetpackStart();
				}
				else
				{
					if (thisCharacter.MovementState.CurrentState == CharacterStates.MovementStates.Jetpacking)
					{
						_jetpack.JetpackStop();	
					}
				}
			}
		}

		public virtual void AssignTarget()
		{
			_target = LevelManager.Instance.Players[0].transform;
		}

		public virtual void UnassignTarget()
		{
			_target = null;
			_brain.Target = null;
		}
	}
}
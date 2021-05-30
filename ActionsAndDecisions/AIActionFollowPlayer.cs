using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	// This action emulates basic AI Follow behaviour, but works with AI Brain from Corgi Engine 5.0
	// Muppo (2018)
	
	public class AIActionFollowPlayer : AIAction
	{
		[MMInformation("This speed value will override character speed. You can make your AI to flee from player. Flip Sprite override "+
		"\"Flip on direction change\" from Character script when this action is performed. Set distances for diferent stats, as in "+
		"standard AI Follow script.",MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]
	    
		[Header("Settings")]
		public float followSpeed;
		public bool flee;
		public bool flipSprite;

		[Header("Distances")]
		public float RunDistance = 10f;
		public float JetpackDistance = 0.2f;

		protected float followSpeedStorage;
		protected  bool AgentFollowsPlayer;
	    protected float _direction;
		protected Transform _target;
	    protected CorgiController _controller;
		protected Character _targetCharacter;
	    protected CharacterHorizontalMovement _characterBasicMovement;
	    protected CharacterRun _characterRun;
	    protected CharacterJump _characterJump;
	    protected CharacterJetpack _jetpack;

		///
	 	protected override void Initialization()
        {
			AgentFollowsPlayer = false;
			_target=LevelManager.Instance.Players[0].transform;

			_controller = GetComponent<CorgiController>();
			_targetCharacter = GetComponent<Character>();
			_characterBasicMovement = GetComponent<CharacterHorizontalMovement>();
			_targetCharacter.MovementState.ChangeState (CharacterStates.MovementStates.Idle);
			_characterRun = GetComponent<CharacterRun>();
			_characterJump = GetComponent<CharacterJump>();
			_jetpack = GetComponent<CharacterJetpack>();
			followSpeedStorage = followSpeed;
        }
		
		///
		public override void PerformAction()
        {
            _characterBasicMovement.MovementSpeed = followSpeedStorage;
			//_targetCharacter.FlipOnDirectionChange = flipSprite;
			AgentFollowsPlayer = true;
        }

		protected virtual void Update() 
		{
			// if the agent is not supposed to follow the player, we do nothing.
			if (!AgentFollowsPlayer)
				return;
				
			// if the Follower doesn't have the required components, we do nothing.
			if ( (_targetCharacter==null) || (_controller==null) )
				return;

			if ((_targetCharacter.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
				|| (_targetCharacter.ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen))
			{
				return;
			}
			
			// we calculate the distance between the target and the agent
			float distance = Mathf.Abs(_target.position.x - transform.position.x);
					
			// we determine the direction
			if(!flee)
			{
				_direction = _target.position.x>transform.position.x ? 1f : -1f;
			}
			else
			{
				_direction = _target.position.x>transform.position.x ? -1f : 1f;
			}


			if (_characterRun != null && _characterRun.AbilityInitialized)
			{
				// depending on the distance between the agent and the player, we set the speed and behavior of the agent.
				if (distance>RunDistance)
				{
		            _characterBasicMovement.MovementSpeed = _characterRun.RunSpeed;
					_characterRun.RunStart();
				}
				else
				{
					_characterRun.RunStop();
		            _characterBasicMovement.MovementSpeed = followSpeedStorage;
				}
			}
			
			// we make the agent move
			followSpeed = _characterBasicMovement.MovementSpeed;
			_characterBasicMovement.SetHorizontalMove(followSpeed*_direction);

			if (_characterJump != null)
			{
				// if there's an obstacle on the left or on the right of the agent, we make it jump. If it's moving, it'll jump over the obstacle.
				if (_controller.State.IsCollidingRight || _controller.State.IsCollidingLeft)
				{
					_characterJump.JumpStart();
				}
			}

			// if the follower is equipped with a jetpack
			if (_jetpack!=null && _jetpack.AbilityInitialized)
			{
				if (_target.position.y>transform.position.y+JetpackDistance)
				{
					_jetpack.JetpackStart();
				}
				else
				{
					if (_targetCharacter.MovementState.CurrentState == CharacterStates.MovementStates.Jetpacking)
					{
						_jetpack.JetpackStop();	
					}
				}
			}
		}
	}
}
using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	public class CharacterLadderExtended : CharacterLadder {

		/// <summary>
		///***This script free to use and no credit is required. 
		///***This is intended to be used with More Mountain's Corgi Engine 4.2+
		///***Extension Written by: Keith Henderson. Any questions can be sent to keith.donaldh@gmail.com
		/// This script is intended to replace the Character Ladder Script on your playercharacter.
		/// This script will allow you to reset your jump counter after attaching to a ladder. It also allows you jump from a ladder with your Run Speed if the Run Button is pressed while jumping off.
		/// </summary>

		//Create a public bool so we can choose to reset jumps or not, by default this will be true
		public bool ResetJumpsOnLadder = true;

		/// <summary>
		/// Called at ProcessAbility(), checks if we're colliding with a ladder and if we need to do something about it
		/// </summary>	
		protected override void HandleLadderClimbing()
		{
			if (!AbilityPermitted
				|| (_condition.CurrentState != CharacterStates.CharacterConditions.Normal && _condition.CurrentState != CharacterStates.CharacterConditions.ControlledMovement ))
			{
				return;
			}

			// if the character is colliding with a ladder
			if (LadderColliding) 
			{
				if ((_movement.CurrentState == CharacterStates.MovementStates.LadderClimbing) // if the character is climbing
					&& _controller.State.IsGrounded) // and is grounded
				{			
					// we make it get off the ladder
					GetOffTheLadder();	
				}

				if (_verticalInput > _inputManager.Threshold.y// if the player is pressing up
					&& (_movement.CurrentState != CharacterStates.MovementStates.LadderClimbing) // and we're not climbing a ladder already
					&& (_movement.CurrentState != CharacterStates.MovementStates.Jetpacking)) // and we're not jetpacking
				{			
					// then the character starts climbing
					StartClimbing();
				}	

				// if the character is climbing the ladder (which means it previously connected with it)
				if (_movement.CurrentState == CharacterStates.MovementStates.LadderClimbing)
				{
					Climbing();
				}

				// if the current ladder does have a ladder platform associated to it
				if (CurrentLadder.LadderPlatform != null)
				{
					if ((_movement.CurrentState == CharacterStates.MovementStates.LadderClimbing) // if the character is climbing
						&& AboveLadderPlatform()) // and is above the ladder platform
					{			
						// we make it get off the ladder
						GetOffTheLadder();	
					}
				}

				if (CurrentLadder.LadderPlatform != null)
				{
					if ((_movement.CurrentState != CharacterStates.MovementStates.LadderClimbing) // if the character is climbing
						&& (_verticalInput < -_inputManager.Threshold.y) // and is pressing down
						&& (AboveLadderPlatform()) // and is above the ladder's platform
						&& _controller.State.IsGrounded) // and is grounded
					{			
						// we make it get off the ladder
						StartClimbingDown();	
					}
				}

				//We use this to check if the current state is climbing or not. If it is not climbing we check the inputs and push through the correct speed for the character.
				if (_movement.CurrentState != CharacterStates.MovementStates.LadderClimbing) {
					if (_inputManager.RunButton.State.CurrentState == MMInput.ButtonStates.ButtonDown || _inputManager.RunButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed) {
						_characterHorizontalMovement.MovementSpeed = gameObject.GetComponent<CharacterRun> ().RunSpeed;
					}
					if (_inputManager.RunButton.State.CurrentState == MMInput.ButtonStates.ButtonUp) {
						_characterHorizontalMovement.ResetHorizontalSpeed ();
					}

				}
			}
			else
			{
				// if we're not colliding with a ladder, but are still in the LadderClimbing state
				if (_movement.CurrentState == CharacterStates.MovementStates.LadderClimbing)
				{
					GetOffTheLadder();
				}
			}

			// we stop our sounds if needed
			if (_movement.CurrentState != CharacterStates.MovementStates.LadderClimbing)
			{
				// we play our exit sound
				PlayAbilityStopFeedbacks();
			}
		}

		/// <summary>
		/// Sets the various flags and states 
		/// </summary>
		protected override void SetClimbingState()
		{
			if (_movement.CurrentState != CharacterStates.MovementStates.LadderClimbing)
			{
				// we start our sounds
				PlayAbilityStartFeedbacks();
			}

			//If selected in the inspector, we reset the number of jumps allowed when the player attaches to the ladder.
			if (ResetJumpsOnLadder) {
				gameObject.GetComponent<CharacterJump> ().ResetNumberOfJumps ();
			}
			// we set its state to LadderClimbing
			_movement.ChangeState(CharacterStates.MovementStates.LadderClimbing);			
			// it can't move freely anymore
			_condition.ChangeState(CharacterStates.CharacterConditions.ControlledMovement);
			// we initialize the ladder climbing speed to zero
			CurrentLadderClimbingSpeed = Vector2.zero;
			// we make sure the controller won't move
			_controller.SetHorizontalForce(0);
			_controller.SetVerticalForce(0);
			// we disable the gravity
			_controller.GravityActive(false);

		}


	}
}

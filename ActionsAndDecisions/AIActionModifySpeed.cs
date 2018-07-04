using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	// Add this component to an AI Brain and agent speed will be override with this value.
	// Muppo (2018)

	public class AIActionModifySpeed : AIAction
	{
		public float newSpeed;

		protected CharacterHorizontalMovement _horizontalMovement;
		protected float originalSpeed;


	 	protected override void Initialization()
        {
            _horizontalMovement = this.gameObject.GetComponent<CharacterHorizontalMovement>();
			originalSpeed = _horizontalMovement.WalkSpeed;
        }
		
		public override void PerformAction()
        {
            _horizontalMovement.MovementSpeed = newSpeed;
        }
	}
}
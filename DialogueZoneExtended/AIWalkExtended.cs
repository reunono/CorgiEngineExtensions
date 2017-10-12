using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	

	public class AIWalkExtended : AIWalk {

		/// <summary>
		///***This script free to use and no credit is required. 
		///***This is intended to be used with More Mountain's Corgi Engine 4.2+
		///***Extension Written by: Keith Henderson. Any questions can be sent to keith.donaldh@gmail.com
		///***This script is to be used in conjunction with DialogueZoneExtended.cs
		/// All this script does is make the Reset() method public so it can be accessed from DialogueZoneExtended.cs
		/// </summary>

		public virtual void Reset()
		{
			// we get the CorgiController2D component
			_controller = GetComponent<CorgiController>();
			_character = GetComponent<Character>();
			_characterHorizontalMovement = GetComponent<CharacterHorizontalMovement>();
			_health = GetComponent<Health> ();
			// initialize the start position
			_startPosition = transform.position;
			// initialize the direction
			_direction = _character.IsFacingRight ? Vector2.right : Vector2.left;

			_initialDirection = _direction;
			_initialScale = transform.localScale;
		}


	}
}

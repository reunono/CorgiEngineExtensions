using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{

	/// <summary>
	///***This script free to use and no credit is required. 
	///***This is intended to be used with More Mountain's Corgi Engine 4.2+
	///***Written by: Keith Henderson. Any questions can be sent to keith.donaldh@gmail.com
	///***This script is to be used in conjunction with ObjectActivatedZone.cs
	/// Add this script to a GameObject and you can call specific methods when all Object Activated Zones are activated. (Ex: Moving Platforms).
	/// </summary>

	public class ObjectActivated : MonoBehaviour {

		[Header("Number of Zones Required to Activate")]
		/// This is the number ObjectActivatedZones that are required to activate this object.
		[Tooltip("Declare how many ObjectActivatedZones are required to activate this object.")]
		public int RequiredZones = 1;

		[Header("Method to Activate")]
		/// The method that is called when all conditions are met
		public UnityEvent EnableAction;

		[Header("Method to Deactivate")]
		/// The method that is called when all conditions are not met
		public UnityEvent DisableAction;

		//We use this to keep track of how many zones have been activated
		[HideInInspector]
		public int Counter;

		public void ConditionCheck()
		{
			//We check to see if the counter matches the number of required zones. If not, we disable and exit
		     if(Counter == RequiredZones)
		     	Enable();

		     else
		    	Disable();
		    	return;
		}

		/// Here we execute the method when all condtions are met
		protected virtual void Enable()
		{
			if (EnableAction != null)
			{
				EnableAction.Invoke ();
			}
		}

		///  Here we execute the method when all conditions are not met
		protected virtual void Disable()
		{
			if (DisableAction != null)
			{
				DisableAction.Invoke ();
			}
		}


	}
}

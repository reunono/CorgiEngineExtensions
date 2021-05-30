using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	[RequireComponent(typeof(Collider2D))]

	/// <summary>
	///***This script free to use and no credit is required. 
	///***This is intended to be used with More Mountain's Corgi Engine 4.2+
	///***Written by: Keith Henderson. Any questions can be sent to keith.donaldh@gmail.com
	///***This script is to be used in conjunction with ObjectActivated.cs
	/// Add this component to an object with a collider to created an Object Activated Zone
	/// In the inspector you can specify which object is required to activate the zone with RequiredObject
	/// You can also set the GameObject that will be activated when all the RequiredObjects are in the correct zones(This is the GameObject you add the ObjectActivated.cs component to).
	/// </summary>

	public class ObjectActivatedZone : MonoBehaviour {

		[Header("Required Object")]
		//This is the required object used to activate the zone, it must be set in inspector
		[Tooltip("Place the GameObject that will activate the zone here.")]
		public GameObject RequiredObject;

		[Header("Activated Object")]
		/// This is the object which is being activated when all conditions are met
		[Tooltip("Place the GameObject that will be activated when all condtions are filled.")]
		public GameObject ActivatedObject;

		//Protected Stuff
		protected Collider2D _collidingObject;
		protected GameObject _activatedObject;


		/// We initialize the ActivatedObject so we can update the Counter
		void Awake (){

			_activatedObject = ActivatedObject;
		
		}

		// On enter we store our colliding object
		protected virtual void OnTriggerEnter2D(Collider2D collider)
			{
				//If the player enters the collider we ignore it and exit
				if (collider.gameObject.MMGetComponentNoAlloc<Character> ()) {
					return;
				} 

				//We check to see if RequiredObject has been set and if the GameObject colliding is the RequiredObject
				else {
					_collidingObject = collider;
					if (RequiredObject != null && _collidingObject.gameObject == RequiredObject)
					{
						//We make sure an ActivatedObject has been set in inspector, if not we do nothing
						if(_activatedObject != null)
						{
							//We update the counter and check to see if conditions are met to activate object
							_activatedObject.GetComponent<ObjectActivated>().Counter += 1;
							_activatedObject.GetComponent<ObjectActivated>().ConditionCheck(); 	
						}
						else
							Debug.Log("Please set the ActivatedObject in the inspector.");
							return; 
					}
					else
							Debug.Log("Either the RequiredObject has not been set, or it is the wrong object in the zone.");
						return;
				
				}
					
			}

		// On exit we store our colliding object
		protected virtual void OnTriggerExit2D(Collider2D collider)
			{
				//If the player enters the collider we ignore it and exit
				if (collider.gameObject.MMGetComponentNoAlloc<Character> ()) {
					return;
				} 

				//We check to see if RequiredObject has been set and if the GameObject colliding is the RequiredObject
				else {
					_collidingObject = collider;
					if (RequiredObject != null && _collidingObject.gameObject == RequiredObject)
					{
						//We make sure an ActivatedObject has been set in inspector, if not we do nothing
						if(_activatedObject != null)
						{
							//We update the counter and check to see if conditions are met to activate object
							_activatedObject.GetComponent<ObjectActivated>().Counter -= 1;
							_activatedObject.GetComponent<ObjectActivated>().ConditionCheck(); 	
						}
						else
							Debug.Log("Please set the ActivatedObject in the inspector.");
							return; 
					}
					else
							Debug.Log("Either the RequiredObject has not been set, or it is the wrong object in the zone.");
						return;
				
				}
					
			}

	}
}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// This script is based on Teleporter script to allow characters to teleport instantly. It's intended to use with Corgi Engine 4.2+
	/// This is free to use, no credit required. However if you improve it, sharing will be appreciated.
	/// Modifications by: Muppo [ muppotronic@gmail.com ]

	/// Add this script to a trigger collider2D to teleport objects from that object to its destination immediately
	/// </summary>
	public class TeleporterInstant : ButtonActivated 
	{
		/// the teleporter's destination
		public TeleporterInstant Destination;
		/// if true, this won't teleport non player characters
		public bool OnlyAffectsPlayer=true;
		/// a gameobject to instantiate when teleporting
		public GameObject TeleportEffect;

		[Header("Camera Settings")]
		[MMInformation("Set the Regular Camera is mandatory for this script to work." +
			"\nBlock Camera allow you to freeze camera movement immediately before " +
			"the teleport is used. Uncheck it for standard behaviour",MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]

		/// Set the Regular Camera which behaviour will be override
		public CameraController regularCamera;
		/// Block camera  useful for small stages like houses
		/// i.e. Check it on an outside door and uncheck it at inside door. Camera will not follow Player until teleport is used again
		public bool _blockCamera = false;

		Vector3 tempPos;
		protected Character _player;
		protected List<Transform> _ignoreList;


		/// <summary>
		/// On start we initialize our ignore list
		/// </summary>
		protected virtual void Start()
		{		
			_ignoreList = new List<Transform>();
		}

		/// <summary>
		/// Triggered when something enters the teleporter
		/// </summary>
		/// <param name="collider">Collider.</param>
		protected override void OnTriggerEnter2D(Collider2D collider)
		{
			// if the object that collides with the teleporter is on its ignore list, we do nothing and exit.
			if (_ignoreList.Contains(collider.transform))
			{
				return;
			}	

			// if the teleporter is supposed to only affect the player (well, corgiControllers), we do nothing and exit
			if (OnlyAffectsPlayer || !AutoActivation)
			{
				base.OnTriggerEnter2D(collider);
				if (collider.GetComponent<Character>()!=null)
				{
					_player=collider.GetComponent<Character>();
				}
			}
			else
			{
				Teleport(collider);
			}
		}

		/// <summary>
		/// If we're button activated and if the button is pressed, we teleport
		/// </summary>
		public override void TriggerButtonAction(GameObject instigator)
		{
			if (!CheckNumberOfUses())
			{
				return;
			}
			if (_player.GetComponent<Collider2D>()!=null)
			{
				base.TriggerButtonAction (instigator);
				Teleport(_player.GetComponent<Collider2D>());
			}
			ActivateZone();
		}

		/// <summary>
		/// Teleports whatever enters the portal to a new destination
		/// </summary>
		protected virtual void Teleport(Collider2D collider)
		{
			// if the teleporter has a destination, we move the colliding object to that destination
			if (Destination!=null)
			{
				collider.transform.position=Destination.transform.position;
				tempPos = collider.transform.position;
				tempPos.z = -10f;
				_ignoreList.Remove(collider.transform);
				Destination.AddToIgnoreList(collider.transform);
				regularCamera.GetComponent<CameraController>().transform.position = tempPos;

				if (_blockCamera)
				{	
					regularCamera.GetComponent<CameraController>().enabled = false;
				}

				else
				{
					regularCamera.GetComponent<CameraController>().enabled = true;
				}

				// we trigger splashs at both portals locations
				Splash ();
				Destination.Splash();
			}
		}

		/// <summary>
		/// When something exits the teleporter, if it's on the ignore list, we remove it from it, so it'll be considered next time it enters.
		/// </summary>
		protected override void OnTriggerExit2D(Collider2D collider)
		{
			if (_ignoreList.Contains(collider.transform))
			{
				_ignoreList.Remove(collider.transform);
			}
			base.OnTriggerExit2D(collider);
		}

		/// <summary>
		/// Adds an object to the ignore list, which will prevent that object to be moved by the teleporter while it's in that list
		/// </summary>
		/// <param name="objectToIgnore">Object to ignore.</param>
		public virtual void AddToIgnoreList(Transform objectToIgnore)
		{
			_ignoreList.Add(objectToIgnore);
		}

		/// <summary>
		/// Creates a splash at the point of entry
		/// </summary>
		public virtual void Splash()
		{	
			if (TeleportEffect != null)
			{
				Instantiate(TeleportEffect ,transform.position,Quaternion.identity);	
			}
		}
	}
}
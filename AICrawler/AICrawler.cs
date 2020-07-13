using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    public class AICrawler : MovingPlatform
    {
        /// This component works similar to Moving Platforms but have custom options for rotating Model prefab on path nodes.
		/// Usage: Set this at Parent object with all components you like then make a Child with, at least, and Animator and set it on the inspector Model slot.
		/// Intented to be used on Corgi Engine 6.4+
		/// v1.0 (Muppo, 2020)

		[System.Serializable]
		public class PathMovementNode
		{
			public float nodeSpeed;
            public bool RotationLeft;
            public bool RotationRight;
			// public MMFeedbacks nodeFeedback; // Check NODE FEEDBACKS below for info regarding this line.
		}

        [Header("Extra Settings")]
        [MMInformation("If a Model is set, Rotation will rotate it on a 90º basis. Start Delay is applied any time the AI respawns."+
		"\n<b>Warning</b>: Path Element Options size <b>must</b> match Path Elements size. Both directions checked means <i>No Rotation</i>.",MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]
        public Transform Model;
 		public float StartDelay;
		[MMReadOnly] public float rotationValue;
		// WalkingToRight means character will rotate -90 degrees on waypoints if True, otherwhise it takes 90 degrees value. It's set by Loop Initial Movement Direction.
		// [HideInInspector]
		[MMReadOnly] public bool WalkingToRight;
        public List<PathMovementNode> PathElementsOptions;
        protected Animator _animator;
		protected Character _character;
		protected float MovementSpeedORG;
 		protected float StartDelayORG;

        /// Awake
		protected override void Awake()
		{
			MovementSpeedORG = MovementSpeed;
			StartDelayORG = StartDelay;
			Initialization();
		}

		/// Initialization
		protected override void Initialization()
		{
			base.Initialization();
            
			_character = GetComponent<Character>();
			CycleOption = CycleOptions.Loop;
			StartDelay = StartDelayORG;

			if (Model != null)
			{
				_animator = Model.gameObject.GetComponent<Animator>();
				_animator.SetBool("Alive", true);
				_animator.SetBool("Walking", true);
            }
			else
				{ Debug.LogWarning("This component needs a Child with and Animator at least to work properly."); }

			if(LoopInitialMovementDirection == MovementDirection.Ascending)
				{ WalkingToRight = true; }

            // We set the model in the right position for the rotation sequence
            if(WalkingToRight) {
                Model.rotation = Quaternion.Euler(0,0,90);
            } else {
                Model.rotation = Quaternion.Euler(0,0,0);
            }

        }

		/// Update with check for delayed start
		protected override void Update() 
		{
			if(PathElements == null || PathElements.Count < 1 || _endReached || !CanMove)
				{ return; }

			// Check if a delay is set and start it before moving.
			if(StartDelay > 0)
			{
				StartCoroutine(WaitABit());
			}
			else
			{
				Move();
			}
		}

		/// Waits for the delay
		IEnumerator WaitABit()
		{
			yield return new WaitForSeconds(StartDelay);
			StartDelay = 0;
		}

        /// Move override with the extra features
		protected override void Move()
		{
			_waiting -= Time.deltaTime;
			if (_waiting > 0)
            {
				CurrentSpeed = Vector3.zero;
				return;
			}

			_initialPosition = transform.position;

			MoveAlongThePath();

			_distanceToNextPoint = (transform.position - (_originalTransformPosition + _currentPoint.Current)).magnitude;
			if(_distanceToNextPoint < MinDistanceToGoal)
			{
				if (PathElements.Count > _currentIndex)
				{
                    _waiting = PathElements[_currentIndex].Delay;

					/// Check if there is any Path Elements features set and, if so, changes next path element to the set value on the array
                   
					/// NODE SPEED
					if (PathElementsOptions.Count > 0)
					{
                        if (PathElementsOptions[_currentIndex].nodeSpeed > 0)
                        {
                            MovementSpeed = PathElementsOptions[_currentIndex].nodeSpeed;
                        }
						
						if (PathElementsOptions[_currentIndex].nodeSpeed == 0)
						{
                            MovementSpeed = MovementSpeedORG;
						}
                    }

					/// NODE ROTATION
					if((PathElementsOptions[_currentIndex].RotationLeft) && (PathElementsOptions[_currentIndex].RotationRight))
					{ return; }

					if (PathElementsOptions[_currentIndex].RotationLeft)
					{
						rotationValue = 30f;
						StartCoroutine(RotationSequence());
					}
					else if (PathElementsOptions[_currentIndex].RotationRight)
					{
						rotationValue = -30f;
						StartCoroutine(RotationSequence());
					}

					// /// NODE FEEDBACKS
					// /// In case you want to have Feedbacks per node, uncomment this and the Feedbacks line on the enum at the start of this script.
					// if(PathElementsOptions[_currentIndex].nodeFeedback != null)
					// {
                    //     PathElementsOptions[_currentIndex].nodeFeedback?.PlayFeedbacks();
					// }
				}

				_previousPoint = _currentPoint.Current;
				_currentPoint.MoveNext();
			}

			_finalPosition = transform.position;
			CurrentSpeed = (_finalPosition-_initialPosition) / Time.deltaTime;

			if (_endReached)
			{
				CurrentSpeed = Vector3.zero;
			}
		}

		///
		protected virtual IEnumerator RotationSequence()
		{
			Model.Rotate (Vector3.forward * rotationValue);
			yield return new WaitForSeconds(0.05f);
			Model.Rotate (Vector3.forward * rotationValue);
			yield return new WaitForSeconds(0.05f);
			Model.Rotate (Vector3.forward * rotationValue);
		}
    }
}
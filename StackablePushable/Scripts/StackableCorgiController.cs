using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Core/Corgi Controller (Stackable)")] 
    public class StackableCorgiController : CorgiController
    {

        protected LayerMask _originalLayer;
		/// <summary>
		/// Move object to another layer to perform collision check, otherwise raycasts it will collide with itself
		/// </summary>
        protected virtual void SetTemporaryLayer() {
            // TODO: Do this if object is on layer existing in PlatformLayerMask
            _originalLayer = this.gameObject.layer;
            this.gameObject.layer = LayerMask.NameToLayer("Default");
        }

        /// <summary>
		/// Set back original object layer
		/// </summary>
        protected virtual void SetOriginalLayer() {
            this.gameObject.layer = _originalLayer;
        }

		/// <summary>
		/// Every frame, we cast a number of rays below our character to check for platform collisions
		/// </summary>
		protected override void CastRaysBelow()
		{
			_friction=0;

			if (_newPosition.y < -_smallValue)
			{
				State.IsFalling = true;
			}
			else
			{
				State.IsFalling = false;
			}

			if ((Parameters.Gravity > 0) && (!State.IsFalling))
			{
				State.IsCollidingBelow = false;
				return;
			}				

			float rayLength = (_boundsHeight / 2) + RayOffset; 	

			if (State.OnAMovingPlatform)
			{
				rayLength *= 2;
			}	

			if (_newPosition.y < 0)
			{
				rayLength += Mathf.Abs(_newPosition.y);
			}			

			_verticalRayCastFromLeft = (_boundsBottomLeftCorner + _boundsTopLeftCorner) / 2;
			_verticalRayCastToRight = (_boundsBottomRightCorner + _boundsTopRightCorner) / 2;	
			_verticalRayCastFromLeft += (Vector2)transform.up * RayOffset;
			_verticalRayCastToRight += (Vector2)transform.up * RayOffset;
			_verticalRayCastFromLeft += (Vector2)transform.right * _newPosition.x;
			_verticalRayCastToRight += (Vector2)transform.right * _newPosition.x;

			if (_belowHitsStorage.Length != NumberOfVerticalRays)
			{
				_belowHitsStorage = new RaycastHit2D[NumberOfVerticalRays];	
			}

            _raysBelowLayerMaskPlatforms = PlatformMask;

            _raysBelowLayerMaskPlatformsWithoutOneWay = PlatformMask & ~MidHeightOneWayPlatformMask & ~OneWayPlatformMask & ~MovingOneWayPlatformMask;
            _raysBelowLayerMaskPlatformsWithoutMidHeight = _raysBelowLayerMaskPlatforms & ~MidHeightOneWayPlatformMask;
            
            // if what we're standing on is a mid height oneway platform, we turn it into a regular platform for this frame only
            if (StandingOnLastFrame != null)
            {
                _savedBelowLayer = StandingOnLastFrame.layer;
                if (MidHeightOneWayPlatformMask.MMContains(StandingOnLastFrame.layer))
                {
                    StandingOnLastFrame.layer = LayerMask.NameToLayer("Platforms");
                }
            }       
            
            // if we were grounded last frame, and not on a one way platform, we ignore any one way platform that would come in our path.
            if (State.WasGroundedLastFrame)
            {
                if (StandingOnLastFrame != null)
                {
                    if (!MidHeightOneWayPlatformMask.MMContains(StandingOnLastFrame.layer))
                    {
                        _raysBelowLayerMaskPlatforms = _raysBelowLayerMaskPlatformsWithoutMidHeight;                        
                    }
                }
            }
            
            // stairs management
            if (State.WasGroundedLastFrame)
            {
                if (StandingOnLastFrame != null)
                {
                    if (StairsMask.MMContains(StandingOnLastFrame.layer))
                    {
                        // if we're still within the bounds of the stairs
                        if (StandingOnCollider.bounds.Contains(_colliderBottomCenterPosition))
                        {
                            _raysBelowLayerMaskPlatforms = _raysBelowLayerMaskPlatforms & ~OneWayPlatformMask | StairsMask;
                        }    
                    }
                }
            }

            if (State.OnAMovingPlatform && (_newPosition.y > 0))
            {
                _raysBelowLayerMaskPlatforms = _raysBelowLayerMaskPlatforms & ~OneWayPlatformMask;
            }

            float smallestDistance = float.MaxValue; 
			int smallestDistanceIndex = 0; 						
			bool hitConnected = false; 		

            SetTemporaryLayer();           

			for (int i = 0; i < NumberOfVerticalRays; i++)
            {
                Vector2 rayOriginPoint = Vector2.Lerp(_verticalRayCastFromLeft, _verticalRayCastToRight, (float)i / (float)(NumberOfVerticalRays - 1));

				if ((_newPosition.y > 0) && (!State.WasGroundedLastFrame))
                {
                    _belowHitsStorage[i] = MMDebug.RayCast (rayOriginPoint,-transform.up,rayLength, _raysBelowLayerMaskPlatformsWithoutOneWay, Color.blue,Parameters.DrawRaycastsGizmos);	
				}					
				else
				{
                    _belowHitsStorage[i] = MMDebug.RayCast (rayOriginPoint,-transform.up,rayLength, _raysBelowLayerMaskPlatforms, Color.blue,Parameters.DrawRaycastsGizmos);					
				}					

				float distance = MMMaths.DistanceBetweenPointAndLine (_belowHitsStorage [smallestDistanceIndex].point, _verticalRayCastFromLeft, _verticalRayCastToRight);	

				if (_belowHitsStorage[i])
				{
					if (_belowHitsStorage[i].collider == _ignoredCollider)
                    {
                        continue;
					}

					hitConnected=true;
					State.BelowSlopeAngle = Vector2.Angle( _belowHitsStorage[i].normal, transform.up )  ;
					_crossBelowSlopeAngle = Vector3.Cross (transform.up, _belowHitsStorage [i].normal);
					if (_crossBelowSlopeAngle.z < 0)
					{
						State.BelowSlopeAngle = -State.BelowSlopeAngle;
					}

					if (_belowHitsStorage[i].distance < smallestDistance)
					{
						smallestDistanceIndex=i;
						smallestDistance = _belowHitsStorage[i].distance;
					}
				}

                if (distance < _smallValue)
                {
                    break;
                }
            }

            SetOriginalLayer();

			if (hitConnected)
			{
				StandingOn = _belowHitsStorage[smallestDistanceIndex].collider.gameObject;
				StandingOnCollider = _belowHitsStorage [smallestDistanceIndex].collider;
                
                // if the character is jumping onto a (1-way) platform but not high enough, we do nothing
                if (
					!State.WasGroundedLastFrame 
					&& (smallestDistance < _boundsHeight / 2) 
					&& (
						OneWayPlatformMask.MMContains(StandingOn.layer)
						||
						MovingOneWayPlatformMask.MMContains(StandingOn.layer)
					) 
				)
				{
                    State.IsCollidingBelow = false;
					return;
				}

				State.IsFalling = false;			
				State.IsCollidingBelow = true;


				// if we're applying an external force (jumping, jetpack...) we only apply that
				if (_externalForce.y>0 && _speed.y > 0)
				{
					_newPosition.y = _speed.y * Time.deltaTime;
					State.IsCollidingBelow = false;
				}
				// if not, we just adjust the position based on the raycast hit
				else
				{
					float distance = MMMaths.DistanceBetweenPointAndLine (_belowHitsStorage [smallestDistanceIndex].point, _verticalRayCastFromLeft, _verticalRayCastToRight);

					_newPosition.y = -distance
						+ _boundsHeight / 2 
						+ RayOffset;
				}

				if (!State.WasGroundedLastFrame && _speed.y > 0)
				{
					_newPosition.y += _speed.y * Time.deltaTime;
				}				

				if (Mathf.Abs(_newPosition.y) < _smallValue)
                {
                    _newPosition.y = 0;
                }					

				// we check if whatever we're standing on applies a friction change
				_frictionTest = _belowHitsStorage[smallestDistanceIndex].collider.gameObject.MMGetComponentNoAlloc<SurfaceModifier>();
				if (_frictionTest != null)
				{
					_friction=_belowHitsStorage[smallestDistanceIndex].collider.GetComponent<SurfaceModifier>().Friction;
				}

				// we check if the character is standing on a moving platform
				_movingPlatformTest = _belowHitsStorage[smallestDistanceIndex].collider.gameObject.MMGetComponentNoAlloc<MMPathMovement>();
				if (_movingPlatformTest != null && State.IsGrounded)
				{
					_movingPlatform=_movingPlatformTest.GetComponent<MMPathMovement>();
				}
				else
				{
					DetachFromMovingPlatform();
				}
			}
			else
			{
				State.IsCollidingBelow=false;
				if(State.OnAMovingPlatform)
				{
					DetachFromMovingPlatform();
				}
			}	

			if (StickToSlopes)
			{
				StickToSlope ();
			}
		}

        /// <summary>
		/// If we're in the air and moving up, we cast rays above the character's head to check for collisions
		/// </summary>
		protected override void CastRaysAbove()
		{			
			/*if (_newPosition.y<0)
				return;*/

			float rayLength = State.IsGrounded ? RayOffset : _newPosition.y;
			rayLength += _boundsHeight / 2;

			bool hitConnected=false; 

			_aboveRayCastStart = (_boundsBottomLeftCorner + _boundsTopLeftCorner) / 2;
			_aboveRayCastEnd = (_boundsBottomRightCorner + _boundsTopRightCorner) / 2;	

			_aboveRayCastStart += (Vector2)transform.right * _newPosition.x;
			_aboveRayCastEnd += (Vector2)transform.right * _newPosition.x;

			if (_aboveHitsStorage.Length != NumberOfVerticalRays)
			{
				_aboveHitsStorage = new RaycastHit2D[NumberOfVerticalRays];	
			}

			float smallestDistance=float.MaxValue;

            SetTemporaryLayer();

            int collidingIndex = 0;
			for (int i=0; i<NumberOfVerticalRays;i++)
			{							
				Vector2 rayOriginPoint = Vector2.Lerp(_aboveRayCastStart,_aboveRayCastEnd,(float)i/(float)(NumberOfVerticalRays-1));
				_aboveHitsStorage[i] = MMDebug.RayCast (rayOriginPoint,(transform.up), rayLength, PlatformMask & ~OneWayPlatformMask & ~MovingOneWayPlatformMask, MMColors.Cyan, Parameters.DrawRaycastsGizmos);	

				if (_aboveHitsStorage[i])
				{
					hitConnected=true;
                    collidingIndex = i;

                    if (_aboveHitsStorage[i].collider == _ignoredCollider)
					{
						break;
					}
					if (_aboveHitsStorage[i].distance<smallestDistance)
					{
						smallestDistance = _aboveHitsStorage[i].distance;
					}
				}					
			}

            SetOriginalLayer();

			if (hitConnected)
			{
                _newPosition.y = smallestDistance - _boundsHeight / 2;
                
				if ((State.IsGrounded) && (_newPosition.y < 0))
				{
					_newPosition.y = 0;
				}

				State.IsCollidingAbove = true;

				if (!State.WasTouchingTheCeilingLastFrame)
				{
					_speed = new Vector2(_speed.x, 0f);
                }

                SetVerticalForce(0);
            }	
		}

        		/// <summary>
		/// Casts rays to the sides of the character, from its center axis.
		/// If we hit a wall/slope, we check its angle and move or not according to it.
		/// </summary>
		protected override void CastRaysToTheSides(float raysDirection) 
		{	
            // we determine the origin of our rays
			_horizontalRayCastFromBottom = (_boundsBottomRightCorner + _boundsBottomLeftCorner) / 2;
			_horizontalRayCastToTop = (_boundsTopLeftCorner + _boundsTopRightCorner) / 2;	
			_horizontalRayCastFromBottom = _horizontalRayCastFromBottom + (Vector2)transform.up * _obstacleHeightTolerance;
			_horizontalRayCastToTop = _horizontalRayCastToTop - (Vector2)transform.up * _obstacleHeightTolerance;

			// we determine the length of our rays
			float horizontalRayLength = Mathf.Abs(_speed.x * Time.deltaTime) + _boundsWidth / 2 + RayOffset * 2;

			// we resize our storage if needed
			if (_sideHitsStorage.Length != NumberOfHorizontalRays)
			{
				_sideHitsStorage = new RaycastHit2D[NumberOfHorizontalRays];	
			}

            SetTemporaryLayer();

            // we cast rays to the sides
            for (int i=0; i < NumberOfHorizontalRays;i++)
			{	
				Vector2 rayOriginPoint = Vector2.Lerp(_horizontalRayCastFromBottom, _horizontalRayCastToTop, (float)i / (float)(NumberOfHorizontalRays-1));

				// if we were grounded last frame and if this is our first ray, we don't cast against one way platforms
				if ( State.WasGroundedLastFrame && i == 0 )		
				{
					_sideHitsStorage[i] = MMDebug.RayCast (rayOriginPoint,raysDirection*(transform.right),horizontalRayLength,PlatformMask, MMColors.Indigo,Parameters.DrawRaycastsGizmos);	
				}						
				else
				{
					_sideHitsStorage[i] = MMDebug.RayCast (rayOriginPoint,raysDirection*(transform.right),horizontalRayLength,PlatformMask & ~OneWayPlatformMask & ~MovingOneWayPlatformMask, MMColors.Indigo,Parameters.DrawRaycastsGizmos);			
				}
				// if we've hit something
				if (_sideHitsStorage[i].distance > 0)
				{	
					// if this collider is on our ignore list, we break
					if (_sideHitsStorage[i].collider == _ignoredCollider)
					{
						break;
					}
                    
					// we determine and store our current lateral slope angle
					float hitAngle = Mathf.Abs(Vector2.Angle(_sideHitsStorage[i].normal, transform.up));
                    
                    if (OneWayPlatformMask.MMContains(_sideHitsStorage[i].collider.gameObject))
                    {
                        if (hitAngle > 90)
                        {
                            break;
                        }
                    }

                    // we check if this is our movement direction
                    if (_movementDirection == raysDirection)
                    {
                        State.LateralSlopeAngle = hitAngle;
                    }                    

					// if the lateral slope angle is higher than our maximum slope angle, then we've hit a wall, and stop x movement accordingly
					if (hitAngle > Parameters.MaximumSlopeAngle)
					{
						if (raysDirection < 0)
						{
							State.IsCollidingLeft = true;
                            State.DistanceToLeftCollider = _sideHitsStorage[i].distance;
						} 
						else
						{
							State.IsCollidingRight = true;
                            State.DistanceToRightCollider = _sideHitsStorage[i].distance;
                        }

                        if (_movementDirection == raysDirection)
                        {
                            CurrentWallCollider = _sideHitsStorage[i].collider.gameObject;
                            State.SlopeAngleOK = false;

                            float distance = MMMaths.DistanceBetweenPointAndLine(_sideHitsStorage[i].point, _horizontalRayCastFromBottom, _horizontalRayCastToTop);
                            if (raysDirection <= 0)
                            {
                                _newPosition.x = -distance
                                    + _boundsWidth / 2
                                    + RayOffset * 2;
                            }
                            else
                            {
                                _newPosition.x = distance
                                    - _boundsWidth / 2
                                    - RayOffset * 2;
                            }

                            // if we're in the air, we prevent the character from being pushed back.
                            if (!State.IsGrounded && (Speed.y != 0))
                            {
                                _newPosition.x = 0;
                            }

                            _contactList.Add(_sideHitsStorage[i]);
                            _speed.x = 0;
                        }

						break;
					}
				}						
			}

            SetOriginalLayer();

		}
    }

}

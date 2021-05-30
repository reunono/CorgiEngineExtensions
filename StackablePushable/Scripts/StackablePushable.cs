using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// A class needed on pushable objects if you want your character to be able to detect them
    /// </summary>
    [AddComponentMenu("Corgi Engine/Environment/Stackable Pushable")]
    [RequireComponent(typeof(Rigidbody2D))]
	public class StackablePushable : Pushable 
	{

        /// don't allow to pull when another stackable is on top
        [Tooltip("don't allow to pull when another stackable is on top")]
        public bool LockWhenStackedUnder = true;

        [Header("Stack collisions")]
        /// the length of the raycast we cast to detect if the object is stacked
        [Tooltip("the length of the raycast we cast to detect if the object is stacked")]
        public float StackedRaycastLength = 1f;
        /// the vertical offset to apply when trying to detect the stack
        [Tooltip("the vertical offset to apply when trying to detect the stack")]
        public float StackedRaycastOffset = 0.1f;
        /// the layers this object considers as part of stack
        [Tooltip("the layers this object considers as part of stack")]
        public LayerMask StackedLayerMask;
        
        [MMReadOnly]
        [Tooltip("whether or not the top of the object touches the another stacked object")]
        public bool StackedContactAbove = false;
        [MMReadOnly]
        [Tooltip("whether or not the bottom of the object touches the another stacked object")]
        public bool StackedContactBelow = false;
        [MMReadOnly]
        [Tooltip("whether or not the left side of the object touches the another stacked object")]
        public bool StackedContactLeft = false;
        [MMReadOnly]
        [Tooltip("whether or not the right side of the object touches the another stacked object")]
        public bool StackedContactRight = false;

        protected bool _collidingWithAnotherStackable;
       
        /// <summary>
        /// On Update, we apply a force if needed
        /// </summary>
        protected override void Update()
        {
            base.Update();
            CheckIfStacked();
            CheckIfGrounded();

            if (_corgiController == null)
            {
                return;
            }

            // If controller thinks is not grounded, check if standing on another stackable
            if(!Grounded && StackedContactBelow) {
                Grounded=true;
            }

            if (this.Pusher!=null) {
                
                bool pusherOnLeft = (Pusher.transform.position.x < this.transform.position.x);
                bool pusherOnRight = (Pusher.transform.position.x > this.transform.position.x);

                if (Grounded) {
                    if ((Pusher.State.IsCollidingLeft && (Pusher.ExternalForce.x < 0) && pusherOnLeft)
                        || (Pusher.State.IsCollidingRight && (Pusher.ExternalForce.x > 0) && pusherOnRight))
                    {
                        _corgiController.SetHorizontalForce(0f);
                    }
                    else
                    {
                        _corgiController.SetHorizontalForce(Pusher.ExternalForce.x);
                    }                
                }

                bool forceTowardsRight = (Pusher.ExternalForce.x > 0.05f);
                bool forceTowardsLeft = (Pusher.ExternalForce.x < -0.05f);

                if(pusherOnLeft && StackedContactRight && _collidingWithAnotherStackable && forceTowardsRight){
                    this.Detach(Pusher);
                }

                if(pusherOnRight && StackedContactLeft && _collidingWithAnotherStackable && forceTowardsLeft) {
                    this.Detach(Pusher);
                }

                if (!Grounded)
                {
                    this.Detach(Pusher);
                }

                if(StackedContactAbove && LockWhenStackedUnder) {
                    this.Detach(Pusher);
                }
            }
        }

        /// <summary>
        /// Casts rays around the object looking for other Pushables around it
        /// </summary>
        protected virtual void CheckIfStacked() {
            
            // Use _collider2D to find raycasts in all directions
            Vector2 minBound = _collider2D.bounds.min;
            Vector2 maxBound = _collider2D.bounds.max;
            
            float middleX = minBound.x + (maxBound.x - minBound.x)/2.0f;
            float middleY = minBound.y + (maxBound.y - minBound.y)/2.0f;

            Vector2 topRaycastStart = new Vector2(middleX, maxBound.y + StackedRaycastOffset);
            Vector2 bottomRaycastStartLeft = new Vector2(minBound.x, minBound.y - StackedRaycastOffset);
            Vector2 bottomRaycastStartRight = new Vector2(maxBound.x, minBound.y - StackedRaycastOffset);
            Vector2 leftRaycastStart = new Vector2(minBound.x - StackedRaycastOffset, middleY);
            Vector2 rightRaycastStart = new Vector2(maxBound.x + StackedRaycastOffset, middleY);

            // Use MMDebug.RayCast to cast rays and set variables accordingly
            RaycastHit2D hitAbove = MMDebug.RayCast(topRaycastStart, Vector2.up, StackedRaycastLength, StackedLayerMask, Color.white, true);
            RaycastHit2D hitBelowLeft = MMDebug.RayCast(bottomRaycastStartLeft, Vector2.down, StackedRaycastLength, StackedLayerMask, Color.white, true);
            RaycastHit2D hitBelowRight = MMDebug.RayCast(bottomRaycastStartRight, Vector2.down, StackedRaycastLength, StackedLayerMask, Color.white, true);
            RaycastHit2D hitLeft = MMDebug.RayCast(leftRaycastStart, Vector2.left, StackedRaycastLength, StackedLayerMask, Color.white, true);
            RaycastHit2D hitRight = MMDebug.RayCast(rightRaycastStart, Vector2.right, StackedRaycastLength, StackedLayerMask, Color.white, true);

            // Set variables           
            StackedContactAbove = hitAbove; // TODO: Potentially requires 2 to be able to detect correctly when stackable is grounded
            StackedContactBelow = (hitBelowLeft | hitBelowRight) ; // Requires 2 raycasts to be able to detect correctly when stackable is grounded
            StackedContactLeft = hitLeft;
            StackedContactRight = hitRight;
        }

        protected virtual void OnTriggerEnter2D(Collider2D collider) {
            LayerMask layer = collider.gameObject.layer;
            LayerMask layerMask = StackedLayerMask.value;
            if (layerMask == (layerMask | 1 << layer)) {
                _collidingWithAnotherStackable = true;
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D collider) {
            LayerMask layer = collider.gameObject.layer;
            LayerMask layerMask = StackedLayerMask.value;
            if (layerMask == (layerMask | 1 << layer)) {
                _collidingWithAnotherStackable = false;
            }
        }

    }
}
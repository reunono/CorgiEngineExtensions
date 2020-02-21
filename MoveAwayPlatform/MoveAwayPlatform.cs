using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
    /// Moving platform extension with Moving Away feature.
    /// Intended for Corgi Engine v6.2.1+
    /// v 1.0 - Muppo (2020)

    public class MoveAwayPlatform : MovingPlatform
    {
        protected override void Start()
        {
            base.Start();
            CycleOption = CycleOptions.StopAtBounds;
            StartMovingWhenPlayerIsColliding = true;
        }
        protected override void ExecuteUpdate () 
		{
			if(PathElements == null || PathElements.Count < 1 || _endReached || !CanMove)
		    { return; }

            // Check if platform should hide or unhide
            if(_collidingWithPlayer == true)
            {
                MoveAway();
            }
            else if(_currentIndex <= 0)
            {
                RestorePosition();
            }

			Move ();
		}

        public override void OnTriggerEnter2D(Collider2D collider)
        {
            base.OnTriggerEnter2D(collider);
            ResetEndReached();
        }

        public virtual void MoveAway()
        {
            if (CycleOption == CycleOptions.StopAtBounds)
                CycleOption = CycleOptions.BackAndForth;
        }
        public virtual void RestorePosition()
        {
            if (CycleOption == CycleOptions.BackAndForth)
                CycleOption = CycleOptions.StopAtBounds;
        }
    }
}
using System.Collections;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;

namespace CorgiEngineExtensions
{
    public class FreezingDamageOnTouch : DamageOnTouch
    {
        [Header("Freezing")]
        [Tooltip("Whether or not to freeze on touch")]
        public bool FreezeOnTouch = true;
        [Tooltip("How long to freeze on touch")]
        public float FreezeTime = 1;
        protected override void Colliding(Collider2D collider)
        {
            base.Colliding(collider);
            if (FreezeOnTouch) StartCoroutine(FreezeCollidingCharacter(collider));
        }

        protected IEnumerator FreezeCollidingCharacter(Collider2D collider)
        {
            var collidingCharacter = collider.gameObject.MMGetComponentNoAlloc<Character>();
            collidingCharacter?.Freeze();
            yield return new WaitForSeconds(FreezeTime);
            collidingCharacter?.UnFreeze();
        }
    }
}

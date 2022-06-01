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
            if (!FreezeOnTouch) return;
            var collidingCharacter = collider.gameObject.MMGetComponentNoAlloc<Character>();
            if (collidingCharacter == null) return;
            collidingCharacter.StartCoroutine(FreezeCharacter(collidingCharacter));
        }

        private IEnumerator FreezeCharacter(Character character)
        {
            character.Freeze();
            yield return new WaitForSeconds(FreezeTime);
            character.UnFreeze();
        }
    }
}
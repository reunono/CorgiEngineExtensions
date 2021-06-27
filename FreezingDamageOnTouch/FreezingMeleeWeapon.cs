using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;

namespace CorgiEngineExtensions
{
    public class FreezingMeleeWeapon : MeleeWeapon
    {
        [MMInspectorGroup("Freezing", true, 72)]
        [Tooltip("The time (in seconds) that the hit character should be frozen")]
        public float FreezeTime = 1;
        protected override void CreateDamageArea()
        {
            _damageArea = new GameObject();
            _damageArea.name = name+"DamageArea";
            _damageArea.transform.position = transform.position;
            _damageArea.transform.rotation = transform.rotation;
            _damageArea.transform.SetParent(transform);

            switch (DamageAreaShape)
            {
                case MeleeDamageAreaShapes.Rectangle:
                    _boxCollider2D = _damageArea.AddComponent<BoxCollider2D>();
                    _boxCollider2D.offset = AreaOffset;
                    _boxCollider2D.size = AreaSize;
                    _damageAreaCollider = _boxCollider2D;
                    break;
                case MeleeDamageAreaShapes.Circle:
                    _circleCollider2D = _damageArea.AddComponent<CircleCollider2D>();
                    _circleCollider2D.transform.position = transform.position + transform.rotation * AreaOffset;
                    _circleCollider2D.radius = AreaSize.x/2;
                    _damageAreaCollider = _circleCollider2D;
                    break;
            }

            _damageAreaCollider.isTrigger = true;

            var rigidBody = _damageArea.AddComponent<Rigidbody2D> ();
            rigidBody.isKinematic = true;

            _damageOnTouch = _damageArea.AddComponent<FreezingDamageOnTouch>();
            _damageOnTouch.TargetLayerMask = TargetLayerMask;
            _damageOnTouch.DamageCaused = DamageCaused;
            _damageOnTouch.DamageCausedKnockbackType = Knockback;
            _damageOnTouch.DamageCausedKnockbackForce = KnockbackForce;
            _damageOnTouch.InvincibilityDuration = InvincibilityDuration;
            var freezingDamageOnTouch = _damageOnTouch as FreezingDamageOnTouch;
            freezingDamageOnTouch.FreezeOnTouch = true;
            freezingDamageOnTouch.FreezeTime = FreezeTime;
        }
    }
}

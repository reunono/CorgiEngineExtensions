using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using UnityEngine;

namespace StatusSystem
{
    public class StatusEffectMeleeWeapon : MeleeWeapon
    {
        [MMInspectorGroup("Status Effects Applied", true, 25)]
        public StatusEffect[] StatusEffects;
        
        protected override void CreateDamageArea()
        {
            _damageArea = new GameObject();
            _damageArea.name = this.name+"DamageArea";
            _damageArea.transform.position = this.transform.position;
            _damageArea.transform.rotation = this.transform.rotation;
            _damageArea.transform.SetParent(this.transform);

            if (DamageAreaShape == MeleeDamageAreaShapes.Rectangle)
            {
                _boxCollider2D = _damageArea.AddComponent<BoxCollider2D>();
                _boxCollider2D.offset = AreaOffset;
                _boxCollider2D.size = AreaSize;
                _damageAreaCollider = _boxCollider2D;
            }
            if (DamageAreaShape == MeleeDamageAreaShapes.Circle)
            {
                _circleCollider2D = _damageArea.AddComponent<CircleCollider2D>();
                _circleCollider2D.transform.position = this.transform.position + this.transform.rotation * AreaOffset;
                _circleCollider2D.radius = AreaSize.x/2;
                _damageAreaCollider = _circleCollider2D;
            }
            _damageAreaCollider.isTrigger = true;

            Rigidbody2D rigidBody = _damageArea.AddComponent<Rigidbody2D> ();
            rigidBody.isKinematic = true;

            _damageOnTouch = _damageArea.AddComponent<ApplyStatusAndDamageOnTouch>();
            _damageOnTouch.TargetLayerMask = TargetLayerMask;
            _damageOnTouch.DamageCaused = DamageCaused;
            _damageOnTouch.DamageCausedKnockbackType = Knockback;
            _damageOnTouch.DamageCausedKnockbackForce = KnockbackForce;
            _damageOnTouch.InvincibilityDuration = InvincibilityDuration;
            ((ApplyStatusAndDamageOnTouch)_damageOnTouch).StatusEffects = StatusEffects;
        }
    }
}

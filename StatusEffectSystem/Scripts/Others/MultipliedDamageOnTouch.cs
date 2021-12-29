using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace StatusSystem
{
    public class MultipliedDamageOnTouch : DamageOnTouch
    {
        private CharacterDamageMultipliers _multipliers;
        protected override void Awake()
        {
            base.Awake();
            // the CharacterDamageMultipliers scriptable object must be in a Resources folder inside your project, like so : Resources/RuntimeSets/CharacterDamageMultipliers
            _multipliers = Resources.Load<CharacterDamageMultipliers>("RuntimeSets/CharacterDamageMultipliers");
        }

        protected override void OnCollideWithDamageable(Health health)
        {
            // if what we're colliding with is a CorgiController, we apply a knockback force
            _colliderCorgiController = health.gameObject.MMGetComponentNoAlloc<CorgiController>();

            ApplyDamageCausedKnockback();
			
            OnHitDamageable?.Invoke();

            HitDamageableFeedback?.PlayFeedbacks(this.transform.position);

            if ((FreezeFramesOnHitDuration > 0) && (Time.timeScale > 0))
            {
                MMFreezeFrameEvent.Trigger(Mathf.Abs(FreezeFramesOnHitDuration));
            }

            // we apply the damage to the thing we've collided with
            var character = Owner.MMGetComponentNoAlloc<Character>();
            if (character != null)
                _colliderHealth.Damage((int)(DamageCaused * _multipliers[character]), gameObject, InvincibilityDuration, InvincibilityDuration, _damageDirection);
            else
                _colliderHealth.Damage(DamageCaused, gameObject, InvincibilityDuration, InvincibilityDuration, _damageDirection);
            if (_colliderHealth.CurrentHealth <= 0)
            {
                OnKill?.Invoke();
            }
			
            SelfDamage(DamageTakenEveryTime + DamageTakenDamageable);
        }
    }
}

using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;
using ProgressionSystem.Scripts.Variables;
using UnityEngine;

namespace ProgressionSystem.Scripts.Extensions
{
    public class MultipliedDamageOnTouch : DamageOnTouch
    {
        [SerializeField] private FloatVariable DamageMultiplier;
        
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
            _colliderHealth.Damage((int)(DamageMultiplier.Value*DamageCaused), gameObject,InvincibilityDuration,InvincibilityDuration, _damageDirection);

            if (_colliderHealth.CurrentHealth <= 0)
            {
                OnKill?.Invoke();
            }
			
            SelfDamage(DamageTakenEveryTime + DamageTakenDamageable);
        }
    }
}

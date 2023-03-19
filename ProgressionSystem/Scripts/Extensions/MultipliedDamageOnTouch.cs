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
            if (health.CanTakeDamageThisFrame())
            {			
                // if what we're colliding with is a CorgiController, we apply a knockback force
                _colliderCorgiController = health.gameObject.MMGetComponentNoAlloc<CorgiController>();
	
                float randomDamage = UnityEngine.Random.Range(MinDamageCaused, Mathf.Max(MaxDamageCaused, MinDamageCaused));
				
                ApplyDamageCausedKnockback(randomDamage, TypedDamages);
				
                OnHitDamageable?.Invoke();
	
                HitDamageableFeedback?.PlayFeedbacks(this.transform.position);
	
                if ((FreezeFramesOnHitDuration > 0) && (Time.timeScale > 0))
                {
                    MMFreezeFrameEvent.Trigger(Mathf.Abs(FreezeFramesOnHitDuration));
                }
	
                // we apply the damage to the thing we've collided with
                if (RepeatDamageOverTime)
                {
                    _colliderHealth.DamageOverTime(DamageMultiplier.Value*randomDamage, gameObject, InvincibilityDuration, InvincibilityDuration, _damageDirection, TypedDamages, AmountOfRepeats, DurationBetweenRepeats, DamageOverTimeInterruptible, RepeatedDamageType);	
                }
                else
                {
                    _colliderHealth.Damage(DamageMultiplier.Value*randomDamage, gameObject, InvincibilityDuration, InvincibilityDuration, _damageDirection, TypedDamages);	
                }
	
                if (_colliderHealth.CurrentHealth <= 0)
                {
                    OnKill?.Invoke();
                }
            }
			
            SelfDamage(DamageTakenEveryTime + DamageTakenDamageable);
        }
    }
}

using MoreMountains.CorgiEngine;

namespace CharacterDamageMultiplier
{
    public class DamageMultiplierMeleeWeapon : MeleeWeapon
    {
        public override void ApplyDamageMultiplier(float multiplier) { _damageOnTouch.DamageCaused = (int)(DamageCaused * multiplier); }
    }
}

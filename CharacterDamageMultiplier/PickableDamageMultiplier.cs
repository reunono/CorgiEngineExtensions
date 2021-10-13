using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

namespace CharacterDamageMultiplier
{
    public class PickableDamageMultiplier : PickableItem
    {
        public float DamageMultiplier = 2f;
        private DamageMultiplierCharacterHandleWeapon _handleWeapon;

        protected override void Pick()
        {
            _handleWeapon.DamageMultiplier = DamageMultiplier;
        }

        protected override bool CheckIfPickable()
        {
            _handleWeapon = _collidingObject.MMGetComponentNoAlloc<Character>()?.FindAbility<DamageMultiplierCharacterHandleWeapon>();
            return _handleWeapon != null;
        }
    }
}

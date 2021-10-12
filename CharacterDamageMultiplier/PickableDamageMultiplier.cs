using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

namespace CharacterDamageMultiplier
{
    public class PickableDamageMultiplier : PickableItem
    {
        public int DamageMultiplier = 2;
        private DamageMultiplierCharacterHandleWeapon _handleWeapon;

        protected override void Pick()
        {
            _handleWeapon.DamageMultiplier = DamageMultiplier;
        }

        protected override bool CheckIfPickable()
        {
            _character = _collidingObject.MMGetComponentNoAlloc<Character>();
            if (_character == null) return false;
            _handleWeapon = _character.FindAbility<DamageMultiplierCharacterHandleWeapon>();
            return _handleWeapon != null;
        }
    }
}

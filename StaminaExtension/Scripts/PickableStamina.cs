using MoreMountains.CorgiEngine;
using UnityEngine;

namespace StaminaExtension
{
    /// <summary>
    /// Gives stamina to the player who collects it
    /// </summary>
    [AddComponentMenu("Corgi Engine/Items/Pickable Stamina")]
    public class PickableStamina : PickableItem
    {
        /// the amount of stamina to give the player when collected
        [Tooltip("the amount of stamina to give the player when collected")]
        public float StaminaToGive = 100;

        /// <summary>
        /// What happens when the object gets picked
        /// </summary>
        protected override void Pick()
        {
            _pickingCollider.GetComponent<Stamina>().CurrentStamina += StaminaToGive;
        }
    }
}
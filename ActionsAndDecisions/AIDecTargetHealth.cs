using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// This checks Brain target health, if it's below the set value, decision will be receive True or False return.
    /// Muppo (2021)
    /// </summary>
    
    public class AIDecTargetHealth : AIDecision
    {
        public float healthAmmountToCheck = 0f;
        public bool returnValue = true;
        [SerializeField] protected Health targetHealth;
        
        
        public override bool Decide()
        {
            return CheckHealth();
        }

        protected virtual bool CheckHealth()
        {
            if (_brain.Target == null)
            {
                return true;
            }

            targetHealth = _brain.Target.GetComponent<Health>();
            if (targetHealth.CurrentHealth <= healthAmmountToCheck)
            {
                targetHealth = null;
                return returnValue;
            }

            return !returnValue;
            // return false;
        }
    }
}

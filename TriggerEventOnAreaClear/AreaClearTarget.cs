using System.Linq;
using MoreMountains.CorgiEngine;
using UnityEngine;

namespace TriggerEventOnAreaClear
{
    /// <summary>
    /// Add this class to an object and it will count as a target to clear (if it is inside a collider with a TriggerEventOnAreaClear component)
    /// </summary>
    public class AreaClearTarget : MonoBehaviour
    {
        private Health _health;
        private TriggerEventOnAreaClear _triggerEventOnAreaClear;
        private bool _alreadyRemoved;

        private void Awake()
        {
            _health = GetComponent<Health>();
            if (_health != null) _health.OnDeath += RemoveTarget;
        }

        private void AddTarget()
        {
            if (_triggerEventOnAreaClear == null) return;
            _triggerEventOnAreaClear.AddTarget();
            _alreadyRemoved = false;
        }
        
        private void RemoveTarget()
        {
            if (_triggerEventOnAreaClear == null || _alreadyRemoved) return;
            _triggerEventOnAreaClear.RemoveTarget();
            _alreadyRemoved = true;
        }
        
        private void OnEnable()
        {
            _triggerEventOnAreaClear = FindObjectsOfType<TriggerEventOnAreaClear>().SingleOrDefault(room => room.GetComponent<Collider2D>().bounds.Contains(transform.position.MMSetZ(room.transform.position.z)));
            AddTarget();
        }

        private void OnDisable() { RemoveTarget(); }
    }
}

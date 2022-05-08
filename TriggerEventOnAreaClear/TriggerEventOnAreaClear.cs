using UnityEngine;
using UnityEngine.Events;

namespace TriggerEventOnAreaClear
{
    /// <summary>
    /// Requires a Collider and objects with the AreaClearTarget component inside said collider. Make sure both this transform and the targets z positions are 0
    /// </summary>
    public class TriggerEventOnAreaClear : MonoBehaviour
    {
        [Tooltip("the event to trigger when the area is cleared (all objects marked with AreaClearTarget have been destroyed)")]
        public UnityEvent OnAreaCleared;
        private int _numberOfTargetsAlive;

        public void AddTarget() { _numberOfTargetsAlive++; }

        public void RemoveTarget()
        {
            _numberOfTargetsAlive--;
            if (_numberOfTargetsAlive == 0) OnAreaCleared?.Invoke();
        }
    }
}
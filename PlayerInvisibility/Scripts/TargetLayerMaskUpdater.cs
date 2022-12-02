using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;

namespace PlayerInvisibility.Scripts
{
    public class TargetLayerMaskUpdater : MonoBehaviour
    {
        public LayerMaskVariable TargetLayerMask;
        public string DetectTargetState = "Patrol";
        private AIDecisionDetectTargetLine[] _detectTargetLines;
        private AIDecisionDetectTargetRadius[] _detectTargetRadii;
        private DamageOnTouch[] _damageOnTouches;
        private AIBrain[] _brains;

        private void Awake()
        {
            _detectTargetLines = GetComponentsInChildren<AIDecisionDetectTargetLine>(true);
            _detectTargetRadii = GetComponentsInChildren<AIDecisionDetectTargetRadius>(true);
            _damageOnTouches = GetComponentsInChildren<DamageOnTouch>(true);
            _brains = GetComponentsInChildren<AIBrain>(true);
        }

        private void OnEnable()
        {
            UpdateTargetLayerMask();
            TargetLayerMask.OnChange += UpdateTargetLayerMask;
        }
        
        private void OnDisable()
        {
            TargetLayerMask.OnChange -= UpdateTargetLayerMask;
        }

        private void UpdateTargetLayerMask()
        {
            foreach (var detectTargetLine in _detectTargetLines)
                detectTargetLine.TargetLayer = TargetLayerMask.Value;
            foreach (var detectTargetRadius in _detectTargetRadii)
                detectTargetRadius.TargetLayer = TargetLayerMask.Value;
            foreach (var damageOnTouch in _damageOnTouches)
                damageOnTouch.TargetLayerMask = TargetLayerMask.Value;
            if (string.IsNullOrWhiteSpace(DetectTargetState)) return;
            foreach (var brain in _brains)
                brain.TransitionToState(DetectTargetState);
        }
    }
}

using MoreMountains.CorgiEngine;
using ProgressionSystem.Scripts.Variables;
using UnityEngine;

namespace ProgressionSystem.Scripts.Links
{
    public class WalkSpeedLink : MonoBehaviour
    {
        [SerializeField] private FloatVariable WalkSpeed;
        private CharacterHorizontalMovement _movement;

        private void Awake() { _movement = GetComponent<CharacterHorizontalMovement>(); }

        private void UpdateWalkSpeed()
        {
            _movement.WalkSpeed = WalkSpeed.Value;
            _movement.ResetHorizontalSpeed();
        }
        private void OnEnable()
        {
            UpdateWalkSpeed();
            WalkSpeed.Changed += UpdateWalkSpeed;
        }

        private void OnDisable()
        {
            WalkSpeed.Changed -= UpdateWalkSpeed;
        }
    }
}

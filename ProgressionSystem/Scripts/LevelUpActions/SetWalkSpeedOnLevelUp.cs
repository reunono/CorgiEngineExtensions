using MoreMountains.Feedbacks;
using MoreMountains.CorgiEngine;
using UnityEngine;

namespace ProgressionSystem
{
    [AddComponentMenu("Custom/Character/Progression System/Set Walk Speed On Level Up")]
    public class SetWalkSpeedOnLevelUp : DoSomethingOnLevelUp
    {
        [SerializeField]
        private AnimationCurve LevelWalkSpeedCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
        [SerializeField]
        private float MinWalkSpeed = 6f;
        [SerializeField]
        private float MaxWalkSpeed = 10f;
        [SerializeField]
        private MMFeedbacks SetWalkSpeedFeedbacks;

        private CharacterHorizontalMovement _movement;

        private void Awake()
        {
            _movement = GetComponent<CharacterHorizontalMovement>();
            _movement.WalkSpeed = MinWalkSpeed;
        }

        protected override void OnLevelUp(int level, int maxLevel)
        {
            var oldWalkSpeed = _movement.WalkSpeed;
            _movement.WalkSpeed = MinWalkSpeed+(MaxWalkSpeed-MinWalkSpeed)*LevelWalkSpeedCurve.Evaluate((float)level/maxLevel);
            _movement.MovementSpeed = _movement.WalkSpeed;
            SetWalkSpeedFeedbacks?.PlayFeedbacks(transform.position, _movement.WalkSpeed - oldWalkSpeed);
        }
    }
}

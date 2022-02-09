using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using UnityEngine;

namespace ProgressionSystem
{
    public abstract class DoSomethingOnLevelUp : MonoBehaviour, MMEventListener<LevelEvent>, MMEventListener<CorgiEngineEvent>
    {
        [SerializeField]
        [Tooltip("if this is false, the player character will be set as target automatically")]
        private bool UseCustomTarget;
        [MMCondition(nameof(UseCustomTarget), true)]
        [SerializeField]
        private GameObject Target;
        protected abstract void OnLevelUp(int level, int maxLevel);
        
        public void OnMMEvent(CorgiEngineEvent corgiEngineEvent)
        {
            if (corgiEngineEvent.EventType == CorgiEngineEventTypes.SpawnCharacterStarts && !UseCustomTarget) Target = LevelManager.Instance.Players[0].gameObject;
        }
        
        public void OnMMEvent(LevelEvent levelEvent)
        {
            if (levelEvent.Target != Target) return;
            if (levelEvent.Type == LevelEventType.LevelUp) OnLevelUp(levelEvent.Level, levelEvent.MaxLevel);
        }

        private void OnEnable()
        {
            this.MMEventStartListening<LevelEvent>();
            this.MMEventStartListening<CorgiEngineEvent>();
        }

        private void OnDisable()
        {
            this.MMEventStopListening<LevelEvent>();
            this.MMEventStopListening<CorgiEngineEvent>();
        }
    }
}

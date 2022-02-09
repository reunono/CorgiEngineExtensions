using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using UnityEngine;

namespace ProgressionSystem
{
    public class ExperienceEventTester : MonoBehaviour, MMEventListener<CorgiEngineEvent>
    {
        public ExperienceEventType Type;
        public GameObject Target;
        public int Experience = 100;
        [MMInspectorButton(nameof(TriggerEvent))] 
        public bool TriggerEventButton;

        private void TriggerEvent()
        {
            ExperienceEvent.Trigger(Type, Target, Experience);
        }

        public void OnMMEvent(CorgiEngineEvent corgiEngineEvent)
        {
            if (corgiEngineEvent.EventType == CorgiEngineEventTypes.SpawnCharacterStarts && Target == null) Target = LevelManager.Instance.Players[0].gameObject;
        }

        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }
    }
}

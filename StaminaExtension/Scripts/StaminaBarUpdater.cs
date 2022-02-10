using System;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;

namespace StaminaExtension
{
    public class StaminaBarUpdater : MonoBehaviour, MMEventListener<StaminaUpdateEvent>, MMEventListener<CorgiEngineEvent>
    {
        [SerializeField]
        [Tooltip("if this is false, the player character will be set as target automatically")]
        private bool UseCustomTarget;
        [MMCondition(nameof(UseCustomTarget), true)]
        [SerializeField]
        private GameObject Target;

        private MMProgressBar _bar;

        private void Awake()
        {
            _bar = GetComponent<MMProgressBar>();
        }

        public void OnMMEvent(StaminaUpdateEvent staminaUpdateEvent)
        {
            if (staminaUpdateEvent.Target != Target) return;
            _bar.UpdateBar(staminaUpdateEvent.Stamina, 0, staminaUpdateEvent.MaxStamina);
        }

        public void OnMMEvent(CorgiEngineEvent corgiEngineEvent)
        {
            if (corgiEngineEvent.EventType == CorgiEngineEventTypes.SpawnCharacterStarts && !UseCustomTarget) Target = LevelManager.Instance.Players[0].gameObject;
        }
        
        private void OnEnable()
        {
            this.MMEventStartListening<StaminaUpdateEvent>();
            this.MMEventStartListening<CorgiEngineEvent>();
        }
    
        private void OnDisable()
        {
            this.MMEventStopListening<StaminaUpdateEvent>();
            this.MMEventStopListening<CorgiEngineEvent>();
        }
    }
}

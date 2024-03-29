﻿using MoreMountains.CorgiEngine;
using ProgressionSystem.Scripts.Variables;
using UnityEngine;

namespace ProgressionSystem.Scripts.Links
{
    public class MaxHealthLink : MonoBehaviour
    {
        [SerializeField] private IntVariable MaxHealth;
        [SerializeField] private bool ResetHealthToMaxHealthOnChange;
        private Health _health;

        private void Awake() { _health = GetComponent<Health>(); }

        private void UpdateMaximumHealth()
        {
            var maxHealthDifference = MaxHealth.Value - _health.MaximumHealth;
            _health.MaximumHealth = MaxHealth.Value;
            if (ResetHealthToMaxHealthOnChange) _health.ResetHealthToMaxHealth();
            else _health.GetHealth(maxHealthDifference, gameObject);
        }

        private void OnEnable()
        {
            UpdateMaximumHealth();
            MaxHealth.Changed += UpdateMaximumHealth;
        }

        private void OnDisable()
        {
            MaxHealth.Changed -= UpdateMaximumHealth;
        }
    }
}

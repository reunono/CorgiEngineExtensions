using System;
using UnityEngine;

namespace PlayerInvisibility.Scripts
{
    public class LayerMaskVariable : ScriptableObject
    {
        public LayerMask Value;
        public event Action OnChange;

        public void Set(LayerMaskVariable value)
        {
            Value = value.Value;
            OnChange?.Invoke();
        }

        private void OnValidate()
        {
            OnChange?.Invoke();
        }
    }
}

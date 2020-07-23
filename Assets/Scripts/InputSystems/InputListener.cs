using System;
using UnityEngine;

namespace InputSystems
{
    public class InputListener : MonoBehaviour
    {
        public static event Action<Vector3> OnPress;
        public static event Action<Vector3> OnDrag;
        public static event Action OnRelease;

        [SerializeField] private float threshold = 2f;


        private Vector2 _lastInputPosition;

        private float SqrThreshold => threshold * threshold;
        private static bool IsPressed => Input.GetMouseButtonDown(0);
        private static bool IsReleased => Input.GetMouseButtonUp(0);
        private static bool IsHeld => Input.GetMouseButton(0);
        private bool IsDrag => IsHeld && Mathf.Abs(Input.mousePosition.sqrMagnitude) >= SqrThreshold;

        private void Update()
        {
            var inputPosition = Input.mousePosition;

            if (IsPressed)
            {
                UpdateLastPosition();
            
                OnPress?.Invoke(inputPosition);
            }

            if (IsDrag)
            {
                DebugWrapper.Log($"Delta: {inputPosition}", DebugColors.Gray);
                UpdateLastPosition();
            
                OnDrag?.Invoke(inputPosition);
            }

            if (IsReleased)
            {
                OnRelease?.Invoke();
            }
        }

        private void UpdateLastPosition()
        {
            _lastInputPosition = Input.mousePosition;
        }
    }
}
using System;
using Generics;
using StaticTools;
using UnityEngine;

namespace InputSystems
{
    public class InputListener : SingletonBehaviourGeneric<InputListener>
    {
        public static event Action<Vector3> OnPress;
        public static event Action<Vector3> OnDrag;
        public static event Action OnRelease;

        [SerializeField] private float threshold = 2f;


        private float SqrThreshold => threshold * threshold;
        private static bool IsPressed => Input.GetMouseButtonDown(0);
        private static bool IsReleased => Input.GetMouseButtonUp(0);
        private static bool IsHeld => Input.GetMouseButton(0);
        private bool IsDrag => IsHeld && Mathf.Abs(Input.mousePosition.sqrMagnitude) >= SqrThreshold;

        private void Update()
        {
            if (StaticUtilities.IsPointerOverUIObject())
                return;

            var inputPosition = Input.mousePosition;

            if (IsPressed)
                OnPress?.Invoke(inputPosition);

            if (IsDrag)
                OnDrag?.Invoke(inputPosition);

            if (IsReleased)
                OnRelease?.Invoke();
        }
    }
}
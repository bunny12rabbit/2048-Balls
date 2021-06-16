using Generics;
using Interfaces;
using Logic;
using UnityEngine;

namespace InputSystems
{
    public class InputController : SingletonBehaviourGeneric<InputController>, IInputHandler
    {
        [SerializeField] private float sensitivity = 700f;
        
        
        [SerializeField] private Camera mainCamera;

        private Ball _target;
        private CollisionHandlerGeneric<uint> _collisionHandler;

        private Vector3 _offset;
        private Vector3 _newPosition;
        private Vector3 _initialPosition;
        private Vector3 _initialTargetPosition;

        private bool _isControlled;
        private bool _isCanControl;


        public static float Sensitivity => Instance.sensitivity; 

        private void OnEnable()
        {
            SubscribeInputEvents();
        }

        private void OnDisable()
        {
            UnSubscribeInputEvents();
        }

        private void SubscribeInputEvents()
        {
            InputListener.OnPress += OnPress;
            InputListener.OnRelease += OnRelease;
            InputListener.OnDrag += OnDrag;
        }

        private void UnSubscribeInputEvents()
        {
            InputListener.OnPress -= OnPress;
            InputListener.OnRelease -= OnRelease;
            InputListener.OnDrag -= OnDrag;
        }

        public void OnPress(Vector3 position)
        {
            if (!_isCanControl)
                return;

            _isControlled = true;
            _initialPosition = GetNewWorldPosition(position);
            _initialTargetPosition = _target.transform.position;
            _offset = _initialPosition;
        }

        public void OnDrag(Vector3 position)
        {
            if (!_isControlled)
                return;

            _newPosition = GetNewWorldPosition(position) - _offset;
        }

        public void OnRelease()
        {
            if (!_isCanControl && !_isControlled)
                return;

            _collisionHandler.SwitchPhysics(true);
            _isControlled = false;
            _isCanControl = false;
        }


        public void UpdateTarget(Ball newTarget)
        {
            _target = newTarget;
            _isCanControl = true;

            _collisionHandler = _target.CollisionHandler;
        }

        private void FixedUpdate()
        {
            if (!_isControlled)
                return;

            _collisionHandler.MoveToPosition(_initialTargetPosition, _newPosition);
        }

        private Vector3 GetNewWorldPosition(Vector3 position) => mainCamera.ScreenToWorldPoint(position);
    }
}
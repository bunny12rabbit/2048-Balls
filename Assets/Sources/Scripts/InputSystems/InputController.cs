using Generics;
using Interfaces;
using Logic;
using Tayx.Graphy;
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
            InputListener.OnTripleTouch += ((IInputHandler)this).OnTripleTouch;
            InputListener.OnPress += ((IInputHandler)this).OnPress;
            InputListener.OnRelease += ((IInputHandler)this).OnRelease;
            InputListener.OnDrag += ((IInputHandler)this).OnDrag;
        }

        private void UnSubscribeInputEvents()
        {
            InputListener.OnTripleTouch -= ((IInputHandler)this).OnTripleTouch;
            InputListener.OnPress -= ((IInputHandler)this).OnPress;
            InputListener.OnRelease -= ((IInputHandler)this).OnRelease;
            InputListener.OnDrag -= ((IInputHandler)this).OnDrag;
        }

        void IInputHandler.OnTripleTouch() => GraphyManager.Instance.ToggleActive();

        void IInputHandler.OnPress(Vector3 position)
        {
            if (!_isCanControl)
                return;

            _isControlled = true;
            _initialPosition = GetNewWorldPosition(position);
            _initialTargetPosition = _target.transform.position;
            _offset = _initialPosition;
        }

        void IInputHandler.OnDrag(Vector3 position)
        {
            if (!_isControlled)
                return;

            _newPosition = GetNewWorldPosition(position) - _offset;
        }

        void IInputHandler.OnRelease()
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
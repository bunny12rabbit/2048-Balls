using JetBrains.Annotations;
using Pixelplacement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class UIButton : MonoBehaviour
    {
        [SerializeField] private UnityEvent onClickAnimationComplete;


        [Header("Pressed animation parameters")]
        [Range(-1, 1)]
        [SerializeField]
        private float animationScalePercent = 0.5f;

        [Min(0)] [SerializeField] private float animationDuration = 0.35f;

        [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Button _button;
        private Transform _transform;

        private Button ButtonComponent
        {
            get
            {
                if (_button == null)
                    _button = GetComponent<Button>();

                return _button;
            }
        }


        public void AddOnClickAnimationCompleteListener(UnityAction action) =>
            onClickAnimationComplete.AddListener(action);
        
        public void RemoveOnClickAnimationCompleteListener(UnityAction action) =>
            onClickAnimationComplete.RemoveListener(action);

        [UsedImplicitly]
        public void PlayButtonPressAnimation()
        {
            _button.enabled = false;
            
            var localScale = _transform.localScale;
            var targetScale = localScale + localScale * animationScalePercent;

            Tween.LocalScale(_transform, targetScale, animationDuration, 0, animationCurve, Tween.LoopType.None,
                null, OnAnimationComplete);
        }

        private void OnAnimationComplete()
        {
            _button.enabled = true;
            onClickAnimationComplete?.Invoke();
        }

        private void Awake()
        {
            _transform = transform;
        }

        private void OnEnable()
        {
            ButtonComponent.onClick.AddListener(PlayButtonPressAnimation);
        }

        private void OnDisable()
        {
            ButtonComponent.onClick.RemoveListener(PlayButtonPressAnimation);
        }
    }
}
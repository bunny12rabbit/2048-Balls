using System;
using System.Collections;
using JetBrains.Annotations;
using Managers;
using Pixelplacement;
using UI;
using UnityEditor;
using UnityEngine;

namespace Generics.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BaseWindow : MonoBehaviour
    {
        public float fadeTimer = 0.2f;

        protected CanvasGroup panel;


        [SerializeField] private Transform windowContent;

        [SerializeField] private bool isCanHideWindow = true;

        [DrawIf(nameof(isCanHideWindow), true, DrawIfAttribute.DisablingTypes.ReadOnly)]
        [SerializeField]
        private UIButton closeButton;

        [SerializeField] private bool isAnimate;

        [Header("Animation parameters")]
        [DrawIf(nameof(isAnimate), true, DrawIfAttribute.DisablingTypes.ReadOnly)]
        [SerializeField]
        private float animationDuration = 0.45f;

        [DrawIf(nameof(isAnimate), true, DrawIfAttribute.DisablingTypes.ReadOnly)]
        [SerializeField] private AnimationCurve animationCurve;

        private AnimationCurve _reversedAnimationCurve;

        public bool IsCanHideWindow => isCanHideWindow;

        public bool IsVisible
        {
            get => Mathf.Approximately(panel.alpha, 0f);
            private set => panel.alpha = value ? 1f : 0;
        }

        public bool IsShown { get; private set; }


        public virtual void Hide() => WindowManager.Instance.HideWindow(this);

        public virtual void OnShow()
        {
            IsShown = true;
            IsVisible = true;

            AnimateTransition();
        }

        public virtual void OnHide()
        {
            if (!IsCanHideWindow)
            {
                return;
            }
            
            IsShown = false;
            IsVisible = false;
            
            AnimateTransition();
        }
        
        [UsedImplicitly]
        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        protected virtual void Awake()
        {
            _reversedAnimationCurve = animationCurve?.Reverse();

            if (panel)
            {
                return;
            }

            panel = GetComponent<CanvasGroup>();
        }

        protected virtual void OnEnable()
        {
            if (!closeButton)
            {
                return;
            }

            closeButton.gameObject.SetActive(IsCanHideWindow);
            closeButton.AddOnClickAnimationCompleteListener(Hide);
        }

        protected virtual void OnDisable()
        {
            if (!closeButton)
            {
                return;
            }
            
            closeButton.RemoveOnClickAnimationCompleteListener(Hide);
        }

        private void AnimateTransition()
        {
            Action callBack = null;

            var newAnimationCurve = IsShown ? animationCurve : _reversedAnimationCurve;

            if (IsShown)
            {
                gameObject.SetActive(true);
            }
            else
            {
                callBack = () => gameObject.SetActive(false);
            }

            StartCoroutine(SmoothlyChangeAlpha(IsShown, callBack));

            if (isAnimate)
            {
                Tween.LocalScale(windowContent, Vector3.zero, Vector3.one, animationDuration, 0, newAnimationCurve,
                    Tween.LoopType.None, null, callBack);
            }
        }

        private IEnumerator SmoothlyChangeAlpha(bool isShow, Action callback = null)
        {
            if (!panel)
            {
                yield break;
            }

            panel.alpha = isShow ? 0 : 1;
            int targetAlpha = isShow ? 1 : 0;
            float startAlpha = panel.alpha;
            var elapsedTime = 0f;
            while (elapsedTime <= fadeTimer)
            {
                elapsedTime += Time.deltaTime;
                panel.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeTimer);

                yield return null;
            }
            
            callback?.Invoke();
        }
    }
}
using System;
using System.Collections.Generic;
using Generics;
using Generics.UI;
using UnityEngine;

namespace Managers
{
    public class WindowManager : SingletonBehaviourGeneric<WindowManager>
    {
        private const string ACTIVE_WINDOWS_CONTAINER_NAME = "ActiveWindowsContainer";
        private const string INACTIVE_WINDOWS_CONTAINER_NAME = "InactiveWindowsContainer";


        public event Action<BaseWindow> OnSomeWindowVisible;
        public event Action<BaseWindow> OnSomeWindowHide;
        
        
        [Space]
        [SerializeField] private GameObject[] windowPrefabs;

        
        private GameObject _canvas;
        
        private Transform _activeWindowsContainer;
        private Transform _inactiveWindowsContainer;
        
        private Dictionary<Type, BaseWindow> _typeToWindow = new Dictionary<Type, BaseWindow>();

        private readonly List<BaseWindow> _shownWindows = new List<BaseWindow>();


        public bool IsHaveTopWindow => _shownWindows != null && _shownWindows.Count != 0;

        private BaseWindow TopWindow => !IsHaveTopWindow ? null : _shownWindows[_shownWindows.Count - 1];


        public BaseWindow ShowWindow(Type windowType)
        {
            var window = GetWindowByType(windowType);

            if (window.IsShown)
            {
                DebugWrapper.LogError(
                    $"This window is already shown: {windowType}\n{StackTraceUtility.ExtractStackTrace()}");
            }

            _shownWindows.Add(window);


            OnSomeWindowVisible?.Invoke(window);

            window.OnShow();
            
            UpdateZOrder(window);

            return window;
        }

        public void HideWindow(Type windowType)
        {
            var window = GetWindowByType(windowType);
            HideWindow(window);
        }

        public void HideWindow(BaseWindow window)
        {
            bool isCannotHide = window == null || !window.IsShown || !window.IsCanHideWindow;
            
            if (isCannotHide)
            {
                return;
            }

            _shownWindows.Remove(window);

            window.OnHide();
            
            UpdateZOrder(window);

            OnSomeWindowHide?.Invoke(window);

            if (IsHaveTopWindow)
            {
                OnSomeWindowVisible?.Invoke(TopWindow);
            }
        }


        protected override void Awake()
        {
            base.Awake();

            Initialize();
        }

        private void Initialize()
        {
            Transform activeWindowsContainer = null;
            Transform inactiveWindowsContainer = null;
            
            if (UiManager.Instance)
            {
                _canvas = UiManager.Instance.Canvas;
                activeWindowsContainer = UiManager.Instance.ActiveWindowsContainer;
                inactiveWindowsContainer = UiManager.Instance.InactiveWindowsContainer;
                
            }
            else
            {
                _canvas = FindObjectOfType<Canvas>().gameObject;
            }
            
            _activeWindowsContainer = activeWindowsContainer != null
                ? activeWindowsContainer
                : _canvas.AddRectTransformChild(ACTIVE_WINDOWS_CONTAINER_NAME).transform;

            _inactiveWindowsContainer = inactiveWindowsContainer != null
                ? inactiveWindowsContainer
                : _canvas.AddRectTransformChild(INACTIVE_WINDOWS_CONTAINER_NAME).transform;
            
            CleanWindowsIn(_activeWindowsContainer);
            CleanWindowsIn(_inactiveWindowsContainer);
            
            CreateWindows();
        }

        private static void CleanWindowsIn(Transform parent)
        {
            int childCount = parent.childCount;
            for (var i = 0; i < childCount; i++)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        private void CreateWindows()
        {
            foreach (var windowPrefab in windowPrefabs)
            {
                var windowObject = _inactiveWindowsContainer.gameObject.AddChild(windowPrefab);
                var window = windowObject.GetComponent<BaseWindow>();
                windowObject.SetActive(false);

                if (!RegisterWindowType(window))
                {
                    Destroy(window);
                }
            }
        }

        private void Update()
        {
            bool isNeedToHideWindow = Input.GetKeyDown(KeyCode.Escape) && IsHaveTopWindow && TopWindow.IsCanHideWindow;
            
            if (isNeedToHideWindow)
            {
                TopWindow.Hide();
            }
        }

        private bool RegisterWindowType(BaseWindow window)
        {
            var windowType = window.GetType();

            if (_typeToWindow.ContainsKey(windowType))
            {
                DebugWrapper.LogError($"This window type is already registered: {windowType}");
                return false;
            }

            _typeToWindow.Add(windowType, window);
            return true;
        }

        private BaseWindow GetWindowByType(Type windowType)
        {
            if (!_typeToWindow.ContainsKey(windowType))
            {
                DebugWrapper.LogError($"This window type is not registered: {windowType}");
                return null;
            }

            var window = _typeToWindow[windowType];

            return window;
        }
        
        private void UpdateZOrder(BaseWindow baseWindow)
        {
            var parent = baseWindow.IsShown ? _activeWindowsContainer : _inactiveWindowsContainer;
            baseWindow.transform.SetParent(parent);
        }
    }
}
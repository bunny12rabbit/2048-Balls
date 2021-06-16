using System;
using System.Collections;
using ObjectPools;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

public static class Extensions
{
    #region DateTimeExtensions

    private static readonly DateTime _epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static DateTime FromUnixTime(long unixTime) => _epochStart.AddSeconds(unixTime);

    public static long ToUnixTime(this DateTime dateTime) => (long)(dateTime - _epochStart).TotalSeconds;

    public static string TimeSpanToString(TimeSpan timeSpan)
    {
        return timeSpan.Days > 0
            ? $"{timeSpan.Days} DAYS {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}"
            : $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
    }

    public static string TimeSpanToString(int timeSpanInSeconds)
    {
        return TimeSpanToString(TimeSpan.FromSeconds(timeSpanInSeconds));
    }

    #endregion

    #region AnimationCurveExtension

    public static AnimationCurve Reverse(this AnimationCurve animationCurve)
    {
        var newAnimationCurveKeys = animationCurve.keys;
        int lastKeyIndex = newAnimationCurveKeys.Length - 1;

        for (var i = 0; i < animationCurve.keys.Length; i++)
        {
            newAnimationCurveKeys[i].value = animationCurve.keys[lastKeyIndex - i].value;
            float time = animationCurve.keys[lastKeyIndex - i].time;
            newAnimationCurveKeys[i].time = 1 - time;
            newAnimationCurveKeys[i].inTangent = -animationCurve.keys[lastKeyIndex - i].outTangent;
            newAnimationCurveKeys[i].outTangent = -animationCurve.keys[lastKeyIndex - i].inTangent;
        }

        return new AnimationCurve(newAnimationCurveKeys);
    }

    #endregion

    #region GameObjectExtensions

    /// <summary>
    /// Push this gameObject back to GameObjectPool
    /// </summary>
    /// <param name="gameObject">gameObject to push back</param>
    public static void PushBackToPool(this GameObject gameObject) => GameObjectPool.PushBackGameObject(gameObject);

    /// <summary>
    /// Trying to get component on this gameObject, if there's none, adding this component and returning it 
    /// </summary>
    /// <param name="gameObject">gameObject to look component on</param>
    /// <typeparam name="T">Component type</typeparam>
    /// <returns>Found or added component</returns>
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component =>
        gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();


    /// <summary>
    /// Instantiate an object and add it to as child to this gameObject
    /// </summary>
    /// <param name="parent">Parent GameObject to attach to</param>
    /// <param name="prefab">GameObject prefab to Instantiate</param>
    /// <returns>Instantiated object</returns>
    public static GameObject AddChild(this GameObject parent, GameObject prefab)
    {
        var gameObject = UnityEngine.Object.Instantiate(prefab);

#if UNITY_EDITOR
        if (!Application.isPlaying)
            Undo.RegisterCreatedObjectUndo(gameObject, "CreateObject");
#endif

        if (gameObject && parent)
            InitializeGameObject(parent, gameObject);

        return gameObject;
    }

    /// <summary>
    /// Create new GameObject and add it as child to this gameObject
    /// </summary>
    /// <param name="parent">Parent GameObject to attach to</param>
    /// <param name="name">Name, the new GameObject is gonna be created with</param>
    /// <returns></returns>
    public static GameObject AddRectTransformChild(this GameObject parent, string name)
    {
        var gameObject = new GameObject(name);
        gameObject.AddComponent<RectTransform>();

        if (parent)
            InitializeGameObject(parent, gameObject);

        return gameObject;
    }
    
    private static void InitializeGameObject(GameObject parent, GameObject gameObject)
    {
        var transform = gameObject.transform;
        bool isRectTransform = transform is RectTransform;
        transform.SetParent(parent.transform, !isRectTransform);
        gameObject.layer = parent.layer;

        if (isRectTransform)
        {
            InitializeRectTransform(gameObject);
            return;
        }
        
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    private static void InitializeRectTransform(GameObject gameObject)
    {
        var rectTransform = gameObject.GetOrAddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
    }

    #endregion

    #region MonoBehaviourExtensions

    /// <summary>
    /// If CoroutineTicker.Instance not equals to null, runs given coroutine on it, otherwise using given monoBehavior
    /// </summary>
    /// <param name="monoBehaviour">this component</param>
    /// <param name="coroutine">Coroutine to run</param>
    /// <returns>Running coroutine instance</returns>
    public static Coroutine TryStartCoroutineOnTicker(this MonoBehaviour monoBehaviour, IEnumerator coroutine) =>
        CoroutineTicker.Instance
            ? CoroutineTicker.Instance.StartCoroutine(coroutine)
            : monoBehaviour.StartCoroutine(coroutine);

    #endregion

    #region ObjectExtensions

    /// <summary>
    /// Performs an action if this reference isn't null or prints error in DebugWrapper if it's null 
    /// </summary>
    /// <param name="reference">this object reference</param>
    /// <param name="action">Action to perform if reference isn't null</param>
    public static void DoActionWithCheckReference(this Object reference, Action action)
    {
        if (reference != null)
            action.Invoke();
        else
            DebugWrapper.LogError($"{nameof(reference)} is null! Assign the reference!",
                DebugColors.Red);
    }

    #endregion
}
﻿using System;
using ObjectPools;
using UnityEngine;

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

    #endregion

    #region ObjectExtensions

    /// <summary>
    /// Performs an action if this reference isn't null or prints error in DebugWrapper if it's null 
    /// </summary>
    /// <param name="reference">this object reference</param>
    /// <param name="action">Action to perform if reference isn't null</param>
    public static void DoActionWithCheckReference(this System.Object reference, Action action)
    {
        if (reference != null)
        {
            action.Invoke();
        }
        else
        {
            DebugWrapper.LogError($"{nameof(reference)} is null! Assign the reference!",
                DebugColors.Red);
        }
    }

    #endregion
}
using System;
using System.Collections.Generic;
using UnityEngine;

public static class Yielders
{
    private class FloatComparer : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y) => Math.Abs(x - y) < 0.01f;

        int IEqualityComparer<float>.GetHashCode(float obj) => obj.GetHashCode();
    }

    private static Dictionary<float, WaitForSeconds> _timeIntervals =
        new Dictionary<float, WaitForSeconds>(100, new FloatComparer());
    
    private static readonly WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();
    private static readonly WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();

    public static WaitForEndOfFrame EndOfFrame => _endOfFrame;
    public static WaitForFixedUpdate FixedUpdate => _fixedUpdate;

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        if (!_timeIntervals.TryGetValue(seconds, out var waitForSeconds))
        {
            _timeIntervals.Add(seconds, waitForSeconds = new WaitForSeconds(seconds));
        }

        return waitForSeconds;
    }
}
using UnityEngine;
#pragma warning disable 162

public enum DebugColors : byte
{
    Black,
    Blue,
    Cyan,
    Green,
    Gray,
    Magenta,
    Red,
    Yellow,
    Teal
}

public static class DebugWrapper
{
    public static void Log(object message, Object context = default)
    {
#if !FULL_LOG
        return;
#endif
        Debug.Log(message, context);
    }

    public static void LogWarning(object message, Object context = default)
    {
#if !FULL_LOG
        return;
#endif
        Debug.LogWarning(message, context);
    }

    public static void LogError(object message, Object context = default)
    {
#if !FULL_LOG
        return;
#endif
        Debug.LogError(message, context);
    }

    #region Colored logs

    private static string FormatMessage(object message, DebugColors color)
    {
        string colorName = GetColorName(color);
        var newLineSplit = message.ToString().Split('\n');
        var newMessage = string.Empty;

        foreach (string str in newLineSplit)
            newMessage += $"<color={colorName}>{str}</color>\n";

        return newMessage;
    }
    
    public static void Log(object message, DebugColors color, Object context = default)
    {
#if !FULL_LOG
        return;
#endif
        string newMessage = FormatMessage(message, color);

        Debug.Log(newMessage, context);
    }

    public static void LogWarning(object message, DebugColors color, Object context = default)
    {
#if !FULL_LOG
        return;
#endif
        string newMessage = FormatMessage(message, color);

        Debug.LogWarning(newMessage, context);
    }

    public static void LogError(object message, DebugColors color, Object context = default)
    {
#if FULL_LOG || (POST_BUILD_HASH && UNITY_EDITOR)
        string newMessage = FormatMessage(message, color);

        Debug.LogError(newMessage, context);
#endif
    }

    private static string GetColorName(DebugColors color)
    {
        switch (color)
        {
            case DebugColors.Black:
                return "black";
            case DebugColors.Blue:
                return "blue";
            case DebugColors.Cyan:
                return "cyan";
            case DebugColors.Green:
                return "green";
            case DebugColors.Gray:
                return "gray";
            case DebugColors.Magenta:
                return "magenta";
            case DebugColors.Red:
                return "red";
            case DebugColors.Yellow:
                return "yellow";
            case DebugColors.Teal:
                return "teal";
        }

        return null;
    }

    #endregion
}
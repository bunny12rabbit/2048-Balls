using UnityEditor;

public class NativeTechTools : EditorWindow
{
    public const string NATIVE_TECH_TOOLS = "Native Tech Tools";
    public const string NATIVE_TECH_TOOLS_SUB_MENU = "Native Tech Tools/";

    private const string SET_PASSWORD = "Set Password";
    private const string SWITCH_FULL_LOG = "Switch FULL_LOG";

    private const string FULL_LOG = "FULL_LOG";
    private static bool _isFullLog;

    private const string PASSWORD = "pass4root";


    [InitializeOnLoad]
    private class StartApp
    {
        static StartApp()
        {
            _isFullLog = ContainsDefine(FULL_LOG);

            SetPassword();
        }
    }

    #region SetPassword

    [MenuItem(NATIVE_TECH_TOOLS_SUB_MENU + SET_PASSWORD)]
    private static void SetPassword()
    {
        PlayerSettings.keyaliasPass = PASSWORD;
        PlayerSettings.keystorePass = PASSWORD;
    }

    #endregion

    #region Switch FULL_LOG

    [MenuItem(NATIVE_TECH_TOOLS_SUB_MENU + SWITCH_FULL_LOG)]
    private static void SwitchFullLog()
    {
        _isFullLog = !_isFullLog;

        if (_isFullLog)
        {
            AddNewDefine(FULL_LOG);
        }
        else
        {
            RemoveDefine(FULL_LOG);
        }
    }

    [MenuItem(NATIVE_TECH_TOOLS_SUB_MENU + SWITCH_FULL_LOG, true)]
    private static bool SwitchFullLogValidation()
    {
        Menu.SetChecked(NATIVE_TECH_TOOLS_SUB_MENU + SWITCH_FULL_LOG, _isFullLog);
        return true;
    }

    #endregion

    #region Defines helpers

    private static bool ContainsDefine(string define)
    {
        string defineString = GetDefineString();
        return defineString.Contains(define);
    }

    private static string GetDefineString()
    {
#if UNITY_ANDROID
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
#elif UNITY_IOS
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
#elif UNITY_STANDALONE
        return PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
#endif
        return string.Empty;
    }

    private static void AddNewDefine(string newDefine)
    {
        string defineString = GetDefineString();

        if (ContainsDefine(newDefine))
        {
            return;
        }

        string newDefineString = $"{defineString};{newDefine};";
#if UNITY_ANDROID
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, newDefineString);
#elif UNITY_IOS
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, newDefineString);
#elif UNITY_STANDALONE
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, newDefineString);
#endif
    }

    private static void RemoveDefine(string defineToRemoved)
    {
        string defineString = GetDefineString();
        var defineArray = defineString.Split(';');
        var newDefineString = string.Empty;

        foreach (string define in defineArray)
        {
            if (!define.Equals(defineToRemoved))
            {
                newDefineString = newDefineString.Equals(string.Empty)
                    ? $"{define};"
                    : $"{newDefineString}{define};";
            }
        }
#if UNITY_ANDROID
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, newDefineString);
#elif UNITY_IOS
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, newDefineString);
#elif UNITY_STANDALONE
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, newDefineString);
#endif
    }

    #endregion
}
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// SceneViewWindow class.
/// </summary>
public class SceneViewWindow : EditorWindow
{
    private const string SCENE_VIEW = "Scene View";

    /// <summary>
    /// Tracks scroll position.
    /// </summary>
    private Vector2 _scrollPos;

    /// <summary>
    /// Initialize window state.
    /// </summary>
    [MenuItem(NativeTechTools.NATIVE_TECH_TOOLS_SUB_MENU + SCENE_VIEW)]
    internal static void Init()
    {
        // EditorWindow.GetWindow() will return the open instance of the specified window or create a new
        // instance if it can't find one. The second parameter is a flag for creating the window as a
        // Utility window; Utility windows cannot be docked like the Scene and Game view windows.
        var window = (SceneViewWindow)GetWindow(typeof(SceneViewWindow), false, "Scene View");
        window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 200f, 400f);
    }

    /// <summary>
    /// Called on GUI events.
    /// </summary>
    internal void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false);

        GUILayout.Label("Scenes In Build", EditorStyles.boldLabel);
        for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            var scene = EditorBuildSettings.scenes[i];
            if (!scene.enabled)
            {
                continue;
            }

            string sceneName = Path.GetFileNameWithoutExtension(scene.path);
            bool isPressed = GUILayout.Button($"{i}: {sceneName}",
                new GUIStyle(GUI.skin.GetStyle("Button")) {alignment = TextAnchor.MiddleLeft});

            if (isPressed)
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scene.path);
                }
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}
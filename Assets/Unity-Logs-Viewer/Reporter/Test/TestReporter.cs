#if UNITY_CHANGE1 || UNITY_CHANGE2 || UNITY_CHANGE3 || UNITY_CHANGE4
#warning UNITY_CHANGE has been set manually
#elif UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define UNITY_CHANGE1
#elif UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#define UNITY_CHANGE2
#else
#define UNITY_CHANGE3
#endif
#if UNITY_2018_3
#define UNITY_CHANGE4
#endif
//use UNITY_CHANGE1 for unity older than "unity 5"
//use UNITY_CHANGE2 for unity 5.0 -> 5.3 
//use UNITY_CHANGE3 for unity 5.3 (fix for new SceneManger system)
//use UNITY_CHANGE4 for unity 2018.3 (Networking system)

using UnityEngine;
using System.Collections;
using System.Threading;
#if UNITY_CHANGE3
using UnityEngine.SceneManagement;

#endif
#if UNITY_CHANGE4
using UnityEngine.Networking;
#endif


//this script used for test purpose ,it do by default 100 logs  + 100 warnings + 100 errors
//so you can check the functionality of in game logs
//just drop this scrip to any empty game object on first scene your game start at
public class TestReporter : MonoBehaviour
{
    public int logTestCount = 100;
    public int threadLogTestCount = 100;
    public bool logEverySecond = true;
    
    private int _currentLogTestCount;
    private Reporter _reporter;
    private GUIStyle _style;
    private Rect _rect1;
    private Rect _rect2;
    private Rect _rect3;
    private Rect _rect4;
    private Rect _rect5;
    private Rect _rect6;

    private Thread _thread;

    private void Start()
    {
        Application.runInBackground = true;

        _reporter = FindObjectOfType(typeof(Reporter)) as Reporter;
        Debug.Log("test long text sdf asdfg asdfg sdfgsdfg sdfg sfg" +
                  "sdfgsdfg sdfg sdf gsdfg sfdg sf gsdfg sdfg asdfg " +
                  "sdfgsdfg sdfg sdf gsdfg sfdg sf gsdfg sdfg asdfg " +
                  "sdfgsdfg sdfg sdf gsdfg sfdg sf gsdfg sdfg asdfg " +
                  "sdfgsdfg sdfg sdf gsdfg sfdg sf gsdfg sdfg asdfg " +
                  "sdfgsdfg sdfg sdf gsdfg sfdg sf gsdfg sdfg asdfg ssssssssssssssssssssss" +
                  "asdf asdf asdf asdf adsf \n dfgsdfg sdfg sdf gsdfg sfdg sf gsdfg sdfg asdf" +
                  "asdf asdf asdf asdf adsf \n dfgsdfg sdfg sdf gsdfg sfdg sf gsdfg sdfg asdf" +
                  "asdf asdf asdf asdf adsf \n dfgsdfg sdfg sdf gsdfg sfdg sf gsdfg sdfg asdf" +
                  "asdf asdf asdf asdf adsf \n dfgsdfg sdfg sdf gsdfg sfdg sf gsdfg sdfg asdf" +
                  "asdf asdf asdf asdf adsf \n dfgsdfg sdfg sdf gsdfg sfdg sf gsdfg sdfg asdf");

        _style = new GUIStyle();
        _style.alignment = TextAnchor.MiddleCenter;
        _style.normal.textColor = Color.white;
        _style.wordWrap = true;

        for (int i = 0; i < 10; i++)
        {
            Debug.Log("Test Collapsed log");
            Debug.LogWarning("Test Collapsed Warning");
            Debug.LogError("Test Collapsed Error");
        }

        for (int i = 0; i < 10; i++)
        {
            Debug.Log("Test Collapsed log");
            Debug.LogWarning("Test Collapsed Warning");
            Debug.LogError("Test Collapsed Error");
        }

        _rect1 = new Rect(Screen.width / 2 - 120, Screen.height / 2 - 225, 240, 50);
        _rect2 = new Rect(Screen.width / 2 - 120, Screen.height / 2 - 175, 240, 100);
        _rect3 = new Rect(Screen.width / 2 - 120, Screen.height / 2 - 50, 240, 50);
        _rect4 = new Rect(Screen.width / 2 - 120, Screen.height / 2, 240, 50);
        _rect5 = new Rect(Screen.width / 2 - 120, Screen.height / 2 + 50, 240, 50);
        _rect6 = new Rect(Screen.width / 2 - 120, Screen.height / 2 + 100, 240, 50);

        _thread = new Thread(new ThreadStart(ThreadLogTest));
        _thread.Start();
    }

    private void OnDestroy()
    {
        _thread.Abort();
    }

    private void ThreadLogTest()
    {
        for (int i = 0; i < threadLogTestCount; i++)
        {
            Debug.Log("Test Log from Thread");
            Debug.LogWarning("Test Warning from Thread");
            Debug.LogError("Test Error from Thread");
        }
    }

    private float _elapsed;

    private void Update()
    {
        int drawn = 0;
        while (_currentLogTestCount < logTestCount && drawn < 10)
        {
            Debug.Log("Test Log " + _currentLogTestCount);
            Debug.LogError("Test LogError " + _currentLogTestCount);
            Debug.LogWarning("Test LogWarning " + _currentLogTestCount);
            drawn++;
            _currentLogTestCount++;
        }

        _elapsed += Time.deltaTime;
        if (_elapsed >= 1)
        {
            _elapsed = 0;
            Debug.Log("One Second Passed");
        }
    }

    private void OnGUI()
    {
        if (_reporter && !_reporter.show)
        {
            GUI.Label(_rect1, "Draw circle on screen to show logs", _style);
            GUI.Label(_rect2, "To use Reporter just create reporter from reporter menu at first scene your game start",
                _style);
            if (GUI.Button(_rect3, "Load ReporterScene"))
            {
#if UNITY_CHANGE3
                SceneManager.LoadScene("ReporterScene");
#else
				Application.LoadLevel("ReporterScene");
#endif
            }

            if (GUI.Button(_rect4, "Load test1"))
            {
#if UNITY_CHANGE3
                SceneManager.LoadScene("test1");
#else
				Application.LoadLevel("test1");
#endif
            }

            if (GUI.Button(_rect5, "Load test2"))
            {
#if UNITY_CHANGE3
                SceneManager.LoadScene("test2");
#else
				Application.LoadLevel("test2");
#endif
            }

            GUI.Label(_rect6, "fps : " + _reporter.fps.ToString("0.0"), _style);
        }
    }
}
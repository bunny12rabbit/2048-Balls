#if UNITY_CHANGE1 || UNITY_CHANGE2 || UNITY_CHANGE3 || UNITY_CHANGE4
#warning UNITY_CHANGE has been set manually
#elif UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define UNITY_CHANGE1
#elif UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#define UNITY_CHANGE2
#else
#define UNITY_CHANGE3
#endif
#if UNITY_2018_3_OR_NEWER
#define UNITY_CHANGE4
#endif
//use UNITY_CHANGE1 for unity older than "unity 5"
//use UNITY_CHANGE2 for unity 5.0 -> 5.3 
//use UNITY_CHANGE3 for unity 5.3 (fix for new SceneManger system)
//use UNITY_CHANGE4 for unity 2018.3 (Networking system)

#if UNITY_CHANGE3
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
#endif
#if UNITY_CHANGE4
using UnityEngine.Networking;

#endif


[Serializable]
public class Images
{
    public Texture2D clearImage;
    public Texture2D collapseImage;
    public Texture2D clearOnNewSceneImage;
    public Texture2D showTimeImage;
    public Texture2D showSceneImage;
    public Texture2D userImage;
    public Texture2D showMemoryImage;
    public Texture2D softwareImage;
    public Texture2D dateImage;
    public Texture2D showFpsImage;
    public Texture2D infoImage;
    public Texture2D saveLogsImage;
    public Texture2D searchImage;
    public Texture2D copyImage;
    public Texture2D closeImage;

    public Texture2D buildFromImage;
    public Texture2D systemInfoImage;
    public Texture2D graphicsInfoImage;
    public Texture2D backImage;

    public Texture2D logImage;
    public Texture2D warningImage;
    public Texture2D errorImage;

    public Texture2D barImage;
    public Texture2D button_activeImage;
    public Texture2D even_logImage;
    public Texture2D odd_logImage;
    public Texture2D selectedImage;

    public GUISkin reporterScrollerSkin;
}

//To use Reporter just create reporter from menu (Reporter->Create) at first scene your game start.
//then set the ” Scrip execution order ” in (Edit -> Project Settings ) of Reporter.cs to be the highest.

//Now to view logs all what you have to do is press a BackQuote (`) button on keyboard if standalone, 
//or make a Zoom gesture using two fingers on the screen to show all these logs
//no coding is required 

public class Reporter : MonoBehaviour
{
    public class Sample
    {
        public float time;
        public byte loadedScene;
        public float memory;
        public float fps;
        public string fpsText;

        public static float MemSize()
        {
            float s = sizeof(float) + sizeof(byte) + sizeof(float) + sizeof(float);
            return s;
        }

        public string GetSceneName()
        {
            if (loadedScene == 255)
            {
                return "AssetBundleScene";
            }

            return _scenes[loadedScene];
        }
    }

    private List<Sample> _samples = new List<Sample>();

    public class Log
    {
        public int count = 1;
        public LogType logType;
        public string condition;
        public string stacktrace;

        public int sampleId;
        //public string   objectName="" ;//object who send error
        //public string   rootName =""; //root of object send error

        public Log CreateCopy()
        {
            return (Log) MemberwiseClone();
        }

        public float GetMemoryUsage()
        {
            return sizeof(int) +
                   sizeof(LogType) +
                   condition.Length * sizeof(char) +
                   stacktrace.Length * sizeof(char) +
                   sizeof(int);
        }
    }

    public static Reporter Instance { get; private set; }

    public Action onErrorLogRaised;

    //contains all uncollapsed log
    private List<Log> _logs = new List<Log>();

    //contains all collapsed logs
    private List<Log> _collapsedLogs = new List<Log>();

    //contain logs which should only appear to user , for example if you switch off show logs + switch off show warnings
    //and your mode is collapse,then this list will contains only collapsed errors
    private List<Log> _currentLog = new List<Log>();

    //used to check if the new coming logs is already exist or new one
    private MultiKeyDictionary<string, string, Log> _logsDic = new MultiKeyDictionary<string, string, Log>();

    //to save memory
    private Dictionary<string, string> _cachedString = new Dictionary<string, string>();

    [HideInInspector]
    //show hide In Game Logs
    public bool show;

    //collapse logs
    private bool _collapse;

    //to decide if you want to clean logs for new loaded scene
    private bool _clearOnNewSceneLoaded;

    private bool _showTime;

    private bool _showScene;

    private bool _showMemory;

    private bool _showFps;

    private bool _showGraph;

    //show or hide logs
    private bool _showLog = true;

    //show or hide warnings
    private bool _showWarning = true;

    //show or hide errors
    private bool _showError = true;

    //total number of logs
    private int _numOfLogs;

    //total number of warnings
    private int _numOfLogsWarning;

    //total number of errors
    private int _numOfLogsError;

    //total number of collapsed logs
    private int _numOfCollapsedLogs;

    //total number of collapsed warnings
    private int _numOfCollapsedLogsWarning;

    //total number of collapsed errors
    private int _numOfCollapsedLogsError;

    //maximum number of allowed logs to view
    //public int maxAllowedLog = 1000 ;

    private bool _showClearOnNewSceneLoadedButton = true;
    private bool _showTimeButton = true;
    private bool _showSceneButton = true;
    private bool _showMemButton = true;
    private bool _showFpsButton = true;
    private bool _showSearchText = true;
    private bool _showCopyButton = true;
    private bool _showSaveButton = true;

    private string _buildDate;
    private string _logDate;
    private float _logsMemUsage;
    private float _graphMemUsage;

    public float TotalMemUsage => _logsMemUsage + _graphMemUsage;

    private float _gcTotalMemory;

    public string userData = "";

    //frame rate per second
    public float fps;
    public string fpsText;

    //List<Texture2D> snapshots = new List<Texture2D>() ;

    private enum ReportView
    {
        None,
        Logs,
        Info,
        Snapshot,
    }

    private ReportView _currentView = ReportView.Logs;

    private enum DetailView
    {
        None,
        StackTrace,
        Graph,
    }

    //used to check if you have In Game Logs multiple time in different scene
    //only one should work and other should be deleted
    private bool _created;
    //public delegate void OnLogHandler( string condition, string stack-trace, LogType type );
    //public event OnLogHandler OnLog ;

    public Images images;

    // gui
    private GUIContent _clearContent;
    private GUIContent _collapseContent;
    private GUIContent _clearOnNewSceneContent;
    private GUIContent _showTimeContent;
    private GUIContent _showSceneContent;
    private GUIContent _userContent;
    private GUIContent _showMemoryContent;
    private GUIContent _softwareContent;
    private GUIContent _dateContent;

    private GUIContent _showFpsContent;

    //GUIContent graphContent;
    private GUIContent _infoContent;
    private GUIContent _saveLogsContent;
    private GUIContent _searchContent;
    private GUIContent _copyContent;
    private GUIContent _closeContent;

    private GUIContent _buildFromContent;
    private GUIContent _systemInfoContent;
    private GUIContent _graphicsInfoContent;
    private GUIContent _backContent;

    //GUIContent cameraContent;

    private GUIContent _logContent;
    private GUIContent _warningContent;
    private GUIContent _errorContent;
    private GUIStyle _barStyle;
    private GUIStyle _buttonActiveStyle;

    private GUIStyle _nonStyle;
    private GUIStyle _lowerLeftFontStyle;
    private GUIStyle _backStyle;
    private GUIStyle _evenLogStyle;
    private GUIStyle _oddLogStyle;
    private GUIStyle _logButtonStyle;
    private GUIStyle _selectedLogStyle;
    private GUIStyle _selectedLogFontStyle;
    private GUIStyle _stackLabelStyle;
    private GUIStyle _scrollerStyle;
    private GUIStyle _searchStyle;
    private GUIStyle _sliderBackStyle;
    private GUIStyle _sliderThumbStyle;
    private GUISkin _toolbarScrollerSkin;
    private GUISkin _logScrollerSkin;
    private GUISkin _graphScrollerSkin;

    public Vector2 size = new Vector2(32, 32);
    public float maxSize = 20;
    private static string[] _scenes;
    private string _currentScene;
    private string _filterText = "";

    private string _deviceModel;
    private string _deviceType;
    private string _deviceName;
    private string _graphicsMemorySize;
#if !UNITY_CHANGE1
    private string _maxTextureSize;
#endif
    private string _systemMemorySize;

    private void Awake()
    {
        if (!Debug.isDebugBuild)
        {
            gameObject.SetActive(false);
            return;
        }
        
        if (!initialized)
        {
            Initialize();
        }

#if UNITY_CHANGE3
        SceneManager.sceneLoaded += _OnLevelWasLoaded;
#endif
    }

    public List<Log> GetLogTypeOf(LogType logType)
    {
        return _logs.Where(log => log.logType == logType).ToList();
    }

    private void OnDestroy()
    {
#if UNITY_CHANGE3
        SceneManager.sceneLoaded -= _OnLevelWasLoaded;
#endif
    }

    private void OnEnable()
    {
        if (_logs.Count == 0) //if recompile while in play mode
        {
            Clear();
        }
    }

    private void OnDisable()
    {
    }

    private void AddSample()
    {
        var sample = new Sample();
        sample.fps = fps;
        sample.fpsText = fpsText;
#if UNITY_CHANGE3
        sample.loadedScene = (byte) SceneManager.GetActiveScene().buildIndex;
#else
		sample.loadedScene = (byte)Application.loadedLevel;
#endif
        sample.time = Time.realtimeSinceStartup;
        sample.memory = _gcTotalMemory;
        _samples.Add(sample);

        _graphMemUsage = (_samples.Count * Sample.MemSize()) / 1024 / 1024;
    }

    public bool initialized;

    public void Initialize()
    {
        if (!_created)
        {
            try
            {
                Instance = this;
                gameObject.SendMessage("OnPreStart");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
#if UNITY_CHANGE3
            _scenes = new string[SceneManager.sceneCountInBuildSettings];
            _currentScene = SceneManager.GetActiveScene().name;
#else
			scenes = new string[Application.levelCount];
			currentScene = Application.loadedLevelName;
#endif
            DontDestroyOnLoad(gameObject);
#if UNITY_CHANGE1
			Application.RegisterLogCallback (new Application.LogCallback (CaptureLog));
			Application.RegisterLogCallbackThreaded (new Application.LogCallback (CaptureLogThread));
#else
            //Application.logMessageReceived += CaptureLog ;
            Application.logMessageReceivedThreaded += CaptureLogThread;
#endif
            _created = true;
            //addSample();
        }
        else
        {
            DebugWrapper.LogWarning("tow manager is exists delete the second");
            DestroyImmediate(gameObject, true);
            return;
        }


        //initialize gui and styles for gui purpose

        _clearContent = new GUIContent("", images.clearImage, "Clear logs");
        _collapseContent = new GUIContent("", images.collapseImage, "Collapse logs");
        _clearOnNewSceneContent = new GUIContent("", images.clearOnNewSceneImage, "Clear logs on new scene loaded");
        _showTimeContent = new GUIContent("", images.showTimeImage, "Show Hide Time");
        _showSceneContent = new GUIContent("", images.showSceneImage, "Show Hide Scene");
        _showMemoryContent = new GUIContent("", images.showMemoryImage, "Show Hide Memory");
        _softwareContent = new GUIContent("", images.softwareImage, "Software");
        _dateContent = new GUIContent("", images.dateImage, "Date");
        _showFpsContent = new GUIContent("", images.showFpsImage, "Show Hide fps");
        _infoContent = new GUIContent("", images.infoImage, "Information about application");
        _saveLogsContent = new GUIContent("", images.saveLogsImage, "Save logs to device");
        _searchContent = new GUIContent("", images.searchImage, "Search for logs");
        _copyContent = new GUIContent("", images.copyImage, "Copy log to clipboard");
        _closeContent = new GUIContent("", images.closeImage, "Hide logs");
        _userContent = new GUIContent("", images.userImage, "User");

        _buildFromContent = new GUIContent("", images.buildFromImage, "Build From");
        _systemInfoContent = new GUIContent("", images.systemInfoImage, "System Info");
        _graphicsInfoContent = new GUIContent("", images.graphicsInfoImage, "Graphics Info");
        _backContent = new GUIContent("", images.backImage, "Back");


        //snapshotContent = new GUIContent("",images.cameraImage,"show or hide logs");
        _logContent = new GUIContent("", images.logImage, "show or hide logs");
        _warningContent = new GUIContent("", images.warningImage, "show or hide warnings");
        _errorContent = new GUIContent("", images.errorImage, "show or hide errors");


        _currentView = (ReportView) PlayerPrefs.GetInt("Reporter_currentView", 1);
        show = (PlayerPrefs.GetInt("Reporter_show") == 1) ? true : false;
        _collapse = (PlayerPrefs.GetInt("Reporter_collapse") == 1) ? true : false;
        _clearOnNewSceneLoaded = (PlayerPrefs.GetInt("Reporter_clearOnNewSceneLoaded") == 1) ? true : false;
        _showTime = (PlayerPrefs.GetInt("Reporter_showTime") == 1) ? true : false;
        _showScene = (PlayerPrefs.GetInt("Reporter_showScene") == 1) ? true : false;
        _showMemory = (PlayerPrefs.GetInt("Reporter_showMemory") == 1) ? true : false;
        _showFps = (PlayerPrefs.GetInt("Reporter_showFps") == 1) ? true : false;
        _showGraph = (PlayerPrefs.GetInt("Reporter_showGraph") == 1) ? true : false;
        _showLog = (PlayerPrefs.GetInt("Reporter_showLog", 1) == 1) ? true : false;
        _showWarning = (PlayerPrefs.GetInt("Reporter_showWarning", 1) == 1) ? true : false;
        _showError = (PlayerPrefs.GetInt("Reporter_showError", 1) == 1) ? true : false;
        _filterText = PlayerPrefs.GetString("Reporter_filterText");
        size.x = size.y = PlayerPrefs.GetFloat("Reporter_size", 32);


        _showClearOnNewSceneLoadedButton =
            (PlayerPrefs.GetInt("Reporter_showClearOnNewSceneLoadedButton", 1) == 1) ? true : false;
        _showTimeButton = (PlayerPrefs.GetInt("Reporter_showTimeButton", 1) == 1) ? true : false;
        _showSceneButton = (PlayerPrefs.GetInt("Reporter_showSceneButton", 1) == 1) ? true : false;
        _showMemButton = (PlayerPrefs.GetInt("Reporter_showMemButton", 1) == 1) ? true : false;
        _showFpsButton = (PlayerPrefs.GetInt("Reporter_showFpsButton", 1) == 1) ? true : false;
        _showSearchText = (PlayerPrefs.GetInt("Reporter_showSearchText", 1) == 1) ? true : false;
        _showCopyButton = (PlayerPrefs.GetInt("Reporter_showCopyButton", 1) == 1) ? true : false;
        _showSaveButton = (PlayerPrefs.GetInt("Reporter_showSaveButton", 1) == 1) ? true : false;


        InitializeStyle();

        initialized = true;

        if (show)
        {
            DoShow();
        }

        _deviceModel = SystemInfo.deviceModel.ToString();
        _deviceType = SystemInfo.deviceType.ToString();
        _deviceName = SystemInfo.deviceName.ToString();
        _graphicsMemorySize = SystemInfo.graphicsMemorySize.ToString();
#if !UNITY_CHANGE1
        _maxTextureSize = SystemInfo.maxTextureSize.ToString();
#endif
        _systemMemorySize = SystemInfo.systemMemorySize.ToString();
    }

    private void InitializeStyle()
    {
        var paddingX = (int) (size.x * 0.2f);
        var paddingY = (int) (size.y * 0.2f);
        _nonStyle = new GUIStyle();
        _nonStyle.clipping = TextClipping.Clip;
        _nonStyle.border = new RectOffset(0, 0, 0, 0);
        _nonStyle.normal.background = null;
        _nonStyle.fontSize = (int) (size.y / 2);
        _nonStyle.alignment = TextAnchor.MiddleCenter;

        _lowerLeftFontStyle = new GUIStyle();
        _lowerLeftFontStyle.clipping = TextClipping.Clip;
        _lowerLeftFontStyle.border = new RectOffset(0, 0, 0, 0);
        _lowerLeftFontStyle.normal.background = null;
        _lowerLeftFontStyle.fontSize = (int) (size.y / 2);
        _lowerLeftFontStyle.fontStyle = FontStyle.Bold;
        _lowerLeftFontStyle.alignment = TextAnchor.LowerLeft;


        _barStyle = new GUIStyle();
        _barStyle.border = new RectOffset(1, 1, 1, 1);
        _barStyle.normal.background = images.barImage;
        _barStyle.active.background = images.button_activeImage;
        _barStyle.alignment = TextAnchor.MiddleCenter;
        _barStyle.margin = new RectOffset(1, 1, 1, 1);

        //barStyle.padding = new RectOffset(paddingX,paddingX,paddingY,paddingY); 
        //barStyle.wordWrap = true ;
        _barStyle.clipping = TextClipping.Clip;
        _barStyle.fontSize = (int) (size.y / 2);


        _buttonActiveStyle = new GUIStyle();
        _buttonActiveStyle.border = new RectOffset(1, 1, 1, 1);
        _buttonActiveStyle.normal.background = images.button_activeImage;
        _buttonActiveStyle.alignment = TextAnchor.MiddleCenter;
        _buttonActiveStyle.margin = new RectOffset(1, 1, 1, 1);
        //buttonActiveStyle.padding = new RectOffset(4,4,4,4);
        _buttonActiveStyle.fontSize = (int) (size.y / 2);

        _backStyle = new GUIStyle();
        _backStyle.normal.background = images.even_logImage;
        _backStyle.clipping = TextClipping.Clip;
        _backStyle.fontSize = (int) (size.y / 2);

        _evenLogStyle = new GUIStyle();
        _evenLogStyle.normal.background = images.even_logImage;
        _evenLogStyle.fixedHeight = size.y;
        _evenLogStyle.clipping = TextClipping.Clip;
        _evenLogStyle.alignment = TextAnchor.UpperLeft;
        _evenLogStyle.imagePosition = ImagePosition.ImageLeft;
        _evenLogStyle.fontSize = (int) (size.y / 2);
        //evenLogStyle.wordWrap = true;

        _oddLogStyle = new GUIStyle();
        _oddLogStyle.normal.background = images.odd_logImage;
        _oddLogStyle.fixedHeight = size.y;
        _oddLogStyle.clipping = TextClipping.Clip;
        _oddLogStyle.alignment = TextAnchor.UpperLeft;
        _oddLogStyle.imagePosition = ImagePosition.ImageLeft;
        _oddLogStyle.fontSize = (int) (size.y / 2);
        //oddLogStyle.wordWrap = true ;

        _logButtonStyle = new GUIStyle();
        //logButtonStyle.wordWrap = true;
        _logButtonStyle.fixedHeight = size.y;
        _logButtonStyle.clipping = TextClipping.Clip;
        _logButtonStyle.alignment = TextAnchor.UpperLeft;
        //logButtonStyle.imagePosition = ImagePosition.ImageLeft ;
        //logButtonStyle.wordWrap = true;
        _logButtonStyle.fontSize = (int) (size.y / 2);
        _logButtonStyle.padding = new RectOffset(paddingX, paddingX, paddingY, paddingY);

        _selectedLogStyle = new GUIStyle();
        _selectedLogStyle.normal.background = images.selectedImage;
        _selectedLogStyle.fixedHeight = size.y;
        _selectedLogStyle.clipping = TextClipping.Clip;
        _selectedLogStyle.alignment = TextAnchor.UpperLeft;
        _selectedLogStyle.normal.textColor = Color.white;
        //selectedLogStyle.wordWrap = true;
        _selectedLogStyle.fontSize = (int) (size.y / 2);

        _selectedLogFontStyle = new GUIStyle();
        _selectedLogFontStyle.normal.background = images.selectedImage;
        _selectedLogFontStyle.fixedHeight = size.y;
        _selectedLogFontStyle.clipping = TextClipping.Clip;
        _selectedLogFontStyle.alignment = TextAnchor.UpperLeft;
        _selectedLogFontStyle.normal.textColor = Color.white;
        //selectedLogStyle.wordWrap = true;
        _selectedLogFontStyle.fontSize = (int) (size.y / 2);
        _selectedLogFontStyle.padding = new RectOffset(paddingX, paddingX, paddingY, paddingY);

        _stackLabelStyle = new GUIStyle();
        _stackLabelStyle.wordWrap = true;
        _stackLabelStyle.fontSize = (int) (size.y / 2);
        _stackLabelStyle.padding = new RectOffset(paddingX, paddingX, paddingY, paddingY);

        _scrollerStyle = new GUIStyle();
        _scrollerStyle.normal.background = images.barImage;

        _searchStyle = new GUIStyle();
        _searchStyle.clipping = TextClipping.Clip;
        _searchStyle.alignment = TextAnchor.LowerCenter;
        _searchStyle.fontSize = (int) (size.y / 2);
        _searchStyle.wordWrap = true;


        _sliderBackStyle = new GUIStyle();
        _sliderBackStyle.normal.background = images.barImage;
        _sliderBackStyle.fixedHeight = size.y;
        _sliderBackStyle.border = new RectOffset(1, 1, 1, 1);

        _sliderThumbStyle = new GUIStyle();
        _sliderThumbStyle.normal.background = images.selectedImage;
        _sliderThumbStyle.fixedWidth = size.x;

        var skin = images.reporterScrollerSkin;

        _toolbarScrollerSkin = Instantiate(skin);
        _toolbarScrollerSkin.verticalScrollbar.fixedWidth = 0f;
        _toolbarScrollerSkin.horizontalScrollbar.fixedHeight = 0f;
        _toolbarScrollerSkin.verticalScrollbarThumb.fixedWidth = 0f;
        _toolbarScrollerSkin.horizontalScrollbarThumb.fixedHeight = 0f;

        _logScrollerSkin = Instantiate(skin);
        _logScrollerSkin.verticalScrollbar.fixedWidth = size.x * 2f;
        _logScrollerSkin.horizontalScrollbar.fixedHeight = 0f;
        _logScrollerSkin.verticalScrollbarThumb.fixedWidth = size.x * 2f;
        _logScrollerSkin.horizontalScrollbarThumb.fixedHeight = 0f;

        _graphScrollerSkin = Instantiate(skin);
        _graphScrollerSkin.verticalScrollbar.fixedWidth = 0f;
        _graphScrollerSkin.horizontalScrollbar.fixedHeight = size.x * 2f;
        _graphScrollerSkin.verticalScrollbarThumb.fixedWidth = 0f;
        _graphScrollerSkin.horizontalScrollbarThumb.fixedHeight = size.x * 2f;
        //inGameLogsScrollerSkin.verticalScrollbarThumb.fixedWidth = size.x * 2;
        //inGameLogsScrollerSkin.verticalScrollbar.fixedWidth = size.x * 2;
    }

    private void Start()
    {
        _logDate = DateTime.Now.ToString();
        StartCoroutine("ReadInfo");
    }

    //clear all logs
    private void Clear()
    {
        _logs.Clear();
        _collapsedLogs.Clear();
        _currentLog.Clear();
        _logsDic.Clear();
        //selectedIndex = -1;
        _selectedLog = null;
        _numOfLogs = 0;
        _numOfLogsWarning = 0;
        _numOfLogsError = 0;
        _numOfCollapsedLogs = 0;
        _numOfCollapsedLogsWarning = 0;
        _numOfCollapsedLogsError = 0;
        _logsMemUsage = 0;
        _graphMemUsage = 0;
        _samples.Clear();
        GC.Collect();
        _selectedLog = null;
    }

    private Rect _screenRect = Rect.zero;
    private Rect _toolBarRect = Rect.zero;
    private Rect _logsRect = Rect.zero;
    private Rect _stackRect = Rect.zero;
    private Rect _graphRect = Rect.zero;
    private Rect _graphMinRect = Rect.zero;
    private Rect _graphMaxRect = Rect.zero;
    private Rect _buttomRect = Rect.zero;
    private Vector2 _stackRectTopLeft;
    private Rect _detailRect = Rect.zero;

    private Vector2 _scrollPosition;
    private Vector2 _scrollPosition2;
    private Vector2 _toolbarScrollPosition;

    //int 	selectedIndex = -1;
    private Log _selectedLog;

    private float _toolbarOldDrag;
    private float _oldDrag;
    private float _oldDrag2;
    private float _oldDrag3;
    private int _startIndex;

    //calculate what is the currentLog : collapsed or not , hide or view warnings ......
    private void CalculateCurrentLog()
    {
        bool filter = !string.IsNullOrEmpty(this._filterText);
        var filterText = "";
        if (filter)
        {
            filterText = this._filterText.ToLower();
        }

        _currentLog.Clear();
        if (_collapse)
        {
            for (var i = 0; i < _collapsedLogs.Count; i++)
            {
                var log = _collapsedLogs[i];
                if (log.logType == LogType.Log && !_showLog)
                {
                    continue;
                }

                if (log.logType == LogType.Warning && !_showWarning)
                {
                    continue;
                }

                if (log.logType == LogType.Error && !_showError)
                {
                    continue;
                }

                if (log.logType == LogType.Assert && !_showError)
                {
                    continue;
                }

                if (log.logType == LogType.Exception && !_showError)
                {
                    continue;
                }

                if (filter)
                {
                    if (log.condition.ToLower().Contains(filterText))
                    {
                        _currentLog.Add(log);
                    }
                }
                else
                {
                    _currentLog.Add(log);
                }
            }
        }
        else
        {
            for (var i = 0; i < _logs.Count; i++)
            {
                var log = _logs[i];
                if (log.logType == LogType.Log && !_showLog)
                {
                    continue;
                }

                if (log.logType == LogType.Warning && !_showWarning)
                {
                    continue;
                }

                if (log.logType == LogType.Error && !_showError)
                {
                    continue;
                }

                if (log.logType == LogType.Assert && !_showError)
                {
                    continue;
                }

                if (log.logType == LogType.Exception && !_showError)
                {
                    continue;
                }

                if (filter)
                {
                    if (log.condition.ToLower().Contains(filterText))
                    {
                        _currentLog.Add(log);
                    }
                }
                else
                {
                    _currentLog.Add(log);
                }
            }
        }

        if (_selectedLog != null)
        {
            int newSelectedIndex = _currentLog.IndexOf(_selectedLog);
            if (newSelectedIndex == -1)
            {
                var collapsedSelected = _logsDic[_selectedLog.condition][_selectedLog.stacktrace];
                newSelectedIndex = _currentLog.IndexOf(collapsedSelected);
                if (newSelectedIndex != -1)
                {
                    _scrollPosition.y = newSelectedIndex * size.y;
                }
            }
            else
            {
                _scrollPosition.y = newSelectedIndex * size.y;
            }
        }
    }

    private Rect _countRect = Rect.zero;
    private Rect _timeRect = Rect.zero;
    private Rect _timeLabelRect = Rect.zero;
    private Rect _sceneRect = Rect.zero;
    private Rect _sceneLabelRect = Rect.zero;
    private Rect _memoryRect = Rect.zero;
    private Rect _memoryLabelRect = Rect.zero;
    private Rect _fpsRect = Rect.zero;
    private Rect _fpsLabelRect = Rect.zero;
    private GUIContent _tempContent = new GUIContent();


    private Vector2 _infoScrollPosition;
    private Vector2 _oldInfoDrag;

    private void DrawInfo()
    {
        GUILayout.BeginArea(_screenRect, _backStyle);

        var drag = GetDrag();
        if ((drag.x != 0) && (_downPos != Vector2.zero))
        {
            _infoScrollPosition.x -= (drag.x - _oldInfoDrag.x);
        }

        if ((drag.y != 0) && (_downPos != Vector2.zero))
        {
            _infoScrollPosition.y += (drag.y - _oldInfoDrag.y);
        }

        _oldInfoDrag = drag;

        GUI.skin = _toolbarScrollerSkin;
        _infoScrollPosition = GUILayout.BeginScrollView(_infoScrollPosition);
        GUILayout.Space(this.size.x);
        GUILayout.BeginHorizontal();
        GUILayout.Space(this.size.x);
        GUILayout.Box(_buildFromContent, _nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
        GUILayout.Space(this.size.x);
        GUILayout.Label(_buildDate, _nonStyle, GUILayout.Height(this.size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(this.size.x);
        GUILayout.Box(_systemInfoContent, _nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
        GUILayout.Space(this.size.x);
        GUILayout.Label(_deviceModel, _nonStyle, GUILayout.Height(this.size.y));
        GUILayout.Space(this.size.x);
        GUILayout.Label(_deviceType, _nonStyle, GUILayout.Height(this.size.y));
        GUILayout.Space(this.size.x);
        GUILayout.Label(_deviceName, _nonStyle, GUILayout.Height(this.size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(this.size.x);
        GUILayout.Box(_graphicsInfoContent, _nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
        GUILayout.Space(this.size.x);
        GUILayout.Label(SystemInfo.graphicsDeviceName, _nonStyle, GUILayout.Height(this.size.y));
        GUILayout.Space(this.size.x);
        GUILayout.Label(_graphicsMemorySize, _nonStyle, GUILayout.Height(this.size.y));
#if !UNITY_CHANGE1
        GUILayout.Space(this.size.x);
        GUILayout.Label(_maxTextureSize, _nonStyle, GUILayout.Height(this.size.y));
#endif
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(this.size.x);
        GUILayout.Space(this.size.x);
        GUILayout.Space(this.size.x);
        GUILayout.Label("Screen Width " + Screen.width, _nonStyle, GUILayout.Height(this.size.y));
        GUILayout.Space(this.size.x);
        GUILayout.Label("Screen Height " + Screen.height, _nonStyle, GUILayout.Height(this.size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(this.size.x);
        GUILayout.Box(_showMemoryContent, _nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
        GUILayout.Space(this.size.x);
        GUILayout.Label(_systemMemorySize + " mb", _nonStyle, GUILayout.Height(this.size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(this.size.x);
        GUILayout.Space(this.size.x);
        GUILayout.Space(this.size.x);
        GUILayout.Label("Mem Usage Of Logs " + _logsMemUsage.ToString("0.000") + " mb", _nonStyle,
            GUILayout.Height(this.size.y));
        GUILayout.Space(this.size.x);
        //GUILayout.Label( "Mem Usage Of Graph " + graphMemUsage.ToString("0.000")  + " mb", nonStyle , GUILayout.Height(size.y));
        //GUILayout.Space( size.x);
        GUILayout.Label("GC Memory " + _gcTotalMemory.ToString("0.000") + " mb", _nonStyle,
            GUILayout.Height(this.size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(this.size.x);
        GUILayout.Box(_softwareContent, _nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
        GUILayout.Space(this.size.x);
        GUILayout.Label(SystemInfo.operatingSystem, _nonStyle, GUILayout.Height(this.size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Space(this.size.x);
        GUILayout.Box(_dateContent, _nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
        GUILayout.Space(this.size.x);
        GUILayout.Label(DateTime.Now.ToString(), _nonStyle, GUILayout.Height(this.size.y));
        GUILayout.Label(" - Application Started At " + _logDate, _nonStyle, GUILayout.Height(this.size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(this.size.x);
        GUILayout.Box(_showTimeContent, _nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
        GUILayout.Space(this.size.x);
        GUILayout.Label(Time.realtimeSinceStartup.ToString("000"), _nonStyle, GUILayout.Height(this.size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(this.size.x);
        GUILayout.Box(_showFpsContent, _nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
        GUILayout.Space(this.size.x);
        GUILayout.Label(fpsText, _nonStyle, GUILayout.Height(this.size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(this.size.x);
        GUILayout.Box(_userContent, _nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
        GUILayout.Space(this.size.x);
        GUILayout.Label(userData, _nonStyle, GUILayout.Height(this.size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(this.size.x);
        GUILayout.Box(_showSceneContent, _nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
        GUILayout.Space(this.size.x);
        GUILayout.Label(_currentScene, _nonStyle, GUILayout.Height(this.size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(this.size.x);
        GUILayout.Box(_showSceneContent, _nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
        GUILayout.Space(this.size.x);
        GUILayout.Label("Unity Version = " + Application.unityVersion, _nonStyle, GUILayout.Height(this.size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        /*GUILayout.BeginHorizontal();
        GUILayout.Space( size.x);
        GUILayout.Box( graphContent ,nonStyle ,  GUILayout.Width(size.x) , GUILayout.Height(size.y));
        GUILayout.Space( size.x);
        GUILayout.Label( "frame " + samples.Count , nonStyle , GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();*/

        drawInfo_enableDisableToolBarButtons();

        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.Space(this.size.x);
        GUILayout.Label("Size = " + this.size.x.ToString("0.0"), _nonStyle, GUILayout.Height(this.size.y));
        GUILayout.Space(this.size.x);
        float size = GUILayout.HorizontalSlider(this.size.x, 16, 64, _sliderBackStyle, _sliderThumbStyle,
            GUILayout.Width(Screen.width * 0.5f));
        if (this.size.x != size)
        {
            this.size.x = this.size.y = size;
            InitializeStyle();
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(this.size.x);
        if (GUILayout.Button(_backContent, _barStyle, GUILayout.Width(this.size.x * 2),
            GUILayout.Height(this.size.y * 2)))
        {
            _currentView = ReportView.Logs;
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }


    private void drawInfo_enableDisableToolBarButtons()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(size.x);
        GUILayout.Label("Hide or Show tool bar buttons", _nonStyle, GUILayout.Height(size.y));
        GUILayout.Space(size.x);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(size.x);

        if (GUILayout.Button(_clearOnNewSceneContent,
            (_showClearOnNewSceneLoadedButton) ? _buttonActiveStyle : _barStyle,
            GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            _showClearOnNewSceneLoadedButton = !_showClearOnNewSceneLoadedButton;
        }

        if (GUILayout.Button(_showTimeContent, (_showTimeButton) ? _buttonActiveStyle : _barStyle,
            GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            _showTimeButton = !_showTimeButton;
        }

        _tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label(_tempRect, Time.realtimeSinceStartup.ToString("0.0"), _lowerLeftFontStyle);
        if (GUILayout.Button(_showSceneContent, (_showSceneButton) ? _buttonActiveStyle : _barStyle,
            GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            _showSceneButton = !_showSceneButton;
        }

        _tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label(_tempRect, _currentScene, _lowerLeftFontStyle);
        if (GUILayout.Button(_showMemoryContent, (_showMemButton) ? _buttonActiveStyle : _barStyle,
            GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            _showMemButton = !_showMemButton;
        }

        _tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label(_tempRect, _gcTotalMemory.ToString("0.0"), _lowerLeftFontStyle);

        if (GUILayout.Button(_showFpsContent, (_showFpsButton) ? _buttonActiveStyle : _barStyle,
            GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            _showFpsButton = !_showFpsButton;
        }

        _tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label(_tempRect, fpsText, _lowerLeftFontStyle);
        /*if( GUILayout.Button( graphContent , (showGraph)?buttonActiveStyle:barStyle , GUILayout.Width(size.x*2) ,GUILayout.Height(size.y*2)))
        {
            showGraph = !showGraph ;
        }
        tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label( tempRect , samples.Count.ToString() , lowerLeftFontStyle );*/
        if (GUILayout.Button(_searchContent, (_showSearchText) ? _buttonActiveStyle : _barStyle,
            GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            _showSearchText = !_showSearchText;
        }

        if (GUILayout.Button(_copyContent, (_showCopyButton) ? _buttonActiveStyle : _barStyle,
            GUILayout.Width(size.x * 2),
            GUILayout.Height(size.y * 2)))
        {
            _showCopyButton = !_showCopyButton;
        }

        if (GUILayout.Button(_saveLogsContent, (_showSaveButton) ? _buttonActiveStyle : _barStyle,
            GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            _showSaveButton = !_showSaveButton;
        }

        _tempRect = GUILayoutUtility.GetLastRect();
        GUI.TextField(_tempRect, _filterText, _searchStyle);


        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private void DrawReport()
    {
        _screenRect.x = 0f;
        _screenRect.y = 0f;
        _screenRect.width = Screen.width;
        _screenRect.height = Screen.height;
        GUILayout.BeginArea(_screenRect, _backStyle);
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        /*GUILayout.Box( cameraContent ,nonStyle ,  GUILayout.Width(size.x) , GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();*/
        GUILayout.Label("Select Photo", _nonStyle, GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Coming Soon", _nonStyle, GUILayout.Height(size.y));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(_backContent, _barStyle, GUILayout.Width(size.x), GUILayout.Height(size.y)))
        {
            _currentView = ReportView.Logs;
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void DrawToolBar()
    {
        _toolBarRect.x = 0f;
        _toolBarRect.y = 0f;
        _toolBarRect.width = Screen.width;
        _toolBarRect.height = size.y * 2f;

        //toolbarScrollerSkin.verticalScrollbar.fixedWidth = 0f;
        //toolbarScrollerSkin.horizontalScrollbar.fixedHeight= 0f  ;

        GUI.skin = _toolbarScrollerSkin;
        var drag = GetDrag();
        if ((drag.x != 0) && (_downPos != Vector2.zero) && (_downPos.y > Screen.height - size.y * 2f))
        {
            _toolbarScrollPosition.x -= (drag.x - _toolbarOldDrag);
        }

        _toolbarOldDrag = drag.x;
        GUILayout.BeginArea(_toolBarRect);
        _toolbarScrollPosition = GUILayout.BeginScrollView(_toolbarScrollPosition);
        GUILayout.BeginHorizontal(_barStyle);

        if (GUILayout.Button(_clearContent, _barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            Clear();
        }

        if (GUILayout.Button(_collapseContent, (_collapse) ? _buttonActiveStyle : _barStyle,
            GUILayout.Width(size.x * 2),
            GUILayout.Height(size.y * 2)))
        {
            _collapse = !_collapse;
            CalculateCurrentLog();
        }

        if (_showClearOnNewSceneLoadedButton && GUILayout.Button(_clearOnNewSceneContent,
                (_clearOnNewSceneLoaded) ? _buttonActiveStyle : _barStyle, GUILayout.Width(size.x * 2),
                GUILayout.Height(size.y * 2)))
        {
            _clearOnNewSceneLoaded = !_clearOnNewSceneLoaded;
        }

        if (_showTimeButton && GUILayout.Button(_showTimeContent, (_showTime) ? _buttonActiveStyle : _barStyle,
                GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            _showTime = !_showTime;
        }

        if (_showSceneButton)
        {
            _tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(_tempRect, Time.realtimeSinceStartup.ToString("0.0"), _lowerLeftFontStyle);
            if (GUILayout.Button(_showSceneContent, (_showScene) ? _buttonActiveStyle : _barStyle,
                GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
            {
                _showScene = !_showScene;
            }

            _tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(_tempRect, _currentScene, _lowerLeftFontStyle);
        }

        if (_showMemButton)
        {
            if (GUILayout.Button(_showMemoryContent, (_showMemory) ? _buttonActiveStyle : _barStyle,
                GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
            {
                _showMemory = !_showMemory;
            }

            _tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(_tempRect, _gcTotalMemory.ToString("0.0"), _lowerLeftFontStyle);
        }

        if (_showFpsButton)
        {
            if (GUILayout.Button(_showFpsContent, (_showFps) ? _buttonActiveStyle : _barStyle,
                GUILayout.Width(size.x * 2),
                GUILayout.Height(size.y * 2)))
            {
                _showFps = !_showFps;
            }

            _tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(_tempRect, fpsText, _lowerLeftFontStyle);
        }
        /*if( GUILayout.Button( graphContent , (showGraph)?buttonActiveStyle:barStyle , GUILayout.Width(size.x*2) ,GUILayout.Height(size.y*2)))
        {
            showGraph = !showGraph ;
        }
        tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label( tempRect , samples.Count.ToString() , lowerLeftFontStyle );*/

        if (_showSearchText)
        {
            GUILayout.Box(_searchContent, _barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2));
            _tempRect = GUILayoutUtility.GetLastRect();
            string newFilterText = GUI.TextField(_tempRect, _filterText, _searchStyle);
            if (newFilterText != _filterText)
            {
                _filterText = newFilterText;
                CalculateCurrentLog();
            }
        }

        if (_showCopyButton)
        {
            if (GUILayout.Button(_copyContent, _barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
            {
                if (_selectedLog == null)
                {
                    GUIUtility.systemCopyBuffer = "No log selected";
                }
                else
                {
                    GUIUtility.systemCopyBuffer = _selectedLog.condition + Environment.NewLine +
                                                  Environment.NewLine + _selectedLog.stacktrace;
                }
            }
        }

        if (_showSaveButton)
        {
            if (GUILayout.Button(_saveLogsContent, _barStyle, GUILayout.Width(size.x * 2),
                GUILayout.Height(size.y * 2)))
            {
                SaveLogsToDevice();
            }
        }

        if (GUILayout.Button(_infoContent, _barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            _currentView = ReportView.Info;
        }


        GUILayout.FlexibleSpace();


        var logsText = " ";
        if (_collapse)
        {
            logsText += _numOfCollapsedLogs;
        }
        else
        {
            logsText += _numOfLogs;
        }

        var logsWarningText = " ";
        if (_collapse)
        {
            logsWarningText += _numOfCollapsedLogsWarning;
        }
        else
        {
            logsWarningText += _numOfLogsWarning;
        }

        var logsErrorText = " ";
        if (_collapse)
        {
            logsErrorText += _numOfCollapsedLogsError;
        }
        else
        {
            logsErrorText += _numOfLogsError;
        }

        GUILayout.BeginHorizontal((_showLog) ? _buttonActiveStyle : _barStyle);
        if (GUILayout.Button(_logContent, _nonStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            _showLog = !_showLog;
            CalculateCurrentLog();
        }

        if (GUILayout.Button(logsText, _nonStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            _showLog = !_showLog;
            CalculateCurrentLog();
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal((_showWarning) ? _buttonActiveStyle : _barStyle);
        if (GUILayout.Button(_warningContent, _nonStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            _showWarning = !_showWarning;
            CalculateCurrentLog();
        }

        if (GUILayout.Button(logsWarningText, _nonStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            _showWarning = !_showWarning;
            CalculateCurrentLog();
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal((_showError) ? _buttonActiveStyle : _nonStyle);
        if (GUILayout.Button(_errorContent, _nonStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            _showError = !_showError;
            CalculateCurrentLog();
        }

        if (GUILayout.Button(logsErrorText, _nonStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            _showError = !_showError;
            CalculateCurrentLog();
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button(_closeContent, _barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            show = false;
            var gui = gameObject.GetComponent<ReporterGUI>();
            DestroyImmediate(gui);

            try
            {
                gameObject.SendMessage("OnHideReporter");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }


        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }


    private Rect _tempRect;

    private void DrawLogs()
    {
        GUILayout.BeginArea(_logsRect, _backStyle);

        GUI.skin = _logScrollerSkin;
        //setStartPos();
        var drag = GetDrag();

        if (drag.y != 0 && _logsRect.Contains(new Vector2(_downPos.x, Screen.height - _downPos.y)))
        {
            _scrollPosition.y += (drag.y - _oldDrag);
        }

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

        _oldDrag = drag.y;


        var totalVisibleCount = (int) (Screen.height * 0.75f / size.y);
        int totalCount = _currentLog.Count;
        /*if( totalCount < 100 )
            inGameLogsScrollerSkin.verticalScrollbarThumb.fixedHeight = 0;
        else 
            inGameLogsScrollerSkin.verticalScrollbarThumb.fixedHeight = 64;*/

        totalVisibleCount = Mathf.Min(totalVisibleCount, totalCount - _startIndex);
        var index = 0;
        var beforeHeight = (int) (_startIndex * size.y);
        //selectedIndex = Mathf.Clamp( selectedIndex , -1 , totalCount -1);
        if (beforeHeight > 0)
        {
            //fill invisible gap before scroller to make proper scroller pos
            GUILayout.BeginHorizontal(GUILayout.Height(beforeHeight));
            GUILayout.Label("---");
            GUILayout.EndHorizontal();
        }

        int endIndex = _startIndex + totalVisibleCount;
        endIndex = Mathf.Clamp(endIndex, 0, totalCount);
        bool scrollerVisible = (totalVisibleCount < totalCount);
        for (int i = _startIndex; (_startIndex + index) < endIndex; i++)
        {
            if (i >= _currentLog.Count)
            {
                break;
            }

            var log = _currentLog[i];

            if (log.logType == LogType.Log && !_showLog)
            {
                continue;
            }

            if (log.logType == LogType.Warning && !_showWarning)
            {
                continue;
            }

            if (log.logType == LogType.Error && !_showError)
            {
                continue;
            }

            if (log.logType == LogType.Assert && !_showError)
            {
                continue;
            }

            if (log.logType == LogType.Exception && !_showError)
            {
                continue;
            }

            if (index >= totalVisibleCount)
            {
                break;
            }

            GUIContent content = null;
            if (log.logType == LogType.Log)
            {
                content = _logContent;
            }
            else if (log.logType == LogType.Warning)
            {
                content = _warningContent;
            }
            else
            {
                content = _errorContent;
            }
            //content.text = log.condition ;

            var currentLogStyle = ((_startIndex + index) % 2 == 0) ? _evenLogStyle : _oddLogStyle;
            if (log == _selectedLog)
            {
                //selectedLog = log ;
                currentLogStyle = _selectedLogStyle;
            }
            else
            {
            }

            _tempContent.text = log.count.ToString();
            var w = 0f;
            if (_collapse)
            {
                w = _barStyle.CalcSize(_tempContent).x + 3;
            }

            _countRect.x = Screen.width - w;
            _countRect.y = size.y * i;
            if (beforeHeight > 0)
            {
                _countRect.y += 8; //i will check later why
            }

            _countRect.width = w;
            _countRect.height = size.y;

            if (scrollerVisible)
            {
                _countRect.x -= size.x * 2;
            }

            var sample = _samples[log.sampleId];
            _fpsRect = _countRect;
            if (_showFps)
            {
                _tempContent.text = sample.fpsText;
                w = currentLogStyle.CalcSize(_tempContent).x + size.x;
                _fpsRect.x -= w;
                _fpsRect.width = size.x;
                _fpsLabelRect = _fpsRect;
                _fpsLabelRect.x += size.x;
                _fpsLabelRect.width = w - size.x;
            }


            _memoryRect = _fpsRect;
            if (_showMemory)
            {
                _tempContent.text = sample.memory.ToString("0.000");
                w = currentLogStyle.CalcSize(_tempContent).x + size.x;
                _memoryRect.x -= w;
                _memoryRect.width = size.x;
                _memoryLabelRect = _memoryRect;
                _memoryLabelRect.x += size.x;
                _memoryLabelRect.width = w - size.x;
            }

            _sceneRect = _memoryRect;
            if (_showScene)
            {
                _tempContent.text = sample.GetSceneName();
                w = currentLogStyle.CalcSize(_tempContent).x + size.x;
                _sceneRect.x -= w;
                _sceneRect.width = size.x;
                _sceneLabelRect = _sceneRect;
                _sceneLabelRect.x += size.x;
                _sceneLabelRect.width = w - size.x;
            }

            _timeRect = _sceneRect;
            if (_showTime)
            {
                _tempContent.text = sample.time.ToString("0.000");
                w = currentLogStyle.CalcSize(_tempContent).x + size.x;
                _timeRect.x -= w;
                _timeRect.width = size.x;
                _timeLabelRect = _timeRect;
                _timeLabelRect.x += size.x;
                _timeLabelRect.width = w - size.x;
            }


            GUILayout.BeginHorizontal(currentLogStyle);
            if (log == _selectedLog)
            {
                GUILayout.Box(content, _nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
                GUILayout.Label(log.condition, _selectedLogFontStyle);
                //GUILayout.FlexibleSpace();
                if (_showTime)
                {
                    GUI.Box(_timeRect, _showTimeContent, currentLogStyle);
                    GUI.Label(_timeLabelRect, sample.time.ToString("0.000"), currentLogStyle);
                }

                if (_showScene)
                {
                    GUI.Box(_sceneRect, _showSceneContent, currentLogStyle);
                    GUI.Label(_sceneLabelRect, sample.GetSceneName(), currentLogStyle);
                }

                if (_showMemory)
                {
                    GUI.Box(_memoryRect, _showMemoryContent, currentLogStyle);
                    GUI.Label(_memoryLabelRect, sample.memory.ToString("0.000") + " mb", currentLogStyle);
                }

                if (_showFps)
                {
                    GUI.Box(_fpsRect, _showFpsContent, currentLogStyle);
                    GUI.Label(_fpsLabelRect, sample.fpsText, currentLogStyle);
                }
            }
            else
            {
                if (GUILayout.Button(content, _nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y)))
                {
                    //selectedIndex = startIndex + index ;
                    _selectedLog = log;
                }

                if (GUILayout.Button(log.condition, _logButtonStyle))
                {
                    //selectedIndex = startIndex + index ;
                    _selectedLog = log;
                }

                //GUILayout.FlexibleSpace();
                if (_showTime)
                {
                    GUI.Box(_timeRect, _showTimeContent, currentLogStyle);
                    GUI.Label(_timeLabelRect, sample.time.ToString("0.000"), currentLogStyle);
                }

                if (_showScene)
                {
                    GUI.Box(_sceneRect, _showSceneContent, currentLogStyle);
                    GUI.Label(_sceneLabelRect, sample.GetSceneName(), currentLogStyle);
                }

                if (_showMemory)
                {
                    GUI.Box(_memoryRect, _showMemoryContent, currentLogStyle);
                    GUI.Label(_memoryLabelRect, sample.memory.ToString("0.000") + " mb", currentLogStyle);
                }

                if (_showFps)
                {
                    GUI.Box(_fpsRect, _showFpsContent, currentLogStyle);
                    GUI.Label(_fpsLabelRect, sample.fpsText, currentLogStyle);
                }
            }

            if (_collapse)
            {
                GUI.Label(_countRect, log.count.ToString(), _barStyle);
            }

            GUILayout.EndHorizontal();
            index++;
        }

        var afterHeight = (int) ((totalCount - (_startIndex + totalVisibleCount)) * size.y);
        if (afterHeight > 0)
        {
            //fill invisible gap after scroller to make proper scroller pos
            GUILayout.BeginHorizontal(GUILayout.Height(afterHeight));
            GUILayout.Label(" ");
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();

        _buttomRect.x = 0f;
        _buttomRect.y = Screen.height - size.y;
        _buttomRect.width = Screen.width;
        _buttomRect.height = size.y;

        if (_showGraph)
        {
            DrawGraph();
        }
        else
        {
            DrawStack();
        }
    }


    private float _graphSize = 4f;
    private int _startFrame;
    private int _currentFrame;
    private Vector3 _tempVector1;
    private Vector3 _tempVector2;
    private Vector2 _graphScrollerPos;
    private float _maxFpsValue;
    private float _minFpsValue;
    private float _maxMemoryValue;
    private float _minMemoryValue;

    private void DrawGraph()
    {
        _graphRect = _stackRect;
        _graphRect.height = Screen.height * 0.25f; //- size.y ;


        //startFrame = samples.Count - (int)(Screen.width / graphSize) ;
        //if( startFrame < 0 ) startFrame = 0 ;
        GUI.skin = _graphScrollerSkin;

        var drag = GetDrag();
        if (_graphRect.Contains(new Vector2(_downPos.x, Screen.height - _downPos.y)))
        {
            if (drag.x != 0)
            {
                _graphScrollerPos.x -= drag.x - _oldDrag3;
                _graphScrollerPos.x = Mathf.Max(0, _graphScrollerPos.x);
            }

            var p = _downPos;
            if (p != Vector2.zero)
            {
                _currentFrame = _startFrame + (int) (p.x / _graphSize);
            }
        }

        _oldDrag3 = drag.x;
        GUILayout.BeginArea(_graphRect, _backStyle);

        _graphScrollerPos = GUILayout.BeginScrollView(_graphScrollerPos);
        _startFrame = (int) (_graphScrollerPos.x / _graphSize);
        if (_graphScrollerPos.x >= (_samples.Count * _graphSize - Screen.width))
        {
            _graphScrollerPos.x += _graphSize;
        }

        GUILayout.Label(" ", GUILayout.Width(_samples.Count * _graphSize));
        GUILayout.EndScrollView();
        GUILayout.EndArea();
        _maxFpsValue = 0;
        _minFpsValue = 100000;
        _maxMemoryValue = 0;
        _minMemoryValue = 100000;
        for (var i = 0; i < Screen.width / _graphSize; i++)
        {
            int index = _startFrame + i;
            if (index >= _samples.Count)
            {
                break;
            }

            var s = _samples[index];
            if (_maxFpsValue < s.fps)
            {
                _maxFpsValue = s.fps;
            }

            if (_minFpsValue > s.fps)
            {
                _minFpsValue = s.fps;
            }

            if (_maxMemoryValue < s.memory)
            {
                _maxMemoryValue = s.memory;
            }

            if (_minMemoryValue > s.memory)
            {
                _minMemoryValue = s.memory;
            }
        }

        //GUI.BeginGroup(graphRect);


        if (_currentFrame != -1 && _currentFrame < _samples.Count)
        {
            var selectedSample = _samples[_currentFrame];
            GUILayout.BeginArea(_buttomRect, _backStyle);
            GUILayout.BeginHorizontal();

            GUILayout.Box(_showTimeContent, _nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Label(selectedSample.time.ToString("0.0"), _nonStyle);
            GUILayout.Space(size.x);

            GUILayout.Box(_showSceneContent, _nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Label(selectedSample.GetSceneName(), _nonStyle);
            GUILayout.Space(size.x);

            GUILayout.Box(_showMemoryContent, _nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Label(selectedSample.memory.ToString("0.000"), _nonStyle);
            GUILayout.Space(size.x);

            GUILayout.Box(_showFpsContent, _nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Label(selectedSample.fpsText, _nonStyle);
            GUILayout.Space(size.x);

            /*GUILayout.Box( graphContent ,nonStyle, GUILayout.Width(size.x) ,GUILayout.Height(size.y));
            GUILayout.Label( currentFrame.ToString() ,nonStyle  );*/
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        _graphMaxRect = _stackRect;
        _graphMaxRect.height = size.y;
        GUILayout.BeginArea(_graphMaxRect);
        GUILayout.BeginHorizontal();

        GUILayout.Box(_showMemoryContent, _nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.Label(_maxMemoryValue.ToString("0.000"), _nonStyle);

        GUILayout.Box(_showFpsContent, _nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.Label(_maxFpsValue.ToString("0.000"), _nonStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        _graphMinRect = _stackRect;
        _graphMinRect.y = _stackRect.y + _stackRect.height - size.y;
        _graphMinRect.height = size.y;
        GUILayout.BeginArea(_graphMinRect);
        GUILayout.BeginHorizontal();

        GUILayout.Box(_showMemoryContent, _nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));

        GUILayout.Label(_minMemoryValue.ToString("0.000"), _nonStyle);


        GUILayout.Box(_showFpsContent, _nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));

        GUILayout.Label(_minFpsValue.ToString("0.000"), _nonStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        //GUI.EndGroup();
    }

    private void DrawStack()
    {
        if (_selectedLog != null)
        {
            var drag = GetDrag();
            if (drag.y != 0 && _stackRect.Contains(new Vector2(_downPos.x, Screen.height - _downPos.y)))
            {
                _scrollPosition2.y += drag.y - _oldDrag2;
            }

            _oldDrag2 = drag.y;


            GUILayout.BeginArea(_stackRect, _backStyle);
            _scrollPosition2 = GUILayout.BeginScrollView(_scrollPosition2);
            Sample selectedSample = null;
            try
            {
                selectedSample = _samples[_selectedLog.sampleId];
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(_selectedLog.condition, _stackLabelStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(size.y * 0.25f);
            GUILayout.BeginHorizontal();
            GUILayout.Label(_selectedLog.stacktrace, _stackLabelStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(size.y);
            GUILayout.EndScrollView();
            GUILayout.EndArea();


            GUILayout.BeginArea(_buttomRect, _backStyle);
            GUILayout.BeginHorizontal();

            GUILayout.Box(_showTimeContent, _nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Label(selectedSample.time.ToString("0.000"), _nonStyle);
            GUILayout.Space(size.x);

            GUILayout.Box(_showSceneContent, _nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Label(selectedSample.GetSceneName(), _nonStyle);
            GUILayout.Space(size.x);

            GUILayout.Box(_showMemoryContent, _nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Label(selectedSample.memory.ToString("0.000"), _nonStyle);
            GUILayout.Space(size.x);

            GUILayout.Box(_showFpsContent, _nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Label(selectedSample.fpsText, _nonStyle);
            /*GUILayout.Space( size.x );
            GUILayout.Box( graphContent ,nonStyle, GUILayout.Width(size.x) ,GUILayout.Height(size.y));
            GUILayout.Label( selectedLog.sampleId.ToString() ,nonStyle  );*/
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        else
        {
            GUILayout.BeginArea(_stackRect, _backStyle);
            GUILayout.EndArea();
            GUILayout.BeginArea(_buttomRect, _backStyle);
            GUILayout.EndArea();
        }
    }


    public void OnGuiDraw()
    {
        if (!show)
        {
            return;
        }

        _screenRect.x = 0;
        _screenRect.y = 0;
        _screenRect.width = Screen.width;
        _screenRect.height = Screen.height;

        GetDownPos();


        _logsRect.x = 0f;
        _logsRect.y = size.y * 2f;
        _logsRect.width = Screen.width;
        _logsRect.height = Screen.height * 0.75f - size.y * 2f;

        _stackRectTopLeft.x = 0f;
        _stackRect.x = 0f;
        _stackRectTopLeft.y = Screen.height * 0.75f;
        _stackRect.y = Screen.height * 0.75f;
        _stackRect.width = Screen.width;
        _stackRect.height = Screen.height * 0.25f - size.y;


        _detailRect.x = 0f;
        _detailRect.y = Screen.height - size.y * 3;
        _detailRect.width = Screen.width;
        _detailRect.height = size.y * 3;

        if (_currentView == ReportView.Info)
        {
            DrawInfo();
        }
        else if (_currentView == ReportView.Logs)
        {
            DrawToolBar();
            DrawLogs();
        }
    }

    private bool IsGestureDone()
    {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return CheckMobileInput();
        }

        return Input.GetKeyDown(KeyCode.BackQuote);
    }

    private Vector2 _initialTouch0Position;
    private Vector2 _initialTouch1Position;
    private bool _zoom;

    private bool CheckMobileInput()
    {
        return Input.touchCount == 3;
    }

    private float GetScaleFactor(Vector2 position1, Vector2 position2, Vector2 oldPosition1, Vector2 oldPosition2)
    {
        float distance = Vector2.Distance(position1, position2);
        float oldDistance = Vector2.Distance(oldPosition1, oldPosition2);

        if (oldDistance == 0 || distance == 0)
        {
            return 1.0f;
        }

        return distance / oldDistance;
    }

    //calculate  pos of first click on screen
    private Vector2 _startPos;

    private Vector2 _downPos;

    private Vector2 GetDownPos()
    {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touches.Length == 1 && Input.touches[0].phase == TouchPhase.Began)
            {
                _downPos = Input.touches[0].position;
                return _downPos;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                _downPos.x = Input.mousePosition.x;
                _downPos.y = Input.mousePosition.y;
                return _downPos;
            }
        }

        return Vector2.zero;
    }
    //calculate drag amount , this is used for scrolling

    private Vector2 _mousePosition;

    private Vector2 GetDrag()
    {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touches.Length != 1)
            {
                return Vector2.zero;
            }

            return Input.touches[0].position - _downPos;
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                _mousePosition = Input.mousePosition;
                return _mousePosition - _downPos;
            }
            else
            {
                return Vector2.zero;
            }
        }
    }

    //calculate the start index of visible log
    private void CalculateStartIndex()
    {
        _startIndex = (int) (_scrollPosition.y / size.y);
        _startIndex = Mathf.Clamp(_startIndex, 0, _currentLog.Count);
    }

    // For FPS Counter
    private int _frames;
    private bool _firstTime = true;
    private float _lastUpdate;
    private const int RequiredFrames = 10;
    private const float UpdateInterval = 0.25f;

#if UNITY_CHANGE1
	float lastUpdate2 = 0;
#endif

    private void DoShow()
    {
        show = true;
        _currentView = ReportView.Logs;
        gameObject.AddComponent<ReporterGUI>();


        try
        {
            gameObject.SendMessage("OnShowReporter");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void Update()
    {
        fpsText = fps.ToString("0.000");
        _gcTotalMemory = (((float) GC.GetTotalMemory(false)) / 1024 / 1024);
        //addSample();

#if UNITY_CHANGE3
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex != -1 && string.IsNullOrEmpty(_scenes[sceneIndex]))
        {
            _scenes[SceneManager.GetActiveScene().buildIndex] = SceneManager.GetActiveScene().name;
        }
#else
		int sceneIndex = Application.loadedLevel;
		if (sceneIndex != -1 && string.IsNullOrEmpty(scenes[Application.loadedLevel]))
			scenes[Application.loadedLevel] = Application.loadedLevelName;
#endif

        CalculateStartIndex();
        if (!show && IsGestureDone())
        {
            DoShow();
        }


        if (_threadedLogs.Count > 0)
        {
            lock (_threadedLogs)
            {
                for (var i = 0; i < _threadedLogs.Count; i++)
                {
                    var l = _threadedLogs[i];
                    AddLog(l.condition, l.stacktrace, l.logType);
                }

                _threadedLogs.Clear();
            }
        }

#if UNITY_CHANGE1
		float elapsed2 = Time.realtimeSinceStartup - lastUpdate2;
		if (elapsed2 > 1) {
			lastUpdate2 = Time.realtimeSinceStartup;
			//be sure no body else take control of log 
			Application.RegisterLogCallback (new Application.LogCallback (CaptureLog));
			Application.RegisterLogCallbackThreaded (new Application.LogCallback (CaptureLogThread));
		}
#endif

        // FPS Counter
        if (_firstTime)
        {
            _firstTime = false;
            _lastUpdate = Time.realtimeSinceStartup;
            _frames = 0;
            return;
        }

        _frames++;
        float dt = Time.realtimeSinceStartup - _lastUpdate;
        if (dt > UpdateInterval && _frames > RequiredFrames)
        {
            fps = _frames / dt;
            _lastUpdate = Time.realtimeSinceStartup;
            _frames = 0;
        }
    }


    private void CaptureLog(string condition, string stacktrace, LogType type)
    {
        AddLog(condition, stacktrace, type);
    }

    private void AddLog(string logCondition, string logStacktrace, LogType type)
    {
        var memUsage = 0f;
        var condition = "";

        if (_cachedString.ContainsKey(logCondition))
        {
            condition = _cachedString[logCondition];
        }
        else
        {
            condition = logCondition;
            _cachedString.Add(condition, condition);
            memUsage += (string.IsNullOrEmpty(condition) ? 0 : condition.Length * sizeof(char));
            memUsage += IntPtr.Size;
        }

        var stacktrace = "";
        if (_cachedString.ContainsKey(logStacktrace))
        {
            stacktrace = _cachedString[logStacktrace];
        }
        else
        {
            stacktrace = logStacktrace;
            _cachedString.Add(stacktrace, stacktrace);
            memUsage += (string.IsNullOrEmpty(stacktrace) ? 0 : stacktrace.Length * sizeof(char));
            memUsage += IntPtr.Size;
        }

        var newLogAdded = false;

        AddSample();
        var log = new Log()
            {logType = type, condition = condition, stacktrace = stacktrace, sampleId = _samples.Count - 1};
        memUsage += log.GetMemoryUsage();
        //memUsage += samples.Count * 13 ;

        _logsMemUsage += memUsage / 1024 / 1024;

        if (TotalMemUsage > maxSize)
        {
            Clear();
            DebugWrapper.Log("Memory Usage Reach" + maxSize + " mb So It is Cleared");
            return;
        }

        var isNew = false;
        //string key = _condition;// + "_!_" + _stacktrace ;
        if (_logsDic.ContainsKey(condition, logStacktrace))
        {
            isNew = false;
            _logsDic[condition][logStacktrace].count++;
        }
        else
        {
            isNew = true;
            _collapsedLogs.Add(log);
            _logsDic[condition][logStacktrace] = log;

            switch (type)
            {
                case LogType.Log:
                    _numOfCollapsedLogs++;
                    break;
                case LogType.Warning:
                    _numOfCollapsedLogsWarning++;
                    break;
                default:
                    _numOfCollapsedLogsError++;
                    break;
            }
        }

        switch (type)
        {
            case LogType.Log:
                _numOfLogs++;
                break;
            case LogType.Warning:
                _numOfLogsWarning++;
                break;
            default:
                _numOfLogsError++;
                break;
        }


        _logs.Add(log);

        if (log.logType == LogType.Error || log.logType == LogType.Exception)
        {
            onErrorLogRaised?.Invoke();
        }

        if (!_collapse || isNew)
        {
            bool skip = log.logType == LogType.Log && !_showLog || log.logType == LogType.Warning && !_showWarning ||
                        log.logType == LogType.Error && !_showError || log.logType == LogType.Assert && !_showError ||
                        log.logType == LogType.Exception && !_showError;

            if (!skip)
            {
                if (string.IsNullOrEmpty(_filterText) || log.condition.ToLower().Contains(_filterText.ToLower()))
                {
                    _currentLog.Add(log);
                    newLogAdded = true;
                }
            }
        }

        if (newLogAdded)
        {
            CalculateStartIndex();
            int totalCount = _currentLog.Count;
            var totalVisibleCount = (int) (Screen.height * 0.75f / size.y);
            if (_startIndex >= (totalCount - totalVisibleCount))
            {
                _scrollPosition.y += size.y;
            }
        }

        try
        {
            gameObject.SendMessage("OnLog", log);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private List<Log> _threadedLogs = new List<Log>();

    private void CaptureLogThread(string condition, string stacktrace, LogType type)
    {
        var log = new Log() {condition = condition, stacktrace = stacktrace, logType = type};
        lock (_threadedLogs)
        {
            _threadedLogs.Add(log);
        }
    }

#if !UNITY_CHANGE3
    class Scene
    {
    }
    class LoadSceneMode
    {
    }
    void OnLevelWasLoaded()
    {
        _OnLevelWasLoaded( null );
    }
#endif
    //new scene is loaded
    private void _OnLevelWasLoaded(Scene null1, LoadSceneMode null2)
    {
        if (_clearOnNewSceneLoaded)
        {
            Clear();
        }

#if UNITY_CHANGE3
        _currentScene = SceneManager.GetActiveScene().name;
        DebugWrapper.Log("Scene " + SceneManager.GetActiveScene().name + " is loaded");
#else
		currentScene = Application.loadedLevelName;
		DebugWrapper.Log("Scene " + Application.loadedLevelName + " is loaded");
#endif
    }

    //save user config
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("Reporter_currentView", (int) _currentView);
        PlayerPrefs.SetInt("Reporter_show", (show == true) ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_collapse", (_collapse == true) ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_clearOnNewSceneLoaded", (_clearOnNewSceneLoaded == true) ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showTime", (_showTime == true) ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showScene", (_showScene == true) ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showMemory", (_showMemory == true) ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showFps", (_showFps == true) ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showGraph", (_showGraph == true) ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showLog", (_showLog == true) ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showWarning", (_showWarning == true) ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showError", (_showError == true) ? 1 : 0);
        PlayerPrefs.SetString("Reporter_filterText", _filterText);
        PlayerPrefs.SetFloat("Reporter_size", size.x);

        PlayerPrefs.SetInt("Reporter_showClearOnNewSceneLoadedButton",
            (_showClearOnNewSceneLoadedButton == true) ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showTimeButton", (_showTimeButton == true) ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showSceneButton", (_showSceneButton == true) ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showMemButton", (_showMemButton == true) ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showFpsButton", (_showFpsButton == true) ? 1 : 0);
        PlayerPrefs.SetInt("Reporter_showSearchText", (_showSearchText == true) ? 1 : 0);

        PlayerPrefs.Save();
    }

    //read build information 
    private IEnumerator ReadInfo()
    {
        var prefFile = "build_info";
        string url = prefFile;

        if (prefFile.IndexOf("://") == -1)
        {
            string streamingAssetsPath = Application.streamingAssetsPath;
            if (streamingAssetsPath == "")
            {
                streamingAssetsPath = Application.dataPath + "/StreamingAssets/";
            }

            url = Path.Combine(streamingAssetsPath, prefFile);
        }

        //if (Application.platform != RuntimePlatform.OSXWebPlayer && Application.platform != RuntimePlatform.WindowsWebPlayer)
        if (!url.Contains("://"))
        {
            url = "file://" + url;
        }


        // float startTime = Time.realtimeSinceStartup;
#if UNITY_CHANGE4
        var www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
#else
		WWW www = new WWW(url);
		yield return www;
#endif

        if (!string.IsNullOrEmpty(www.error))
        {
            DebugWrapper.LogError(www.error);
        }
        else
        {
#if UNITY_CHANGE4
            _buildDate = www.downloadHandler.text;
#else
			buildDate = www.text;
#endif
        }

        yield break;
    }

    private void SaveLogsToDevice()
    {
        string filePath = Application.persistentDataPath + "/logs.txt";
        var fileContentsList = new List<string>();
        DebugWrapper.Log("Saving logs to " + filePath);
        File.Delete(filePath);

        foreach (var log in _logs)
        {
            fileContentsList.Add(log.logType + "\n" + log.condition + "\n" + log.stacktrace);
        }

        File.WriteAllLines(filePath, fileContentsList.ToArray());
    }
}
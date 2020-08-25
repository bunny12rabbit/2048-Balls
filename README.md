# 2048-Balls
The pet-project for showcase, due to all working projects is under NDA.
Used:
- Unity 2019.4.5.f1;
- UnityUI system;
- GitHub repository;
- Surge TweenEngine (http://surge.pixelplacement.com/tween.html)
- 2D Dev Sprites from AssetStore to fasten prototyping.

Target archicture: Android.

The game starts from AppStart scene. To add more comfort using Editor, i've created "Native Tech Tools" menu, where can be found these options:
- Set Password (passwords for alias and keystore);
- Switch FULL_LOG (adds / removes this define, used by DebugWrapper to show logs or not);
- Scene View (opens up the window with list of scenes, included into BuildSettings, to comfortable move between scenes).

Some of the used technics:
- CoroutineTicke (Persistent singleton to run coroutines on, in order to it keep running if source gameObject got disabled);
- DebugWrapper (Shows logs only with FULL_LOG define and allows to use colored text with predefined colors)
- Yielders (Static readonly Dictionary with time intervals, fixedUpdate, endOfFrame to use instead of every time calling new [YieldInstruction], drastically decrease GC allocations)
- Some StaticUtilities (Like ChangeColorBrightness, IsPointerOverUIObject, e.t.c);
- Extensions (DateTime: FromUnixTime, ToUnixTime, TimeSpanToString; AnimationCurve Reverse; GameObject: PushBackToPool, GetOrAddComponent, AddChild from prefab, AddRectTransformChild; MonoBehaviour TryStartCoroutineOnTicker; Object DoActionWithCheckReference);
- Interfaces and Generics to ease project scale;
- GameObjectPool;
- ScriptableObjects to easly configure game data;

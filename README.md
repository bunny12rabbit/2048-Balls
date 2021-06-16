# 2048-Balls
The pet-project for showcase, due to all working projects is under NDA.
Used:
- Unity 2020.1.17f1;
- UnityUI system;
- GitHub repository;
- Surge TweenEngine (http://surge.pixelplacement.com/tween.html);
- 2D Dev Sprites from AssetStore to fasten prototyping;
- Debug Console from AssetStore to ease debuging on mobiles (Tap with 3 fingers).

Target archicture: Android.

The game starts from AppStart scene. To add more comfort using Editor, i've created "Native Tech Tools" menu, where can be found these options:
- Set Password (passwords for alias and keystore);
- Switch FULL_LOG (adds / removes this define, used by DebugWrapper to show logs or not);
- Scene View (opens up the window with list of scenes, included into BuildSettings, to comfortable move between scenes).

Some of the used technics:
- CoroutineTicker (Persistent singleton to run coroutines on, in order to it keep running if source gameObject got disabled);
- DebugWrapper (Shows logs only with FULL_LOG define and allows to use colored text with predefined colors)
- Yielders (Static readonly Dictionary with time intervals, fixedUpdate, endOfFrame to use instead of every time calling new [YieldInstruction], drastically decrease GC allocations)
- Some StaticUtilities (Like ChangeColorBrightness, IsPointerOverUIObject, e.t.c);
- Extensions (DateTime: FromUnixTime, ToUnixTime, TimeSpanToString; AnimationCurve Reverse; GameObject: PushBackToPool, GetOrAddComponent, AddChild from prefab, AddRectTransformChild; MonoBehaviour TryStartCoroutineOnTicker; Object DoActionWithCheckReference);
- Interfaces and Generics to ease project scale;
- GameObjectPool;
- ScriptableObjects to easly configure game data;

<details>
  <summary>Screenshots:</summary>
    <p align="center">
      <img src="https://i.ibb.co/BtcknsC/Screenshot-20200825-130322.jpg" width="350">
      <img src="https://i.ibb.co/JFTjF4f/Screenshot-20200825-130329.jpg" width="350">
      <img src="https://i.ibb.co/f8dZt48/Screenshot-20200825-130334.jpg" width="350">
      <img src="https://i.ibb.co/9vL7Zcc/Screenshot-20200825-133446.jpg" width="350">
      <img src="https://i.ibb.co/LP0x9Mb/Screenshot-20200825-133500.jpg" width="350">
      <img src="https://i.ibb.co/98chXTd/Screenshot-20200825-130355.jpg" width="350">
    </p>
</details>

<details>
  <summary>Video:</summary>
    <div align="center">
      <a href="https://www.youtube.com/watch?v=9YVaEyz1bAE"><img src="https://i.ibb.co/1KWM52B/image.png" alt="Click to play on YouTube"></a>
    </div>
</details>

<div align="left">
  <a href="https://github.com/bunny12rabbit/2048-Balls/releases/tag/v0.1"> Download Release v0.1</a>
</div>

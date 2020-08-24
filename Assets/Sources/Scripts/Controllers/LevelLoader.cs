using System.Collections;
using Generics;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controllers
{
    public class LevelLoader : SingletonBehaviourPersistentGeneric<LevelLoader>
    {
        private static AsyncOperation _asyncOperation;


        public void LoadLevel(Constants.SceneIndexes scene) =>
            CoroutineTicker.Instance.StartCoroutine(LoadSceneAsynchronously((int)scene));

        private void Start()
        {
            SceneTransitionManager.Instance.onLoadingAnimationDone += () =>
            {
                if (_asyncOperation == null)
                {
                    return;
                }
                
                _asyncOperation.allowSceneActivation = true;
            };

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            switch (scene.buildIndex)
            {
                case (int)Constants.SceneIndexes.MainMenu:
                    UiManager.Instance.ShowMainMenuUi();
                    break;
                case (int)Constants.SceneIndexes.Game:
                    UiManager.Instance.ShowGameUi();
                    break;
            }
        }

        private IEnumerator LoadSceneAsynchronously(int sceneIndex)
        {
            _asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);

            ShowLoadingScreen();

            yield return new WaitUntil(() => _asyncOperation.isDone);
            
            HideLoadingScreen();
        }

        private void ShowLoadingScreen()
        {
            _asyncOperation.allowSceneActivation = false;
            SceneTransitionManager.Instance.PlayLoadingScreenAnimation(true);
        }

        private void HideLoadingScreen() => SceneTransitionManager.Instance.PlayLoadingScreenAnimation(false);
    }
}
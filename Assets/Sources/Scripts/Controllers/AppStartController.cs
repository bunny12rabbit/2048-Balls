using System.Collections;
using Pixelplacement;
using UnityEngine;

namespace Controllers
{
    public class AppStartController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup panel;
        
        [SerializeField] private float fadeDuration = 0.2f;
        [SerializeField] private float logoDuration = 1f;


        private void Start()
        {
            if (panel == null)
            {
                return;
            }

            CoroutineTicker.Instance.StartCoroutine(StartLogoAnimation());
        }

        private IEnumerator StartLogoAnimation()
        {
            panel.alpha = 0;
            
            yield return CoroutineTicker.Instance.StartCoroutine(Fade(1));

            yield return CoroutineTicker.Instance.StartCoroutine(Fade(0));
            
            LevelLoader.Instance.LoadLevel(Constants.SceneIndexes.MainMenu);
        }

        private IEnumerator Fade(float endValue)
        {
            Tween.CanvasGroupAlpha(panel, endValue, fadeDuration, 0);

            yield return Yielders.WaitForSeconds(logoDuration);
        }
    }
}
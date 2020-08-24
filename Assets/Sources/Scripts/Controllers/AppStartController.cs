using System.Collections;
using Pixelplacement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class AppStartController : MonoBehaviour
    {
        [SerializeField] private Image panel;
        
        [SerializeField] private TMP_Text text;
        
        [Space]
        private float _fadeDuration;
        [SerializeField] private float logoDuration = 1f;

        [SerializeField] private Color targetColor = Color.white;


        private CanvasGroup _textCanvasGroup;

        private void Start()
        {
            if (panel == null)
            {
                return;
            }

            _textCanvasGroup = text.gameObject.GetOrAddComponent<CanvasGroup>();
            _fadeDuration = logoDuration * 0.25f;
            
            CoroutineTicker.Instance.StartCoroutine(StartLogoAnimation());
        }

        private IEnumerator StartLogoAnimation()
        {
            _textCanvasGroup.alpha = 0;
            var textTargetColor = targetColor == Color.white ? Color.black : Color.white;
            float duration = logoDuration * 0.5f;
            
            yield return CoroutineTicker.Instance.StartCoroutine(Fade(1));

            Tween.Value(text.color, textTargetColor, color => text.color = color, duration, 0);
            ChangeColorOverTime(targetColor);

            yield return Yielders.WaitForSeconds(duration);

            LevelLoader.Instance.LoadLevel(Constants.SceneIndexes.MainMenu);
        }

        private IEnumerator Fade(float endValue)
        {
            Tween.CanvasGroupAlpha(_textCanvasGroup, endValue, _fadeDuration, 0);
            yield return Yielders.WaitForSeconds(_fadeDuration);
        }

        private void ChangeColorOverTime(Color color) => Tween.Color(panel, color, logoDuration * 0.5f, 0);
    }
}
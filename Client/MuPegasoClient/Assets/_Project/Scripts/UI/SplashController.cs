using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace MuPegaso.Client.UI
{
    public class SplashController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private CanvasGroup panelWebzen;
        [SerializeField] private CanvasGroup panelMuPegaso;

        [Header("Config")]
        [SerializeField] private float fadeTime  = 1.5f;
        [SerializeField] private float holdTime  = 2.0f;
        [SerializeField] private string nextScene = "Login";

        void Start() => StartCoroutine(PlaySplash());

        IEnumerator Fade(CanvasGroup cg, float from, float to, float duration)
        {
            float t = 0f;
            cg.alpha = from;
            while (t < duration)
            {
                t += Time.deltaTime;
                cg.alpha = Mathf.Lerp(from, to, t / duration);
                yield return null;
            }
            cg.alpha = to;
        }

        IEnumerator PlaySplash()
        {
            panelWebzen.alpha   = 0f;
            panelMuPegaso.alpha = 0f;

            // Webzen
            yield return StartCoroutine(Fade(panelWebzen, 0f, 1f, fadeTime));
            yield return new WaitForSeconds(holdTime);
            yield return StartCoroutine(Fade(panelWebzen, 1f, 0f, 1.0f));

            yield return new WaitForSeconds(0.3f);

            // MU Pegaso
            yield return StartCoroutine(Fade(panelMuPegaso, 0f, 1f, 2f));
            yield return new WaitForSeconds(holdTime);
            yield return StartCoroutine(Fade(panelMuPegaso, 1f, 0f, 1.0f));

            yield return new WaitForSeconds(0.2f);

            SceneManager.LoadScene(nextScene);
        }
    }
}

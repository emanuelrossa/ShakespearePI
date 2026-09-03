using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Comecar : MonoBehaviour
{
    [Header("Configurações de Fade")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1.0f;

    private bool isTransitioning = false;

    private void Start()
    {
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 1f;
            StartCoroutine(FadeIn());
        }
    }

    public void ComecarGame()
    {
        if (!isTransitioning)
        {
            StartCoroutine(FadeOutAndLoadScene());
        }
    }

    private IEnumerator FadeIn()
    {
        float counter = 0f;

        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            fadeCanvasGroup.alpha = 1f - (counter / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;

        fadeCanvasGroup.blocksRaycasts = false;
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        isTransitioning = true;

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.blocksRaycasts = true;
            float counter = 0f;

            while (counter < fadeDuration)
            {
                counter += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Clamp01(counter / fadeDuration);
                yield return null;
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;

        SceneManager.LoadScene("Começo");
    }
}
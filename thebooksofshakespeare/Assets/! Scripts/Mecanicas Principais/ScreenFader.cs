using System.Collections;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    public CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    public void FadeToBlack(float duration = 1.5f)
    {
        StartCoroutine(Fade(0f, 1f, duration));
    }

    public void FadeFromBlack(float duration = 1.5f)
    {
        StartCoroutine(Fade(1f, 0f, duration));
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        canvasGroup.blocksRaycasts = true;

        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, time / duration);
            yield return null;
        }

        canvasGroup.alpha = to;
    }
}
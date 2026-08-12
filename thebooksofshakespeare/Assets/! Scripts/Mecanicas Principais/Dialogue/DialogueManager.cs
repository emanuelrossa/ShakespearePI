using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseSystm : MonoBehaviour
{
    [Header("Pause")]
    public GameObject PausePanel;

    [Header("Animação")]
    public float slideDuration = 0.3f;
    public float slideDistance = 500f;

    private RectTransform pauseRect;

    private Vector2 shownPosition;
    private Vector2 hiddenPosition;

    private bool isPaused = false;

    private Coroutine animationCoroutine;


    private void Start()
    {
        if (PausePanel == null)
        {
            Debug.LogError(
                "PauseSystm: PausePanel não foi configurado!"
            );

            return;
        }

        pauseRect = PausePanel.GetComponent<RectTransform>();

        if (pauseRect == null)
        {
            Debug.LogError(
                "PauseSystm: PausePanel precisa ser um objeto UI!"
            );

            return;
        }

        // Posição normal definida no Inspector
        shownPosition = pauseRect.anchoredPosition;

        // Começa escondido à direita
        hiddenPosition =
            shownPosition +
            new Vector2(slideDistance, 0f);

        pauseRect.anchoredPosition = hiddenPosition;

        PausePanel.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }


    public void PauseGame()
    {
        if (isPaused)
            return;

        isPaused = true;

        // Para o jogo
        Time.timeScale = 0f;

        // Mostra o cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine =
            StartCoroutine(SlideIn());
    }


    public void ResumeGame()
    {
        if (!isPaused)
            return;

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine =
            StartCoroutine(SlideOut());
    }


    private IEnumerator SlideIn()
    {
        PausePanel.SetActive(true);

        pauseRect.anchoredPosition =
            hiddenPosition;

        float time = 0f;

        while (time < slideDuration)
        {
            time += Time.unscaledDeltaTime;

            float t =
                time / slideDuration;

            // Ease Out
            t =
                1f - Mathf.Pow(1f - t, 3f);

            pauseRect.anchoredPosition =
                Vector2.Lerp(
                    hiddenPosition,
                    shownPosition,
                    t
                );

            yield return null;
        }

        pauseRect.anchoredPosition =
            shownPosition;

        animationCoroutine = null;
    }


    private IEnumerator SlideOut()
    {
        float time = 0f;

        Vector2 startPosition =
            pauseRect.anchoredPosition;

        while (time < slideDuration)
        {
            time += Time.unscaledDeltaTime;

            float t =
                time / slideDuration;

            // Ease In
            t = Mathf.Pow(t, 3f);

            pauseRect.anchoredPosition =
                Vector2.Lerp(
                    startPosition,
                    hiddenPosition,
                    t
                );

            yield return null;
        }

        pauseRect.anchoredPosition =
            hiddenPosition;

        PausePanel.SetActive(false);

        isPaused = false;

        // Despausa depois da animação
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        animationCoroutine = null;
    }
}
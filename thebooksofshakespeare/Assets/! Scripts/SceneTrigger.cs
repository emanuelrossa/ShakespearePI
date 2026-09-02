using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [Header("Configuração de Cena")]
    public string sceneToLoad;
    public string playerTag = "Player";

    [Header("Configuração de Fade")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1.0f;

    private bool isTransitioning = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !isTransitioning)
        {
            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                StartCoroutine(FadeAndLoadScene());
            }
        }
    }

    private IEnumerator FadeAndLoadScene()
    {
        isTransitioning = true;

        if (fadeCanvasGroup != null)
        {
            float counter = 0f;

            while (counter < fadeDuration)
            {
                counter += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Clamp01(counter / fadeDuration);
                yield return null;
            }
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}
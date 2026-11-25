using System.Collections;
using UnityEngine;
using TMPro;

public class TextoFade : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI texto;

    [Header("Tempos (segundos)")]
    [SerializeField] private float duracaoFadeIn = 2f;
    [SerializeField] private float tempoVisivel = 7f;
    [SerializeField] private float duracaoFadeOut = 2f;

    private void Start()
    {
        if (texto == null)
            texto = GetComponent<TextMeshProUGUI>();

        SetAlpha(0f);
        StartCoroutine(Sequencia());
    }

    private IEnumerator Sequencia()
    {
        yield return Fade(0f, 1f, duracaoFadeIn);
        yield return new WaitForSeconds(tempoVisivel);
        yield return Fade(1f, 0f, duracaoFadeOut);
    }

    private IEnumerator Fade(float de, float para, float duracao)
    {
        float t = 0f;
        while (t < duracao)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(de, para, t / duracao));
            yield return null;
        }
        SetAlpha(para);
    }

    private void SetAlpha(float a)
    {
        Color c = texto.color;
        c.a = a;
        texto.color = c;
    }
}
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class FinalScene : MonoBehaviour
{
    [System.Serializable]
    public struct DialogoLinha
    {
        public string nomeFalante;
        [TextArea] public string fala;
    }

    [Header("Diálogos Finais (Antes do Preto)")]
    public DialogoLinha[] dialogoCreditos;

    [Header("Referências")]
    public DialogueManager dialogueManager;

    [Header("Efeito de Tela Preta")]
    public Image telaPreta;
    public float duracaoFadePreto = 2f;
    public float delayAposDialogo = 1.5f;

    [Header("Créditos")]
    public GameObject painelCreditos;
    public RectTransform textoCreditosRect;
    public float posicaoFinalCreditosY = 1500f;
    public float velocidadeSubidaCreditos = 50f;

    private void Start()
    {
        if (telaPreta != null)
        {
            Color c = telaPreta.color;
            c.a = 0f;
            telaPreta.color = c;
            telaPreta.gameObject.SetActive(true);
        }

        if (painelCreditos != null)
            painelCreditos.SetActive(false);

        StartCoroutine(RotinaFinal());
    }

    private IEnumerator RotinaFinal()
    {
        yield return new WaitForSeconds(0.5f);

        yield return MostrarDialogo(dialogoCreditos);

        yield return new WaitForSeconds(delayAposDialogo);

        if (dialogueManager != null && dialogueManager.dialogueBox != null)
        {
            dialogueManager.dialogueBox.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 3. Fade para preto
        if (telaPreta != null)
        {
            float t = 0;
            Color c = telaPreta.color;
            while (t < duracaoFadePreto)
            {
                t += Time.deltaTime;
                c.a = Mathf.Clamp01(t / duracaoFadePreto);
                telaPreta.color = c;
                yield return null;
            }
        }

        if (painelCreditos != null)
        {
            painelCreditos.SetActive(true);

            if (textoCreditosRect != null)
            {
                textoCreditosRect.anchorMin = new Vector2(0.5f, 0.5f);
                textoCreditosRect.anchorMax = new Vector2(0.5f, 0.5f);
                textoCreditosRect.pivot = new Vector2(0.5f, 0.5f);

                Vector2 posInicial = textoCreditosRect.anchoredPosition;
                posInicial.y = -Screen.height - 200f;
                textoCreditosRect.anchoredPosition = posInicial;

                while (textoCreditosRect.anchoredPosition.y < posicaoFinalCreditosY)
                {
                    posInicial.y += velocidadeSubidaCreditos * Time.deltaTime;
                    textoCreditosRect.anchoredPosition = posInicial;
                    yield return null;
                }
            }
        }
    }

    private IEnumerator MostrarDialogo(DialogoLinha[] linhas)
    {
        if (linhas == null || linhas.Length == 0) yield break;

        if (dialogueManager == null)
            dialogueManager = DialogueManager.Instance;

        if (dialogueManager == null)
        {
            yield break;
        }

        foreach (var linha in linhas)
        {
            dialogueManager.StartDialogue(linha.nomeFalante, new string[] { linha.fala }, null);

            while (dialogueManager.IsTalking)
            {
                yield return null;
            }
        }
    }
}
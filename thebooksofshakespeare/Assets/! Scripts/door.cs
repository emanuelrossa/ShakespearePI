using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class door : MonoBehaviour
{
    [Header("Configurações")]
    public string nomeDaCena;
    public KeyCode teclaInteracao = KeyCode.E;

    [Header("Interface (UI)")]
    public TextMeshProUGUI textoInteracao;
    public string mensagemNaTela = "Aperte E para interagir";

    [Header("Efeito Fade")]
    public CanvasGroup painelFade;
    public float duracaoFade = 1f;

    private bool jogadorPerto = false;
    private bool aTransicionar = false;

    void Start()
    {
        if (textoInteracao != null)
        {
            textoInteracao.gameObject.SetActive(false);
        }

        if (painelFade != null)
        {
            painelFade.alpha = 0f;
            painelFade.blocksRaycasts = false;
        }
    }

    void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(teclaInteracao) && !aTransicionar)
        {
            StartCoroutine(CarregarCenaComFade());
        }
    }

    IEnumerator CarregarCenaComFade()
    {
        aTransicionar = true;

        if (textoInteracao != null)
        {
            textoInteracao.gameObject.SetActive(false);
        }

        if (painelFade != null)
        {
            painelFade.blocksRaycasts = true;
            float tempo = 0f;
            while (tempo < duracaoFade)
            {
                tempo += Time.deltaTime;
                painelFade.alpha = Mathf.Lerp(0f, 1f, tempo / duracaoFade);
                yield return null;
            }
            painelFade.alpha = 1f;
        }

        SceneManager.LoadScene(nomeDaCena);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = true;

            if (textoInteracao != null && !aTransicionar)
            {
                textoInteracao.text = mensagemNaTela;
                textoInteracao.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;

            if (textoInteracao != null)
            {
                textoInteracao.gameObject.SetActive(false);
            }
        }
    }
}
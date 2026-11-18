using UnityEngine;
using System.Collections;

public class MoverTexto : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public Vector2 direcaoMovimento = new Vector2(0f, 1f);
    public float velocidade = 100f;

    public float tempoDeEspera = 2f;

    [Header("Opcional: Reposicionar ao sair da tela")]
    public bool reiniciarAoChegarAoFim = false;
    public float posicaoReinicioY = -500f;
    public float posicaoFimY = 800f;

    private RectTransform rectTransform;
    private bool podeMover = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        StartCoroutine(EsperarEComecar());
    }

    IEnumerator EsperarEComecar()
    {
        yield return new WaitForSeconds(tempoDeEspera);

        podeMover = true;
    }

    void Update()
    {
        if (rectTransform == null || !podeMover) return;

        rectTransform.anchoredPosition += direcaoMovimento.normalized * velocidade * Time.deltaTime;

     
        if (reiniciarAoChegarAoFim && rectTransform.anchoredPosition.y > posicaoFimY)
        {
            Vector2 novaPos = rectTransform.anchoredPosition;
            novaPos.y = posicaoReinicioY;
            rectTransform.anchoredPosition = novaPos;
        }
    }
}
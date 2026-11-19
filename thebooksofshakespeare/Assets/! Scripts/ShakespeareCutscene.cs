using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShakespeareCutscene : MonoBehaviour
{
    [Header("Player (Shakespeare)")]
    public GameObject player;

    [Header("Configurações do Personagem")]
    public string nomeDoPersonagem = "Shakespeare";

    [System.Serializable]
    public struct DialogoLinha
    {
        [TextArea] public string fala;
    }

    [Header("Roteiro: Diálogos")]
    public DialogoLinha[] dialogoShakespeare;
    public DialogoLinha[] dialogoMissao;
    public DialogoLinha[] dialogoInteracaoCama;

    [Header("Integração com Missão")]
    public string alvoMissao = "Cama";
    public string textoMissao = "Vá para a cama";

    [Header("Interação com a Cama (Por Distância)")]
    [Tooltip("Arraste o objeto da cama aqui (só pra medir a distância).")]
    public GameObject camaObjeto;
    [Tooltip("Distância que o jogador precisa chegar pra poder apertar E.")]
    public float distanciaInteracao = 4f;
    [Tooltip("A tecla que vai deixar a tela preta e mudar de cena.")]
    public KeyCode teclaInteracao = KeyCode.E;

    [Header("UI de Aviso (Opcional)")]
    [Tooltip("Um texto tipo 'Pressione E para dormir' que aparece quando chega perto.")]
    public GameObject textoAvisoInteracao;

    [Header("Fade e Troca de Cena")]
    public Image telaPreta;
    public string nomeProximaCena = "02. Quarto";
    public float duracaoFade = 2f;

    private bool interagiuComCama = false;
    private bool jaEstaNoFade = false;
    private bool missaoAtiva = false;

    private void Awake()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (telaPreta != null)
        {
            Color c = telaPreta.color;
            c.a = 0f;
            telaPreta.color = c;
        }

        if (textoAvisoInteracao != null)
            textoAvisoInteracao.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(RotinaCutscene());
    }

    private IEnumerator RotinaCutscene()
    {
        yield return new WaitForSeconds(0.5f);

        yield return MostrarDialogo(dialogoShakespeare);
        yield return MostrarDialogo(dialogoMissao);

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.StartQuest(QuestManager.QuestType.Find, alvoMissao, "", textoMissao);
        }

        missaoAtiva = true;
        Debug.Log("Missão Iniciada! Vá até a cama e aperte E.");

        yield return new WaitUntil(() => interagiuComCama);

        if (textoAvisoInteracao != null)
            textoAvisoInteracao.SetActive(false);

        yield return MostrarDialogo(dialogoInteracaoCama);
        yield return FadeParaPreto();

        yield return new WaitForSeconds(0.5f);
        if (!string.IsNullOrEmpty(nomeProximaCena))
        {
            SceneManager.LoadScene(nomeProximaCena);
        }
    }

    private void Update()
    {
        // Se já interagiu, tá no fade, ou a missão não começou, ignora
        if (!missaoAtiva || interagiuComCama || jaEstaNoFade)
            return;

        // Se o jogador estiver dentro da área e apertar E
        if (jogadorPerto && Input.GetKeyDown(teclaInteracao))
        {
            Debug.Log("Apertou E perto da cama! Iniciando transição...");
            interagiuComCama = true;

            if (QuestManager.Instance != null)
                QuestManager.Instance.CompleteQuest();
        }
    }

    private bool jogadorPerto = false;

    // O Unity chama isso automaticamente quando ALGO entra no Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = true;
            if (textoAvisoInteracao != null) textoAvisoInteracao.SetActive(true);
            Debug.Log("Jogador chegou perto da cama!");
        }
    }

    // O Unity chama isso quando ALGO sai do Trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
            if (textoAvisoInteracao != null) textoAvisoInteracao.SetActive(false);
            Debug.Log("Jogador saiu de perto da cama.");
        }
    }

    private IEnumerator FadeParaPreto()
    {
        if (telaPreta == null) yield break;

        jaEstaNoFade = true;
        float t = 0;
        Color c = telaPreta.color;
        while (t < duracaoFade)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(t / duracaoFade);
            telaPreta.color = c;
            yield return null;
        }
    }

    private IEnumerator MostrarDialogo(DialogoLinha[] linhas)
    {
        if (linhas == null || linhas.Length == 0) yield break;
        if (DialogueManager.Instance == null) yield break;

        foreach (var linha in linhas)
        {
            DialogueManager.Instance.StartDialogue(nomeDoPersonagem, new string[] { linha.fala }, null);
            while (DialogueManager.Instance.IsTalking)
            {
                yield return null;
            }
        }
    }
}
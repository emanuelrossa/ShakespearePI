using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShakespeareCutscene : MonoBehaviour
{
    [Header("Player (Shakespeare)")]
    [Tooltip("Arraste o objeto do jogador aqui.")]
    public GameObject player;
    [Tooltip("Onde o jogador vai nascer.")]
    public Transform playerSpawnPoint;

    [Header("Configurações do Personagem")]
    [Tooltip("Nome que vai aparecer na caixa de diálogo.")]
    public string nomeDoPersonagem = "Shakespeare";

    [System.Serializable]
    public struct DialogoLinha
    {
        [TextArea] public string fala;
    }

    [Header("Roteiro: Diálogos")]
    [Tooltip("O que Shakespeare fala no início (pode ser pensamento ou falando sozinho).")]
    public DialogoLinha[] dialogoShakespeare;

    [Tooltip("A missão que ele recebe.")]
    public DialogoLinha[] dialogoMissao;

    [Header("Interação com a Cama")]
    [Tooltip("Arraste o objeto que tem o BoxCollider (IsTrigger) da cama aqui.")]
    public GameObject areaInteracaoCama;
    [Tooltip("Fala que aparece quando o jogador chega perto da cama.")]
    public DialogoLinha[] dialogoInteracaoCama;

    [Header("Fade e Troca de Cena")]
    [Tooltip("Uma Image de UI preta que cobre a tela. Comece com Alpha = 0.")]
    public Image telaPreta;
    public string nomeProximaCena = "02. Quarto";
    public float duracaoFade = 2f;

    private bool interagiuComCama = false;

    private void Awake()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        // Posiciona o Player/Shakespeare no ponto de spawn
        if (player != null && playerSpawnPoint != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.transform.position = playerSpawnPoint.position;
            player.transform.rotation = playerSpawnPoint.rotation;

            if (cc != null) cc.enabled = true;
        }

        // Deixa a tela preta invisível no começo
        if (telaPreta != null)
        {
            Color c = telaPreta.color;
            c.a = 0f;
            telaPreta.color = c;
        }
    }

    private void Start()
    {
        StartCoroutine(RotinaCutscene());
    }

    private IEnumerator RotinaCutscene()
    {
        yield return new WaitForSeconds(0.5f);

        // 1. Shakespeare (o player) fala sozinho / pensa
        yield return MostrarDialogo(dialogoShakespeare);

        // 2. Recebe a missão
        yield return MostrarDialogo(dialogoMissao);

        // 3. Espera o jogador (Shakespeare) ir até a cama e pisar no trigger
        yield return new WaitUntil(() => interagiuComCama);

        // 4. Fala ao chegar na cama
        yield return MostrarDialogo(dialogoInteracaoCama);

        // 5. Fade para preto
        yield return FadeParaPreto();

        // 6. Troca de cena
        yield return new WaitForSeconds(0.5f);
        if (!string.IsNullOrEmpty(nomeProximaCena))
        {
            SceneManager.LoadScene(nomeProximaCena);
        }
    }

    private IEnumerator FadeParaPreto()
    {
        if (telaPreta == null) yield break;

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
            // Puxa o nome direto da variável "nomeDoPersonagem" definida no Inspector
            DialogueManager.Instance.StartDialogue(nomeDoPersonagem, new string[] { linha.fala }, null);
            while (DialogueManager.Instance.IsTalking)
            {
                yield return null;
            }
        }
    }

    // Método chamado pelo script da Cama (CamaTrigger)
    public void AvisarQueChegouNaCama()
    {
        interagiuComCama = true;
    }
}
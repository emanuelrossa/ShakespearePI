using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShakespeareCutscene : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;
    public Transform playerSpawnPoint;

    [Header("Shakespeare")]
    public GameObject shakespeare;
    public Transform shakespeareSpawnPoint;

    [System.Serializable]
    public struct DialogoLinha
    {
        public string nomeFalante;
        [TextArea] public string fala;
    }

    [Header("Roteiro: Diálogos")]
    public DialogoLinha[] dialogoShakespeare;
    public DialogoLinha[] dialogoMissao;

    [Header("Interação com a Cama")]
    public GameObject areaInteracaoCama;
    public DialogoLinha[] dialogoInteracaoCama;

    [Header("Fade e Troca de Cena")]
    public Image telaPreta;
    public string nomeProximaCena = "02. Quarto";
    public float duracaoFade = 2f;

    private bool interagiuComCama = false;

    private void Awake()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        SpawnAllCharacters();

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

    public void SpawnAllCharacters()
    {
        if (player != null && playerSpawnPoint != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.transform.position = playerSpawnPoint.position;
            player.transform.rotation = playerSpawnPoint.rotation;

            if (cc != null) cc.enabled = true;
        }

        if (shakespeare != null && shakespeareSpawnPoint != null)
        {
            shakespeare.transform.position = shakespeareSpawnPoint.position;
            shakespeare.transform.rotation = shakespeareSpawnPoint.rotation;
        }
    }

    private IEnumerator RotinaCutscene()
    {
        yield return new WaitForSeconds(0.5f);

        yield return MostrarDialogo(dialogoShakespeare);
        yield return MostrarDialogo(dialogoMissao);

        yield return new WaitUntil(() => interagiuComCama);

        yield return MostrarDialogo(dialogoInteracaoCama);
        yield return FadeParaPreto();

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
            DialogueManager.Instance.StartDialogue(linha.nomeFalante, new string[] { linha.fala }, null);
            while (DialogueManager.Instance.IsTalking)
            {
                yield return null;
            }
        }
    }

    public void AvisarQueChegouNaCama()
    {
        interagiuComCama = true;
    }
}
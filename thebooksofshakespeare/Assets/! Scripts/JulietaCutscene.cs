using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JulietaCutscene : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;
    public Transform playerSpawnPoint;

    [Header("Julieta")]
    public GameObject julieta;
    public Transform julietaSpawnPoint;

    [Header("Moradores (Opcional)")]
    public GameObject[] moradores;
    public Transform[] moradoresSpawnPoints;

    [System.Serializable]
    public struct DialogoLinha
    {
        public string nomeFalante;
        [TextArea] public string fala;
    }

    [Header("Diálogos da Nova Cena (Múltiplos Falantes)")]
    public DialogoLinha[] dialogoLinhas;

    [Header("Fade para Branco")]
    [Tooltip("Image de UI em tela cheia, cor branca, que cobre a tela. Deve começar com alpha = 0.")]
    public Image whiteOverlay;
    [Tooltip("Duração do fade para branco (segundos).")]
    public float fadeDuration = 3f;
    [Tooltip("Se marcado, o fade começa junto com o diálogo. Se desmarcado, espera o diálogo terminar.")]
    public bool fadeDuringDialogue = true;
    [Tooltip("Atraso antes de iniciar o fade (segundos).")]
    public float fadeDelay = 0f;

    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        SpawnAllCharacters();
    }

    private void Start()
    {
        StartCoroutine(IniciarDialogoNovaCena());

        if (fadeDuringDialogue)
        {
            StartCoroutine(FadeParaBranco());
        }
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

        if (julieta != null && julietaSpawnPoint != null)
        {
            julieta.transform.position = julietaSpawnPoint.position;
            julieta.transform.rotation = julietaSpawnPoint.rotation;
        }

        if (moradores != null && moradoresSpawnPoints != null)
        {
            int limit = Mathf.Min(moradores.Length, moradoresSpawnPoints.Length);
            for (int i = 0; i < limit; i++)
            {
                if (moradores[i] != null && moradoresSpawnPoints[i] != null)
                {
                    moradores[i].transform.position = moradoresSpawnPoints[i].position;
                    moradores[i].transform.rotation = moradoresSpawnPoints[i].rotation;
                }
            }
        }
    }

    private IEnumerator IniciarDialogoNovaCena()
    {
        yield return new WaitForSeconds(0.5f);

        if (DialogueManager.Instance != null && dialogoLinhas != null && dialogoLinhas.Length > 0)
        {
            foreach (var linha in dialogoLinhas)
            {
                string[] falaUnica = new string[] { linha.fala };
                DialogueManager.Instance.StartDialogue(linha.nomeFalante, falaUnica, null);

                while (DialogueManager.Instance.IsTalking)
                {
                    yield return null;
                }
            }
        }

        // Se o fade NÃO é durante o diálogo, começa só depois que o diálogo terminar
        if (!fadeDuringDialogue)
        {
            StartCoroutine(FadeParaBranco());
        }
    }

    private IEnumerator FadeParaBranco()
    {
        if (whiteOverlay == null)
        {
            Debug.LogWarning("[JulietaCutscene] whiteOverlay não atribuído — fade ignorado.");
            yield break;
        }

        if (fadeDelay > 0f)
            yield return new WaitForSeconds(fadeDelay);

        Color c = whiteOverlay.color;
        c.a = 0f;
        whiteOverlay.color = c;
        whiteOverlay.gameObject.SetActive(true);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / fadeDuration);
            c.a = alpha;
            whiteOverlay.color = c;
            yield return null;
        }

        // Garante alpha final = 1
        c.a = 1f;
        whiteOverlay.color = c;
    }
}
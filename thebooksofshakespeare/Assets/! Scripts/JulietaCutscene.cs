using System.Collections;
using UnityEngine;

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

        if (DialogueManager.Instance != null && dialogoLinhas.Length > 0)
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
    }
}
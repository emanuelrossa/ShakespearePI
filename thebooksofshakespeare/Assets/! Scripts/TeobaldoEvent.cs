using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeobaldoEvent : MonoBehaviour
{
    [Header("UI & Áudio")]
    public Image blackScreenOverlay;
    public AudioSource fightAudioSource;

    [Header("Transição de Cena")]
    public string proximaCena = "NomeDaProximaCena";

    [Header("Personagem a Desaparecer")]
    public GameObject npcSeguidor;

    [Header("Teleporte do Player")]
    public Transform playerTransform;
    public Transform playerTeleportTarget;

    [Header("NPCs")]
    public GameObject romeu;
    public Transform romeuTeleportTarget;
    public GameObject mercurio;
    public Transform mercurioTeleportTarget;
    public GameObject teobaldo;
    public Transform teobaldoTeleportTarget;
    public GameObject teobaldoCaido;
    public Transform teobaldoCaidoTeleportTarget;

    private void Start()
    {
        if (blackScreenOverlay != null)
            blackScreenOverlay.gameObject.SetActive(false);

        if (teobaldoCaido != null)
            teobaldoCaido.SetActive(false);

        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }
    }

    public void TriggerEvent()
    {
        StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        if (npcSeguidor != null)
        {
            npcSeguidor.SetActive(false);
        }

        if (blackScreenOverlay != null)
        {
            blackScreenOverlay.gameObject.SetActive(true);
        }

        if (fightAudioSource != null)
        {
            fightAudioSource.Play();
            yield return new WaitForSeconds(fightAudioSource.clip.length);
        }
        else
        {
            yield return new WaitForSeconds(2.0f);
        }

        if (romeu != null && romeuTeleportTarget != null)
        {
            TeleportObject(romeu, romeuTeleportTarget);
        }

        if (mercurio != null && mercurioTeleportTarget != null)
        {
            TeleportObject(mercurio, mercurioTeleportTarget);
        }

        if (teobaldo != null && teobaldoTeleportTarget != null)
        {
            TeleportObject(teobaldo, teobaldoTeleportTarget);
        }

        if (playerTransform != null && playerTeleportTarget != null)
        {
            TeleportObject(playerTransform.gameObject, playerTeleportTarget);
        }

        yield return new WaitForSeconds(0.5f);

        if (blackScreenOverlay != null)
        {
            blackScreenOverlay.gameObject.SetActive(false);
        }

        string[] dialogo1 = new string[] {
            "Romeu: Mercúrio!",
            "Teobaldo: Isso foi culpa sua, Romeu!"
        };
        yield return StartCoroutine(PlayDialogueCoroutine("Teobaldo", dialogo1));

        if (blackScreenOverlay != null)
        {
            blackScreenOverlay.gameObject.SetActive(true);
        }

        if (fightAudioSource != null)
        {
            fightAudioSource.Play();
            yield return new WaitForSeconds(fightAudioSource.clip.length);
        }
        else
        {
            yield return new WaitForSeconds(2.5f);
        }

        if (teobaldo != null)
        {
            teobaldo.SetActive(false);
        }

        if (teobaldoCaido != null && teobaldoCaidoTeleportTarget != null)
        {
            TeleportObject(teobaldoCaido, teobaldoCaidoTeleportTarget);
        }

        yield return new WaitForSeconds(0.5f);

        if (blackScreenOverlay != null)
        {
            blackScreenOverlay.gameObject.SetActive(false);
        }

        string[] dialogoFinal = new string[] {
            "Shakespeare: Isso está ficando cada vez pior...",
            "Guarda: Romeu Montecchio!",
            "Guarda: Você está banido de Verona!",
            "Romeu: Julieta...",
            "Romeu: O que vai acontecer agora?"
        };
        yield return StartCoroutine(PlayDialogueCoroutine("Guarda", dialogoFinal));

        if (blackScreenOverlay != null)
        {
            blackScreenOverlay.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(1.0f);

        if (!string.IsNullOrEmpty(proximaCena))
        {
            SceneManager.LoadScene(proximaCena);
        }
    }

    private IEnumerator PlayDialogueCoroutine(string speakerName, string[] lines)
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(speakerName, lines, null);
            while (DialogueManager.Instance.IsTalking)
            {
                yield return null;
            }
        }
    }

    private void TeleportObject(GameObject obj, Transform target)
    {
        if (obj == null || target == null) return;

        if (!obj.activeSelf) obj.SetActive(true);

        NPCFollow follow = obj.GetComponent<NPCFollow>();
        if (follow != null) follow.enabled = false;

        Animator anim = obj.GetComponentInChildren<Animator>();
        if (anim != null) anim.enabled = false;

        NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
        CharacterController cc = obj.GetComponent<CharacterController>();

        if (agent != null) agent.enabled = false;
        if (cc != null) cc.enabled = false;

        obj.transform.position = target.position;
        obj.transform.rotation = target.rotation;

        if (agent != null)
        {
            agent.enabled = true;
            agent.Warp(target.position);
        }

        if (cc != null) cc.enabled = true;
        if (anim != null) anim.enabled = true;
    }
}
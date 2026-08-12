using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public TMP_Text nameText;

    [Header("Player")]
    public FirstPersonController player;

    private string[] lines;
    private int currentLine;
    private NPC currentNPC;

    private bool canAdvance;

    public bool IsTalking { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (player == null)
        {
            player = FindFirstObjectByType<FirstPersonController>();
        }

        if (player == null)
        {
            Debug.LogError(
                "DialogueManager: não encontrou FirstPersonController na cena!"
            );
        }
    }

    public void StartDialogue(
        string npcName,
        string[] dialogue,
        NPC npc
    )
    {
        if (IsTalking)
            return;

        if (player == null)
        {
            Debug.LogError(
                "DialogueManager: Player não configurado!"
            );
            return;
        }

        if (dialogue == null || dialogue.Length == 0)
            return;

        currentNPC = npc;

        lines = dialogue;
        currentLine = 0;

        IsTalking = true;
        canAdvance = false;

        // PARA movimento E câmera
        player.canMove = false;

        dialogueBox.SetActive(true);

        nameText.text = npcName;
        dialogueText.text = lines[0];

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(InputDelay());
    }

    private IEnumerator InputDelay()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        canAdvance = true;
    }

    private void Update()
    {
        if (!IsTalking)
            return;

        if (!canAdvance)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            currentLine++;

            if (currentLine >= lines.Length)
            {
                EndDialogue();
            }
            else
            {
                dialogueText.text = lines[currentLine];
            }
        }
    }

    private void EndDialogue()
    {
        IsTalking = false;
        canAdvance = false;

        dialogueBox.SetActive(false);

        // LIBERA movimento E câmera
        if (player != null)
        {
            player.canMove = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (currentNPC != null)
        {
            currentNPC.FinishInteraction();
            currentNPC = null;
        }
    }
}
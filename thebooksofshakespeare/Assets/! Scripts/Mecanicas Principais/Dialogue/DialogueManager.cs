using UnityEngine;
using TMPro;
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

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (player == null)
        {
            player = FindFirstObjectByType<FirstPersonController>(FindObjectsInactive.Include);
        }
    }

    public void StartDialogue(string npcName, string[] dialogue, NPC npc)
    {
        if (player == null)
        {
            player = FindFirstObjectByType<FirstPersonController>(FindObjectsInactive.Include);
        }

        if (dialogue == null || dialogue.Length == 0)
            return;

        currentNPC = npc;
        lines = dialogue;
        currentLine = 0;

        IsTalking = true;
        canAdvance = false;

        if (dialogueBox != null)
            dialogueBox.SetActive(true);

        if (nameText != null)
            nameText.text = npcName;

        if (dialogueText != null)
            dialogueText.text = lines[0];

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(InputDelay());
    }

    IEnumerator InputDelay()
    {
        yield return new WaitForSeconds(0.2f);
        canAdvance = true;
    }

    void Update()
    {
        if (!IsTalking || !canAdvance)
            return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            currentLine++;

            if (currentLine >= lines.Length)
            {
                EndDialogue();
            }
            else
            {
                if (dialogueText != null)
                    dialogueText.text = lines[currentLine];
            }
        }
    }

    void EndDialogue()
    {
        IsTalking = false;
        canAdvance = false;

        if (dialogueBox != null)
            dialogueBox.SetActive(false);

        if (player != null)
        {
            player.enabled = true;
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
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

    void Awake()
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
            Debug.LogError("DialogueManager: não encontrou FirstPersonController na cena!");
        }
    }

    public void StartDialogue(string npcName, string[] dialogue, NPC npc)
    {

        player.canMove = false;
        Debug.Log("Dialogue abriu | canMove = " + player.canMove);

        if (player != null)
            player.canMove = false;

        if (IsTalking)
            return;

        if (dialogue == null || dialogue.Length == 0)
            return;

        currentNPC = npc;
        lines = dialogue;
        currentLine = 0;

        IsTalking = true;
        canAdvance = false;

        dialogueBox.SetActive(true);

        nameText.text = npcName;
        dialogueText.text = lines[0];

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        StartCoroutine(InputDelay());
    }

    IEnumerator InputDelay()
    {
        yield return new WaitForSeconds(0.2f);
        canAdvance = true;
    }

    void Update()
    {
        if (!IsTalking)
            return;

        if (!canAdvance)
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
                dialogueText.text = lines[currentLine];
            }
        }
    }

    void EndDialogue()
    {
        IsTalking = false;
        canAdvance = false;

        dialogueBox.SetActive(false);

        if (player != null)
            player.canMove = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (currentNPC != null)
        {
            currentNPC.FinishInteraction();
            currentNPC = null;
        }
    }
}
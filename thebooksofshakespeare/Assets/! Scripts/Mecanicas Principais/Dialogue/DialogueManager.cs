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
        Instance = this;
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
        player.canMove = true;
        Debug.Log("Dialogue fechou | canMove = " + player.canMove);

        if (player != null)
            player.canMove = true;

        IsTalking = false;
        canAdvance = false;

        dialogueBox.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (currentNPC != null)
        {
            currentNPC.FinishInteraction();
            currentNPC = null;
        }

        Time.timeScale = 1f;
    }
}
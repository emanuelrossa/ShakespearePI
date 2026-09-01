using UnityEngine;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour
{
    public enum Type
    {
        NPC,
        Item,
        Delivery
    }

    public enum DestroyCondition
    {
        Never,
        AfterFirstDialogue,
        AfterQuestDone
    }

    [Header("Identificação")]
    public Type type;
    public string id;
    public string npcName;

    [Header("Diálogos")]
    [TextArea] public string[] dialogue;
    [TextArea] public string[] questDoneDialogue;

    [Header("NPC/Objeto a Deletar")]
    public DestroyCondition destroyWhen = DestroyCondition.Never;
    public GameObject targetToDestroy;

    [Header("Troca de Cena (Sem Fade)")]
    public bool changeSceneAfterQuestDone = false;
    public string sceneToLoad;

    [Header("Missão")]
    public bool givesQuest;
    public QuestManager.QuestType questType;
    public string target;
    public string requiredItem;

    [Header("Comportamento de Seguir (Opcional)")]
    public NPCFollow followScript;

    [Header("Estado")]
    public bool isCompleted = false;

    [Header("Outline")]
    public Outline outline;

    private float interactCooldown;
    private Transform playerTransform;

    private void Start()
    {
        if (outline == null)
            outline = GetComponent<Outline>();

        if (outline != null)
            outline.enabled = false;

        if (followScript == null)
            followScript = GetComponent<NPCFollow>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    public void SetOutline(bool enabled)
    {
        if (outline != null)
            outline.enabled = enabled;
    }

    public void Interact()
    {
        if (Time.time < interactCooldown)
            return;

        if (DialogueManager.Instance == null || DialogueManager.Instance.IsTalking)
            return;

        if (type == Type.Item)
        {
            QuestManager.Instance.AddItem(id);
            QuestManager.Instance.Interact("Item", id);
            interactCooldown = Time.time + 0.5f;
            Destroy(gameObject);
            return;
        }

        if (followScript != null)
            followScript.StopFollowing();

        LookAtPlayer();

        if (!isCompleted && !string.IsNullOrEmpty(requiredItem))
        {
            if (QuestManager.Instance.HasItem(requiredItem))
            {
                isCompleted = true;
            }
        }

        string[] currentDialogueLines = (isCompleted && questDoneDialogue != null && questDoneDialogue.Length > 0)
            ? questDoneDialogue
            : dialogue;

        DialogueManager.Instance.StartDialogue(
            npcName,
            currentDialogueLines,
            this
        );
    }

    public void FinishInteraction()
    {
        interactCooldown = Time.time + 0.5f;

        QuestManager.Instance.Interact(type.ToString(), id);

        if (givesQuest)
        {
            QuestManager.Instance.StartQuest(questType, target, requiredItem);
            givesQuest = false;
        }

        if (isCompleted && changeSceneAfterQuestDone && !string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
            return;
        }

        if (followScript != null)
        {
            followScript.StartFollowing();
        }

        ExecuteDestroyLogic();
    }

    private void LookAtPlayer()
    {
        if (playerTransform == null) return;

        Vector3 lookPosition = playerTransform.position;
        lookPosition.y = transform.position.y;

        transform.LookAt(lookPosition);
    }

    private void ExecuteDestroyLogic()
    {
        if (destroyWhen == DestroyCondition.AfterFirstDialogue && !isCompleted)
        {
            DestroyTarget();
        }
        else if (destroyWhen == DestroyCondition.AfterQuestDone && isCompleted)
        {
            DestroyTarget();
        }
    }

    private void DestroyTarget()
    {
        if (targetToDestroy != null)
        {
            Destroy(targetToDestroy);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
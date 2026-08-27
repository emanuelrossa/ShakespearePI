using UnityEngine;

public class NPC : MonoBehaviour
{
    public enum Type
    {
        NPC,
        Item,
        Delivery
    }

    [Header("Identificação")]
    public Type type;
    public string id;
    public string npcName;

    [Header("Diálogos")]
    [TextArea] public string[] dialogue;            // Diálogo inicial
    [TextArea] public string[] questDoneDialogue;   // Diálogo APÓS entregar/completar

    [Header("Missão")]
    public bool givesQuest;
    public QuestManager.QuestType questType;
    public string target;
    public string requiredItem;

    [Header("Estado")]
    public bool isCompleted = false; // Fica true assim que a missão é finalizada

    [Header("Outline")]
    public Outline outline;

    private float interactCooldown;

    private void Start()
    {
        if (outline == null)
            outline = GetComponent<Outline>();

        if (outline != null)
            outline.enabled = false;
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

        // 1. Se for um item coletável no chão
        if (type == Type.Item)
        {
            QuestManager.Instance.AddItem(id);
            QuestManager.Instance.Interact("Item", id);
            interactCooldown = Time.time + 0.5f;
            Destroy(gameObject);
            return;
        }

        // 2. Se o NPC for de entrega (Delivery) ou se a missão requerer um item que o player JÁ TEM
        if (!isCompleted && !string.IsNullOrEmpty(requiredItem))
        {
            if (QuestManager.Instance.HasItem(requiredItem))
            {
                isCompleted = true; // Marca que o diálogo pós-item deve ser usado
            }
        }

        // 3. Escolhe qual array de falas usar no diálogo
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

        // Envia a interação para o QuestManager validar/completar a missão ativa
        QuestManager.Instance.Interact(
            type.ToString(),
            id
        );

        // Se o NPC é o entregador de uma missão (ou o alvo), marcamos como completa
        if (type == Type.Delivery || (type == Type.NPC && !givesQuest))
        {
            isCompleted = true;
        }

        if (givesQuest)
        {
            QuestManager.Instance.StartQuest(
                questType,
                target,
                requiredItem
            );

            givesQuest = false;
        }
    }
}
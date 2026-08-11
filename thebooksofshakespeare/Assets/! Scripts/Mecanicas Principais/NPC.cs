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

    [Header("Diálogo")]
    [TextArea]
    public string[] dialogue;

    [Header("Missão")]
    public bool givesQuest;

    public QuestManager.QuestType questType;

    public string target;

    public string requiredItem;

    private float interactCooldown;

    public void Interact()
    {
        if (Time.time < interactCooldown)
            return;

        if (DialogueManager.Instance == null)
            return;

        if (DialogueManager.Instance.IsTalking)
            return;


        // =========================
        // ITEM
        // =========================

        if (type == Type.Item)
        {
            QuestManager.Instance.AddItem(id);

            QuestManager.Instance.Interact(
                "Item",
                id
            );

            interactCooldown = Time.time + 0.5f;

            Destroy(gameObject);

            return;
        }


        // =========================
        // NPC
        // =========================

        DialogueManager.Instance.StartDialogue(
            npcName,
            dialogue,
            this
        );
    }


    public void FinishInteraction()
    {
        interactCooldown = Time.time + 0.5f;


        // Verifica se essa interação cumpre
        // a missão atual.

        QuestManager.Instance.Interact(
            type.ToString(),
            id
        );


        // =========================
        // DAR NOVA MISSÃO
        // =========================

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
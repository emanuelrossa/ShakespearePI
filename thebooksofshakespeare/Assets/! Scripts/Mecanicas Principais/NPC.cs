using UnityEngine;

public class NPC : MonoBehaviour
{
    public enum Type
    {
        NPC,
        Item,
        Delivery
    }

    public Type type;

    public string id;

    public string npcName;

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

        if (DialogueManager.Instance.IsTalking)
            return;

        if (type == Type.Item)
        {
            QuestManager.Instance.Interact("Item", id);
            Destroy(gameObject);
            return;
        }

        DialogueManager.Instance.StartDialogue(
            npcName,
            dialogue,
            this
        );
    }

    public void FinishInteraction()
    {
        interactCooldown = Time.time + 0.3f;

        QuestManager.Instance.Interact(type.ToString(), id);

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
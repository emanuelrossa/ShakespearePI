using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public enum QuestType
    {
        Talk,
        Collect,
        Deliver
    }

    [Header("Missão Atual")]
    public QuestType questType;

    public string targetID;
    public string requiredItem;

    public bool completed;

    private void Awake()
    {
        Instance = this;
    }

    public void Interact(string type, string id)
    {
        if (completed) return;

        switch (questType)
        {
            case QuestType.Talk:

                if (type == "NPC" && id == targetID)
                {
                    Complete();
                }

                break;

            case QuestType.Collect:

                if (type == "Item" && id == targetID)
                {
                    Inventory.Instance.AddItem(id);
                    Complete();
                }

                break;

            case QuestType.Deliver:

                if (type == "Delivery" && id == targetID)
                {
                    if (Inventory.Instance.HasItem(requiredItem))
                    {
                        Inventory.Instance.RemoveItem(requiredItem);
                        Complete();
                    }
                }

                break;
        }
    }

    void Complete()
    {
        completed = true;
        Debug.Log("Missão concluída!");
    }

    public void StartQuest(QuestType type, string target, string item = "")
    {
        questType = type;
        targetID = target;
        requiredItem = item;
        completed = false;
    }
}
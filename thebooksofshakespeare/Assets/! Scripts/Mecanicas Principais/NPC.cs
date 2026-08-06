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

    public void Interact()
    {
        // Abre diálogo apenas para NPCs e entregas
        if (type == Type.NPC || type == Type.Delivery)
        {
            Debug.Log("Conversando...");
            // DialogueManager.Instance.StartDialogue(...);
        }

        QuestManager.Instance.Interact(type.ToString(), id);

        if (type == Type.Item)
            Destroy(gameObject);
    }
}
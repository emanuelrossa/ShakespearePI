using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public enum QuestType
    {
        Talk,
        Find,
        Collect,
        Deliver
    }

    [Header("UI da missão atual")]
    public GameObject questBox;
    public TMP_Text questText;

    [Header("Notificação de nova missão")]
    public GameObject questNotification;
    public TMP_Text questNotificationText;
    public float notificationTime = 3f;

    [Header("Missão atual")]
    public bool questActive;

    public QuestType currentQuestType;
    public string currentTarget;
    public string currentRequiredItem;

    private HashSet<string> inventory = new HashSet<string>();

    private Coroutine notificationCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        UpdateQuestUI();

        if (questNotification != null)
            questNotification.SetActive(false);
    }

    // =========================
    // INICIAR MISSÃO
    // =========================

    public void StartQuest(
        QuestType type,
        string target,
        string requiredItem = ""
    )
    {
        if (questActive)
            return;

        questActive = true;

        currentQuestType = type;
        currentTarget = target;
        currentRequiredItem = requiredItem;

        Debug.Log(
            "Missão iniciada: " +
            type +
            " | Alvo: " +
            target +
            " | Item: " +
            requiredItem
        );

        UpdateQuestUI();

        ShowQuestNotification();
    }

    // =========================
    // NOTIFICAÇÃO
    // =========================

    private void ShowQuestNotification()
    {
        if (questNotification == null || questNotificationText == null)
            return;

        questNotificationText.text = GetQuestText();

        questNotification.SetActive(true);

        if (notificationCoroutine != null)
            StopCoroutine(notificationCoroutine);

        notificationCoroutine = StartCoroutine(HideNotification());
    }

    private IEnumerator HideNotification()
    {
        yield return new WaitForSeconds(notificationTime);

        questNotification.SetActive(false);
        notificationCoroutine = null;
    }

    // =========================
    // TEXTO DA MISSÃO
    // =========================

    private string GetQuestText()
    {
        switch (currentQuestType)
        {
            case QuestType.Talk:
                return "Fale com: " + currentTarget;

            case QuestType.Find:
                return "Encontre: " + currentTarget;

            case QuestType.Collect:
                return "Encontre o item: " + currentTarget;

            case QuestType.Deliver:
                return "Leve " +
                       currentRequiredItem +
                       " para " +
                       currentTarget;
        }

        return "";
    }

    // =========================
    // INTERAÇÃO
    // =========================

    public void Interact(string type, string id)
    {
        if (!questActive)
            return;

        switch (currentQuestType)
        {
            case QuestType.Talk:

                if (type == "NPC" && id == currentTarget)
                {
                    CompleteQuest();
                }

                break;

            case QuestType.Find:

                if (id == currentTarget)
                {
                    CompleteQuest();
                }

                break;

            case QuestType.Collect:

                if (type == "Item" && id == currentTarget)
                {
                    CompleteQuest();
                }

                break;

            case QuestType.Deliver:

                if (type == "NPC" && id == currentTarget)
                {
                    if (HasItem(currentRequiredItem))
                    {
                        RemoveItem(currentRequiredItem);
                        CompleteQuest();
                    }
                    else
                    {
                        Debug.Log(
                            "Você precisa do item: " +
                            currentRequiredItem
                        );
                    }
                }

                break;
        }

        UpdateQuestUI();
    }

    // =========================
    // INVENTÁRIO
    // =========================

    public void AddItem(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
            return;

        inventory.Add(itemID);

        Debug.Log("Item obtido: " + itemID);

        UpdateQuestUI();
    }

    public bool HasItem(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
            return false;

        return inventory.Contains(itemID);
    }

    public void RemoveItem(string itemID)
    {
        if (inventory.Contains(itemID))
        {
            inventory.Remove(itemID);

            Debug.Log("Item usado: " + itemID);
        }
    }

    // =========================
    // COMPLETAR MISSÃO
    // =========================

    public void CompleteQuest()
    {
        if (!questActive)
            return;

        Debug.Log("MISSÃO CONCLUÍDA!");

        questActive = false;

        currentTarget = "";
        currentRequiredItem = "";

        UpdateQuestUI();
    }

    // =========================
    // UI DA MISSÃO
    // =========================

    private void UpdateQuestUI()
    {
        if (questBox == null || questText == null)
            return;

        if (!questActive)
        {
            questBox.SetActive(false);
            return;
        }

        questBox.SetActive(true);

        questText.text = GetQuestText();
    }
}
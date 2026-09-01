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

    [Header("UI da miss�o atual")]
    public GameObject questBox;
    public TMP_Text questText;

    [Header("Anima��o da miss�o")]
    public float questBoxSlideDuration = 0.35f;
    public float questBoxSlideDistance = 300f;

    [Header("Notifica��o de nova miss�o")]
    public GameObject questNotification;
    public TMP_Text questNotificationText;

    [Tooltip("Quanto tempo a notifica��o fica parada na tela")]
    public float notificationTime = 3f;

    [Tooltip("Velocidade da anima��o da notifica��o")]
    public float slideDuration = 0.35f;

    [Header("Miss�o atual")]
    public bool questActive;

    public QuestType currentQuestType;
    public string currentTarget;
    public string currentRequiredItem;
    private string currentCustomText = "";

    private HashSet<string> inventory = new HashSet<string>();

    private Coroutine notificationCoroutine;

    private RectTransform notificationRect;
    private Vector2 notificationHiddenPosition;
    private Vector2 notificationShownPosition;

    private Coroutine questBoxCoroutine;

    private RectTransform questBoxRect;
    private Vector2 questBoxShownPosition;
    private Vector2 questBoxHiddenPosition;

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
        if (questBox != null)
        {
            questBoxRect =
                questBox.GetComponent<RectTransform>();

            if (questBoxRect != null)
            {
                questBoxShownPosition =
                    questBoxRect.anchoredPosition;

                questBoxHiddenPosition =
                    questBoxShownPosition +
                    new Vector2(-questBoxSlideDistance, 0f);

                questBoxRect.anchoredPosition =
                    questBoxHiddenPosition;
            }

            questBox.SetActive(false);
        }

        if (questNotification != null)
        {
            notificationRect =
                questNotification.GetComponent<RectTransform>();

            if (notificationRect != null)
            {
                notificationShownPosition =
                    notificationRect.anchoredPosition;

                notificationHiddenPosition =
                    notificationShownPosition +
                    new Vector2(0f, 150f);

                notificationRect.anchoredPosition =
                    notificationHiddenPosition;
            }

            questNotification.SetActive(false);
        }

        if (questActive)
        {
            UpdateQuestText();
            ShowQuestBox();
        }
    }

    // NOVO: parametro customText opcional
    public void StartQuest(
        QuestType type,
        string target,
        string requiredItem = "",
        string customText = ""
    )
    {
        if (questActive)
            return;


        questActive = true;

        currentQuestType = type;
        currentTarget = target;
        currentRequiredItem = requiredItem;
        currentCustomText = customText;

        UpdateQuestText();

        ShowQuestBox();

        ShowQuestNotification();
    }

    private string GetQuestText()
    {
        // NOVO: se tiver texto customizado, usa ele
        if (!string.IsNullOrEmpty(currentCustomText))
            return currentCustomText;

        switch (currentQuestType)
        {
            case QuestType.Talk:

                return "Fale com: " +
                       currentTarget;


            case QuestType.Find:

                return "Encontre: " +
                       currentTarget;


            case QuestType.Collect:

                return "Encontre o item: " +
                       currentTarget;


            case QuestType.Deliver:

                return "Leve " +
                       currentRequiredItem +
                       " para " +
                       currentTarget;
        }

        return "";
    }


    private void UpdateQuestText()
    {
        if (questText == null)
            return;

        if (!questActive)
            return;

        questText.text = GetQuestText();
    }

    private void ShowQuestBox()
    {
        if (questBox == null)
            return;

        if (questBoxRect == null)
            questBoxRect =
                questBox.GetComponent<RectTransform>();


        if (questBoxCoroutine != null)
        {
            StopCoroutine(questBoxCoroutine);
        }


        questBoxCoroutine =
            StartCoroutine(QuestBoxSlideIn());
    }


    private IEnumerator QuestBoxSlideIn()
    {
        questBox.SetActive(true);

        if (questBoxRect == null)
            yield break;


        questBoxRect.anchoredPosition =
            questBoxHiddenPosition;


        float time = 0f;


        while (time < questBoxSlideDuration)
        {
            time += Time.unscaledDeltaTime;

            float t =
                time / questBoxSlideDuration;

            t =
                1f - Mathf.Pow(1f - t, 3f);


            questBoxRect.anchoredPosition =
                Vector2.Lerp(
                    questBoxHiddenPosition,
                    questBoxShownPosition,
                    t
                );


            yield return null;
        }


        questBoxRect.anchoredPosition =
            questBoxShownPosition;


        questBoxCoroutine = null;
    }

    private void HideQuestBox()
    {
        if (questBox == null)
            return;


        if (questBoxRect == null)
            questBoxRect =
                questBox.GetComponent<RectTransform>();


        if (questBoxCoroutine != null)
        {
            StopCoroutine(questBoxCoroutine);
        }


        questBoxCoroutine =
            StartCoroutine(QuestBoxSlideOut());
    }


    private IEnumerator QuestBoxSlideOut()
    {
        if (questBoxRect == null)
        {
            questBox.SetActive(false);
            yield break;
        }


        float time = 0f;

        Vector2 startPosition =
            questBoxRect.anchoredPosition;


        while (time < questBoxSlideDuration)
        {
            time += Time.unscaledDeltaTime;

            float t =
                time / questBoxSlideDuration;

            t =
                Mathf.Pow(t, 3f);


            questBoxRect.anchoredPosition =
                Vector2.Lerp(
                    startPosition,
                    questBoxHiddenPosition,
                    t
                );


            yield return null;
        }


        questBoxRect.anchoredPosition =
            questBoxHiddenPosition;


        questBox.SetActive(false);

        questBoxCoroutine = null;
    }


    private void ShowQuestNotification()
    {
        if (questNotification == null ||
            questNotificationText == null)
            return;


        questNotificationText.text =
            GetQuestText();


        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
        }


        notificationCoroutine =
            StartCoroutine(
                QuestNotificationAnimation()
            );
    }


    private IEnumerator QuestNotificationAnimation()
    {
        questNotification.SetActive(true);


        if (notificationRect != null)
        {
            notificationRect.anchoredPosition =
                notificationHiddenPosition;
        }

        float time = 0f;


        while (time < slideDuration)
        {
            time += Time.unscaledDeltaTime;

            float t =
                time / slideDuration;

            t =
                1f - Mathf.Pow(1f - t, 3f);


            if (notificationRect != null)
            {
                notificationRect.anchoredPosition =
                    Vector2.Lerp(
                        notificationHiddenPosition,
                        notificationShownPosition,
                        t
                    );
            }


            yield return null;
        }


        if (notificationRect != null)
        {
            notificationRect.anchoredPosition =
                notificationShownPosition;
        }

        yield return new WaitForSecondsRealtime(
            notificationTime
        );

        time = 0f;


        while (time < slideDuration)
        {
            time += Time.unscaledDeltaTime;

            float t =
                time / slideDuration;

            t =
                Mathf.Pow(t, 3f);


            if (notificationRect != null)
            {
                notificationRect.anchoredPosition =
                    Vector2.Lerp(
                        notificationShownPosition,
                        notificationHiddenPosition,
                        t
                    );
            }


            yield return null;
        }


        if (notificationRect != null)
        {
            notificationRect.anchoredPosition =
                notificationHiddenPosition;
        }


        questNotification.SetActive(false);

        notificationCoroutine = null;
    }

    public void Interact(string type, string id)
    {
        if (!questActive)
            return;


        switch (currentQuestType)
        {

            case QuestType.Talk:

                if (type == "NPC" &&
                    id == currentTarget)
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

                if (type == "Item" &&
                    id == currentTarget)
                {
                    CompleteQuest();
                }

                break;

            case QuestType.Deliver:

                if (type == "NPC" &&
                    id == currentTarget)
                {
                    if (HasItem(currentRequiredItem))
                    {
                        RemoveItem(currentRequiredItem);

                        CompleteQuest();
                    }
                }

                break;
        }


        UpdateQuestText();
    }

    public void AddItem(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
            return;


        inventory.Add(itemID);

        UpdateQuestText();
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
        }
    }
    public void CompleteQuest()
    {
        if (!questActive)
            return;

        questActive = false;

        currentTarget = "";
        currentRequiredItem = "";
        currentCustomText = "";

        HideQuestBox();
    }
}
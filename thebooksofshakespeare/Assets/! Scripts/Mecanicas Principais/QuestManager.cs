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

    [Header("Animação da missão")]
    public float questBoxSlideDuration = 0.35f;
    public float questBoxSlideDistance = 300f;

    [Header("Notificação de nova missão")]
    public GameObject questNotification;
    public TMP_Text questNotificationText;

    [Tooltip("Quanto tempo a notificação fica parada na tela")]
    public float notificationTime = 3f;

    [Tooltip("Velocidade da animação da notificação")]
    public float slideDuration = 0.35f;

    [Header("Missão atual")]
    public bool questActive;

    public QuestType currentQuestType;
    public string currentTarget;
    public string currentRequiredItem;

    private HashSet<string> inventory = new HashSet<string>();

    // ---------------------------------------------------------
    // NOTIFICAÇÃO
    // ---------------------------------------------------------

    private Coroutine notificationCoroutine;

    private RectTransform notificationRect;
    private Vector2 notificationHiddenPosition;
    private Vector2 notificationShownPosition;

    // ---------------------------------------------------------
    // QUEST BOX
    // ---------------------------------------------------------

    private Coroutine questBoxCoroutine;

    private RectTransform questBoxRect;
    private Vector2 questBoxShownPosition;
    private Vector2 questBoxHiddenPosition;


    // =========================================================
    // UNITY
    // =========================================================

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
        // -----------------------------------------------------
        // CONFIGURA QUEST BOX
        // -----------------------------------------------------

        if (questBox != null)
        {
            questBoxRect =
                questBox.GetComponent<RectTransform>();

            if (questBoxRect != null)
            {
                // Guarda a posição normal
                questBoxShownPosition =
                    questBoxRect.anchoredPosition;

                // Posição escondida à esquerda
                questBoxHiddenPosition =
                    questBoxShownPosition +
                    new Vector2(-questBoxSlideDistance, 0f);

                // Começa escondida
                questBoxRect.anchoredPosition =
                    questBoxHiddenPosition;
            }

            questBox.SetActive(false);
        }


        // -----------------------------------------------------
        // CONFIGURA NOTIFICAÇÃO
        // -----------------------------------------------------

        if (questNotification != null)
        {
            notificationRect =
                questNotification.GetComponent<RectTransform>();

            if (notificationRect != null)
            {
                // Guarda a posição normal
                notificationShownPosition =
                    notificationRect.anchoredPosition;

                // Posição escondida acima
                notificationHiddenPosition =
                    notificationShownPosition +
                    new Vector2(0f, 150f);

                // Começa escondida
                notificationRect.anchoredPosition =
                    notificationHiddenPosition;
            }

            questNotification.SetActive(false);
        }


        // -----------------------------------------------------
        // CASO JÁ TENHA UMA QUEST ATIVA
        // -----------------------------------------------------

        if (questActive)
        {
            UpdateQuestText();
            ShowQuestBox();
        }
    }


    // =========================================================
    // INICIAR MISSÃO
    // =========================================================

    public void StartQuest(
        QuestType type,
        string target,
        string requiredItem = ""
    )
    {
        // Não começa outra missão enquanto
        // uma já estiver ativa.
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


        // Atualiza o texto antes de mostrar
        UpdateQuestText();

        // Caixa da missão desliza da esquerda
        ShowQuestBox();

        // Notificação "NOVA MISSÃO" desliza de cima
        ShowQuestNotification();
    }


    // =========================================================
    // TEXTO DA MISSÃO
    // =========================================================

    private string GetQuestText()
    {
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


    // =========================================================
    // QUEST BOX - ENTRADA
    // =========================================================

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

            // Ease Out
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


    // =========================================================
    // QUEST BOX - SAÍDA
    // =========================================================

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

            // Ease In
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


    // =========================================================
    // NOTIFICAÇÃO - "NOVA MISSÃO"
    // =========================================================

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


        // -----------------------------------------------------
        // ENTRADA
        // -----------------------------------------------------

        float time = 0f;


        while (time < slideDuration)
        {
            time += Time.unscaledDeltaTime;

            float t =
                time / slideDuration;


            // Ease Out
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


        // -----------------------------------------------------
        // FICA NA TELA
        // -----------------------------------------------------

        yield return new WaitForSecondsRealtime(
            notificationTime
        );


        // -----------------------------------------------------
        // SAÍDA
        // -----------------------------------------------------

        time = 0f;


        while (time < slideDuration)
        {
            time += Time.unscaledDeltaTime;

            float t =
                time / slideDuration;


            // Ease In
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


    // =========================================================
    // INTERAÇÃO
    // =========================================================

    public void Interact(string type, string id)
    {
        if (!questActive)
            return;


        switch (currentQuestType)
        {
            // -------------------------------------------------
            // TALK
            // -------------------------------------------------

            case QuestType.Talk:

                if (type == "NPC" &&
                    id == currentTarget)
                {
                    CompleteQuest();
                }

                break;


            // -------------------------------------------------
            // FIND
            // -------------------------------------------------

            case QuestType.Find:

                if (id == currentTarget)
                {
                    CompleteQuest();
                }

                break;


            // -------------------------------------------------
            // COLLECT
            // -------------------------------------------------

            case QuestType.Collect:

                if (type == "Item" &&
                    id == currentTarget)
                {
                    CompleteQuest();
                }

                break;


            // -------------------------------------------------
            // DELIVER
            // -------------------------------------------------

            case QuestType.Deliver:

                if (type == "NPC" &&
                    id == currentTarget)
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


        UpdateQuestText();
    }


    // =========================================================
    // INVENTÁRIO
    // =========================================================

    public void AddItem(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
            return;


        inventory.Add(itemID);


        Debug.Log(
            "Item obtido: " +
            itemID
        );


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


            Debug.Log(
                "Item usado: " +
                itemID
            );
        }
    }


    // =========================================================
    // COMPLETAR MISSÃO
    // =========================================================

    public void CompleteQuest()
    {
        if (!questActive)
            return;


        Debug.Log("MISSÃO CONCLUÍDA!");


        questActive = false;

        currentTarget = "";
        currentRequiredItem = "";


        // Agora a caixa sai deslizando
        // em vez de simplesmente desaparecer.
        HideQuestBox();
    }
}
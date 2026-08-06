using UnityEngine;
using TMPro; // Necessário para o texto da UI

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [Header("UI de Missão")]
    public TextMeshProUGUI questTitleText;
    public TextMeshProUGUI questDescriptionText;
    public GameObject questPanel;

    [Header("Missão Atual")]
    public string currentQuestTitle;
    public string currentQuestDescription;
    public bool hasQuest = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateQuestUI();
    }

    public void AcceptQuest(string title, string description)
    {
        currentQuestTitle = title;
        currentQuestDescription = description;
        hasQuest = true;
        questPanel.SetActive(true);
        UpdateQuestUI();
    }

    public void CompleteQuest()
    {
        currentQuestTitle = "Missão Concluída!";
        currentQuestDescription = "Bom trabalho!";
        hasQuest = false;
        UpdateQuestUI();

        // Esconde o painel após 3 segundos
        Invoke("HideQuestPanel", 3f);
    }

    void UpdateQuestUI()
    {
        if (questTitleText != null) questTitleText.text = currentQuestTitle;
        if (questDescriptionText != null) questDescriptionText.text = currentQuestDescription;
    }

    void HideQuestPanel()
    {
        if (!hasQuest) questPanel.SetActive(false);
    }
}
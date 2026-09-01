using System.Collections;
using UnityEngine;
using StarterAssets;

public class IntroSequence : MonoBehaviour
{
    public FirstPersonController player;

    [TextArea]
    public string[] introDialogue = new string[]
    {
        "*bocejo*",
        "Acho que já está um pouco tarde...",
        "Melhor eu ir deitar agora."
    };

    [Header("Missão que aparece depois do monólogo")]
    public string questTargetId = "Cama";
    public string questCustomText = "Deitar na cama";

    void Start()
    {
        StartCoroutine(BeginIntro());
    }

    IEnumerator BeginIntro()
    {
        yield return null; // espera 1 frame pros singletons (DialogueManager, QuestManager) rodarem o Awake

        // dispara o monólogo direto, sem passar por nenhum NPC
        DialogueManager.Instance.StartDialogue("Shakespeare", introDialogue, null);

        // espera o diálogo acabar
        while (DialogueManager.Instance.IsTalking)
            yield return null;

        // dá a missão manualmente
        QuestManager.Instance.StartQuest(
            QuestManager.QuestType.Find,
            questTargetId,
            "",
            questCustomText
        );
    }
}
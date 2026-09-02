using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    public bool introMonologueDone;
    public bool questTriggered;
    public string currentQuestId = "";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
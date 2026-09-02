using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    string savePath;

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
            return;
        }

        savePath = Path.Combine(Application.persistentDataPath, "save.json");
    }

    public void Save()
    {
        SaveData data = new SaveData();
        data.currentScene = SceneManager.GetActiveScene().name;

        if (GameState.Instance != null)
        {
            data.introMonologueDone = GameState.Instance.introMonologueDone;
            data.questTriggered = GameState.Instance.questTriggered;
            data.currentQuestId = GameState.Instance.currentQuestId;
        }

        // acha TODOS os objetos com SaveableEntity na cena (player + npcs)
        SaveableEntity[] entities = FindObjectsOfType<SaveableEntity>();
        foreach (SaveableEntity entity in entities)
        {
            data.entities.Add(entity.GetSaveData());
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log("Save feito com " + entities.Length + " entidades em: " + savePath);
    }

    public void Load()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Não existe save ainda.");
            return;
        }

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (SceneManager.GetActiveScene().name != data.currentScene)
        {
            SceneManager.LoadScene(data.currentScene);
        }

        StartCoroutine(ApplyLoadedData(data));
    }

    System.Collections.IEnumerator ApplyLoadedData(SaveData data)
    {
        yield return null; // espera 1 frame a cena carregar

        // monta um dicionário id -> dados pra achar rápido
        Dictionary<string, EntitySaveData> lookup = new Dictionary<string, EntitySaveData>();
        foreach (EntitySaveData e in data.entities)
            lookup[e.id] = e;

        SaveableEntity[] entities = FindObjectsOfType<SaveableEntity>();
        foreach (SaveableEntity entity in entities)
        {
            if (lookup.ContainsKey(entity.id))
                entity.LoadFromData(lookup[entity.id]);
        }

        if (GameState.Instance != null)
        {
            GameState.Instance.introMonologueDone = data.introMonologueDone;
            GameState.Instance.questTriggered = data.questTriggered;
            GameState.Instance.currentQuestId = data.currentQuestId;
        }
    }

    public bool HasSave() => File.Exists(savePath);
}
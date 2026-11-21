using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System.Collections;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private string savePath;

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

        if (data.entities == null)
            data.entities = new List<EntitySaveData>();

        SaveableEntity[] entities = Object.FindObjectsByType<SaveableEntity>(FindObjectsSortMode.None);
        foreach (SaveableEntity entity in entities)
        {
            if (entity != null)
            {
                data.entities.Add(entity.GetSaveData());
            }
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log("Save realizado com sucesso! Entidades salvas: " + entities.Length);
    }

    public void Load()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Ainda não existe nenhum arquivo de save.");
            return;
        }

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (SceneManager.GetActiveScene().name != data.currentScene)
        {
            StartCoroutine(LoadSceneAndApply(data));
        }
        else
        {
            StartCoroutine(ApplyLoadedData(data));
        }
    }

    IEnumerator LoadSceneAndApply(SaveData data)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(data.currentScene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return ApplyLoadedDataRoutine(data);
    }

    IEnumerator ApplyLoadedData(SaveData data)
    {
        yield return null;
        yield return ApplyLoadedDataRoutine(data);
    }

    IEnumerator ApplyLoadedDataRoutine(SaveData data)
    {
        yield return new WaitForSeconds(0.1f);

        if (data.entities != null)
        {
            Dictionary<string, EntitySaveData> lookup = new Dictionary<string, EntitySaveData>();
            foreach (EntitySaveData e in data.entities)
            {
                if (e != null && !string.IsNullOrEmpty(e.id))
                    lookup[e.id] = e;
            }

            SaveableEntity[] entities = Object.FindObjectsByType<SaveableEntity>(FindObjectsSortMode.None);
            foreach (SaveableEntity entity in entities)
            {
                if (entity != null && lookup.ContainsKey(entity.id))
                {
                    entity.LoadFromData(lookup[entity.id]);
                }
            }
        }

        if (GameState.Instance != null)
        {
            GameState.Instance.introMonologueDone = data.introMonologueDone;
            GameState.Instance.questTriggered = data.questTriggered;
            GameState.Instance.currentQuestId = data.currentQuestId;
        }

        Debug.Log("Dados do save aplicados com sucesso!");
    }

    public bool HasSave() => File.Exists(savePath);
}
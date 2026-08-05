using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static string CaminhoArquivo => Application.persistentDataPath + "/savegame.json";

    public static void Salvar(SaveData dados)
    {
        string json = JsonUtility.ToJson(dados, true);
        File.WriteAllText(CaminhoArquivo, json);
    }

    public static SaveData Carregar()
    {
        if (!File.Exists(CaminhoArquivo))
            return null;

        string json = File.ReadAllText(CaminhoArquivo);
        return JsonUtility.FromJson<SaveData>(json);
    }
}
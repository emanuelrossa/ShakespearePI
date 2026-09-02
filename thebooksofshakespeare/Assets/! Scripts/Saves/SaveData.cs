using System.Collections.Generic;

[System.Serializable]
public class EntitySaveData
{
    public string id;
    public float posX, posY, posZ;
    public float rotY; // rotação no eixo Y, geralmente é só isso que importa em jogo 2D/3D top-down
}

[System.Serializable]
public class SaveData
{
    public string currentScene;
    public bool introMonologueDone;
    public bool questTriggered;
    public string currentQuestId;

    public List<EntitySaveData> entities = new List<EntitySaveData>();
}
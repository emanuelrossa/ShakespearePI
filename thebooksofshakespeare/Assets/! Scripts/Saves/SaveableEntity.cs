using UnityEngine;

public class SaveableEntity : MonoBehaviour
{
    [Tooltip("Tem que ser ÚNICO pra cada objeto (player, npc1, npc2...)")]
    public string id;

    public EntitySaveData GetSaveData()
    {
        EntitySaveData data = new EntitySaveData();
        data.id = id;
        data.posX = transform.position.x;
        data.posY = transform.position.y;
        data.posZ = transform.position.z;
        data.rotY = transform.eulerAngles.y;
        return data;
    }

    public void LoadFromData(EntitySaveData data)
    {
        transform.position = new Vector3(data.posX, data.posY, data.posZ);
        transform.eulerAngles = new Vector3(0, data.rotY, 0);
    }
}
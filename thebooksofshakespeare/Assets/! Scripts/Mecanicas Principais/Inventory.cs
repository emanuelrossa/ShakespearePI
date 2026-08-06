using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public List<string> items = new List<string>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddItem(string item)
    {
        if (!items.Contains(item))
            items.Add(item);

        Debug.Log("Pegou: " + item);
    }

    public bool HasItem(string item)
    {
        return items.Contains(item);
    }

    public void RemoveItem(string item)
    {
        if (items.Contains(item))
            items.Remove(item);
    }
}
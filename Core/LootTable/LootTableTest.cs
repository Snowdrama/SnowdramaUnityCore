using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public int id;
    public string name;
}

[System.Serializable]
public class LooTableItem
{
    public Item item;
    public double weight;
}
public class LootTableTest : MonoBehaviour
{
    [SerializeField] private List<LooTableItem> lootItems = new List<LooTableItem>();

    [SerializeField] private List<Item> items = new List<Item>();
    private LootTable<Item> lootTable = new LootTable<Item>();

    private void Start()
    {
        foreach (var item in lootItems)
        {
            lootTable.Add(item.item, item.weight);
        }

        int currentLoop = 0;
        while (items.Count < 5)
        {
            currentLoop++;
            Debug.Log($"Roll: {currentLoop}***********************");
            var weighted = lootTable.GetRandomFullTableWeightIncrease();
            for (int j = 0; j < weighted.Count; j++)
            {
                items.Add(weighted[j]);
                Debug.Log($"[{j}]: {weighted[j].name}");
            }
        }
    }
}

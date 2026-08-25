using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootTable_DiminishingTest : MonoBehaviour
{
    [SerializeField] private TMP_InputField items;
    [SerializeField] private TMP_InputField output;

    [SerializeField] private Button OneItem;
    [SerializeField] private Button FiveItems;
    [SerializeField] private Button TenItems;
    [SerializeField] private Button ThirtyItems;
    [SerializeField] private Button OneHundredItems;

    private LootTable<string> table;

    private void Start()
    {
        table = new LootTable<string>();
        items.text = this.SortList(items.text);
        var itemString = items.text;
        var itemList = itemString.Split('\n');
        foreach (var itemRecord in itemList)
        {
            if (!string.IsNullOrEmpty(itemRecord))
            {
                var item = itemRecord.Split(':');
                table.Add(item[0], double.Parse(item[1]));
            }
        }

        OneItem.onClick.AddListener(this.TakeOneItem);
        FiveItems.onClick.AddListener(this.TakeFiveItems);
        TenItems.onClick.AddListener(this.TakeTenItems);
        ThirtyItems.onClick.AddListener(this.TakeThirtyItems);
        OneHundredItems.onClick.AddListener(this.TakeOneHundredItems);
    }

    public void TakeOneItem()
    {
        var items = table.GetRandomDiminishing_Debug(1);
        this.OutputItems(items);
    }
    public void TakeFiveItems()
    {
        var items = table.GetRandomDiminishing_Debug(5);
        this.OutputItems(items);
    }
    public void TakeTenItems()
    {
        var items = table.GetRandomDiminishing_Debug(10);
        this.OutputItems(items);
    }
    public void TakeThirtyItems()
    {
        var items = table.GetRandomDiminishing_Debug(30);
        this.OutputItems(items);
    }
    public void TakeOneHundredItems()
    {
        var items = table.GetRandomDiminishing_Debug(100);
        this.OutputItems(items);
    }

    public void OutputItems(List<(string item, double weight, double roll, double min, double max)> items)
    {
        var outString = "";
        foreach (var item in items)
        {
            outString += $"{item.item}, weight:{item.weight:F2} - {item.min:F2} < {item.roll:F2} < {item.max:F2}\n";
        }
        output.text += outString;
        output.text = this.SortList(output.text);
    }


    public string SortList(string list)
    {
        var split = list.Split('\n');
        var sorted = new List<string>(split).OrderBy(x => x).ToList();
        var outputString = "";
        foreach (var item in sorted)
        {
            if (!string.IsNullOrEmpty(item))
            {
                outputString += $"{item}\n";
            }
        }
        return outputString;
    }
}

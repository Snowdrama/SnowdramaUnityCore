using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootTable_WeightIncreaseLootTest : MonoBehaviour
{
    [SerializeField] private TMP_InputField items;
    [SerializeField] private TMP_InputField output;
    [SerializeField] private Button RollTableButton;
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

        RollTableButton.onClick.AddListener(this.RollTable);
    }

    public void RollTable()
    {
        var items = table.GetRandomFullTableWeightIncreaseDebug(5.0f);
        var outString = "";
        foreach (var item in items)
        {
            outString += $"{item.Item1:F2} - Rolled: {item.Item2:F2} < {item.Item3:F2}\n";
        }
        output.text = this.SortList(outString);
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

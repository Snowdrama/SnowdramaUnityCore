using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommandConsole_TextArea : MonoBehaviour
{
    private static CommandConsole_TextArea instance;
    private static TMP_Text textArea;
    //private static int consoleHistoryLength = 64;
    //private static bool needsUpdating;

    private static Queue<string> consoleStrings = new Queue<string>();


    public void OnEable()
    {
        instance = this;
    }




    public static void PrintText(string lineToPush)
    {
        consoleStrings.Enqueue(lineToPush);
        UpdateText();
    }

    private static void UpdateText()
    {
        string output = "";
        foreach (string s in consoleStrings)
        {
            output += $"{s}\n";
        }

        textArea.text = output;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogSysten : MonoBehaviour
{
    public int testNumber = 3;

    [SerializeField] private TextAsset dialogAsset;
    [SerializeField] private UnitySerializedDictionary<string, DialogLine> dialogLines;
    private void Start()
    {
        string[] lines = dialogAsset.text.Split(Environment.NewLine);
        Debug.Log($"Parsed Line Count: {lines.Length}");
        ParseDialog(lines);

    }
    public List<DialogLine> GetLinesWithIndentation(string[] lines, ref int index)
    {



        return new List<DialogLine>();
    }

    public void ParseDialog(string[] textLines)
    {
        //first we need to parse the line and figure out what it is
        //let's make a list of all the lines with their indentation
        List<DialogLine> tempLines = new List<DialogLine>();

        for (int i = 0; i < textLines.Length; i++)
        {
            string line = textLines[i];
            int indent = 0;


            //for every 4 spaces we count that as 1 indentation
            while (line.StartsWith("    "))
            {
                line = line.Substring(4, line.Length - 4);
                indent++;
            }

            if (line.Contains("//"))
            {
                var index = line.IndexOf("//");
                line = line.Substring(0, index);
            }

            //then trim any remaining white space
            line = line.Trim();




            DialogLine lineInfo = new DialogLine()
            {
                indent = indent,
                lineIndex = i,
                text = line,
                type = DialogInstructionType.None,
                childLines = new List<DialogLine>()
            };


            if (line.Length == 0)
            {
                //a 0 length should be ignored
                continue;
            }

            if (line.StartsWith("#"))
            {
                //this is the start of a node
                lineInfo.type = DialogInstructionType.Bookmark;
            }
            else if (line.StartsWith("*"))
            {
                //this is the start of a node
                lineInfo.type = DialogInstructionType.Metadata;
            }
            else if (line.StartsWith("->"))
            {
                lineInfo.type = DialogInstructionType.Choice;
            }
            else if (line.StartsWith("=>"))
            {
                lineInfo.type = DialogInstructionType.Jump;
            }
            else if (line.StartsWith("!if") || line.StartsWith("!elseif") || line.StartsWith("!else"))
            {
                lineInfo.type = DialogInstructionType.Conditional;
            }
            else if (line.StartsWith("@"))
            {
                //this is mapped to some action/handler thing
                lineInfo.type = DialogInstructionType.Command;
            }
            else if (line.StartsWith("%"))
            {
                //randomized dialog
                lineInfo.type = DialogInstructionType.DialogueRandom;
            }
            else
            {
                lineInfo.type = DialogInstructionType.Dialogue;
            }

            Debug.Log($"Line[{i}]: {lineInfo.type} -> indentation:{indent} -> {line}");

            tempLines.Add(lineInfo);
        }

        //now let's collect all the bookmarks
        foreach (var item in tempLines)
        {
            if (item.type == DialogInstructionType.Bookmark)
            {
                var name = item.text.Replace("#", "");
                dialogLines.Add(name, item);
            }
        }

        //now lets collapse the results for those that 
        //have custom types


        foreach (var item in tempLines)
        {
            if (item.type == DialogInstructionType.Bookmark)
            {
                var name = item.text.Replace("#", "");
                dialogLines.Add(name, item);
            }
        }


        Stack<DialogLine> lineStack = new Stack<DialogLine>();

        foreach (var dl in tempLines)
        {
            Debug.LogWarning($"Testing Line - [{dl.lineIndex}]:{dl.text}");
            //assume we're a root if we have no stack.
            if (lineStack.Count == 0)
            {
                Debug.Log($"Line is Root!");
                lineStack.Push(dl);
                continue;
            }

            while (lineStack.Count > 0)
            {
                //  #bookmark
                //      @chocice Let's Go to the store
                //          Steve: Let's go to the store!
                //      @chocice Let's Go to the mall
                //          Steve: Let's go to the Mall!

                var prev = lineStack.Peek();
                //if the lines are the same. we want to pop the last line
                //then check if the last line is greater


                Debug.Log($"Past Line - [{dl.lineIndex}]:{dl.indent}");
                Debug.Log($"Curr Line - [{dl.lineIndex}]:{dl.indent}");


                Debug.Log($"[{dl.indent}] <= {prev.indent} = {dl.indent <= prev.indent}");
                if (dl.indent <= prev.indent)
                {
                    lineStack.Pop();
                    continue;
                }
                //now check if we're a child by looking if we're
                //indented more than the last line
                Debug.Log($"[{dl.indent}] > {prev.indent} = {dl.indent <= prev.indent}");
                if (dl.indent > prev.indent)
                {
                    Debug.Log($"Adding Child");
                    //dl is a child of peek
                    prev.childLines.Add(dl);
                    lineStack.Push(dl);
                    break;
                }
            }
        }
    }
}


[System.Serializable]
public class DialogLine
{
    public DialogInstructionType type;
    public int indent;
    public int lineIndex;
    public string text;

    //child 
    public List<DialogLine> childLines;
}

public class RandomLine : DialogLine
{
    private TableList<DialogLine, float> randomLineTable;

    public DialogLine GetRandom()
    {
        if (randomLineTable.left.Count == 1)
        {
            //return the first early if there's only 1
            return randomLineTable.left[0];
        }

        float totalPercents = 0;
        foreach (var lootRoll in randomLineTable)
        {
            totalPercents += (float)lootRoll.Right;
        }
        //roll for each of the loot we want.
        float roll = UnityEngine.Random.Range(0, totalPercents);

        //current starts at 0 and adds the lootRoll value
        float currentRoll = 0.0f;
        foreach (var lootRoll in randomLineTable)
        {
            // 0 < roll < 1
            // 1 < roll < 2 etc.
            if (currentRoll < roll && roll <= (currentRoll + lootRoll.Right))
            {
                return lootRoll.Left;
            }
            currentRoll += lootRoll.Right;
        }
        //just default to the first one
        return randomLineTable.left[0];
    }
}

public enum DialogInstructionType
{
    None,
    Bookmark,           // #Label
    Metadata,           // *key = value
    Jump,               // => Maybe? To Similar to ->?
    Choice,             // ->

    Conditional,        // !if / !elseif / !else
    Command,            // @command
    Dialogue,           // Speaker: Text
    DialogueRandom,     // %N Speaker: Text

    //Depends on if we want to handle differently maybe?
    ConditionalElseIf,  // !elseif
    ConditionalElse,    // !else
}
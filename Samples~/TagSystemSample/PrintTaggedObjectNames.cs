using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintTaggedObjectNames : MonoBehaviour
{
    public string tagToPrint;
    public TMPro.TMP_Text text;
    // Start is called before the first frame update
    public void PrintTaggedObjects()
    {
        var taggedObjects =  TagObjects.GetObjectsByTag(tagToPrint);
        Debug.LogError(taggedObjects);
        if (taggedObjects != null)
        {
            string print = "";
            foreach (var item in taggedObjects)
            {
                print += string.Format("{0} \n", item.name);
            }
            text.text = print;
        }
        else
        {
            text.text = string.Format("No Objects Tagged with Tag {0}", tagToPrint);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagObjects
{
    public static Dictionary<string, List<GameObject>> taggedObjects;

    public static List<GameObject> GetObjectsByTag(string tag)
    {
        ConfirmInit();
        if (taggedObjects.ContainsKey(tag))
        {
            return taggedObjects[tag];
        }
        taggedObjects.Add(tag, new List<GameObject>());
        return taggedObjects[tag];
    }

    public static void RegisterTaggedObject(string tag, GameObject go)
    {
        ConfirmInit();
        if (!taggedObjects.ContainsKey(tag))
        {
            taggedObjects.Add(tag, new List<GameObject>());
        }
        if (taggedObjects.ContainsKey(tag))
        {
            if (!taggedObjects[tag].Contains(go))
            {
                taggedObjects[tag].Add(go);
            }
            else
            {
                Debug.LogErrorFormat("Object {0} is already tagged with {1}", go.name, tag);
            }
        }
    }

    public static void UnregisterTaggedObject(string tag, GameObject go)
    {
        ConfirmInit();
        if (taggedObjects.ContainsKey(tag))
        {
            if (taggedObjects[tag].Contains(go))
            {
                taggedObjects[tag].Remove(go);
            }
            else
            {
                Debug.LogErrorFormat("Object {0} isn't tagged with tag {1}", go.name, tag);
            }
        }
        else
        {
            Debug.LogErrorFormat("Tag {0} doesn't exist in the dictionary", tag);
        }
    }

    private static void ConfirmInit()
    {
        if(taggedObjects == null)
        {
            taggedObjects = new Dictionary<string, List<GameObject>>();
        }
    }
}

using UnityEngine;

public class TagObjectExample : MonoBehaviour
{
    public GameObject objectWithTagScript;
    public void Toggle()
    {
        objectWithTagScript.SetActive(!objectWithTagScript.activeSelf);
    }
}

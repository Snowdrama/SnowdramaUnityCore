using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class VersionText : MonoBehaviour
{
    private void Start()
    {
        this.GetComponent<TMP_Text>().text = $"{Application.version}";
    }
}

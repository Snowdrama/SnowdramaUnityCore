using TMPro;
using UnityEngine;

public class TestGameData_LoadFloatFromSave : MonoBehaviour
{
    [SerializeField] private string key;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        this.GetComponent<TMP_Text>().text = $"FloatData['{key}']: {GameData.GetFloat(key, 0.0f)}";
    }
}

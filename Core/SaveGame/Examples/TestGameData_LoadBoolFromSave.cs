using TMPro;
using UnityEngine;

public class TestGameData_LoadBoolFromSave : MonoBehaviour
{
    [SerializeField] private string key;
    private void Start()
    {
        this.GetComponent<TMP_Text>().text = $"FloatData['{key}']: {GameData.GetBool(key, false)}";
    }
}

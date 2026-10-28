using TMPro;
using UnityEngine;
using Snowdrama;
public class TestGameData_LoadBoolFromSave : MonoBehaviour
{
    [SerializeField] private string key;
    private void Start()
    {
        this.GetComponent<TMP_Text>().text = $"FloatData['{key}']: {GameDataManager.GetBool(key, false)}";
    }
}

using TMPro;
using UnityEngine;

public class TestGameData_LoadStringFromSave : MonoBehaviour
{
    [SerializeField] private string key;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        this.GetComponent<TMP_Text>().text = $"FloatData['{key}']: {GameDataManager.GetString(key, "N/A")}";
    }
}

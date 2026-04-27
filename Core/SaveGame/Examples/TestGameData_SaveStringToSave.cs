using TMPro;
using UnityEngine;

public class TestGameData_SaveStringToSave : MonoBehaviour
{
    private TMP_InputField inputField;
    private string state;
    [SerializeField] private string key;
    private void Start()
    {
        inputField = this.GetComponent<TMP_InputField>();
        var value = GameDataManager.GetString(key, "N/A");
        //Debug.Log($"Loaded {key} from GameDataManager, Value: {value}");
        inputField.SetTextWithoutNotify(value);
        inputField.onValueChanged.AddListener(this.OnChanged);
    }

    public void OnChanged(string newState)
    {
        GameDataManager.SetString(key, newState);
    }
}

using TMPro;
using UnityEngine;

public class TestGameData_SaveFloatToSave : MonoBehaviour
{
    private TMP_InputField inputField;
    private float state;
    [SerializeField] private string key;
    private void Start()
    {
        inputField = this.GetComponent<TMP_InputField>();
        var value = GameDataManager.GetFloat(key, 0.0f);
        //Debug.Log($"Loaded {key} from GameDataManager, Value: {value}");
        inputField.SetTextWithoutNotify($"{value}");
        inputField.onValueChanged.AddListener(this.OnChanged);
    }

    public void OnChanged(string newState)
    {
        if (float.TryParse(newState, out var value))
        {
            GameDataManager.SetFloat(key, value);
        }
    }
}

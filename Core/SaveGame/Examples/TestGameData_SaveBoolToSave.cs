using UnityEngine;
using UnityEngine.UI;

public class TestGameData_SaveBoolToSave : MonoBehaviour
{
    private Toggle toggle;
    private bool state;
    [SerializeField] private string key;
    private void Start()
    {
        toggle = GetComponent<Toggle>();
        var value = GameDataManager.GetBool(key, false);
        Debug.Log($"Loaded {key} from GameDataManager, Value: {value}");
        toggle.SetIsOnWithoutNotify(value);
        toggle.onValueChanged.AddListener(OnChanged);
    }

    public void OnChanged(bool newState)
    {
        GameDataManager.SetBool(key, newState);
    }
}

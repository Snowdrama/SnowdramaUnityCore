using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class SetOptionDropdown : MonoBehaviour
{
    public string dropdownName;
    public int defaultValue = 0;
    private TMP_Dropdown dropdown;
    // Start is called before the first frame update
    private void Start()
    {
        dropdown = this.GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(OnDropdownChanged);
        var value = Options.GetIntValue(dropdownName, defaultValue);
        Debug.Log($"Dropdown Value: {value}");
        dropdown.SetValueWithoutNotify(value);
    }
    private void OnDropdownChanged(int option)
    {
        Options.SetIntValue(dropdownName, option);
    }

}

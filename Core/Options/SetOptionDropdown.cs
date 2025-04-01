using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore;

[RequireComponent(typeof(TMP_Dropdown))]
public class SetOptionDropdown : MonoBehaviour
{
    public OptionsObject optionsObject;
    public string dropdownName;
    public int defaultValue = 0;
    TMP_Dropdown dropdown;
    // Start is called before the first frame update
    void Start()
    {
        dropdown = this.GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(OnDropdownChanged);
        var value = optionsObject.GetIntValue(dropdownName, defaultValue);
        Debug.Log($"Dropdown Value: {value}");
        dropdown.SetValueWithoutNotify(value);
    }
    void OnDropdownChanged(int option)
    {
        optionsObject.SetIntValue(dropdownName, option);
    }

}

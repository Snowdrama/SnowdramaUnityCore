using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetOptionToggle : MonoBehaviour
{
    public OptionsObject optionsObject;
    public string toggleName;
    public bool defaultToggle;
    Toggle toggle;
    void Start()
    {
        toggle = this.GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnValueChanged);
        toggle.SetIsOnWithoutNotify(optionsObject.GetBoolValue(toggleName, defaultToggle));
    }

    public void OnValueChanged(bool value)
    {
        optionsObject.SetBoolValue(toggleName, value);
    }
}

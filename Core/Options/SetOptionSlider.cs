using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SetOptionSlider : MonoBehaviour
{
    public OptionsObject optionsObject;
    public string sliderName;
    public float defaultValue;
    Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        slider = this.GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnValueChanged);
        slider.SetValueWithoutNotify(optionsObject.GetFloatValue(sliderName, defaultValue));
    }

    public void OnValueChanged(float value)
    {
        optionsObject.SetFloatValue(sliderName, slider.value);
    }
}

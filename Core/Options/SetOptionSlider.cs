using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SetOptionSlider : MonoBehaviour
{
    public string sliderName;
    public float defaultValue;
    private Slider slider;
    // Start is called before the first frame update
    private void Start()
    {
        slider = this.GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnValueChanged);
        slider.SetValueWithoutNotify(Options.GetFloatValue(sliderName, defaultValue));
    }

    public void OnValueChanged(float value)
    {
        Options.SetFloatValue(sliderName, slider.value);
    }
}

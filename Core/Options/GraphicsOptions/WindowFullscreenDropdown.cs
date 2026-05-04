using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WindowFullscreenDropdown : MonoBehaviour
{
    private TMP_Dropdown _dropdown;
    private readonly List<FullScreenMode> _modes = new()
    {
        FullScreenMode.FullScreenWindow,     // Borderless
        FullScreenMode.ExclusiveFullScreen,  // Exclusive
        FullScreenMode.Windowed              // Windowed
    };

    private void Awake()
    {
        _dropdown = this.GetComponent<TMP_Dropdown>();
    }

    private void OnEnable()
    {
        this.Populate();
        _dropdown.onValueChanged.AddListener(this.OnChanged);
    }

    private void OnDisable()
    {
        _dropdown.onValueChanged.RemoveListener(this.OnChanged);
    }

    private void Populate()
    {
        _dropdown.ClearOptions();

        var options = new List<TMP_Dropdown.OptionData>
        {
            new TMP_Dropdown.OptionData("Borderless"),
            new TMP_Dropdown.OptionData("Exclusive Fullscreen"),
            new TMP_Dropdown.OptionData("Windowed")
        };

        _dropdown.AddOptions(options);
        _dropdown.value = this.GetCurrentIndex();
        _dropdown.RefreshShownValue();
    }

    private int GetCurrentIndex()
    {
        var current = Screen.fullScreenMode;

        for (var i = 0; i < _modes.Count; i++)
        {
            if (_modes[i] == current)
                return i;
        }

        return 0;
    }

    private void OnChanged(int index)
    {
        var mode = _modes[index];
        WindowSettingsManager.SetFullscreenMode(mode);
    }
}
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowResolutionDropdown : MonoBehaviour
{
    private TMP_Dropdown _dropdown;

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

        var options = new List<TMP_Dropdown.OptionData>();

        if (WindowSettingsManager.FullScreenMode == FullScreenMode.ExclusiveFullScreen)
        {
            foreach (var res in WindowSettingsManager.UniqueResolutions)
            {
                options.Add(new TMP_Dropdown.OptionData($"{res.width}x{res.height} @ {res.refreshRate.numerator / res.refreshRate.denominator}Hz"));
            }
        }
        else
        {
            foreach (var res in WindowSettingsManager.UniqueResolutions)
            {
                options.Add(new TMP_Dropdown.OptionData($"{res.width}x{res.height} @ {res.refreshRate.numerator / res.refreshRate.denominator}Hz"));
            }
        }

        _dropdown.AddOptions(options);
        // Optional: set current value
        _dropdown.value = this.GetCurrentResolutionIndex();
        _dropdown.RefreshShownValue();
    }

    public void Refresh()
    {
        this.Populate();
    }

    private int GetCurrentResolutionIndex()
    {
        var current = new Vector2Int(Screen.width, Screen.height);
        var list = WindowSettingsManager.UniqueResolutions;

        for (var i = 0; i < list.Count; i++)
        {
            if (list[i].width == current.x && list[i].height == current.y)
            {
                return i;
            }
        }
        return 0;
    }

    private void OnChanged(int index)
    {
        var res = WindowSettingsManager.UniqueResolutions[index];

        // When resolution changes, pick best refresh automatically
        var options = WindowSettingsManager.GetOptionsForResolution(res);
        var bestIndex = options.Count - 1;

        var globalIndex = this.FindGlobalIndex(options[bestIndex]);

        WindowSettingsManager.SetResolution(globalIndex);

        this.Populate();
    }

    private int FindGlobalIndex(WindowSettingsManager.ResolutionOption option)
    {
        var list = WindowSettingsManager.Resolutions;

        for (var i = 0; i < list.Count; i++)
        {
            var r = list[i];
            if (r.width == option.width &&
                r.height == option.height &&
                r.refreshRate.numerator == option.refreshRate.numerator)
            {
                return i;
            }
        }

        return 0;
    }
}
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static WindowSettingsManager;

public class WindowFullscreenButtonSwitcher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text modeText;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button applyButton;

    [SerializeField, EditorReadOnly] private FullScreenMode[] modes;
    [SerializeField, EditorReadOnly] private int index = 0;
    [SerializeField, EditorReadOnly] private bool needsApplying;

    private void Start()
    {
        Debug.Log($"Binding to buttons: {this.name}");
        leftButton.onClick.AddListener(this.PreviousMode);
        rightButton.onClick.AddListener(this.NextMode);
        applyButton.onClick.AddListener(this.ApplyMode);

        // Define supported modes (you can customize order here)
        modes = new[]
        {
            FullScreenMode.ExclusiveFullScreen,
            FullScreenMode.FullScreenWindow,
            FullScreenMode.MaximizedWindow,
            FullScreenMode.Windowed
        };

        // Load saved mode
        var currentMode = WindowSettingsManager.CurrentFullScreenMode;
        index = Array.IndexOf(modes, currentMode);
        if (index < 0) index = 0;
        this.UpdateStuff();
    }
    private void OnEnable()
    {
        var currentMode = WindowSettingsManager.CurrentFullScreenMode;
        index = Array.IndexOf(modes, currentMode);
        if (index < 0) index = 0;
        this.UpdateStuff();
    }

    public void NextMode()
    {
        Debug.Log($"[{this.name}]Next Was Clicked!", this.gameObject);
        index++;
        index = index.WrapClamp(0, modes.Length);
        needsApplying = true;
        this.UpdateStuff();
    }

    public void PreviousMode()
    {
        Debug.Log($"[{this.name}]Previous Was Clicked!", this.gameObject);
        index--;
        index = index.WrapClamp(0, modes.Length);
        needsApplying = true;
        this.UpdateStuff();
    }

    private void UpdateStuff()
    {
        applyButton.interactable = needsApplying;

        if (index >= 0 && index < modes.Length)
        {
            modeText.text = this.FormatMode(modes[index]);
        }
    }

    public void ApplyMode()
    {
        Debug.Log($"[{this.name}]Apply Was Clicked!", this.gameObject);
        WindowSettingsManager.SetFullscreenMode(modes[index]);
        needsApplying = false;
        this.UpdateStuff();
    }

    private string FormatMode(FullScreenMode mode)
    {
        return mode switch
        {
            FullScreenMode.ExclusiveFullScreen => "Exclusive Fullscreen",
            FullScreenMode.FullScreenWindow => "Borderless Fullscreen",
            FullScreenMode.MaximizedWindow => "Maximized Window",
            FullScreenMode.Windowed => "Windowed",
            _ => mode.ToString()
        };
    }
}
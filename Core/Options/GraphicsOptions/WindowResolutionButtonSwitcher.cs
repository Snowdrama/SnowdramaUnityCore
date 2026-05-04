using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowResolutionButtonSwitcher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text resolutionText;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button applyButton;

    [SerializeField, EditorReadOnly] private List<WindowSettingsManager.ResolutionOption> resolutionOptions;
    [SerializeField, EditorReadOnly] private int index = 0;
    [SerializeField, EditorReadOnly] private bool needsApplying;
    private void Start()
    {
        Debug.Log($"Binding to buttons: {this.name}");
        leftButton.onClick.AddListener(this.PreviousResolution);
        rightButton.onClick.AddListener(this.NextResolution);
        applyButton.onClick.AddListener(this.ApplyResolution);
        resolutionOptions = WindowSettingsManager.UniqueResolutions.ToList();
        index = WindowSettingsManager.CurrentResolutionIndex;
        this.UpdateStuff();
    }
    private void OnEnable()
    {
        resolutionOptions = WindowSettingsManager.UniqueResolutions.ToList();
        index = WindowSettingsManager.CurrentResolutionIndex;
        this.UpdateStuff();
    }

    public void NextResolution()
    {
        Debug.Log($"[{this.name}]Next Was Clicked!", this.gameObject);
        index++;
        index = index.WrapClamp(0, resolutionOptions.Count);
        needsApplying = true;
        this.UpdateStuff();
    }

    public void PreviousResolution()
    {
        Debug.Log($"[{this.name}]Previous Was Clicked!", this.gameObject);
        index--;
        index = index.WrapClamp(0, resolutionOptions.Count);
        needsApplying = true;
        this.UpdateStuff();
    }

    public void UpdateStuff()
    {
        applyButton.interactable = needsApplying;

        if (index >= 0 && index < resolutionOptions.Count)
        {
            resolutionText.text = resolutionOptions[index].ToString();
        }
    }

    public void ApplyResolution()
    {
        Debug.Log($"[{this.name}]Apply Was Clicked!", this.gameObject);
        WindowSettingsManager.SetResolution(index);
        needsApplying = false;
        this.UpdateStuff();
    }
}

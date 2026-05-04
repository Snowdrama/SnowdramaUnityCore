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
        leftButton.onClick.AddListener(this.PreviousResolution);
        rightButton.onClick.AddListener(this.NextResolution);
        applyButton.onClick.AddListener(this.ApplyResolution);
        resolutionOptions = WindowSettingsManager.UniqueResolutions.ToList();
        index = WindowSettingsManager.CurrentResolutionIndex;
        this.UpdateStuff();
    }

    public void NextResolution()
    {
        index++;
        index = index.WrapClamp(0, resolutionOptions.Count);
        needsApplying = true;
        this.UpdateStuff();
    }

    public void PreviousResolution()
    {
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
        WindowSettingsManager.SetResolution(index);
        needsApplying = false;
        this.UpdateStuff();
    }
}

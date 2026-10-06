using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Snowdrama
{
    public class WindowResolutionButtonSwitcher : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text resolutionText;
        [SerializeField] private Button prevButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button applyButton;
        private SelectableInt resolutionSelection;
        [SerializeField, EditorReadOnly] private List<WindowSettingsManager.ResolutionOption> resolutionOptions;
        private void OnEnable()
        {
            resolutionOptions = WindowSettingsManager.Resolutions.ToList();
            resolutionSelection = new SelectableInt(WindowSettingsManager.ResolutionIndex, resolutionOptions.Count);

            prevButton.onClick.AddListener(this.Prev);
            nextButton.onClick.AddListener(this.Next);
            applyButton.onClick.AddListener(this.Apply);
            resolutionSelection.OnChanged += this.OnChange;
            resolutionSelection.OnApplied += this.OnApplied;

            resolutionSelection.SetValueForceEvents(WindowSettingsManager.ResolutionIndex);
        }

        private void OnDisable()
        {
            prevButton.onClick.RemoveListener(this.Prev);
            nextButton.onClick.RemoveListener(this.Next);
            applyButton.onClick.RemoveListener(this.Apply);
            resolutionSelection.OnChanged -= this.OnChange;
            resolutionSelection.OnApplied -= this.OnApplied;
        }

        public void Prev()
        {
            resolutionSelection.Previous();
        }
        public void Next()
        {
            resolutionSelection.Next();
        }
        public void Apply()
        {
            resolutionSelection.Apply();
        }

        public void OnChange(int newIndex, bool needsApplying)
        {
            Debug.Log("Resolution Changed!");
            resolutionText.text = resolutionOptions[newIndex].ToString();
            applyButton.interactable = needsApplying;
        }
        public void OnApplied(int finalIndex)
        {
            WindowSettingsManager.SetResolution(finalIndex);
        }
    }
}

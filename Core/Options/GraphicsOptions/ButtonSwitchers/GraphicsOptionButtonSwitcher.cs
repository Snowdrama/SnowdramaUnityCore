using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsOptionButtonSwitcher : MonoBehaviour
{
    [SerializeField] private IntOption IntValue;
    [Header("References")]
    [SerializeField] private TMP_Text _modeText;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _previousButton;
    [SerializeField] private Button _applyButton;
    [Header("Quality List")]
    [Header("BE SURE this is in the same order as 'Edit -> Player Settings -> Quality'")]
    [SerializeField]
    private List<string> _qualityNames = new List<string>()
    {
        "Very Low",
        "Low",
        "Medium",
        "High",
        "Very High",
        "Ultra",
    };
    private void OnEnable()
    {
        //bind our button callbacks
        _nextButton.onClick.AddListener(this.Next);
        _previousButton.onClick.AddListener(this.Prev);
        _applyButton.onClick.AddListener(this.Apply);

        //always make a new one
        IntValue = new IntOption(QualitySettings.GetQualityLevel(), _qualityNames.Count);
        //bind our callbacks for the option changing
        IntValue.OnChanged += this.OnChanged;
        IntValue.OnApplied += this.OnApplied;
        IntValue.OnNeedsApplying += this.OnNeedsApplying;

        //if we're enablign this set the value and force the events to update
        //the buttons and text fields
        IntValue.SetValueForceEvents(QualitySettings.GetQualityLevel());
    }
    private void OnDisable()
    {
        _nextButton.onClick.RemoveListener(this.Next);
        _previousButton.onClick.RemoveListener(this.Prev);
        _applyButton.onClick.RemoveListener(this.Apply);

        //on disable just free the memory
        //we make a new one OnEnable
        IntValue = null;
    }
    private void Next()
    {
        IntValue.Next();
    }
    private void Prev()
    {
        IntValue.Previous();
    }
    private void Apply()
    {
        IntValue.Apply();
    }
    private void OnChanged(int newValue)
    {
        _modeText.text = _qualityNames[newValue];
    }
    private void OnApplied(int newValue)
    {
        _modeText.text = _qualityNames[newValue];
    }
    private void OnNeedsApplying(bool needsApplying)
    {
        _applyButton.interactable = needsApplying;
    }
}

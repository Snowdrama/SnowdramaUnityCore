using System;
using UnityEngine;
/// <summary>
/// A wrapper class for option toggles that are float
/// 
/// See things like "GraphicsOptionButtonSwitcher" for an example of use
/// 
/// The goal is to make a reusable object that has some range of options from 0 to N 
/// 
/// You can press prev/next to increment or decrement the option
/// 
/// You can set the option to something in range
/// 
/// Doing so provides an action callback for:
/// 
/// When the value is changed
/// When the value is applied
/// When the value needs applying
/// 
/// </summary>
[System.Serializable]
public class FloatOption
{
    public Action<float> OnChanged;
    public Action<float> OnApplied;
    public Action<bool> OnNeedsApplying;
    [SerializeField, EditorReadOnly] private float _currentValue = 0;
    public float CurrentValue
    {
        get { return _currentValue; }
        set
        {
            //only modify on change
            if (_currentValue != value)
            {
                _currentValue = value;
                OnApplied?.Invoke(_currentValue);
                //since we applied it, applying is no longer needed
                this.NeedsApplying = false;
            }
        }
    }

    /// <summary>
    /// The temp value is 
    /// </summary>
    [SerializeField, EditorReadOnly] private float _tempValue = 0;
    public float TempValue
    {
        get { return _tempValue; }
        set
        {
            //wrap the value
            value = value.WrapClamp(_minValue, _maxValue);

            //only modify on change
            if (_tempValue != value)
            {
                _tempValue = value;
                OnChanged?.Invoke(_tempValue);
                if (_tempValue != _currentValue)
                {
                    this.NeedsApplying = true;
                }
                else
                {
                    this.NeedsApplying = false;
                }
            }
        }
    }
    [SerializeField, EditorReadOnly] private bool _needsApplying;
    public bool NeedsApplying
    {
        get { return _needsApplying; }
        set
        {
            //only modify on change
            if (_needsApplying != value)
            {
                _needsApplying = value;
                OnNeedsApplying?.Invoke(_needsApplying);
            }
        }
    }

    private float _minValue = 0;
    private float _maxValue = 0;

    public FloatOption(float defaultValue, float maxValue, float minValue = 0)
    {
        _minValue = minValue;
        _maxValue = maxValue;
        _currentValue = defaultValue;
        _tempValue = defaultValue;
    }

    public void Next()
    {
        this.TempValue++;
    }

    public void Previous()
    {
        this.TempValue--;
    }

    public void Apply()
    {
        this.CurrentValue = this.TempValue;
    }

    /// <summary>
    /// Sets the value without triggering the change/apply actions
    /// </summary>
    /// <param name="newValue"></param>
    public void SetValueNoAction(float newValue)
    {
        _currentValue = _tempValue = newValue;
        _needsApplying = false;
    }

    /// <summary>
    /// This sets the value then triggers an apply
    /// 
    /// The events will be triggered as normal if the value has changed
    /// </summary>
    /// <param name="newValue"></param>
    public void SetValue(float newValue)
    {
        this.TempValue = newValue;
        this.Apply();
    }

    /// <summary>
    /// This sets the values, and then forces all events to trigger
    /// 
    /// This is useful during initialization so callbacks get fired
    /// when the default value is set
    /// </summary>
    /// <param name="newValue"></param>
    public void SetValueForceEvents(float newValue)
    {
        _currentValue = _tempValue = newValue;
        _needsApplying = false;
        //the value may not trigger actions
        //due to already being 0 or -1 or something
        //force trigger all events on SetValue
        OnChanged?.Invoke(_tempValue);
        OnApplied?.Invoke(_tempValue);
        OnNeedsApplying?.Invoke(false);
    }

    public void SetMinValue(float newMinValue)
    {
        _minValue = newMinValue;
    }

    public void SetMaxValue(float newMaxValue)
    {
        _maxValue = newMaxValue;
    }
}

using System;
using UnityEngine;
/// <summary>
/// A wrapper class for option toggles that are double
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
public class SelectableDouble
{
    public Action<double, bool> OnChanged;
    public Action<double> OnApplied;
    [SerializeField, EditorReadOnly] private double _currentValue = 0;
    public double CurrentValue
    {
        get { return _currentValue; }
        set
        {
            //only modify on change
            if (_currentValue != value)
            {
                _currentValue = value;
                OnApplied?.Invoke(_currentValue);
            }
        }
    }

    /// <summary>
    /// The temp value is 
    /// </summary>
    [SerializeField, EditorReadOnly] private double _tempValue = 0;
    public double TempValue
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
                OnChanged?.Invoke(_tempValue, _tempValue != _currentValue);
            }
        }
    }

    private double _minValue = 0;
    private double _maxValue = 0;
    private double _changeValue = 0;
    public SelectableDouble(double defaultValue, double maxValue, double minValue = 0, double changeValue = 1)
    {
        _changeValue = changeValue;
        _minValue = minValue;
        _maxValue = maxValue;
        _currentValue = defaultValue;
        _tempValue = defaultValue;
    }

    public void Next()
    {
        this.TempValue += _changeValue;
    }

    public void Previous()
    {
        this.TempValue -= _changeValue;
    }

    public void Apply()
    {
        this.CurrentValue = this.TempValue;
    }

    /// <summary>
    /// Sets the value without triggering the change/apply actions
    /// </summary>
    /// <param name="newValue"></param>
    public void SetValueNoAction(double newValue)
    {
        _currentValue = _tempValue = newValue;
    }

    /// <summary>
    /// This sets the value then triggers an apply
    /// 
    /// The events will be triggered as normal if the value has changed
    /// 
    /// if the value is not different events will not trigger
    /// </summary>
    /// <param name="newValue"></param>
    public void SetValue(double newValue)
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
    public void SetValueForceEvents(double newValue)
    {
        _currentValue = _tempValue = newValue;
        //the value may not trigger actions
        //when using SetValue if value is
        //the same as the current value
        //instead we force events here to ensure that it
        //both applies as current AND
        //triggers the event callbacks
        OnChanged?.Invoke(_tempValue, _tempValue != _currentValue);
        OnApplied?.Invoke(_tempValue);
    }

    public void SetMinValue(double newMinValue)
    {
        _minValue = newMinValue;
    }

    public void SetMaxValue(double newMaxValue)
    {
        _maxValue = newMaxValue;
    }
}

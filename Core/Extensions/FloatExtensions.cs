using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatExtensions
{

    public static float Clamp(this float f, float min, float max)
    {
        return Mathf.Clamp(f, min, max);
    }

    /// <summary>
    /// Takes a Value from one range and remaps it relative to a different range.
    /// For example 0.5 in a 0 to 1 range, would map to 5 in a 0 to 10 range.
    /// </summary>
    /// <param name="value">The Value to remap</param>
    /// <param name="fromMin">Original Min</param>
    /// <param name="fromMax">Original Max</param>
    /// <param name="toMin">New Min</param>
    /// <param name="toMax">New Max</param>
    /// <returns></returns>
    public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }

    /// <summary>
    /// Clamp a Value and wrap around to based on the difference
    ///
    /// the wrap clamp maxValue is EXCLUSIVE so 
    /// WrapClamp(0, 5, 4.99f) = 4.99f
    /// WrapClamp(0, 5, 5.00f) = 0.0f
    /// </summary>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float WrapClamp(this float x, float min, float max)
    {
        return (((x - min) % (max - min)) + (max - min)) % (max - min) + min;
    }

    public static int FloorToInt(this float val)
    {
        return Mathf.FloorToInt(val);
    }

    public static int CeilToInt(this float val)
    {
        return Mathf.CeilToInt(val);
    }

    public static int RoundToInt(this float val)
    {
        return Mathf.RoundToInt(val);
    }

    public static float Floor(this float val)
    {
        return Mathf.Floor(val);
    }

    public static float Ceil(this float val)
    {
        return Mathf.Ceil(val);
    }

    public static float Round(this float val)
    {
        return Mathf.Round(val);
    }

    /// <summary>
    /// Returns the normalized Value of the Value between min and max;
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float InverseLerp(this float value, float min, float max)
    {
        return Mathf.Clamp01(value - min) / (max - min);

    }
    /// <summary>
    /// Returns the normalized Value of the Value between min and max unclamped
    /// For example InverseLerpUnclamped(Value = 20, min = 0, max = 10); will 
    /// return 2 since 20 is double the size of the range
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float InverseLerpUnclamped(this float value, float min, float max)
    {
        return (value - min) / (max - min);

    }
    public static bool InRange(this int value, int min, int max) 
    {
        return value >= min && value <= max;
    } 
}
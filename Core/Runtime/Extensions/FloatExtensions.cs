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

    public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }

    /// <summary>
    /// Clamp a value and wrap around to based on the difference
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
}
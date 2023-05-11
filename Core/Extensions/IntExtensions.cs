using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IntExtensions
{
    /// <summary>
    /// Clamp a value and wrap around to based on the difference
    /// </summary>
    /// <param name="value"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public static int WrapClamp(this int x, int min, int max)
    {
        return (((x - min) % (max - min)) + (max - min)) % (max - min) + min;
    }

    public static int Clamp(this int f, int min, int max)
    {
        return Mathf.Clamp(f, min, max);
    }
}
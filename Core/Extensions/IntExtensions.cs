using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IntExtensions
{
    /// <summary>
    /// Clamp a Value and wrap around to based on the difference
    /// </summary>
    /// <param name="value"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public static int WrapClamp(this int x, int min, int max)
    {
        return (((x - min) % (max - min)) + (max - min)) % (max - min) + min;
    }

    /// <summary>
    /// A slightly more complex modulo function that deals well with negative values and wraps correctly 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="m"></param>
    /// <returns></returns>
    public static int BetterMod(this int x, int m)
    {
        if (m == 0)
        {
            //should this be an error?
            return x;
        }

        return (x % m + m) % m;
    }

    public static int Clamp(this int f, int min, int max)
    {
        return Mathf.Clamp(f, min, max);
    }
}
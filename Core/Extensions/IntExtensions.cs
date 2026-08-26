using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        return ((((x - min) % (max - min)) + (max - min)) % (max - min)) + min;
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

        return ((x % m) + m) % m;
    }

    public static int Clamp(this int f, int min, int max)
    {
        return Mathf.Clamp(f, min, max);
    }

    /// <summary>
    /// Gets the value of the position of the digit
    /// 
    /// For example:
    /// 
    /// 29345.InPlace(0) == 5
    /// 29345.InPlace(1) == 4
    /// 29345.InPlace(2) == 3
    /// 29345.InPlace(3) == 9
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static int InPlace(this int source, int index)
    {
        //int    5 = (53 / (int)Math.Pow(10, 2 - 1)) % 10
        //int    5 = (53 / 10) % 10
        //int    5 = 5.3 & 10
        //note the Abs is needed so that negative numbers work correctly and are always positive
        var result = Mathf.Abs(source) / (int)Math.Pow(10, index - 1) % 10;
        return result;
    }
}
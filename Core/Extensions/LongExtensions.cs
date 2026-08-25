using System;
using UnityEngine;

public static class LongExtensions
{
    /// <summary>
    /// Clamp a Value and wrap around to based on the difference
    /// </summary>
    /// <param name="value"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public static long WrapClamp(this long x, long min, long max)
    {
        return ((((x - min) % (max - min)) + (max - min)) % (max - min)) + min;
    }

    /// <summary>
    /// A slightly more complex modulo function that deals well with negative values and wraps correctly 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="m"></param>
    /// <returns></returns>
    public static long BetterMod(this long x, long m)
    {
        if (m == 0)
        {
            //should this be an error?
            return x;
        }

        return ((x % m) + m) % m;
    }

    public static long Clamp(this long f, long min, long max)
    {
        return Math.Clamp(f, min, max);
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
    public static long InPlace(this long source, long index)
    {
        //long    5 = (53 / (long)Math.Pow(10, 2 - 1)) % 10
        //long    5 = (53 / 10) % 10
        //long    5 = 5.3 & 10
        //note the Abs is needed so that negative numbers work correctly and are always positive
        var result = Mathf.Abs(source) / (long)Math.Pow(10, index - 1) % 10;
        return (long)Math.Floor(result);
    }
}

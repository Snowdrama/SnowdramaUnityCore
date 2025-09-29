using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DoubleExtensions
{

    public static double Clamp(this double f, double min, double max)
    {
        return Math.Clamp(f, min, max);
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
    public static double Remap(this double value, double fromMin, double fromMax, double toMin, double toMax)
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
    public static double WrapClamp(this double x, double min, double max)
    {
        return (((x - min) % (max - min)) + (max - min)) % (max - min) + min;
    }

    /// <summary>
    /// A slightly more complex modulo function that deals well with negative values and wraps correctly 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="m"></param>
    /// <returns></returns>
    public static double BetterMod(this double x, double m)
    {
        if (m == 0)
        {
            //should this be an error?
            return x;
        }

        return (x % m + m) % m;
    }

    public static int FloorToInt(this double val)
    {
        return (int)Math.Floor(val);
    }

    public static int CeilToInt(this double val)
    {
        return (int)Math.Ceiling(val);
    }

    public static int RoundToInt(this double val)
    {
        return (int)Math.Round(val);
    }

    public static double Floor(this double val)
    {
        return Math.Floor(val);
    }

    public static double Ceil(this double val)
    {
        return Math.Ceiling(val);
    }

    public static double Round(this double val)
    {
        return Math.Round(val);
    }

    /// <summary>
    /// Returns the normalized Value of the Value between min and max;
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static double InverseLerp(this double value, double min, double max)
    {
        return Math.Clamp((value - min) / (max - min), 0, 1);

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
    public static double InverseLerpUnclamped(this double value, double min, double max)
    {
        return (value - min) / (max - min);

    }
    public static bool InRange(this int value, int min, int max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Takes a value from 0 to 1.0 and converts it to decibels.
    /// </summary>
    /// <param name="linear">A value from 0.0 to 1.0</param>
    /// <returns>A value in Decibels from -144.0 to 0.0</returns>
    public static double LinearToDecibel(this double linear)
    {
        double dB;
        if (linear != 0)
            dB = 20.0f * Math.Log10(linear);
        else
            dB = -144.0f;
        return dB;
    }

    /// <summary>
    /// Takes a value in decibels and converts it to linear.
    /// </summary>
    /// <param name="linear">A value in dB</param>
    /// <returns>A value from 0-1 linearly</returns>
    public static double DecibelToLinear(double dB)
    {
        double linear = Math.Pow(10.0f, dB / 20.0f);
        return linear;
    }


    /// <summary>
    /// Rounds a number to the nearest value of that number. 
    /// 
    /// Example:
    /// var value = 1.39f;
    /// var rounded = value.RoundTo(0.1f); // 1.4f
    /// </summary>
    /// <param name="value">The value to round</param>
    /// <param name="snapTarget">A target to snap to for example 0.25f</param>
    /// <returns>The value rounded to the nearest snap target</returns>
    public static double RoundTo(this double value, double snapTarget)
    {
        return Math.Round(value / snapTarget) * snapTarget;
    }
}

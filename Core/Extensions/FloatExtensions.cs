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

    /// <summary>
    /// A slightly more complex modulo function that deals well with negative values and wraps correctly 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="m"></param>
    /// <returns></returns>
    public static float BetterMod(this float x, float m)
    {
        if (m == 0)
        {
            //should this be an error?
            return x;
        }

        return (x % m + m) % m;
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
        return Mathf.Clamp01((value - min) / (max - min));

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

    /// <summary>
    /// Takes a value from 0 to 1.0 and converts it to decibels.
    /// </summary>
    /// <param name="linear">A value from 0.0 to 1.0</param>
    /// <returns>A value in Decibels from -144.0 to 0.0</returns>
    public static float LinearToDecibel(this float linear)
    {
        float dB;
        if (linear != 0)
            dB = 20.0f * Mathf.Log10(linear);
        else
            dB = -144.0f;
        return dB;
    }

    /// <summary>
    /// Takes a value in decibels and converts it to linear.
    /// </summary>
    /// <param name="linear">A value in dB</param>
    /// <returns>A value from 0-1 linearly</returns>
    public static float DecibelToLinear(float dB)
    {
        float linear = Mathf.Pow(10.0f, dB / 20.0f);
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
    public static float RoundTo(this float value, float snapTarget)
    {
        return Mathf.Round(value / snapTarget) * snapTarget;
    }


    /// <summary>
    /// Converts a time into a speed to multiply delta time by
    /// 
    /// Since delta time is "per second" the time is the inverse of the speed
    /// 
    /// for example 
    /// If something should take 1.0 second then the speed is 1.0
    /// If something shoudl take 2.0 seconds then the speed is 0.5
    /// if something should take 0.5 seconds then the speed is 2.0
    /// 
    /// This is the opposite of CreateTimeFromSpeed
    /// 
    /// This is because I'm dumb and always forget the math is just 1.0/time
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static float CreateSpeedFromTime(this float time)
    {
        if (time == 0)
        {
            //it will take 0 time then the speed is infiite
            return float.PositiveInfinity;
        }

        /// If something should take 1.0 second then the speed is 1.0
        /// 1.0 / 1.0 = 1.0
        /// If something shoudl take 2.0 seconds then the speed is 0.5
        /// 1.0 / 2.0 = 0.5
        /// if something should take 0.5 seconds then the speed is 2.0
        /// 1.0 / 0.5 = 2.0
        return 1.0f / time;
    }

    /// <summary>
    /// 
    /// Converts a speed into a duration per second
    /// 
    /// Since speed is units "per second" the speed is the inverse of the time
    /// 
    /// for example
    /// if somethign moves at 1.0 speed, it would travel 1 unit in 1.0 seconds
    /// if something moves at 2.0 speed, it would travel 1 unit in 0.5 seconds
    /// if something moves at 0.5 speed, it would travel 1 unit in 2.0 seconds
    /// 
    /// This is the opposite of CreateSpeedFromTime
    /// 
    /// This is because I'm dumb and always forget the math is just 1.0/speed
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    public static float CreateTimeFromSpeed(this float speed)
    {
        if (speed == 0)
        {
            //it will take infinite time if the speed is 0
            return float.PositiveInfinity;
        }

        // 1.0 / 2.0 = 0.5
        /// if somethign moves at 1.0 speed, it would travel 1 unit in 1.0 seconds
        /// 1.0 / 1.0 = 1.0
        /// if something moves at 2.0 speed, it would travel 1 unit in 0.5 seconds
        /// 1.0 / 2.0 = 0.5
        /// if something moves at 0.5 speed, it would travel 1 unit in 2.0 seconds
        /// 1.0 / 0.5 = 2.0

        return 1.0f / speed;
    }
}
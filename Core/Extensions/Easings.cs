
using System;
using UnityEngine;

public static class Easings
{
    public static float EaseIn(float x)
    {
        return 1.0f - Mathf.Cos(x.Clamp(0.0f, 1.0f) * Mathf.PI * 0.5f);
    }
    public static float EaseOut(float x)
    {
        return 1.0f - Mathf.Sin(x.Clamp(0.0f, 1.0f) * Mathf.PI * 0.5f);
    }
    public static float EaseInOut(float x)
    {
        return -(Mathf.Cos(x.Clamp(0.0f, 1.0f) * Mathf.PI) - 1) * 0.5f;
    }
    public static float EaseInOutBack(float x)
    {
        var c1 = 1.70158f;
        var c2 = c1 * 1.525f;

        return x < 0.5f
          ? Mathf.Pow(2.0f * x, 2.0f) * (((c2 + 1.0f) * 2.0f * x) - c2) / 2.0f
          : ((Mathf.Pow((2.0f * x) - 2.0f, 2.0f) * (((c2 + 1.0f) * ((x * 2.0f) - 2.0f)) + c2)) + 2.0f) / 2.0f;
    }
}

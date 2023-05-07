using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtension
{
    public static Color ParseStringAsColor(this string hexString, Color fallback = default)
    {
        Color color = fallback;
        ColorUtility.TryParseHtmlString(hexString, out color);
        return color;
    }

    public static Color GetColorFromRainbow(float t, float of = 1, Gradient gradient = null)
    {
        Gradient g;
        if (gradient == null)
        {
            g = gradient;
        }
        else
        {
            g = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[8];
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0].alpha = 1;
            alphaKeys[1].alpha = 1;
            alphaKeys[0].time = 0;
            alphaKeys[0].time = 1;

            colorKeys[0].color = Color.red;
            colorKeys[1].color = Color.yellow;
            colorKeys[2].color = Color.green;
            colorKeys[3].color = Color.cyan;
            colorKeys[4].color = Color.blue;
            colorKeys[5].color = Color.magenta;
            colorKeys[6].color = Color.white;
            colorKeys[7].color = Color.black;

            colorKeys[0].time = 0.0f;
            colorKeys[1].time = 0.14f * 1;
            colorKeys[2].time = 0.14f * 2;
            colorKeys[3].time = 0.14f * 3;
            colorKeys[4].time = 0.14f * 4;
            colorKeys[5].time = 0.14f * 5;
            colorKeys[6].time = 0.14f * 6;
            colorKeys[7].time = 1.0f;
            g.SetKeys(colorKeys, alphaKeys);
        }
        t = Mathf.Clamp01(t/ (of - 1));
        return g.Evaluate(t);
    }
}
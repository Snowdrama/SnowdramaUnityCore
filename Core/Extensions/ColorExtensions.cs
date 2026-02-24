using UnityEngine;

public static class ColorExtensions
{

    /// <summary>
    /// Converts a hex string like "#FF00FF" into a color.Takes a fallback in case the parse fails.
    /// </summary>
    /// <param name="hexString"></param>
    /// <param name="fallback"></param>
    /// <returns></returns>
    public static Color ParseStringAsColor(this string hexString, Color fallback = default)
    {
        Color color = fallback;
        ColorUtility.TryParseHtmlString(hexString, out color);
        return color;
    }

    /// <summary>
    /// A quick tool for getting a random color from a rainbow gradient of colors.
    /// 
    /// Passing it a value from 0-1 returns a color
    /// 
    /// The color order is:
    /// 0.00f -> Color.red
    /// 0.14f -> Color.yellow
    /// 0.28f -> Color.green
    /// 0.42f -> Color.cyan
    /// 0.56f -> Color.blue
    /// 0.70f -> Color.magenta
    /// 0.84f -> Color.white
    /// 1.00f -> Color.black
    /// 
    /// </summary>
    /// <param name="t">a value 0-1 that determines the color</param>
    /// <param name="gradient"></param>
    /// <returns></returns>
    public static Color GetColorFromRainbow(float t, Gradient gradient = null)
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

            colorKeys[0].time = 0.0f;      //0.00f
            colorKeys[1].time = 0.14f * 1; //0.14f
            colorKeys[2].time = 0.14f * 2; //0.28f
            colorKeys[3].time = 0.14f * 3; //0.42f
            colorKeys[4].time = 0.14f * 4; //0.56f
            colorKeys[5].time = 0.14f * 5; //0.70f
            colorKeys[6].time = 0.14f * 6; //0.84f
            colorKeys[7].time = 1.0f;      //1.00f
            g.SetKeys(colorKeys, alphaKeys);
        }

        t = Mathf.Clamp01(t);
        return g.Evaluate(t);
    }
}
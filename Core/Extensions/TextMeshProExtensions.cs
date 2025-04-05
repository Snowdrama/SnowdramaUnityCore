using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextMeshProExtensions
{

    /// <summary>
    /// Bolds the text
    /// </summary>
    public static string Bold(this string input)
    { 
        return $"<b>{input}</b>"; 
    }

    /// <summary>
    /// Sets the text to italics
    /// </summary>
    public static string Italics(this string input)
    { 
        return $"<i>{input}</i>";
    }

    /// <summary>
    /// Sets the size of the text
    /// </summary>
    public static string Size(this string input, int size)
    {
        return $"<size={size}>{input}</size>";
    }

    /// <summary>
    /// Sets the color of the text according to the parameter Value.
    /// </summary>
    public static string Color(this string input, Color color)
    { 
        return $"<color={color.ColorToHex()}>{input}</color>";
    }

    /// <summary>
    /// Sets the color of the text according to the parameter Value in hex
    /// </summary>
    public static string Color(this string input, string hexColor)
    {
        return $"<color={hexColor.HexToColor()}> {input}</color>";
    }
}

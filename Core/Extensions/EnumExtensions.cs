using System;

public static class EnumExtensions
{
    /// <summary>
    /// Shorthand for turning an enum into a string
    /// 
    /// Same as 'nameof(Enum.Value)' but formated for parity with 'ToEnumType()'
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToEnumName(this Enum value)
    {
        return nameof(value);
    }

    /// <summary>
    /// Converts the string to an enum type. If it can't parse the name use the default value
    /// 
    /// Used with 'ToEnumName()' or 'nameof(Enum.Value)' Converts the name back into an enum
    /// for serialization 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="defaultType"></param>
    /// <returns></returns>
    public static T ToEnumType<T>(this string value, T defaultType) where T : Enum
    {
        if (Enum.TryParse(typeof(T), value, out var result))
        {
            return (T)result;
        }
        return defaultType;
    }
}
﻿using System;
using System.Linq;
using System.Reflection;

public static class EnumAttributeExtension
{
    /// <summary>
    ///     A generic extension method that aids in reflecting 
    ///     and retrieving any attribute that is applied to an `Enum`.
    /// </summary>
    public static TAttribute GetAttribute<TAttribute>(this Enum value)
        where TAttribute : Attribute
    {

        Type enumType = value.GetType();
        FieldInfo[] fields = enumType.GetFields();
        FieldInfo fi = fields.Where(tField =>
            tField.IsLiteral &&
            tField.GetValue(null).Equals(value)
            ).First();

        // If we didn't get, return null
        if (fi == null) return null;

        // We found the element (which we always should in an enum)
        // return the attribute if it exists.
        return (TAttribute)(fi.GetCustomAttribute(typeof(TAttribute)));
    }
}
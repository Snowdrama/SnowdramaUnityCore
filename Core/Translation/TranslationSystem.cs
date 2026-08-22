using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NOT FINISHED!
/// NOT FINISHED!
/// NOT FINISHED!
/// NOT FINISHED!
/// NOT FINISHED!
/// THIS IS UNFINISHED BUT PUSHED DUE TO NEEDING TO SHIP A BUGFIX
/// NOT FINISHED!
/// NOT FINISHED! 
/// NOT FINISHED!
/// NOT FINISHED!
/// NOT FINISHED!
/// </summary>
public class TranslationSystem : MonoBehaviour
{
    private static Dictionary<string, string> translationByKey = new Dictionary<string, string>();
    //private static Dictionary<string, string> translationByValue = new Dictionary<string, string>();

    private static TranslationLanguageType _currentLanguage = TranslationLanguageType.English;
    public static TranslationLanguageType CurrentLanguage
    {
        get { return _currentLanguage; }
        set
        {
            //only modify on change
            if (_currentLanguage != value)
            {
                _currentLanguage = value;
                LoadTranslation();
            }
        }
    }

    /// <summary>
    /// Bootstrap the translation tool before scene load
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        //use the current lanuage if the Option doesn't exist for Language which should be english
        //
        CurrentLanguage = Options.GetStringValue(
                "Language", //get the string in the 'Language' key in the options
                nameof(CurrentLanguage) //This will be "English" by default
                                        //Shortcut to convert a string to an enum
            ).ToEnumType<TranslationLanguageType>(TranslationLanguageType.English);
    }

    private static void LoadTranslation()
    {
        //Get the CSV for the current language
        //If it doesn't exist load english
        //parse the CSV
        //Load the translation data in to the dictionary by key/value
    }

    public static string TR(string key)
    {
        if (translationByKey.ContainsKey(key))
        {
            return translationByKey[key];
        }

        //return the key if we don't have a translation
        return key;
    }
}

public enum TranslationLanguageType
{
    English,
    Spanish,
}
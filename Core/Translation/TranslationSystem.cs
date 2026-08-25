using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama
{
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

        private static SupportedLanguages SupportedLanguages;
        private static string _currentLanguage = "en-us";
        public static string CurrentLanguage
        {
            get { return _currentLanguage; }
            private set
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
            //load a list of all supported languages from the resources
            var supportedLanguageJSON = Resources.Load<TextAsset>("Translation/SupportedLanguages");
            if (supportedLanguageJSON == null)
            {
                Debug.LogError("Can't find Translation/SupportedLanguages.jsonc " +
                    "Please use the menu item: Snowdrama -> Transitions -> Create SupportedLanguages.jsonc " +
                    "to create one in the Resources folder");
                return;
            }
            SupportedLanguages = JsonConvert.DeserializeObject<SupportedLanguages>(supportedLanguageJSON.text);

            //use the current lanuage if the Option doesn't exist for Language which should be english
            _currentLanguage = Options.GetStringValue("Language", "en");

            //check if the thing loaded from the options is supported
            if (SupportedLanguages.LanguageCodeSupported(_currentLanguage))
            {
                _currentLanguage = "en";
            }
            //note '_currentLanguage' is used above because we manually call LoadTranslation
            //Everywhere else 'CurrentLanguage' should be used
            LoadTranslation();
        }

        private static void LoadTranslation()
        {
            //Get the CSV for the current language
            //If it doesn't exist load english
            //parse the CSV
            //Load the translation data in to the dictionary by key/value

            // NOT FINISHED!
            // NOT FINISHED!
            // NOT FINISHED!
            // NOT FINISHED!
            // NOT FINISHED!
            // THIS IS UNFINISHED BUT PUSHED DUE TO NEEDING TO SHIP A BUGFIX
            // NOT FINISHED!
            // NOT FINISHED! 
            // NOT FINISHED!
            // NOT FINISHED!
            // NOT FINISHED!

            //first check if it's in the application persistent data folder

            // NOT FINISHED!
            // NOT FINISHED!
            // NOT FINISHED!
            // NOT FINISHED!
            // NOT FINISHED!
            // THIS IS UNFINISHED BUT PUSHED DUE TO NEEDING TO SHIP A BUGFIX
            // NOT FINISHED!
            // NOT FINISHED! 
            // NOT FINISHED!
            // NOT FINISHED!
            // NOT FINISHED!


            //then check if we have an offocial translation:
            //TODO: Move out of resources?
            var loadedTranslation = Resources.Load<TextAsset>($"Translation/lang/{CurrentLanguage}");
            if (loadedTranslation == null)
            {
                //we didn't load anything so somehow a "supported translation" was not valid
            }
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

    public class LanguageCodeAttribute : Attribute
    {
        private string code;
        public LanguageCodeAttribute(string code)
        {
            this.code = code;
        }
    }

    [System.Serializable]
    public struct LanguageData
    {
        /// <summary>
        /// The real name of the language like, English, Spanish, German, Japanese
        /// </summary>
        public string LanguageName;

        /// <summary>
        /// The language code like 'en', 'es', 'de', 'jp'
        /// </summary>
        public string LanguageCode;
    }

    [System.Serializable]
    public class SupportedLanguages
    {
        public LanguageData[] languages;

        public bool LanguageCodeSupported(string languageCode)
        {
            foreach (var item in languages)
            {
                if (item.LanguageCode == languageCode)
                {
                    return true;
                }
            }
            return false;
        }
        public bool LanguageSupported(string languageName)
        {
            foreach (var item in languages)
            {
                if (item.LanguageName == languageName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
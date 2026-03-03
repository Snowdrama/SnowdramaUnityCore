using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

//basically if you listen to this and only update the local copies of the value
//when the options change. 
//public class OptionValueChanged : AMessage { }
public class OptionsData
{
    public Dictionary<string, int> intValues;
    public Dictionary<string, float> floatValues;
    public Dictionary<string, bool> boolValues;
    public Dictionary<string, string> stringValues;

    /// <summary>
    /// Merges the 2 options together to update options if
    /// the game is updated. Ensuring the default is set
    /// if the options don't exist yet.
    /// </summary>
    /// <param name="otherData"></param>
    public void AddUnusedOptions(OptionsData otherData)
    {
        foreach (var item in otherData.intValues)
        {
            if (!intValues.ContainsKey(item.Key))
            {
                intValues.Add(item.Key, item.Value);
            }
        }
        foreach (var item in otherData.floatValues)
        {
            if (!floatValues.ContainsKey(item.Key))
            {
                floatValues.Add(item.Key, item.Value);
            }
        }
        foreach (var item in otherData.boolValues)
        {
            if (!boolValues.ContainsKey(item.Key))
            {
                boolValues.Add(item.Key, item.Value);
            }
        }
        foreach (var item in otherData.stringValues)
        {
            if (!stringValues.ContainsKey(item.Key))
            {
                stringValues.Add(item.Key, item.Value);
            }
        }
    }
}
public class Options : MonoBehaviour
{
    private static OptionsData data;
    private static JsonSerializerSettings jsonSetting = new JsonSerializerSettings()
    {
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    };

    #region Bootstrap
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        //load on bootstrap
        Load();
    }
    #endregion

    private static void Save()
    {
        if (data == null)
        {
            Debug.LogError("Can't save because data is null!");
            return;
        }

        var jsonString = JsonConvert.SerializeObject(data, jsonSetting);
        File.WriteAllText($"{Application.persistentDataPath}/options.json", jsonString);
    }

    private static void Load()
    {
        var optionsDefaultJson = Resources.Load<TextAsset>("DefaultOptions");
        var defaultSettings = JsonConvert.DeserializeObject<OptionsData>(optionsDefaultJson.text, jsonSetting);
        //check if we have a file already made
        if (File.Exists($"{Application.persistentDataPath}/options.json"))
        {
            var fileJson = File.ReadAllText($"{Application.persistentDataPath}/options.json");
            data = JsonConvert.DeserializeObject<OptionsData>(fileJson, jsonSetting);

            data.AddUnusedOptions(defaultSettings);
            //we should save after merging
            Save();
        }
        //otherwise load the defaults
        else
        {
            data = defaultSettings;
            //we should save the default settings.
            Save();
        }
    }

    private static Dictionary<string, Action<string, int>> intCallbacks = new Dictionary<string, Action<string, int>>();
    private static Dictionary<string, Action<string, float>> floatCallbacks = new Dictionary<string, Action<string, float>>();
    private static Dictionary<string, Action<string, bool>> boolCallbacks = new Dictionary<string, Action<string, bool>>();
    private static Dictionary<string, Action<string, string>> stringCallbacks = new Dictionary<string, Action<string, string>>();

    #region Callback Registration

    public static void RegisterIntOptionCallback(string optionName, Action<string, int> callback)
    {
        if (!intCallbacks.ContainsKey(optionName))
        {
            intCallbacks.Add(optionName, callback);
        }
        else
        {
            intCallbacks[optionName] += callback;
        }
    }
    public static void UnregisterIntOptionCallback(string optionName, Action<string, int> callback)
    {
        if (intCallbacks.ContainsKey(optionName))
        {
            intCallbacks[optionName] -= callback;
        }
    }
    public static void RegisterFloatOptionCallback(string optionName, Action<string, float> callback)
    {
        if (!floatCallbacks.ContainsKey(optionName))
        {
            floatCallbacks.Add(optionName, callback);
        }
        else
        {
            floatCallbacks[optionName] += callback;
        }
    }
    public static void UnregisterFloatOptionCallback(string optionName, Action<string, float> callback)
    {
        if (floatCallbacks.ContainsKey(optionName))
        {
            floatCallbacks[optionName] -= callback;
        }
    }
    public static void RegisterBoolOptionCallback(string optionName, Action<string, bool> callback)
    {
        if (!boolCallbacks.ContainsKey(optionName))
        {
            boolCallbacks.Add(optionName, callback);
        }
        else
        {
            boolCallbacks[optionName] += callback;
        }
    }
    public static void UnregisterBoolOptionCallback(string optionName, Action<string, bool> callback)
    {
        if (boolCallbacks.ContainsKey(optionName))
        {
            boolCallbacks[optionName] -= callback;
        }
    }
    public static void RegisterStringOptionCallback(string optionName, Action<string, string> callback)
    {
        if (!stringCallbacks.ContainsKey(optionName))
        {
            stringCallbacks.Add(optionName, callback);
        }
        else
        {
            stringCallbacks[optionName] += callback;
        }
    }
    public static void UnregisterStringOptionCallback(string optionName, Action<string, string> callback)
    {
        if (stringCallbacks.ContainsKey(optionName))
        {
            stringCallbacks[optionName] -= callback;
        }
    }

    #endregion

    #region Setters/Getters
    public static void SetStringValue(string name, string value)
    {
        if (stringCallbacks.ContainsKey(name))
        {
            stringCallbacks[name]?.Invoke(name, value);
        }

        if (data.stringValues.ContainsKey(name))
        {
            data.stringValues[name] = value;
        }
        else
        {
            data.stringValues.Add(name, value);
        }
        Save();
    }
    public static string GetStringValue(string name, string defaultValue = "")
    {
        if (data.stringValues.ContainsKey(name))
        {
            return data.stringValues[name];
        }
        else
        {
            return defaultValue;
        }
    }

    public static void SetIntValue(string name, int value)
    {
        if (intCallbacks.ContainsKey(name))
        {
            intCallbacks[name]?.Invoke(name, value);
        }

        if (data.intValues.ContainsKey(name))
        {
            data.intValues[name] = value;
        }
        else
        {
            data.intValues.Add(name, value);
        }
        Save();
    }

    public static int GetIntValue(string name, int defaultValue = 0)
    {

        if (data.intValues.ContainsKey(name))
        {
            return data.intValues[name];
        }
        else
        {
            //Debug.LogWarning($"No Key! {name} : {defaultValue}");
            return defaultValue;
        }
    }

    public static void SetBoolValue(string name, bool value)
    {
        if (boolCallbacks.ContainsKey(name))
        {
            boolCallbacks[name]?.Invoke(name, value);
        }

        if (data.boolValues.ContainsKey(name))
        {
            data.boolValues[name] = value;
        }
        else
        {
            data.boolValues.Add(name, value);
        }
        Save();
    }

    public static bool GetBoolValue(string name, bool defaultValue = false)
    {
        if (data.boolValues.ContainsKey(name))
        {
            return data.boolValues[name];
        }
        else
        {
            return defaultValue;
        }
    }

    public static void SetFloatValue(string name, float value)
    {
        if (floatCallbacks.ContainsKey(name))
        {
            floatCallbacks[name]?.Invoke(name, value);
        }

        if (data.floatValues.ContainsKey(name))
        {
            data.floatValues[name] = value;
        }
        else
        {
            data.floatValues.Add(name, value);
        }
        Save();
    }

    public static float GetFloatValue(string name, float defaultValue = 0.0f)
    {
        if (data.floatValues.ContainsKey(name))
        {
            return data.floatValues[name];
        }
        else
        {
            return defaultValue;
        }
    }
    #endregion



#if UNITY_EDITOR

    [MenuItem("Snowdrama/Required/Create Default Options JSON")]
    public static void CreateSceneJSON()
    {
        OptionsData defaultData = new()
        {
            intValues = new Dictionary<string, int>()
            {
                {
                    "screen_setting",
                    0 //0 should be fullscreen
                },
                {
                    "screen_resolution_option",
                    0 //0 should be 1920x1080
                },
            },
            floatValues = new Dictionary<string, float>()
            {
                //we always want to default with some audio values
                {
                    "master_volume",
                    0.5f
                },
                {
                    "music_volume",
                    0.5f
                },
                {
                    "sound_volume",
                    0.5f
                },
                {
                    "voice_volume",
                    0.5f
                },
                {
                    "footstep_volume",
                    0.5f
                },
                //and some basic input sensitivity stuff
                {
                    "mouse_sensitivity_x",
                    1.0f
                },
                {
                    "mouse_sensitivity_y",
                    1.0f
                },
                {
                    "gamepad_sensitivity_x",
                    1.0f
                },
                {
                    "gamepad_sensitivity_y",
                    1.0f
                },
            },

            //basic input values
            boolValues = new Dictionary<string, bool>()
            {
                {
                    "invert_x",
                    false
                },
                {
                    "invert_y",
                    false
                },
                //graphics options
                {
                    "vsync",
                    true
                },
            },
            stringValues = new Dictionary<string, string>()
            {

            },
        };
        var dataString = JsonConvert.SerializeObject(defaultData, new JsonSerializerSettings() { Formatting = Formatting.Indented });
        if (!File.Exists($"Assets/Resources/DefaultOptions.json"))
        {
            File.WriteAllText($"Assets/Resources/DefaultOptions.json", dataString);
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError($"DANGER! ENSURE YOU ACTUALLY WANT TO DO THIS!!!" +
                $"Can't overwrite DefaultOptions.json because it already exists. " +
                $"If this intended please manually delete the DefaultOptions.json and run again");
        }
    }
#endif
}

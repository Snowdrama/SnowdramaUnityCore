using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

        //check if we have a file already made
        if (File.Exists($"{Application.persistentDataPath}/options.json"))
        {
            var fileJson = File.ReadAllText($"{Application.persistentDataPath}/options.json");
            data = JsonConvert.DeserializeObject<OptionsData>(fileJson, jsonSetting);
        }
        //otherwise load the defaults
        else
        {
            data = JsonConvert.DeserializeObject<OptionsData>(optionsDefaultJson.text, jsonSetting);
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
}

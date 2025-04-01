using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "OptionsObject", menuName = "OptionsObject")]
public class OptionsObject : ScriptableObject
{
    public List<FloatOption> defaultFloats = new List<FloatOption>();
    public List<IntOption> defaultInts = new List<IntOption>();
    public List<BoolOption> defaultBools = new List<BoolOption>();
    public List<StringOption> defaultStrings = new List<StringOption>();

    [Header("Debug [Edits Do Nothing]")]
    public List<FloatOption> debugFloats = new List<FloatOption>();
    public List<IntOption> debugInts = new List<IntOption>();
    public List<BoolOption> debugBools = new List<BoolOption>();
    public List<StringOption> debugStrings = new List<StringOption>();

    [System.Serializable]
    struct OptionData
    {
        public Dictionary<string, int> intValues;
        public Dictionary<string, float> floatValues;
        public Dictionary<string, bool> boolValues;
        public Dictionary<string, string> stringValues;
    }

    [System.NonSerialized]
    OptionData data = new OptionData();

    public void Save()
    {
        if (data.intValues == null) { data.intValues = new Dictionary<string, int>(); }
        if (data.floatValues == null) { data.floatValues = new Dictionary<string, float>(); }
        if (data.boolValues == null) { data.boolValues = new Dictionary<string, bool>(); }
        if (data.stringValues == null) { data.stringValues = new Dictionary<string, string>(); }
        AddDefaults();
        Debug.Log($"Saving To ApplicationDataFolder: {Application.persistentDataPath}/options.json");
        var jsonString = JsonConvert.SerializeObject(data);
        Debug.Log(jsonString);
        File.WriteAllText($"{Application.persistentDataPath}/options.json", jsonString);
        Debug.Log($"Done Saving!!!");
        UpdateDebug();
    }

    public void Load()
    {
        Debug.Log($"Loading From ApplicationDataFolder: {Application.persistentDataPath}/options.json");
        
        if (File.Exists($"{Application.persistentDataPath}/options.json"))
        {
            var jsonString = File.ReadAllText($"{Application.persistentDataPath}/options.json");
            Debug.Log(jsonString);
            data = JsonConvert.DeserializeObject<OptionData>(jsonString);
        }
        else
        {
            data = new OptionData();
            data.intValues = new Dictionary<string, int>();
            data.floatValues = new Dictionary<string, float>();
            data.boolValues = new Dictionary<string, bool>();
            data.stringValues = new Dictionary<string, string>();
        }
        AddDefaults();
        Save();
        Debug.Log($"Done Loading!!!");
        UpdateDebug();
    }

    public void AddDefaults()
    {
        if (data.intValues == null)
        { 
            data.intValues = new Dictionary<string, int>();
        }
        if (data.floatValues == null)
        { 
            data.floatValues = new Dictionary<string, float>(); 
        }
        if (data.boolValues == null)
        { 
            data.boolValues = new Dictionary<string, bool>();
        }
        if (data.stringValues == null)
        { 
            data.stringValues = new Dictionary<string, string>();
        }
        foreach (var item in defaultInts)
        {
            if (!data.intValues.ContainsKey(item.name))
            {
                data.intValues.Add(item.name, item.defaultValue);
            }
        }
        foreach (var item in defaultFloats)
        {
            if (!data.floatValues.ContainsKey(item.name))
            {
                data.floatValues.Add(item.name, item.defaultValue);
            }
        }
        foreach (var item in defaultBools)
        {
            if (!data.boolValues.ContainsKey(item.name))
            {
                data.boolValues.Add(item.name, item.defaultValue);
            }
        }
        foreach (var item in defaultStrings)
        {
            if (!data.stringValues.ContainsKey(item.name))
            {
                data.stringValues.Add(item.name, item.defaultValue);
            }
        }
    }


    public void SetStringValue(string name, string value)
    {
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
    public string GetStringValue(string name, string defaultValue = "")
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

    public void SetIntValue(string name, int value)
    {
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

    public int GetIntValue(string name, int defaultValue = 0)
    {
        foreach (var item in data.intValues)
        {
            Debug.LogWarning($"{item.Key} : {item.Value}");
        }


        if (data.intValues.ContainsKey(name))
        {
            return data.intValues[name];
        }
        else
        {
            Debug.LogWarning($"No key! {name} : {defaultValue}");
            return defaultValue;
        }
    }

    public void SetBoolValue(string name, bool value)
    {
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

    public bool GetBoolValue(string name, bool defaultValue = false)
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

    public void SetFloatValue(string name, float value)
    {
        Debug.Log($"Setting Float Value! {name}, {value}");
        if (data.floatValues.ContainsKey(name))
        {
            data.floatValues[name] = value;
        }
        else
        {
            Debug.Log($"Adding To List! {name}, {value}");
            data.floatValues.Add(name, value);
        }
        Debug.Log($"Saving!");
        Save();
    }

    public float GetFloatValue(string name, float defaultValue = 0.0f)
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

    [System.NonSerialized] bool initialized;

    public void OnEnable()
    {
        Debug.LogWarning("OnEnable");
        Load();
    }

    //private void OnDisable()
    //{
    //    Debug.LogWarning("OnDisable");
    //    Debug.LogWarning($"Disabling and Saving! {initialized}");
    //    Save();
    //}

    public void UpdateDebug()
    {
        debugInts = new List<IntOption>();
        foreach (var item in data.intValues)
        {
            var key = item.Key;
            var value = item.Value;
            debugInts.Add(new IntOption()
            {
                name = key,
                defaultValue = value
            });
        }
        debugFloats = new List<FloatOption>();
        foreach (var item in data.floatValues)
        {
            var key = item.Key;
            var value = item.Value;
            debugFloats.Add(new FloatOption()
            {
                name = key,
                defaultValue = value
            });
        }
        debugBools = new List<BoolOption>();
        foreach (var item in data.boolValues)
        {
            var key = item.Key;
            var value = item.Value;
            debugBools.Add(new BoolOption()
            {
                name = key,
                defaultValue = value
            });
        }
        debugStrings = new List<StringOption>();
        foreach (var item in data.stringValues)
        {
            var key = item.Key;
            var value = item.Value;
            debugStrings.Add(new StringOption()
            {
                name = key,
                defaultValue = value
            });
        }
    }
}

[System.Serializable]
public struct FloatOption
{
    public string name;
    public float defaultValue;
}
[System.Serializable]
public struct IntOption
{
    public string name;
    public int defaultValue;
}
[System.Serializable]
public struct BoolOption
{
    public string name;
    public bool defaultValue;
}
[System.Serializable]
public struct StringOption
{
    public string name;
    public string defaultValue;
}
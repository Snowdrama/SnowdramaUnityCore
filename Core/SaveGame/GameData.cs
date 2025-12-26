using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct GameDataStruct
{
    public string SceneToLoadOnLoad;
    public Dictionary<string, bool> boolData;
    public Dictionary<string, int> intData;
    public Dictionary<string, float> floatData;
    public Dictionary<string, double> doubleData;
    public Dictionary<string, string> stringData;

    //TODO: Add save/load images with B64 encoding? Save game screenshot images?
    //public Dictionary<int, string> imageData = new Dictionary<int, string>();

    //TODO: Add save/load images using byte arrays? Save game screenshot images?
    //public Dictionary<int, byte[]> imageDataBytes = new Dictionary<int, byte[]>();

    //TODO: Add save/load structs generically? 
    //public Dictionary<Type, Dictionary<int, Object>> structData = new Dictionary<Type, Dictionary<int, System.Object>>()
}
public class GameData : MonoBehaviour
{
    private static GameDataStruct data = new();
    private void Awake()
    {
        //On Awake the SaveManager should be loaded already!
        data = SaveManager.GetGameData();
    }

    #region BoolData
    public static void SetBool(string name, bool value)
    {

        if (!data.boolData.ContainsKey(name))
        {
            data.boolData.Add(name, value);
        }
        else
        {
            data.boolData[name] = value;
        }
    }

    public static bool GetBool(string name, bool defaultValue = default)
    {
        if (data.boolData.TryGetValue(name, out bool value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region IntData
    public static void SetInt(string name, int value)
    {

        Debug.Log($"Setting Int for name: {name} = {name}");
        if (!data.intData.ContainsKey(name))
        {
            data.intData.Add(name, value);
        }
        else
        {
            data.intData[name] = value;
        }
    }

    public static int GetInt(string name, int defaultValue = default)
    {
        Debug.Log($"Getting Float for name: {name} = {name} Has Key? {data.floatData.ContainsKey(name)}");
        if (data.intData.TryGetValue(name, out int value))
        {
            Debug.Log($"Found Value! Returning: {value}");
            return value;
        }
        Debug.LogWarning($"No Value Found Returning: {value}");
        return defaultValue;
    }
    #endregion

    #region FloatData
    public static void SetFloat(string name, float value)
    {

        Debug.Log($"Setting Float for name: {name} = {name}");
        if (!data.floatData.ContainsKey(name))
        {
            data.floatData.Add(name, value);
        }
        else
        {
            data.floatData[name] = value;
        }
    }

    public static float GetFloat(string name, float defaultValue = default)
    {

        Debug.Log($"Getting Float for name: {name} = {name} Has Key? {data.floatData.ContainsKey(name)}");
        if (data.floatData.TryGetValue(name, out float value))
        {
            Debug.Log($"Found Value! Returning: {value}");
            return value;
        }
        Debug.LogWarning($"No Value Found Returning: {value}");
        return defaultValue;
    }
    #endregion

    #region DoubleData
    public static void SetDouble(string name, double value)
    {

        if (!data.doubleData.ContainsKey(name))
        {
            data.doubleData.Add(name, value);
        }
        else
        {
            data.doubleData[name] = value;
        }
    }

    public static double GetDouble(string name, double defaultValue = default)
    {
        if (data.doubleData.TryGetValue(name, out double value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region StringData
    public static void SetString(string name, string value)
    {

        if (!data.stringData.ContainsKey(name))
        {
            data.stringData.Add(name, value);
        }
        else
        {
            data.stringData[name] = value;
        }
    }

    public static string GetString(string name, string defaultValue = default)
    {
        if (data.stringData.TryGetValue(name, out string value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    //TODO: Add save/load images with B64 encoding? Save game screenshot images?
    //TODO: Add save/load images using byte arrays? Save game screenshot images?
    //TODO: Add save/load structs generically? 
    public static void SetSceneToLoadOnLoad(string sceneName)
    {
        data.SceneToLoadOnLoad = sceneName;
    }
    public static GameDataStruct GetGameData()
    {
        return data;
    }



#if UNITY_EDITOR

    [MenuItem("Snowdrama/Required/Create Default Game Data JSON")]
    public static void CreateSceneJSON()
    {
        GameDataStruct defaultGameDataStruct = new()
        {
            SceneToLoadOnLoad = "MainMenuScene",
            floatData = new(),
            boolData = new(),
            doubleData = new(),
            intData = new(),
            stringData = new(),
        };

        var dataString = JsonConvert.SerializeObject(defaultGameDataStruct, new JsonSerializerSettings() { Formatting = Formatting.Indented });
        if (!File.Exists($"Assets/Resources/DefaultSave.jsonc"))
        {
            File.WriteAllText($"Assets/Resources/DefaultSave.jsonc", dataString);
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError($"DANGER! ENSURE YOU ACTUALLY WANT TO DO THIS!!!" +
                $"Can't overwrite DefaultSave.jsonc because it already exists. " +
                $"If this intended please manually delete the DefaultSave.jsonc and run again");
        }
    }
#endif
}

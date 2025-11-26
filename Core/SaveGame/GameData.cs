using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameDataStruct
{
    public string SceneToLoadOnLoad;
    public Dictionary<string, bool> boolData = new Dictionary<string, bool>();
    public Dictionary<string, int> intData = new Dictionary<string, int>();
    public Dictionary<string, float> floatData = new Dictionary<string, float>();
    public Dictionary<string, double> doubleData = new Dictionary<string, double>();
    public Dictionary<string, string> stringData = new Dictionary<string, string>();

    //TODO: Add save/load images with B64 encoding? Save game screenshot images?
    //public Dictionary<int, string> imageData = new Dictionary<int, string>();

    //TODO: Add save/load images using byte arrays? Save game screenshot images?
    //public Dictionary<int, byte[]> imageDataBytes = new Dictionary<int, byte[]>();

    //TODO: Add save/load structs generically? 
    //public Dictionary<Type, Dictionary<int, Object>> structData = new Dictionary<Type, Dictionary<int, System.Object>>()
}
public class GameData : MonoBehaviour
{
    private static GameDataStruct data = new GameDataStruct();
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

    public static bool GetBool(string name, bool defaultValue)
    {
        bool value = defaultValue;
        if (data.boolData.TryGetValue(name, out value))
        {
            return value;
        }
        return value;
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

    public static int GetInt(string name, int defaultValue)
    {
        int value = defaultValue;

        Debug.Log($"Getting Float for name: {name} = {name} Has Key? {data.floatData.ContainsKey(name)}");
        if (data.intData.TryGetValue(name, out value))
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

    public static float GetFloat(string name, float defaultValue)
    {
        float value = defaultValue;

        Debug.Log($"Getting Float for name: {name} = {name} Has Key? {data.floatData.ContainsKey(name)}");
        if (data.floatData.TryGetValue(name, out value))
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

    public static double GetDouble(string name, double defaultValue)
    {
        double value = defaultValue;
        if (data.doubleData.TryGetValue(name, out value))
        {
            return value;
        }
        return value;
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

    public static string GetString(string name, string defaultValue)
    {
        string value = defaultValue;
        if (data.stringData.TryGetValue(name, out value))
        {
            return value;
        }
        return value;
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
}

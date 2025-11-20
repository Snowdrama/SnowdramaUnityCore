using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameDataStruct
{
    public string saveName = "SaveGame";
    public Dictionary<int, bool> boolData = new Dictionary<int, bool>();
    public Dictionary<int, int> intData = new Dictionary<int, int>();
    public Dictionary<int, float> floatData = new Dictionary<int, float>();
    public Dictionary<int, double> doubleData = new Dictionary<int, double>();
    public Dictionary<int, string> stringData = new Dictionary<int, string>();

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
        int hash = name.GetHashCode();
        if (!data.boolData.ContainsKey(hash))
        {
            data.boolData.Add(hash, value);
        }
        else
        {
            data.boolData[hash] = value;
        }
    }

    public static bool GetBool(string name, bool defaultValue)
    {
        bool value = defaultValue;
        if (data.boolData.TryGetValue(name.GetHashCode(), out value))
        {
            return value;
        }
        return value;
    }
    #endregion

    #region IntData
    public static void SetInt(string name, int value)
    {
        int hash = name.GetHashCode();
        Debug.Log($"Setting Int for hash: {name} = {hash}");
        if (!data.intData.ContainsKey(hash))
        {
            data.intData.Add(hash, value);
        }
        else
        {
            data.intData[hash] = value;
        }
    }

    public static int GetInt(string name, int defaultValue)
    {
        int value = defaultValue;
        int hash = name.GetHashCode();
        Debug.Log($"Getting Float for hash: {name} = {hash} Has Key? {data.floatData.ContainsKey(hash)}");
        if (data.intData.TryGetValue(hash, out value))
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
        int hash = name.GetHashCode();
        Debug.Log($"Setting Float for hash: {name} = {hash}");
        if (!data.floatData.ContainsKey(hash))
        {
            data.floatData.Add(hash, value);
        }
        else
        {
            data.floatData[hash] = value;
        }
    }

    public static float GetFloat(string name, float defaultValue)
    {
        float value = defaultValue;
        int hash = name.GetHashCode();
        Debug.Log($"Getting Float for hash: {name} = {hash} Has Key? {data.floatData.ContainsKey(hash)}");
        if (data.floatData.TryGetValue(hash, out value))
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
        int hash = name.GetHashCode();
        if (!data.doubleData.ContainsKey(hash))
        {
            data.doubleData.Add(hash, value);
        }
        else
        {
            data.doubleData[hash] = value;
        }
    }

    public static double GetDouble(double name, double defaultValue)
    {
        double value = defaultValue;
        if (data.doubleData.TryGetValue(name.GetHashCode(), out value))
        {
            return value;
        }
        return value;
    }
    #endregion

    #region StringData
    public static void SetString(string name, string value)
    {
        int hash = name.GetHashCode();
        if (!data.stringData.ContainsKey(hash))
        {
            data.stringData.Add(hash, value);
        }
        else
        {
            data.stringData[hash] = value;
        }
    }

    public static string GetString(string name, string defaultValue)
    {
        string value = defaultValue;
        if (data.stringData.TryGetValue(name.GetHashCode(), out value))
        {
            return value;
        }
        return value;
    }
    #endregion

    //TODO: Add save/load images with B64 encoding? Save game screenshot images?
    //TODO: Add save/load images using byte arrays? Save game screenshot images?
    //TODO: Add save/load structs generically? 

    public static GameDataStruct GetGameData()
    {
        return data;
    }
}

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct GameData
{
    public string SceneToLoadOnLoad;
    public Dictionary<string, bool> boolData;
    public Dictionary<string, int> intData;
    public Dictionary<string, float> floatData;
    public Dictionary<string, double> doubleData;
    public Dictionary<string, string> stringData;
    public Dictionary<string, Vector2> Vector2Data;
    public Dictionary<string, Vector2Int> Vector2IntData;
    public Dictionary<string, Vector3> Vector3Data;
    public Dictionary<string, Vector3Int> Vector3IntData;
    public Dictionary<string, Vector4> Vector4Data;
    public Dictionary<string, bool[]> boolArrayData;
    public Dictionary<string, int[]> intArrayData;
    public Dictionary<string, float[]> floatArrayData;
    public Dictionary<string, double[]> doubleArrayData;
    public Dictionary<string, string[]> stringArrayData;

    public Dictionary<string, Vector2[]> Vector2ArrayData;
    public Dictionary<string, Vector2Int[]> Vector2IntArrayData;
    public Dictionary<string, Vector3[]> Vector3ArrayData;
    public Dictionary<string, Vector3Int[]> Vector3IntArrayData;
    public Dictionary<string, Vector4[]> Vector4ArrayData;

    //TODO: Add save/load images with B64 encoding? Save game screenshot images?
    //public Dictionary<int, string> imageData = new Dictionary<int, string>();

    //TODO: Add save/load images using byte arrays? Save game screenshot images?
    //public Dictionary<int, byte[]> imageDataBytes = new Dictionary<int, byte[]>();

    //TODO: ensure this serializes and stuff correctly...
    //public Dictionary<Type, Dictionary<string, System.Object>> structData;
}
public class GameDataManager : MonoBehaviour
{
    private static GameData data = new();
    private void Start()
    {
        //On Awake the SaveManager should be loaded already!
        data = SaveManager.GetGameData();

        Debug.LogWarning($"Save was loaded? Dispatching SaveGameLoadedMessage!");
        saveGameLoadedMessage.Dispatch();
    }

    #region BoolData
    public static void SetBool(string name, bool value)
    {
        if (data.boolData == null)
        {
            data.boolData = new Dictionary<string, bool>();
        }

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
        if (data.intData == null)
        {
            data.intData = new Dictionary<string, int>();
        }
        //Debug.Log($"Setting Int for name: {name} = {name}");
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
        //Debug.Log($"Getting Float for name: {name} = {name} Has Key? {data.floatData.ContainsKey(name)}");
        if (data.intData.TryGetValue(name, out int value))
        {
            //Debug.Log($"Found Value! Returning: {value}");
            return value;
        }
        //Debug.LogWarning($"No Value Found Returning: {value}");
        return defaultValue;
    }
    #endregion

    #region FloatData
    public static void SetFloat(string name, float value)
    {
        if (data.floatData == null)
        {
            data.floatData = new Dictionary<string, float>();
        }

        //Debug.Log($"Setting Float for name: {name} = {name}");
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
        //Debug.Log($"Getting Float for name: {name} = {name} Has Key? {data.floatData.ContainsKey(name)}");
        if (data.floatData.TryGetValue(name, out float value))
        {
            //Debug.Log($"Found Value! Returning: {value}");
            return value;
        }
        //Debug.LogWarning($"No Value Found Returning: {value}");
        return defaultValue;
    }
    #endregion

    #region DoubleData
    public static void SetDouble(string name, double value)
    {
        if (data.doubleData == null)
        {
            data.doubleData = new Dictionary<string, double>();
        }

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
        if (data.stringData == null)
        {
            data.stringData = new Dictionary<string, string>();
        }

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

    #region Vector2Data
    public static void SetVector2(string name, Vector2 value)
    {
        if (data.Vector2Data == null)
        {
            data.Vector2Data = new Dictionary<string, Vector2>();
        }

        if (!data.Vector2Data.ContainsKey(name))
        {
            data.Vector2Data.Add(name, value);
        }
        else
        {
            data.Vector2Data[name] = value;
        }
    }

    public static Vector2 GetVector2(string name, Vector2 defaultValue = default)
    {
        if (data.Vector2Data.TryGetValue(name, out Vector2 value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Vector2IntData
    public static void SetVector2Int(string name, Vector2Int value)
    {
        if (data.Vector2IntData == null)
        {
            data.Vector2IntData = new Dictionary<string, Vector2Int>();
        }

        if (!data.Vector2IntData.ContainsKey(name))
        {
            data.Vector2IntData.Add(name, value);
        }
        else
        {
            data.Vector2IntData[name] = value;
        }
    }

    public static Vector2Int GetVector2Int(string name, Vector2Int defaultValue = default)
    {
        if (data.Vector2IntData.TryGetValue(name, out Vector2Int value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Vector3Data
    public static void SetVector3(string name, Vector3 value)
    {

        if (data.Vector3Data == null)
        {
            data.Vector3Data = new Dictionary<string, Vector3>();
        }

        if (!data.Vector3Data.ContainsKey(name))
        {
            data.Vector3Data.Add(name, value);
        }
        else
        {
            data.Vector3Data[name] = value;
        }
    }

    public static Vector3 GetVector3(string name, Vector3 defaultValue = default)
    {
        if (data.Vector3Data.TryGetValue(name, out Vector3 value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Vector3IntData
    public static void SetVector3Int(string name, Vector3Int value)
    {
        if (data.Vector3IntData == null)
        {
            data.Vector3IntData = new Dictionary<string, Vector3Int>();
        }

        if (!data.Vector3IntData.ContainsKey(name))
        {
            data.Vector3IntData.Add(name, value);
        }
        else
        {
            data.Vector3IntData[name] = value;
        }
    }

    public static Vector3Int GetVector3Int(string name, Vector3Int defaultValue = default)
    {
        if (data.Vector3IntData.TryGetValue(name, out Vector3Int value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Vector4Data
    public static void SetVector4(string name, Vector4 value)
    {
        if (data.Vector4Data == null)
        {
            data.Vector4Data = new Dictionary<string, Vector4>();
        }

        if (!data.Vector4Data.ContainsKey(name))
        {
            data.Vector4Data.Add(name, value);
        }
        else
        {
            data.Vector4Data[name] = value;
        }
    }

    public static Vector4 GetVector4(string name, Vector4 defaultValue = default)
    {
        if (data.Vector4Data.TryGetValue(name, out Vector4 value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region boolArrayData
    public static void SetVector4(string name, bool[] value)
    {
        if (data.boolArrayData == null)
        {
            data.boolArrayData = new Dictionary<string, bool[]>();
        }

        if (!data.boolArrayData.ContainsKey(name))
        {
            data.boolArrayData.Add(name, value);
        }
        else
        {
            data.boolArrayData[name] = value;
        }
    }

    public static bool[] GetVector4(string name, bool[] defaultValue = default)
    {
        if (data.boolArrayData.TryGetValue(name, out bool[] value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region intArrayData
    public static void SetVector4(string name, int[] value)
    {
        if (data.intArrayData == null)
        {
            data.intArrayData = new Dictionary<string, int[]>();
        }

        if (!data.intArrayData.ContainsKey(name))
        {
            data.intArrayData.Add(name, value);
        }
        else
        {
            data.intArrayData[name] = value;
        }
    }

    public static int[] GetVector4(string name, int[] defaultValue = default)
    {
        if (data.intArrayData.TryGetValue(name, out int[] value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region floatArrayData
    public static void SetVector4(string name, float[] value)
    {
        if (data.floatArrayData == null)
        {
            data.floatArrayData = new Dictionary<string, float[]>();
        }

        if (!data.floatArrayData.ContainsKey(name))
        {
            data.floatArrayData.Add(name, value);
        }
        else
        {
            data.floatArrayData[name] = value;
        }
    }

    public static float[] GetVector4(string name, float[] defaultValue = default)
    {
        if (data.floatArrayData.TryGetValue(name, out float[] value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region doubleArrayData
    public static void SetVector4(string name, double[] value)
    {

        if (data.doubleArrayData == null)
        {
            data.doubleArrayData = new Dictionary<string, double[]>();
        }

        if (!data.doubleArrayData.ContainsKey(name))
        {
            data.doubleArrayData.Add(name, value);
        }
        else
        {
            data.doubleArrayData[name] = value;
        }
    }

    public static double[] GetVector4(string name, double[] defaultValue = default)
    {
        if (data.doubleArrayData.TryGetValue(name, out double[] value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region stringArrayData
    public static void SetVector4(string name, string[] value)
    {
        if (data.stringArrayData == null)
        {
            data.stringArrayData = new Dictionary<string, string[]>();
        }

        if (!data.stringArrayData.ContainsKey(name))
        {
            data.stringArrayData.Add(name, value);
        }
        else
        {
            data.stringArrayData[name] = value;
        }
    }

    public static string[] GetVector4(string name, string[] defaultValue = default)
    {
        if (data.stringArrayData.TryGetValue(name, out string[] value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Vector2ArrayData
    public static void SetVector4(string name, Vector2[] value)
    {
        if (data.Vector2ArrayData == null)
        {
            data.Vector2ArrayData = new Dictionary<string, Vector2[]>();
        }

        if (!data.Vector2ArrayData.ContainsKey(name))
        {
            data.Vector2ArrayData.Add(name, value);
        }
        else
        {
            data.Vector2ArrayData[name] = value;
        }
    }

    public static Vector2[] GetVector4(string name, Vector2[] defaultValue = default)
    {
        if (data.Vector2ArrayData.TryGetValue(name, out Vector2[] value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Vector2IntArrayData
    public static void SetVector2Int(string name, Vector2Int[] value)
    {
        if (data.Vector2IntArrayData == null)
        {
            data.Vector2IntArrayData = new Dictionary<string, Vector2Int[]>();
        }

        if (!data.Vector2IntArrayData.ContainsKey(name))
        {
            data.Vector2IntArrayData.Add(name, value);
        }
        else
        {
            data.Vector2IntArrayData[name] = value;
        }
    }

    public static Vector2Int[] GetVector2Int(string name, Vector2Int[] defaultValue = default)
    {
        if (data.Vector2IntArrayData.TryGetValue(name, out Vector2Int[] value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Vector3ArrayData
    public static void SetVector4(string name, Vector3[] value)
    {
        if (data.Vector3ArrayData == null)
        {
            data.Vector3ArrayData = new Dictionary<string, Vector3[]>();
        }

        if (!data.Vector3ArrayData.ContainsKey(name))
        {
            data.Vector3ArrayData.Add(name, value);
        }
        else
        {
            data.Vector3ArrayData[name] = value;
        }
    }

    public static Vector3[] GetVector4(string name, Vector3[] defaultValue = default)
    {
        if (data.Vector3ArrayData.TryGetValue(name, out Vector3[] value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Vector3IntArrayData
    public static void SetVector3Int(string name, Vector3Int[] value)
    {
        if (data.Vector3IntArrayData == null)
        {
            data.Vector3IntArrayData = new Dictionary<string, Vector3Int[]>();
        }

        if (!data.Vector3IntArrayData.ContainsKey(name))
        {
            data.Vector3IntArrayData.Add(name, value);
        }
        else
        {
            data.Vector3IntArrayData[name] = value;
        }
    }

    public static Vector3Int[] GetVector3Int(string name, Vector3Int[] defaultValue = default)
    {
        if (data.Vector3IntArrayData.TryGetValue(name, out Vector3Int[] value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Vector4ArrayData
    public static void SetVector4(string name, Vector4[] value)
    {
        if (data.Vector4ArrayData == null)
        {
            data.Vector4ArrayData = new Dictionary<string, Vector4[]>();
        }

        if (!data.Vector4ArrayData.ContainsKey(name))
        {
            data.Vector4ArrayData.Add(name, value);
        }
        else
        {
            data.Vector4ArrayData[name] = value;
        }
    }

    public static Vector4[] GetVector4(string name, Vector4[] defaultValue = default)
    {
        if (data.Vector4ArrayData.TryGetValue(name, out Vector4[] value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region StructData
    //public static void SetStruct<T>(string name, T value)
    //{
    //    if (data.structData == null)
    //    {
    //        data.structData = new Dictionary<Type, Dictionary<string, object>>();
    //    }

    //    if (!data.structData.ContainsKey(typeof(T)))
    //    {
    //        data.structData.Add(typeof(T), new Dictionary<string, object>());
    //    }

    //    if (!data.structData[typeof(T)].ContainsKey(name))
    //    {
    //        data.structData[typeof(T)].Add(name, value);
    //    }
    //    else
    //    {
    //        data.structData[typeof(T)][name] = value;
    //    }
    //}

    //public static T GetStruct<T>(string name, T defaultValue = default)
    //{
    //    Debug.Log($"Cehcking Type: {typeof(T)}");
    //    if (data.structData.ContainsKey(typeof(T)))
    //    {
    //        Debug.Log($"Type Exists: {typeof(T)} Checking name {name}");
    //        if (data.structData[typeof(T)].ContainsKey(name))
    //        {
    //            Debug.Log($"Name Exists: {name}");
    //            return (T)data.structData[typeof(T)][name];
    //        }
    //    }

    //    //if (data.structData.ContainsKey(typeof(T)) && data.structData[typeof(T)].ContainsKey("name"))
    //    //{
    //    //    return (T)data.structData[typeof(T)][name];
    //    //}
    //    Debug.LogError($"Type {typeof(T)} AND Name {name} not found??");
    //    return defaultValue;
    //}
    #endregion

    private RequestSaveMessage requestSaveMessage;
    private PreparingToSaveMessage preparingToSaveMessage;
    private SaveGameLoadedMessage saveGameLoadedMessage;
    private void OnEnable()
    {
        requestSaveMessage = Messages.Get<RequestSaveMessage>();
        preparingToSaveMessage = Messages.Get<PreparingToSaveMessage>();
        saveGameLoadedMessage = Messages.Get<SaveGameLoadedMessage>();
        requestSaveMessage.AddListener(OnRequestSave);
    }

    private void OnDisable()
    {
        requestSaveMessage.RemoveListener(OnRequestSave);
        Messages.Return<SaveGameLoadedMessage>();
        Messages.Return<PreparingToSaveMessage>();
        Messages.Return<RequestSaveMessage>();
    }

    private void OnRequestSave(int saveSlot)
    {
        Debug.Log("Save Requested, Preparing to save");
        preparingToSaveMessage?.Dispatch();

        Debug.Log($"ISavables triggered, Writing Save");
        if (saveSlot == -1)
        {
            SaveManager.AutoSave(data);
        }
        else
        {
            SaveManager.SaveGame(saveSlot, data, true);
        }
    }

    //TODO: Add save/load images with B64 encoding? Save game screenshot images?
    //TODO: Add save/load images using byte arrays? Save game screenshot images?
    //TODO: Add save/load structs generically? 
    public static void SetSceneToLoadOnLoad(string sceneName)
    {
        data.SceneToLoadOnLoad = sceneName;
    }
    public static GameData GetGameData()
    {
        return data;
    }



#if UNITY_EDITOR

    [MenuItem("Snowdrama/Required/Create Default Game Data JSON")]
    public static void CreateSceneJSON()
    {
        GameData defaultGameDataStruct = new()
        {
            SceneToLoadOnLoad = "MainMenuScene",
            floatData = new(),
            boolData = new(),
            doubleData = new(),
            intData = new(),
            stringData = new(),
            Vector2Data = new Dictionary<string, Vector2>(),
            Vector2IntData = new Dictionary<string, Vector2Int>(),
            Vector3Data = new Dictionary<string, Vector3>(),
            Vector3IntData = new Dictionary<string, Vector3Int>(),
            Vector4Data = new Dictionary<string, Vector4>(),
            boolArrayData = new Dictionary<string, bool[]>(),
            intArrayData = new Dictionary<string, int[]>(),
            floatArrayData = new Dictionary<string, float[]>(),
            doubleArrayData = new Dictionary<string, double[]>(),
            stringArrayData = new Dictionary<string, string[]>(),
            Vector2ArrayData = new Dictionary<string, Vector2[]>(),
            Vector2IntArrayData = new Dictionary<string, Vector2Int[]>(),
            Vector3ArrayData = new Dictionary<string, Vector3[]>(),
            Vector3IntArrayData = new Dictionary<string, Vector3Int[]>(),
            Vector4ArrayData = new Dictionary<string, Vector4[]>(),
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

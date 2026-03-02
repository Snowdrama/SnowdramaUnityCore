using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
public class GameDataManager : MonoBehaviour
{
    private static GameData data = new();
    private void Start()
    {
        //On Awake the SaveManager should be loaded already!
        data = SaveManager.GetGameData();



        saveGameLoadedMessage.Dispatch();
    }

    #region BoolData
    public static void SetBool(string name, bool value)
    {
        data.SetBool(name, value);
    }

    public static bool GetBool(string name, bool defaultValue = default)
    {
        return data.GetBool(name, defaultValue);
    }
    #endregion

    #region IntData
    public static void SetInt(string name, int value)
    {
        data.SetInt(name, value);
    }

    public static int GetInt(string name, int defaultValue = default)
    {
        return data.GetInt(name, defaultValue);
    }
    #endregion

    #region FloatData
    public static void SetFloat(string name, float value)
    {
        data.SetFloat(name, value);
    }

    public static float GetFloat(string name, float defaultValue = default)
    {
        return data.GetFloat(name, defaultValue);
    }
    #endregion

    #region DoubleData
    public static void SetDouble(string name, double value)
    {
        data.SetDouble(name, value);
    }

    public static double GetDouble(string name, double defaultValue = default)
    {
        return data.GetDouble(name, defaultValue);
    }
    #endregion

    #region StringData
    public static void SetString(string name, string value)
    {
        data.SetString(name, value);
    }

    public static string GetString(string name, string defaultValue = default)
    {
        return data.GetString(name, defaultValue);
    }
    #endregion

    #region Vector2Data
    public static void SetVector2(string name, Vector2 value)
    {
        data.SetVector2(name, value);
    }

    public static Vector2 GetVector2(string name, Vector2 defaultValue = default)
    {
        return data.GetVector2(name, defaultValue);
    }
    #endregion

    #region Vector2IntData

    public static void SetVector2Int(string name, Vector2Int value)
    {
        data.SetVector2Int(name, value);
    }

    public static Vector2Int GetVector2Int(string name, Vector2Int defaultValue = default)
    {
        return data.GetVector2Int(name, defaultValue);
    }
    #endregion

    #region Vector3Data
    public static void SetVector3(string name, Vector3 value)
    {
        data.SetVector3(name, value);
    }

    public static Vector3 GetVector3(string name, Vector3 defaultValue = default)
    {
        return data.GetVector3(name, defaultValue);
    }
    #endregion

    #region Vector3IntData
    public static void SetVector3Int(string name, Vector3Int value)
    {
        data.SetVector3Int(name, value);
    }

    public static Vector3Int GetVector3Int(string name, Vector3Int defaultValue = default)
    {
        return data.GetVector3Int(name, defaultValue);
    }
    #endregion

    #region Vector4Data
    public static void SetVector4(string name, Vector4 value)
    {
        data.SetVector4(name, value);
    }

    public static Vector4 GetVector4(string name, Vector4 defaultValue = default)
    {
        return data.GetVector4(name, defaultValue);
    }
    #endregion

    #region BoolArrayData
    public static void SetBoolArray(string name, bool[] value)
    {
        data.SetBoolArray(name, value);
    }

    public static bool[] GetBoolArray(string name, bool[] defaultValue = default)
    {
        return data.GetBoolArray(name, defaultValue);
    }
    #endregion

    #region IntArrayData
    public static void SetIntArray(string name, int[] value)
    {
        data.SetIntArray(name, value);
    }

    public static int[] GetIntArray(string name, int[] defaultValue = default)
    {
        return data.GetIntArray(name, defaultValue);
    }
    #endregion

    #region FloatArrayData
    public static void SetFloatArray(string name, float[] value)
    {
        data.SetFloatArray(name, value);
    }

    public static float[] GetFloatArray(string name, float[] defaultValue = default)
    {
        return data.GetFloatArray(name, defaultValue);
    }
    #endregion

    #region DoubleArrayData
    public static void SetDoubleArray(string name, double[] value)
    {
        data.SetDoubleArray(name, value);
    }

    public static double[] GetDoubleArray(string name, double[] defaultValue = default)
    {
        return data.GetDoubleArray(name, defaultValue);
    }
    #endregion

    #region StringArrayData
    public static void SetStringArray(string name, string[] value)
    {
        data.SetStringArray(name, value);
    }

    public static string[] GetStringArray(string name, string[] defaultValue = default)
    {
        return data.GetStringArray(name, defaultValue);
    }
    #endregion

    #region Vector2ArrayData
    public static void SetVector2Array(string name, Vector2[] value)
    {
        data.SetVector2Array(name, value);
    }

    public static Vector2[] GetVector2Array(string name, Vector2[] defaultValue = default)
    {
        return data.GetVector2Array(name, defaultValue);
    }
    #endregion

    #region Vector2IntArrayData

    public static void SetVector2IntArray(string name, Vector2Int[] value)
    {
        data.SetVector2IntArray(name, value);
    }

    public static Vector2Int[] GetVector2IntArray(string name, Vector2Int[] defaultValue = default)
    {
        return data.GetVector2IntArray(name, defaultValue);
    }
    #endregion

    #region Vector3ArrayData
    public static void SetVector3Array(string name, Vector3[] value)
    {
        data.SetVector3Array(name, value);
    }

    public static Vector3[] GetVector3Array(string name, Vector3[] defaultValue = default)
    {
        return data.GetVector3Array(name, defaultValue);
    }
    #endregion

    #region Vector3IntArrayData
    public static void SetVector3IntArray(string name, Vector3Int[] value)
    {
        data.SetVector3IntArray(name, value);
    }

    public static Vector3Int[] GetVector3IntArray(string name, Vector3Int[] defaultValue = default)
    {
        return data.GetVector3IntArray(name, defaultValue);
    }
    #endregion

    #region Vector4ArrayData
    public static void SetVector4Array(string name, Vector4[] value)
    {
        data.SetVector4Array(name, value);
    }

    public static Vector4[] GetVector4Array(string name, Vector4[] defaultValue = default)
    {
        return data.GetVector4Array(name, defaultValue);
    }
    #endregion

    #region Object/Struct Data
    public static void SetData<T>(string name, System.Object dataToSet)
    {
        if (data.objectData == null)
        {
            data.objectData = new Dictionary<Type, Dictionary<string, System.Object>>();
        }

        if (!data.objectData.ContainsKey(typeof(T)))
        {
            //create the dictionary if it's not already created
            data.objectData.Add(typeof(T), new Dictionary<string, System.Object>());
        }

        data.objectData[typeof(T)].Add(name, dataToSet);
    }

    public static T GetData<T>(string name)
    {
        if (data.objectData == null)
        {
            data.objectData = new Dictionary<Type, Dictionary<string, System.Object>>();
        }

        if (data.objectData.ContainsKey(typeof(T)) && data.objectData[typeof(T)].ContainsKey(name))
        {
            return (T)data.objectData[typeof(T)][name];
        }

        return (T)default;
    }
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
            objectData = new(),
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

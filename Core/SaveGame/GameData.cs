using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[JsonConverter(typeof(GameDataConverter))]

public class GameData
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
    public Dictionary<Type, Dictionary<string, System.Object>> objectData;

    #region BoolData
    public void SetBool(string name, bool value)
    {
        if (boolData == null)
        {
            boolData = new Dictionary<string, bool>();
        }

        if (!boolData.ContainsKey(name))
        {
            boolData.Add(name, value);
        }
        else
        {
            boolData[name] = value;
        }
    }

    public bool GetBool(string name, bool defaultValue = default)
    {
        if (boolData == null)
        {
            boolData = new Dictionary<string, bool>();
        }
        if (boolData.TryGetValue(name, out bool value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region IntData
    public void SetInt(string name, int value)
    {
        if (intData == null)
        {
            intData = new Dictionary<string, int>();
        }
        //Debug.Log($"Setting Int for name: {name} = {name}");
        if (!intData.ContainsKey(name))
        {
            intData.Add(name, value);
        }
        else
        {
            intData[name] = value;
        }
    }

    public int GetInt(string name, int defaultValue = default)
    {
        if (intData == null)
        {
            intData = new Dictionary<string, int>();
        }
        //Debug.Log($"Getting Float for name: {name} = {name} Has Key? {floatData.ContainsKey(name)}");
        if (intData.TryGetValue(name, out int value))
        {
            //Debug.Log($"Found Value! Returning: {value}");
            return value;
        }
        //Debug.LogWarning($"No Value Found Returning: {value}");
        return defaultValue;
    }
    #endregion

    #region FloatData
    public void SetFloat(string name, float value)
    {
        if (floatData == null)
        {
            floatData = new Dictionary<string, float>();
        }

        //Debug.Log($"Setting Float for name: {name} = {name}");
        if (!floatData.ContainsKey(name))
        {
            floatData.Add(name, value);
        }
        else
        {
            floatData[name] = value;
        }
    }

    public float GetFloat(string name, float defaultValue = default)
    {
        if (floatData == null)
        {
            Debug.LogError($"Float dictionary is null, setting it to empty dictionary");
            floatData = new Dictionary<string, float>();
        }
        if (floatData.TryGetValue(name, out float value))
        {
            //Debug.Log($"Found Value! Returning: {value}");
            return value;
        }
        //Debug.LogWarning($"No Value Found Returning: {value}");
        return defaultValue;
    }
    #endregion

    #region DoubleData
    public void SetDouble(string name, double value)
    {
        if (doubleData == null)
        {
            doubleData = new Dictionary<string, double>();
        }

        if (!doubleData.ContainsKey(name))
        {
            doubleData.Add(name, value);
        }
        else
        {
            doubleData[name] = value;
        }
    }

    public double GetDouble(string name, double defaultValue = default)
    {
        if (doubleData == null)
        {
            doubleData = new Dictionary<string, double>();
        }

        if (doubleData.TryGetValue(name, out double value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region StringData
    public void SetString(string name, string value)
    {
        if (stringData == null)
        {
            stringData = new Dictionary<string, string>();
        }

        if (!stringData.ContainsKey(name))
        {
            stringData.Add(name, value);
        }
        else
        {
            stringData[name] = value;
        }
    }

    public string GetString(string name, string defaultValue = default)
    {
        if (stringData == null)
        {
            stringData = new Dictionary<string, string>();
        }
        if (stringData.TryGetValue(name, out string value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Vector2Data
    public void SetVector2(string name, Vector2 value)
    {
        if (Vector2Data == null)
        {
            Vector2Data = new Dictionary<string, Vector2>();
        }

        if (!Vector2Data.ContainsKey(name))
        {
            Vector2Data.Add(name, value);
        }
        else
        {
            Vector2Data[name] = value;
        }
    }

    public Vector2 GetVector2(string name, Vector2 defaultValue = default)
    {
        if (Vector2Data == null)
        {
            Vector2Data = new Dictionary<string, Vector2>();
        }

        if (Vector2Data.TryGetValue(name, out Vector2 value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Vector2IntData
    public void SetVector2Int(string name, Vector2Int value)
    {
        if (Vector2IntData == null)
        {
            Vector2IntData = new Dictionary<string, Vector2Int>();
        }

        if (!Vector2IntData.ContainsKey(name))
        {
            Vector2IntData.Add(name, value);
        }
        else
        {
            Vector2IntData[name] = value;
        }
    }

    public Vector2Int GetVector2Int(string name, Vector2Int defaultValue = default)
    {
        if (Vector2IntData == null)
        {
            Vector2IntData = new Dictionary<string, Vector2Int>();
        }

        if (Vector2IntData.TryGetValue(name, out Vector2Int value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Vector3Data
    public void SetVector3(string name, Vector3 value)
    {

        if (Vector3Data == null)
        {
            Vector3Data = new Dictionary<string, Vector3>();
        }

        if (!Vector3Data.ContainsKey(name))
        {
            Vector3Data.Add(name, value);
        }
        else
        {
            Vector3Data[name] = value;
        }
    }

    public Vector3 GetVector3(string name, Vector3 defaultValue = default)
    {
        if (Vector3Data == null)
        {
            Vector3Data = new Dictionary<string, Vector3>();
        }

        if (Vector3Data.TryGetValue(name, out Vector3 value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Vector3IntData
    public void SetVector3Int(string name, Vector3Int value)
    {
        if (Vector3IntData == null)
        {
            Vector3IntData = new Dictionary<string, Vector3Int>();
        }

        if (!Vector3IntData.ContainsKey(name))
        {
            Vector3IntData.Add(name, value);
        }
        else
        {
            Vector3IntData[name] = value;
        }
    }

    public Vector3Int GetVector3Int(string name, Vector3Int defaultValue = default)
    {
        if (Vector3IntData == null)
        {
            Vector3IntData = new Dictionary<string, Vector3Int>();
        }

        if (Vector3IntData.TryGetValue(name, out Vector3Int value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Vector4Data
    public void SetVector4(string name, Vector4 value)
    {
        if (Vector4Data == null)
        {
            Vector4Data = new Dictionary<string, Vector4>();
        }

        if (!Vector4Data.ContainsKey(name))
        {
            Vector4Data.Add(name, value);
        }
        else
        {
            Vector4Data[name] = value;
        }
    }

    public Vector4 GetVector4(string name, Vector4 defaultValue = default)
    {
        if (Vector4Data == null)
        {
            Vector4Data = new Dictionary<string, Vector4>();
        }

        if (Vector4Data.TryGetValue(name, out Vector4 value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region boolArrayData
    public void SetBoolArray(string name, bool[] value)
    {
        if (boolArrayData == null)
        {
            boolArrayData = new Dictionary<string, bool[]>();
        }

        if (!boolArrayData.ContainsKey(name))
        {
            boolArrayData.Add(name, value);
        }
        else
        {
            boolArrayData[name] = value;
        }
    }

    public bool[] GetBoolArray(string name, bool[] defaultValue = default)
    {
        if (boolArrayData == null)
        {
            boolArrayData = new Dictionary<string, bool[]>();
        }
        if (boolArrayData.TryGetValue(name, out bool[] value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region intArrayData
    public void SetIntArray(string name, int[] value)
    {
        if (intArrayData == null)
        {
            intArrayData = new Dictionary<string, int[]>();
        }

        if (!intArrayData.ContainsKey(name))
        {
            intArrayData.Add(name, value);
        }
        else
        {
            intArrayData[name] = value;
        }
    }

    public int[] GetIntArray(string name, int[] defaultValue = default)
    {
        if (intArrayData == null)
        {
            intArrayData = new Dictionary<string, int[]>();
        }
        if (intArrayData.TryGetValue(name, out int[] value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region floatArrayData
    public void SetFloatArray(string name, float[] value)
    {
        if (floatArrayData == null)
        {
            floatArrayData = new Dictionary<string, float[]>();
        }

        if (!floatArrayData.ContainsKey(name))
        {
            floatArrayData.Add(name, value);
        }
        else
        {
            floatArrayData[name] = value;
        }
    }

    public float[] GetFloatArray(string name, float[] defaultValue = default)
    {
        if (floatArrayData == null)
        {
            floatArrayData = new Dictionary<string, float[]>();
        }
        if (floatArrayData.TryGetValue(name, out float[] value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region doubleArrayData
    public void SetDoubleArray(string name, double[] value)
    {

        if (doubleArrayData == null)
        {
            doubleArrayData = new Dictionary<string, double[]>();
        }

        if (!doubleArrayData.ContainsKey(name))
        {
            doubleArrayData.Add(name, value);
        }
        else
        {
            doubleArrayData[name] = value;
        }
    }

    public double[] GetDoubleArray(string name, double[] defaultValue = default)
    {
        if (doubleArrayData == null)
        {
            doubleArrayData = new Dictionary<string, double[]>();
        }
        if (doubleArrayData.TryGetValue(name, out double[] value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region stringArrayData
    public void SetStringArray(string name, string[] value)
    {
        if (stringArrayData == null)
        {
            stringArrayData = new Dictionary<string, string[]>();
        }

        if (!stringArrayData.ContainsKey(name))
        {
            stringArrayData.Add(name, value);
        }
        else
        {
            stringArrayData[name] = value;
        }
    }

    public string[] GetStringArray(string name, string[] defaultValue = default)
    {
        if (stringArrayData == null)
        {
            stringArrayData = new Dictionary<string, string[]>();
        }
        if (stringArrayData.TryGetValue(name, out string[] value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Vector2ArrayData
    public void SetVector2Array(string name, Vector2[] value)
    {
        if (Vector2ArrayData == null)
        {
            Vector2ArrayData = new Dictionary<string, Vector2[]>();
        }

        if (!Vector2ArrayData.ContainsKey(name))
        {
            Vector2ArrayData.Add(name, value);
        }
        else
        {
            Vector2ArrayData[name] = value;
        }
    }

    public Vector2[] GetVector2Array(string name, Vector2[] defaultValue = default)
    {
        if (Vector2ArrayData == null)
        {
            Vector2ArrayData = new Dictionary<string, Vector2[]>();
        }
        if (Vector2ArrayData.TryGetValue(name, out Vector2[] value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Vector2IntArrayData
    public void SetVector2IntArray(string name, Vector2Int[] value)
    {
        if (Vector2IntArrayData == null)
        {
            Vector2IntArrayData = new Dictionary<string, Vector2Int[]>();
        }

        if (!Vector2IntArrayData.ContainsKey(name))
        {
            Vector2IntArrayData.Add(name, value);
        }
        else
        {
            Vector2IntArrayData[name] = value;
        }
    }

    public Vector2Int[] GetVector2IntArray(string name, Vector2Int[] defaultValue = default)
    {
        if (Vector2IntArrayData == null)
        {
            Vector2IntArrayData = new Dictionary<string, Vector2Int[]>();
        }
        if (Vector2IntArrayData.TryGetValue(name, out Vector2Int[] value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Vector3ArrayData
    public void SetVector3Array(string name, Vector3[] value)
    {
        if (Vector3ArrayData == null)
        {
            Vector3ArrayData = new Dictionary<string, Vector3[]>();
        }

        if (!Vector3ArrayData.ContainsKey(name))
        {
            Vector3ArrayData.Add(name, value);
        }
        else
        {
            Vector3ArrayData[name] = value;
        }
    }

    public Vector3[] GetVector3Array(string name, Vector3[] defaultValue = default)
    {
        if (Vector3ArrayData == null)
        {
            Vector3ArrayData = new Dictionary<string, Vector3[]>();
        }
        if (Vector3ArrayData.TryGetValue(name, out Vector3[] value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Vector3IntArrayData
    public void SetVector3IntArray(string name, Vector3Int[] value)
    {
        if (Vector3IntArrayData == null)
        {
            Vector3IntArrayData = new Dictionary<string, Vector3Int[]>();
        }

        if (!Vector3IntArrayData.ContainsKey(name))
        {
            Vector3IntArrayData.Add(name, value);
        }
        else
        {
            Vector3IntArrayData[name] = value;
        }
    }

    public Vector3Int[] GetVector3IntArray(string name, Vector3Int[] defaultValue = default)
    {
        if (Vector3IntArrayData == null)
        {
            Vector3IntArrayData = new Dictionary<string, Vector3Int[]>();
        }
        if (Vector3IntArrayData.TryGetValue(name, out Vector3Int[] value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Vector4ArrayData
    public void SetVector4Array(string name, Vector4[] value)
    {
        if (Vector4ArrayData == null)
        {
            Vector4ArrayData = new Dictionary<string, Vector4[]>();
        }

        if (!Vector4ArrayData.ContainsKey(name))
        {
            Vector4ArrayData.Add(name, value);
        }
        else
        {
            Vector4ArrayData[name] = value;
        }
    }

    public Vector4[] GetVector4Array(string name, Vector4[] defaultValue = default)
    {
        if (Vector4ArrayData == null)
        {
            Vector4ArrayData = new Dictionary<string, Vector4[]>();
        }

        if (Vector4ArrayData.TryGetValue(name, out Vector4[] value))
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Object/Struct Data
    public void SetData<T>(string name, System.Object dataToSet)
    {
        if (objectData == null)
        {
            objectData = new Dictionary<Type, Dictionary<string, System.Object>>();
        }

        if (!objectData.ContainsKey(typeof(T)))
        {
            //create the dictionary if it's not already created
            objectData.Add(typeof(T), new Dictionary<string, System.Object>());
        }

        objectData[typeof(T)].Add(name, dataToSet);
    }

    public T GetData<T>(string name)
    {
        if (objectData == null)
        {
            objectData = new Dictionary<Type, Dictionary<string, System.Object>>();
        }

        if (objectData.ContainsKey(typeof(T)) && objectData[typeof(T)].ContainsKey(name))
        {
            return (T)objectData[typeof(T)][name];
        }

        return (T)default;
    }
    #endregion



}

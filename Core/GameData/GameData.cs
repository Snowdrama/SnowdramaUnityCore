using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.Core.GameData
{
    [System.Serializable]
    public class GameData
    {
        public StringIntDictionary intFlags = new StringIntDictionary();
        public StringStringDictionary stringFlags = new StringStringDictionary();
        public StringFloatDictionary floatFlags = new StringFloatDictionary();
        public StringBoolDictionary boolFlags = new StringBoolDictionary();
        public StringVector2Dictionary vector2Flags = new StringVector2Dictionary();
        public StringVector3Dictionary vector3Flags = new StringVector3Dictionary();
        public StringVector2IntDictionary vector2IntFlags = new StringVector2IntDictionary();
        public StringVector3IntDictionary vector3IntFlags = new StringVector3IntDictionary();
        public StringColorDictionary colorFlags = new StringColorDictionary();
        public void Init()
        {
            intFlags = new StringIntDictionary();
            stringFlags = new StringStringDictionary();
            floatFlags = new StringFloatDictionary();
            boolFlags = new StringBoolDictionary();
            vector2Flags = new StringVector2Dictionary();
            vector3Flags = new StringVector3Dictionary();
            vector2IntFlags = new StringVector2IntDictionary();
            vector3IntFlags = new StringVector3IntDictionary();
            colorFlags = new StringColorDictionary();
        }

        public void ClearAllData()
        {
            intFlags.Clear();
            stringFlags.Clear();
            floatFlags.Clear();
            boolFlags.Clear();
            vector2Flags.Clear();
            vector3Flags.Clear();
            colorFlags.Clear();
        }

        #region Ints
        public int GetFlagInt(string key, int fallbackState = 0)
        {
            return intFlags.ContainsKey(key) ? intFlags[key] : fallbackState;
        }
        public void SetFlagInt(string key, int value)
        {
            if (intFlags.ContainsKey(key))
            {
                intFlags[key] = value;
            }
            else
            {
                intFlags.Add(key, value);
            }
        }
        #endregion

        #region Strings
        public string GetFlagString(string key, string fallbackState = null)
        {
            return stringFlags.ContainsKey(key) ? stringFlags[key] : fallbackState;
        }

        public void SetFlagString(string key, string value)
        {
            if (stringFlags.ContainsKey(key))
            {
                stringFlags[key] = value;
            }
            else
            {
                stringFlags.Add(key, value);
            }
        }
        #endregion

        #region Floats
        public float GetFlagFloat(string key, float fallbackState = default)
        {
            return (floatFlags.ContainsKey(key)) ? floatFlags[key] : fallbackState;
        }

        public void SetFlagFloat(string key, float value)
        {
            if (floatFlags.ContainsKey(key))
            {
                floatFlags[key] = value;
            }
            else
            {
                floatFlags.Add(key, value);
            }
        }
        #endregion

        #region Bools

        public bool GetFlagBool(string key, bool fallbackState = false)
        {
            return boolFlags.ContainsKey(key) ? boolFlags[key] : fallbackState;
        }

        public void SetFlagBool(string key, bool value)
        {
            if (boolFlags.ContainsKey(key))
            {
                boolFlags[key] = value;
            }
            else
            {
                boolFlags.Add(key, value);
            }
        }
        #endregion

        #region Vector2
        public Vector2 GetFlagVector2(string key, Vector2 fallbackState = default)
        {
            return vector2Flags.ContainsKey(key) ? vector2Flags[key] : fallbackState;
        }
        public void SetFlagVector2(string key, Vector2 value)
        {
            if (vector2Flags.ContainsKey(key))
            {
                vector2Flags[key] = value;
            }
            else
            {
                vector2Flags.Add(key, value);
            }
        }
        #endregion

        #region Vector3
        public Vector3 GetFlagVector3(string key, Vector3 fallbackState = default)
        {
            return vector3Flags.ContainsKey(key) ? vector3Flags[key] : fallbackState;
        }

        public void SetFlagVector3(string key, Vector3 value)
        {
            if (vector3Flags.ContainsKey(key))
            {
                vector3Flags[key] = value;
            }
            else
            {
                vector3Flags.Add(key, value);
            }
        }
        #endregion

        #region Vector2Int

        public Vector2Int GetFlagVector2Int(string key, Vector2Int fallbackState = default)
        {
            return vector2IntFlags.ContainsKey(key) ? vector2IntFlags[key] : fallbackState;
        }

        public void SetFlagVector2Int(string key, Vector2Int value)
        {
            if (vector2IntFlags.ContainsKey(key))
            {
                vector2IntFlags[key] = value;
            }
            else
            {
                vector2IntFlags.Add(key, value);
            }
        }
        #endregion

        #region Vector3Int
        public Vector3Int GetFlagVector3Int(string key, Vector3Int fallbackState = default)
        {
            return vector3IntFlags.ContainsKey(key) ? vector3IntFlags[key] : fallbackState;
        }
        public void SetFlagVector3Int(string key, Vector3Int value)
        {
            if (vector3Flags.ContainsKey(key))
            {
                vector3Flags[key] = value;
            }
            else
            {
                vector3Flags.Add(key, value);
            }
        }
        #endregion

        #region Colors

        public Color GetFlagColor(string key, Color fallbackState = default)
        {
            return colorFlags.ContainsKey(key) ? colorFlags[key] : fallbackState;
        }

        public void SetFlagColor(string key, Color value)
        {
            if (colorFlags.ContainsKey(key))
            {
                colorFlags[key] = value;
            }
            else
            {
                colorFlags.Add(key, value);
            }
        }
        #endregion

        public string GetDataAsJSON()
        {
            var convertedJSON = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            Debug.Log(convertedJSON);
            return convertedJSON;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3Int FloorToInt(this Vector3 dir)
    {
        return new Vector3Int(Mathf.FloorToInt(dir.x), Mathf.FloorToInt(dir.y), Mathf.FloorToInt(dir.z));
    }

    public static Vector3Int CeilToInt(this Vector3 dir)
    {
        return new Vector3Int(Mathf.CeilToInt(dir.x), Mathf.CeilToInt(dir.y), Mathf.CeilToInt(dir.z));
    }

    public static Vector3Int RoundToInt(this Vector3 dir)
    {
        return new Vector3Int(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y), Mathf.RoundToInt(dir.z));
    }

    public static Vector3 Floor(this Vector3 dir)
    {
        return new Vector3(Mathf.Floor(dir.x), Mathf.Floor(dir.y), Mathf.Floor(dir.z));
    }

    public static Vector3 Ceil(this Vector3 dir)
    {
        return new Vector3(Mathf.Ceil(dir.x), Mathf.Ceil(dir.y), Mathf.Ceil(dir.z));
    }

    public static Vector3 Round(this Vector3 dir)
    {
        return new Vector3(Mathf.Round(dir.x), Mathf.Round(dir.y), Mathf.Round(dir.z));
    }

    public static Vector3Int Clamp(this Vector3Int val, Vector3Int min, Vector3Int max)
    {
        return new Vector3Int(Mathf.Clamp(val.x, min.x, max.x), Mathf.Clamp(val.y, min.y, max.y), Mathf.Clamp(val.z, min.z, max.z));
    }
    public static Vector3 Clamp(this Vector3 val, Vector3 min, Vector3 max)
    {
        return new Vector3(Mathf.Clamp(val.x, min.x, max.x), Mathf.Clamp(val.y, min.y, max.y), Mathf.Clamp(val.z, min.z, max.z));
    }

}

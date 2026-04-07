using UnityEngine;

public static class Vector3IntExtensions
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
    public static Vector3Int Clamp(this Vector3Int val, Vector3Int min, Vector3Int max)
    {
        return new Vector3Int(Mathf.Clamp(val.x, min.x, max.x), Mathf.Clamp(val.y, min.y, max.y), Mathf.Clamp(val.z, min.z, max.z));
    }
}

using UnityEngine;

public static class Vector2IntExtensions
{
    public static Vector2Int GetCoordinateFromIndex(this Vector2Int size, int index)
    {
        return new Vector2Int(Mathf.FloorToInt(index / size.x), index % size.y);
    }
    public static int GetIndexFromCoordinate(this Vector2Int size, Vector2Int pos)
    {
        return (size.x * pos.y) + pos.x;
    }
}

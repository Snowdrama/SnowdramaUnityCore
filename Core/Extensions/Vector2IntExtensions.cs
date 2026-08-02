using UnityEngine;

public static class Vector2IntExtensions
{
    public static Vector2Int Clamp(this Vector2Int val, Vector2Int min, Vector2Int max)
    {
        return new Vector2Int(Mathf.Clamp(val.x, min.x, max.x), Mathf.Clamp(val.y, min.y, max.y));
    }
    public static Vector2Int GetCoordinateFromIndex(this Vector2Int size, int index)
    {
        return new Vector2Int(Mathf.FloorToInt(index / size.x), index % size.y);
    }
    public static int GetIndexFromCoordinate(this Vector2Int size, Vector2Int pos)
    {
        return (size.x * pos.y) + pos.x;
    }
    /// <summary>
    /// Silly wrapper because I can never remember the order of how to subtract a vector to get a direction.
    /// </summary>
    /// <param name="from">The starting point. </param>
    /// <param name="to">The end point. </param>
    /// <returns>A vector that goes from the starting point to the end point. </returns>
    public static Vector2Int DirectionTo(this Vector2Int from, Vector2Int to)
    {
        return to - from;
    }
}

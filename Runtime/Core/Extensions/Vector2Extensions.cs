using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extensions{
    public static Vector2 VectorFromAngleRads(float angle)
    {
        Vector2 V = new Vector2();
        V.x = Mathf.Cos(angle);
        V.y = Mathf.Sin(angle);
        return V.normalized;
    }

    public static Vector2 VectorFromAngle(float angle)
    {
        angle = Mathf.Deg2Rad * angle;
        return VectorFromAngleRads(angle).normalized;
    }

    public static Vector2Int FloorToInt(this Vector2 dir)
    {
        return new Vector2Int(Mathf.FloorToInt(dir.x), Mathf.FloorToInt(dir.y));
    }

    public static Vector2Int CeilToInt(this Vector2 dir)
    {
        return new Vector2Int(Mathf.CeilToInt(dir.x), Mathf.CeilToInt(dir.y));
    }

    public static Vector2Int RoundToInt(this Vector2 dir)
    {
        return new Vector2Int(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y));
    }

    public static Vector2 Floor(this Vector2 dir)
    {
        return new Vector2(Mathf.Floor(dir.x), Mathf.Floor(dir.y));
    }

    public static Vector2 Ceil(this Vector2 dir)
    {
        return new Vector2(Mathf.Ceil(dir.x), Mathf.Ceil(dir.y));
    }

    public static Vector2 Round(this Vector2 dir)
    {
        return new Vector2(Mathf.Round(dir.x), Mathf.Round(dir.y));
    }

    public static Vector2Int Clamp(this Vector2Int val, Vector2Int min, Vector2Int max)
    {
        return new Vector2Int(Mathf.Clamp(val.x, min.x, max.x), Mathf.Clamp(val.y, min.y, max.y));
    }
    public static Vector2 Clamp(this Vector2 val, Vector2 min, Vector2 max)
    {
        return new Vector2(Mathf.Clamp(val.x, min.x, max.x), Mathf.Clamp(val.y, min.y, max.y));
    }

    public static float AngleFromVector(this Vector2 dir)
    {
        float angle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        if (angle < 0)
        {
            angle += 360;
        }
        return angle;
    }

    public static float AngleFromVectorRads(this Vector2 dir)
    {
        var angle = Mathf.Atan2(dir.y , dir.x);
        if (angle < 0)
        {
            angle += 2 * Mathf.PI;
        }
        return angle;
    }

    public static Vector2 PerpendicularClockwise(this Vector2 vec)
    {
        return new Vector2(vec.y, -vec.x);
    }

    public static Vector2 PerpendicularCounterClockwise(this Vector2 vec)
    {
        return new Vector2(-vec.y, vec.x);
    }
    public static float AngleTo(this Vector2 self, Vector2 to)
    {
        Vector2 direction = to - self;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;
        return angle;
    }
}
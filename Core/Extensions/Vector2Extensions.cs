using UnityEngine;
public static class Vector2Extensions
{
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
        var angle = Mathf.Atan2(dir.y, dir.x);
        if (angle < 0)
        {
            angle += 2 * Mathf.PI;
        }
        return angle;
    }

    /// <summary>
    /// Gets a vector that's rotated 90 degrees clockwise.
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static Vector2 PerpendicularClockwise(this Vector2 vec)
    {
        return new Vector2(vec.y, -vec.x);
    }

    /// <summary>
    /// Gets a vector that's rotated 90 degrees counter clockwise.
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static Vector2 PerpendicularCounterClockwise(this Vector2 vec)
    {
        return new Vector2(-vec.y, vec.x);
    }
    /// <summary>
    /// Given 2 angles, gets the difference in angle between them
    /// 
    /// For example if you give it(1, 1) and(1, 0) it would return 45 degrees.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static float AngleTo(this Vector2 self, Vector2 to)
    {
        Vector2 direction = to - self;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;
        return angle;
    }
    /// <summary>
    /// Given 2 angles, gets the difference in angle between them
    /// 
    /// For example if you give it(1, 1) and(1, 0) it would return 45 degrees * Mathf.Deg2Rad.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static float AngleToInRads(this Vector2 self, Vector2 to)
    {
        Vector2 direction = to - self;
        float angle = Mathf.Atan2(direction.y, direction.x);
        if (angle < 0f) angle += 360f;
        return angle;
    }

    /// <summary>
    /// Given a size, and a targetSize, finds the scale factor needed to 
    /// fit the size into the target size, Including fractional proportions 
    /// when the size would need to be decreased to fit the size
    /// 
    /// For example if I have a size of(2, 2) and a target of(16, 9), it 
    /// would return a scale factor of(4.5, 4.5) as `(2 * 4.5, 2 * 4.5)` 
    /// would be(9, 9) and is the smallest that would fit inside(16, 9). 
    /// 
    /// If the `stretchToFit` option is enabled, this will return a non 
    /// uniform scale.For example finding the scale factor of (2, 2) to 
    /// (16, 9) would in this case be(8, 4.5) as multiplying `(2 * 8, 2 * 4.5)`
    /// would give you the target scale of(16, 9). 
    /// 
    /// </summary>
    /// <param name="size"></param>
    /// <param name="targetSize"></param>
    /// <param name="stretchToFit"></param>
    /// <returns></returns>
    public static Vector2 FindScaleFactor(this Vector2 size, Vector2 targetSize, bool stretchToFit = false)
    {
        if (!stretchToFit)
        {
            var fullScale = targetSize / size;
            var minimumNeededToFit = Mathf.Min(fullScale.x, fullScale.y);
            return new Vector2(minimumNeededToFit, minimumNeededToFit);
        }
        return targetSize / size;
    }
}
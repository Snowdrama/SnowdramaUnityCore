using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;
public static class Vector2Extensions
{
    private static System.Random rand = new System.Random();
    public static Vector2 VectorFromAngleRads(float angle)
    {
        var V = new Vector2();
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
        var angle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        if (angle < 0)
        {
            angle += 360;
        }
        return angle;
    }

    public static float AngleFromVectorDegrees(this Vector2 dir)
    {
        return Mathf.Rad2Deg * dir.AngleFromVectorRads();
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
        var direction = to - self;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
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
        var direction = to - self;
        var angle = Mathf.Atan2(direction.y, direction.x);
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
    public static Vector2 FindScaleFactorIntegerScaled(this Vector2 size, Vector2 targetSize)
    {
        //we're trying to find the scale factor that will fit the 
        var fullScale = targetSize / size;
        var intSize = Mathf.FloorToInt(Mathf.Min(fullScale.x, fullScale.y));

        return new Vector2(intSize, intSize);
    }


    /// <summary>
    /// calculates the aspect ratio from the resolution
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static Vector2 FindAspectRatio(this Vector2 size)
    {
        return new Vector2();
    }
    /// <summary>
    /// Silly wrapper because I can never remember the order of how to subtract a vector to get a direction.
    /// </summary>
    /// <param name="from">The starting point. </param>
    /// <param name="to">The end point. </param>
    /// <returns>A vector that goes from the starting point to the end point. </returns>
    public static Vector2 DirectionTo(this Vector2 from, Vector2 to)
    {
        return to - from;
    }


    /// <summary>
    /// Get a random direction
    /// </summary>
    /// <returns></returns>
    public static Vector2 RandomDirection()
    {
        return Vector2Extensions.VectorFromAngle((float)UnityEngine.Random.Range(0, 360));
    }

    public static float AsRandomRange(this Vector2 range)
    {
        return UnityEngine.Random.Range(range.x, range.y);
    }

    public static Vector2 RandomPositionInRange(float minX, float maxX, float minY, float maxY, bool capUnitLength = true)
    {
        var x = UnityEngine.Random.Range(minX, maxX);
        var y = UnityEngine.Random.Range(minY, maxY);

        var value = new Vector2(x, y);

        if (capUnitLength && value.magnitude >= 1.0f)
        {
            return value.normalized;
        }

        return value;
    }
    public static Vector2 RoundToGrid(this Vector2 pos, Vector2 gridSize, Vector2 offset = new Vector2())
    {
        return new Vector2(
            (Mathf.Round((pos.x + offset.x) / gridSize.x) * gridSize.x) - offset.x,
            (Mathf.Round((pos.y + offset.y) / gridSize.y) * gridSize.y) - offset.x);
    }
    public static Vector2Int RoundToGridCell(this Vector2 pos, Vector2 gridSize, Vector2Int offset = new Vector2Int())
    {
        return new Vector2Int(
            Mathf.RoundToInt((pos.x + offset.x) / gridSize.x) - offset.x,
            Mathf.RoundToInt((pos.y + offset.y) / gridSize.y) - offset.x);
    }
    public static Vector2 FloorToGrid(this Vector2 pos, Vector2 gridSize, Vector2 offset = new Vector2())
    {
        return new Vector2(
            (Mathf.Floor((pos.x + offset.x) / gridSize.x) * gridSize.x) - offset.x,
            (Mathf.Floor((pos.y + offset.y) / gridSize.y) * gridSize.y) - offset.x);
    }

    public static Vector2Int FloorToGridCell(this Vector2 pos, Vector2 gridSize, Vector2Int offset = new Vector2Int())
    {
        return new Vector2Int(
            Mathf.FloorToInt((pos.x + offset.x) / gridSize.x) - offset.x,
            Mathf.FloorToInt((pos.y + offset.y) / gridSize.y) - offset.x);
    }
    public static Vector2 CeilToGrid(this Vector2 pos, Vector2 gridSize, Vector2 offset = new Vector2())
    {
        return new Vector2(
            (Mathf.Ceil((pos.x + offset.x) / gridSize.x) * gridSize.x) - offset.x,
            (Mathf.Ceil((pos.y + offset.y) / gridSize.y) * gridSize.y) - offset.x);
    }
    public static Vector2Int CeilToGridCell(this Vector2 pos, Vector2 gridSize, Vector2Int offset = new Vector2Int())
    {
        return new Vector2Int(
            Mathf.CeilToInt((pos.x + offset.x) / gridSize.x) - offset.x,
            Mathf.CeilToInt((pos.y + offset.y) / gridSize.y) - offset.x);
    }

    public static Vector2 RadialClamp(this Vector2 pos, Vector2 center, float range)
    {
        var dir = pos - center;
        var len = dir.magnitude;
        if (len > range)
        {
            return center + (dir.normalized * range);
        }
        return pos;
    }


    public static Vector2Int Clamp(this Vector2Int val, Vector2Int min, Vector2Int max)
    {
        return new Vector2Int(Mathf.Clamp(val.x, min.x, max.x), Mathf.Clamp(val.y, min.y, max.y));
    }

    /// <summary>
    /// Given a Vector2, return a float between the X and Y value
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static float RandomBetweenXY(this Vector2 val)
    {
        return RandomAndNoise.RandomRange(val.x, val.y);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="val">The current position</param>
    /// <param name="magnitude">the distance from the original position that you want the point</param>
    /// <returns>A new position some distance away from the original position</returns>
    public static Vector2 RandomDirectionOffset(this Vector2 val, float magnitude = 1.0f)
    {
        return val + (RandomDirection() * magnitude);
    }


    public static Vector2 Random(float minX, float maxX, float minY, float maxY, bool normalized = true)
    {
        if (normalized)
        {
            return new Vector2((float)Mathf.Lerp(minX, maxX, (float)rand.NextDouble()), (float)Mathf.Lerp(minY, maxY, (float)rand.NextDouble())).normalized;
        }
        return new Vector2((float)Mathf.Lerp(minX, maxX, (float)rand.NextDouble()), (float)Mathf.Lerp(minY, maxY, (float)rand.NextDouble()));
    }

    /// <summary>
    /// Gets a point that is the combination of the smallest value of both pointArr 
    /// 
    /// For example if you have the pointArr (3,5) and (2,9) the result would be (2, 5)
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns>a new vector that has the smallest value of both pointArr</returns>
    public static Vector2 Min(Vector2 A, Vector2 B)
    {
        return new Vector2(Mathf.Min(A.x, B.x), Mathf.Min(A.y, B.y));
    }

    /// <summary>
    /// Gets a point that is the combination of the largest value of both pointArr 
    /// 
    /// For example if you have the pointArr (3,5) and (2,9) the result would be (3, 9)
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns>a new vector that has the largest value of both pointArr</returns>
    public static Vector2 Max(Vector2 A, Vector2 B)
    {
        return new Vector2(Mathf.Max(A.x, B.x), Mathf.Max(A.y, B.y));
    }


    public static bool InBounds(this Vector2 pos, Vector2 TopLeftPos, Vector2 BottomRightPos)
    {
        if (pos.x >= TopLeftPos.x &&
            pos.x < BottomRightPos.x &&
            pos.y >= TopLeftPos.y &&
            pos.y < BottomRightPos.y)
        {
            return true;
        }
        return false;
    }
    public static bool InBounds(this Vector2Int pos, Vector2Int TopLeftPos, Vector2Int BottomRightPos)
    {
        if (pos.x >= TopLeftPos.x &&
            pos.x < BottomRightPos.x &&
            pos.y >= TopLeftPos.y &&
            pos.y < BottomRightPos.y)
        {
            return true;
        }
        return false;
    }
    public static bool InBounds(this Vector2Int pos, Vector2 TopLeftPos, Vector2 BottomRightPos)
    {
        return InBounds(pos, TopLeftPos.RoundToInt(), BottomRightPos.RoundToInt());
    }

    public static Vector2 Lerp(Vector2 start, Vector2 end, float t)
    {
        return new Vector2(Mathf.Lerp(start.x, end.x, t), Mathf.Lerp(start.y, end.y, t));
    }

    public static Vector2 MoveTowards(this Vector2 from, Vector2 to, float delta)
    {
        var translation = to - from;

        var scale = Mathf.Min(translation.magnitude, delta);

        var newVector = from + (translation.normalized * scale);

        return newVector;
    }

    public static int IndexFromPosition(this Vector2Int position, Vector2Int size)
    {
        return position.x + (position.y * size.x);
    }

    public static Vector2Int PositionFromIndex(this int position, Vector2Int size)
    {
        var y = Mathf.FloorToInt(position / size.x);
        var x = position - (y * size.x);

        return new Vector2Int(x, y);
    }
}
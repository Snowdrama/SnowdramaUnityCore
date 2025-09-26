using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class UICurve
{
    [Tooltip("Control points in screen percentage (0 to 1)")]
    public List<Vector2> controlPoints = new List<Vector2>();


    public Vector3 Evaluate(float t)
    {
        if (controlPoints == null || controlPoints.Count < 2)
        {
            Debug.LogWarning("UICurve requires at least 2 control points.");
            return Vector3.zero;
        }

        int segmentCount = controlPoints.Count - 1;
        float scaledT = t * segmentCount;
        int i = Mathf.FloorToInt(scaledT);
        i = Mathf.Clamp(i, 0, segmentCount - 1);

        float localT = scaledT - i;

        Vector2 p0 = GetPoint(i - 1);
        Vector2 p1 = GetPoint(i);
        Vector2 p2 = GetPoint(i + 1);
        Vector2 p3 = GetPoint(i + 2);

        var interpolated = CatmullRom(p0, p1, p2, p3, localT);
        return interpolated;
    }


    public Vector3 EvaluateRect(float t, RectTransform containerUIRect)
    {
        //get the percentage and then map to a rect
        var normalizedPos = Evaluate(t);
        var relativePoint = NormalizedPointToRect(containerUIRect, normalizedPos);

        return relativePoint;
    }

    private Vector2 NormalizedPointToRect(RectTransform containerUIRect, Vector2 normalizedPos)
    {
        Rect containerRect = containerUIRect.rect;

        var x = (containerRect.size.x * normalizedPos.x) + containerRect.x;
        var y = (containerRect.size.y * normalizedPos.y) + containerRect.y;

        return new Vector2(x, y);
    }

    /// <summary>
    /// Clamp index and get control point
    /// </summary>
    private Vector2 GetPoint(int index)
    {
        index = Mathf.Clamp(index, 0, controlPoints.Count - 1);
        return controlPoints[index];
    }

    /// <summary>
    /// Catmull-Rom spline interpolation
    /// </summary>
    private Vector2 CatmullRom(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

    /// <summary>
    /// Evaluate the curve at normalized time t [0, 1]
    /// </summary>
    public Vector3 EvaluateScreen(float t)
    {
        if (controlPoints == null || controlPoints.Count < 2)
        {
            Debug.LogWarning("UICurve requires at least 2 control points.");
            return Vector3.zero;
        }

        int segmentCount = controlPoints.Count - 1;
        float scaledT = t * segmentCount;
        int i = Mathf.FloorToInt(scaledT);
        i = Mathf.Clamp(i, 0, segmentCount - 1);

        float localT = scaledT - i;

        Vector2 p0 = GetPoint(i - 1);
        Vector2 p1 = GetPoint(i);
        Vector2 p2 = GetPoint(i + 1);
        Vector2 p3 = GetPoint(i + 2);

        Vector2 interpolated = CatmullRom(p0, p1, p2, p3, localT);
        return PercentageToScreenPosition(interpolated);
    }
    //public Vector3 EvaluateScreenRect(float t, RectTransform containerUIRect)
    //{
    //    var screenPos = EvaluateScreen(t);
    //    Vector2 anchoredPos;
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //        containerUIRect,
    //        screenPos,
    //        null,
    //        out anchoredPos
    //    );
    //    return anchoredPos;
    //}
    /// <summary>
    /// Convert (0-1) percentage to screen space position
    /// </summary>
    private Vector2 PercentageToScreenPosition(Vector2 percent)
    {
        return new Vector2(percent.x * Screen.width, percent.y * Screen.height);
    }

}


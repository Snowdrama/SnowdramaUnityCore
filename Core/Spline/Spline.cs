using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Spline : MonoBehaviour
{
    public enum SplineMode { _2D, _3D }

    [SerializeField] public SplineMode Mode = SplineMode._3D;
    [SerializeField] private bool closed = false;
    [SerializeField] private bool drawGizmosAlways = false;

    [Header("Debug Vectors")]
    [SerializeField] private bool showTangents = false;
    [SerializeField] private bool showNormals = false;
    [SerializeField] private bool showBinormals = false;
    [SerializeField] private bool showFrenetFrames = false;

    [Header("Twist Settings")]
    [SerializeField] private bool adaptiveTwistReference = true;

    [Header("Control Points")]
    [SerializeField]
    private List<Vector3> controlPoints = new List<Vector3>
    {
        new Vector3(-3f, 0f, 0f),
        new Vector3(-1f, 2f, 0f),
        new Vector3(1f, -2f, 0f),
        new Vector3(3f, 0f, 0f)
    };

    [SerializeField] private List<float> controlPointTwists = new List<float> { 0f, 0f, 0f, 0f };

    [Range(2, 64)] public int resolution = 16;

    public List<Vector3> ControlPoints => controlPoints;
    public List<float> ControlPointTwists => controlPointTwists;
    public bool Closed => closed;
    public bool AdaptiveTwistReference => adaptiveTwistReference;

    private void OnValidate() => SyncTwistList();

    private void SyncTwistList()
    {
        while (controlPointTwists.Count < controlPoints.Count)
            controlPointTwists.Add(0f);
        while (controlPointTwists.Count > controlPoints.Count)
            controlPointTwists.RemoveAt(controlPointTwists.Count - 1);
    }

    // ---------------- Inspector Utility ----------------

    public void AddPoint()
    {
        Vector3 last = controlPoints.Count > 0 ? controlPoints[controlPoints.Count - 1] : Vector3.zero;
        controlPoints.Add(last + Vector3.right);
        controlPointTwists.Add(0f);
    }

    public void RemoveLastPoint()
    {
        if (controlPoints.Count > 2)
        {
            controlPoints.RemoveAt(controlPoints.Count - 1);
            controlPointTwists.RemoveAt(controlPointTwists.Count - 1);
        }
    }

    public void Reverse()
    {
        controlPoints.Reverse();
        controlPointTwists.Reverse();
    }

    public void CenterOnOrigin()
    {
        if (controlPoints.Count == 0) return;
        Vector3 center = Vector3.zero;
        foreach (var p in controlPoints) center += p;
        center /= controlPoints.Count;
        for (int i = 0; i < controlPoints.Count; i++) controlPoints[i] -= center;
    }

    // ---------------- Catmull-Rom math ----------------

    public Vector3 Evaluate(int i, float t)
    {
        int count = controlPoints.Count;

        int p0 = WrapIndex(i - 1, count);
        int p1 = WrapIndex(i, count);
        int p2 = WrapIndex(i + 1, count);
        int p3 = WrapIndex(i + 2, count);

        Vector3 P0 = controlPoints[p0];
        Vector3 P1 = controlPoints[p1];
        Vector3 P2 = controlPoints[p2];
        Vector3 P3 = controlPoints[p3];

        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * P1) +
            (-P0 + P2) * t +
            (2f * P0 - 5f * P1 + 4f * P2 - P3) * t2 +
            (-P0 + 3f * P1 - 3f * P2 + P3) * t3
        );
    }

    public Vector3 EvaluateTangent(int i, float t)
    {
        int count = controlPoints.Count;

        int p0 = WrapIndex(i - 1, count);
        int p1 = WrapIndex(i, count);
        int p2 = WrapIndex(i + 1, count);
        int p3 = WrapIndex(i + 2, count);

        Vector3 P0 = controlPoints[p0];
        Vector3 P1 = controlPoints[p1];
        Vector3 P2 = controlPoints[p2];
        Vector3 P3 = controlPoints[p3];

        float t2 = t * t;

        return 0.5f * (
            (-P0 + P2) +
            (4f * P0 - 10f * P1 + 8f * P2 - 2f * P3) * t +
            (-3f * P0 + 9f * P1 - 9f * P2 + 3f * P3) * t2
        );
    }

    public Vector3 EvaluateSecondDerivative(int i, float t)
    {
        int count = controlPoints.Count;

        int p0 = WrapIndex(i - 1, count);
        int p1 = WrapIndex(i, count);
        int p2 = WrapIndex(i + 1, count);
        int p3 = WrapIndex(i + 2, count);

        Vector3 P0 = controlPoints[p0];
        Vector3 P1 = controlPoints[p1];
        Vector3 P2 = controlPoints[p2];
        Vector3 P3 = controlPoints[p3];

        return 0.5f * (
            (4f * P0 - 10f * P1 + 8f * P2 - 2f * P3) +
            2f * (-3f * P0 + 9f * P1 - 9f * P2 + 3f * P3) * t
        );
    }

    private int WrapIndex(int i, int count)
    {
        if (closed) return (i + count) % count;
        return Mathf.Clamp(i, 0, count - 1);
    }

    private float EvaluateTwist(int i, float t)
    {
        int count = controlPointTwists.Count;

        int p0 = WrapIndex(i - 1, count);
        int p1 = WrapIndex(i, count);
        int p2 = WrapIndex(i + 1, count);
        int p3 = WrapIndex(i + 2, count);

        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * controlPointTwists[p1]) +
            (-controlPointTwists[p0] + controlPointTwists[p2]) * t +
            (2f * controlPointTwists[p0] - 5f * controlPointTwists[p1] +
             4f * controlPointTwists[p2] - controlPointTwists[p3]) * t2 +
            (-controlPointTwists[p0] + 3f * controlPointTwists[p1] -
             3f * controlPointTwists[p2] + controlPointTwists[p3]) * t3
        );
    }

    public List<Vector3> GetPoints()
    {
        List<Vector3> points = new List<Vector3>();
        int segmentCount = closed ? controlPoints.Count : controlPoints.Count - 1;

        for (int i = 0; i < segmentCount; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                float t = j / (float)(resolution - 1);
                points.Add(Evaluate(i, t));
            }
        }
        return points;
    }

    // ---------------- Gizmos ----------------

    private void OnDrawGizmos()
    {
        if (drawGizmosAlways) DrawGizmoLines();
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmosAlways) DrawGizmoLines();
    }

    private void DrawGizmoLines()
    {
        var points = GetPoints();
        Gizmos.color = Color.green;

        for (int i = 0; i < points.Count - 1; i++)
            Gizmos.DrawLine(transform.TransformPoint(points[i]), transform.TransformPoint(points[i + 1]));

        int segmentCount = closed ? controlPoints.Count : controlPoints.Count - 1;

        for (int i = 0; i < segmentCount; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                float t = j / (float)(resolution - 1);
                Vector3 pos = transform.TransformPoint(Evaluate(i, t));

                Vector3 tangent = EvaluateTangent(i, t).normalized;
                Vector3 normal, binormal;

                if (Mode == SplineMode._2D)
                {
                    tangent.z = 0f;
                    tangent.Normalize();
                    normal = new Vector3(-tangent.y, tangent.x, 0f);
                    binormal = Vector3.forward;
                }
                else
                {
                    Vector3 second = EvaluateSecondDerivative(i, t);
                    normal = Vector3.Cross(Vector3.Cross(tangent, second), tangent).normalized;
                    if (normal == Vector3.zero) normal = Vector3.up;
                    binormal = Vector3.Cross(tangent, normal).normalized;
                }

                // --- Apply twist ---
                float twistAngle = EvaluateTwist(i, t);
                Quaternion twistRot = Quaternion.AngleAxis(twistAngle, tangent);
                normal = twistRot * normal;
                binormal = twistRot * binormal;

                if (showFrenetFrames)
                {
                    float size = 0.4f;
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(pos, pos + transform.TransformDirection(tangent) * size);
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(pos, pos + transform.TransformDirection(normal) * size);
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(pos, pos + transform.TransformDirection(binormal) * size);
                }
                else
                {
                    if (showTangents)
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawLine(pos, pos + transform.TransformDirection(tangent) * 0.5f);
                    }
                    if (showNormals)
                    {
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawLine(pos, pos + transform.TransformDirection(normal) * 0.5f);
                    }
                    if (showBinormals)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawLine(pos, pos + transform.TransformDirection(binormal) * 0.5f);
                    }
                }
            }
        }
    }
}

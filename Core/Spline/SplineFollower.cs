using UnityEngine;

[ExecuteAlways]
public class SplineFollower : MonoBehaviour
{
    [SerializeField] private Spline spline;
    [SerializeField] private float speed = 1f;
    [SerializeField] private bool loop = true;

    [Range(0f, 1f)]
    [SerializeField] private float t = 0f; // normalized position along spline

    [Header("Debug Gizmos")]
    [SerializeField] private bool showDebugFrame = true;
    [SerializeField] private float gizmoSize = 0.5f;

    private int currentSegment = 0;
    private Vector3 tangent, normal, binormal;

    private void Update()
    {
        if (spline == null || spline.ControlPoints.Count < 2) return;

        UpdateFollower();
    }

    private void UpdateFollower()
    {
        int segmentCount = spline.Closed ? spline.ControlPoints.Count : spline.ControlPoints.Count - 1;
        float scaledT = t * segmentCount;

        currentSegment = Mathf.FloorToInt(scaledT);
        float localT = scaledT - currentSegment;

        // Clamp for safety
        currentSegment = Mathf.Clamp(currentSegment, 0, segmentCount - 1);

        Vector3 pos = spline.Evaluate(currentSegment, localT);
        tangent = spline.EvaluateTangent(currentSegment, localT).normalized;

        if (spline.Mode == Spline.SplineMode._2D)
        {
            tangent.z = 0f;
            tangent.Normalize();
            normal = new Vector3(-tangent.y, tangent.x, 0f);
            binormal = Vector3.forward;
        }
        else
        {
            Vector3 second = spline.EvaluateSecondDerivative(currentSegment, localT);
            normal = Vector3.Cross(Vector3.Cross(tangent, second), tangent).normalized;
            if (normal == Vector3.zero) normal = Vector3.up;
            binormal = Vector3.Cross(tangent, normal).normalized;

            // Apply twist from spline
            float twist = GetTwist(currentSegment, localT);
            Quaternion twistRot = Quaternion.AngleAxis(twist, tangent);
            normal = twistRot * normal;
            binormal = twistRot * binormal;
        }

        // Apply transform
        transform.position = spline.transform.TransformPoint(pos);
        transform.rotation = Quaternion.LookRotation(
            spline.transform.TransformDirection(tangent),
            spline.transform.TransformDirection(normal)
        );
    }

    private float GetTwist(int segment, float localT)
    {
        var twists = spline.ControlPointTwists;
        int count = twists.Count;

        int p0 = WrapIndex(segment - 1, count);
        int p1 = WrapIndex(segment, count);
        int p2 = WrapIndex(segment + 1, count);
        int p3 = WrapIndex(segment + 2, count);

        float t2 = localT * localT;
        float t3 = t2 * localT;

        return 0.5f * (
            (2f * twists[p1]) +
            (-twists[p0] + twists[p2]) * localT +
            (2f * twists[p0] - 5f * twists[p1] + 4f * twists[p2] - twists[p3]) * t2 +
            (-twists[p0] + 3f * twists[p1] - 3f * twists[p2] + twists[p3]) * t3
        );
    }

    private int WrapIndex(int i, int count)
    {
        if (spline.Closed) return (i + count) % count;
        return Mathf.Clamp(i, 0, count - 1);
    }

    private void OnDrawGizmos()
    {
        if (!showDebugFrame || spline == null) return;

        // Draw local Frenet frame at follower’s position
        Vector3 pos = transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos, pos + transform.TransformDirection(tangent) * gizmoSize);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(pos, pos + transform.TransformDirection(normal) * gizmoSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pos, pos + transform.TransformDirection(binormal) * gizmoSize);
    }
}

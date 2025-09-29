using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.Spline
{
    [ExecuteAlways]
    public class Spline : MonoBehaviour
    {
        public enum SplineMode { _2D, _3D }
        [SerializeField]
        [Tooltip("When in 2D the spline snaps to the Z of the transform.")]
        public SplineMode Mode = SplineMode._3D;

        [SerializeField]
        private bool _closed = false;
        [SerializeField]
        public bool Closed
        {
            get
            {
                return _closed;
            }
            set
            {
                _closed = value;
                CalculateCache();
            }
        }
        [SerializeField] public bool DrawGizmosAlways = false;

        [Header("Debug Vectors")]
        [SerializeField] public bool ShowTangents = false;
        [SerializeField] public bool ShowNormals = false;
        [SerializeField] public bool ShowBinormals = false;
        [SerializeField] public bool ShowFrenetFrames = false;

        [Header("Control Points")]
        [SerializeField]
        public List<Vector3> ControlPoints = new List<Vector3>
        {
            new Vector3(-3f, 0f, 0f),
            new Vector3(-1f, 2f, 0f),
            new Vector3(1f, -2f, 0f),
            new Vector3(3f, 0f, 0f)
        };

        [SerializeField] public List<float> ControlPointTwists = new List<float> { 0f, 0f, 0f, 0f };

        [Range(2, 150)] public int Resolution = 50;

        [Header("Debug")]
        [SerializeField, EditorReadOnly] private List<Vector3> _points = new List<Vector3>();
        [SerializeField, EditorReadOnly] private List<Vector3> _tangents = new List<Vector3>();
        [SerializeField, EditorReadOnly] private List<Vector3> _normals = new List<Vector3>();
        [SerializeField, EditorReadOnly] private List<Vector3> _binormals = new List<Vector3>();
        #region External Evaluate Stuff
        public Vector3 GetPosition(float t)
        {
            t = Mathf.Clamp01(t);
            float pos = Mathf.Lerp(0, _points.Count - 2, t);
            int index = Mathf.FloorToInt(pos);
            float newT = pos - index;
            Vector3 point0 = _points[index];
            Vector3 point1 = _points[index + 1];
            return this.transform.TransformPoint(Vector3.Lerp(point0, point1, newT));
        }
        public Vector3 GetNormal(float t)
        {
            t = Mathf.Clamp01(t);
            float pos = Mathf.Lerp(0, _normals.Count - 2, t);
            int index = Mathf.FloorToInt(pos);
            float newT = pos - index;
            Vector3 point0 = _normals[index].normalized;
            Vector3 point1 = _normals[index + 1].normalized;
            return this.transform.TransformDirection(Vector3.Lerp(point0, point1, newT));

        }
        public Vector3 GetTangent(float t)
        {
            t = Mathf.Clamp01(t);
            float pos = Mathf.Lerp(0, _tangents.Count - 2, t);
            int index = Mathf.FloorToInt(pos);
            float newT = pos - index;
            Vector3 point0 = _tangents[index].normalized;
            Vector3 point1 = _tangents[index + 1].normalized;
            return this.transform.TransformDirection(Vector3.Lerp(point0, point1, newT));
        }

        #endregion

        #region Catmull-Rom Stuff
        public Vector3 EvaluatePosition(int i, float t)
        {
            int count = ControlPoints.Count;
            int p0 = WrapIndex(i - 1, count);
            int p1 = WrapIndex(i, count);
            int p2 = WrapIndex(i + 1, count);
            int p3 = WrapIndex(i + 2, count);

            Vector3 P0 = ControlPoints[p0];
            Vector3 P1 = ControlPoints[p1];
            Vector3 P2 = ControlPoints[p2];
            Vector3 P3 = ControlPoints[p3];

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
            int count = ControlPoints.Count;
            int p0 = WrapIndex(i - 1, count);
            int p1 = WrapIndex(i, count);
            int p2 = WrapIndex(i + 1, count);
            int p3 = WrapIndex(i + 2, count);

            Vector3 P0 = ControlPoints[p0];
            Vector3 P1 = ControlPoints[p1];
            Vector3 P2 = ControlPoints[p2];
            Vector3 P3 = ControlPoints[p3];

            float t2 = t * t;

            return 0.5f * (
                (-P0 + P2) +
                (4f * P0 - 10f * P1 + 8f * P2 - 2f * P3) * t +
                (-3f * P0 + 9f * P1 - 9f * P2 + 3f * P3) * t2
            );
        }
        /// <summary>
        /// Evaluates the normal at a given segment and parameter t (0..1)
        /// directly from the spline derivatives and twist.
        /// </summary>
        public Vector3 EvaluateNormal(int segment, float t)
        {
            Vector3 tangent = EvaluateTangent(segment, t).normalized;

            Vector3 normal;
            if (Mode == SplineMode._2D)
            {
                tangent.z = 0f;
                tangent.Normalize();
                normal = new Vector3(-tangent.y, tangent.x, 0f);
            }
            else
            {
                Vector3 second = EvaluateSecondDerivative(segment, t);
                normal = Vector3.Cross(Vector3.Cross(tangent, second), tangent).normalized;
                if (normal == Vector3.zero)
                    normal = Vector3.up; // fallback if curvature is zero
            }

            // Apply twist
            float twistAngle = EvaluateTwist(segment, t);
            Quaternion twistRot = Quaternion.AngleAxis(twistAngle, tangent);
            normal = twistRot * normal;

            return normal.normalized;
        }
        public Vector3 EvaluateSecondDerivative(int i, float t)
        {
            int count = ControlPoints.Count;
            int p0 = WrapIndex(i - 1, count);
            int p1 = WrapIndex(i, count);
            int p2 = WrapIndex(i + 1, count);
            int p3 = WrapIndex(i + 2, count);

            Vector3 P0 = ControlPoints[p0];
            Vector3 P1 = ControlPoints[p1];
            Vector3 P2 = ControlPoints[p2];
            Vector3 P3 = ControlPoints[p3];

            return 0.5f * (
                (4f * P0 - 10f * P1 + 8f * P2 - 2f * P3) +
                2f * (-3f * P0 + 9f * P1 - 9f * P2 + 3f * P3) * t
            );
        }
        private float EvaluateTwist(int i, float t)
        {
            int count = ControlPointTwists.Count;
            int p0 = WrapIndex(i - 1, count);
            int p1 = WrapIndex(i, count);
            int p2 = WrapIndex(i + 1, count);
            int p3 = WrapIndex(i + 2, count);

            float t2 = t * t;
            float t3 = t2 * t;

            return 0.5f * (
                (2f * ControlPointTwists[p1]) +
                (-ControlPointTwists[p0] + ControlPointTwists[p2]) * t +
                (2f * ControlPointTwists[p0] - 5f * ControlPointTwists[p1] +
                 4f * ControlPointTwists[p2] - ControlPointTwists[p3]) * t2 +
                (-ControlPointTwists[p0] + 3f * ControlPointTwists[p1] -
                 3f * ControlPointTwists[p2] + ControlPointTwists[p3]) * t3
            );
        }

        #endregion

        public void SetPointPosition(int i, Vector3 pos)
        {
            ControlPoints[i] = pos;
            CalculateCache();
        }

        public void SetResolution(int res)
        {
            Resolution = res;
            CalculateCache();
        }

        public void CalculateCache()
        {
            if (_points == null) { _points = new List<Vector3>(); }
            if (_tangents == null) { _tangents = new List<Vector3>(); }
            if (_normals == null) { _normals = new List<Vector3>(); }
            if (_binormals == null) { _binormals = new List<Vector3>(); }

            _points.Clear();
            _tangents.Clear();
            _normals.Clear();
            _binormals.Clear();
            int segmentCount = Closed ? ControlPoints.Count : ControlPoints.Count - 1;

            for (int i = 0; i < segmentCount; i++)
            {
                for (int j = 0; j < Resolution; j++)
                {
                    float t = j / (float)(Resolution - 1);
                    _points.Add(EvaluatePosition(i, t));
                }
            }

            if (_points.Count < 2) return;

            // Compute tangents
            for (int i = 0; i < _points.Count; i++)
            {
                Vector3 tan;
                if (i < _points.Count - 1)
                    tan = (_points[i + 1] - _points[i]).normalized;
                else
                    tan = (_points[i] - _points[i - 1]).normalized;
                _tangents.Add(tan);
            }

            // Initialize first
            // normal
            Vector3 refVec = Vector3.up;
            if (Vector3.Dot(refVec, _tangents[0]) > 0.99f)
                refVec = Vector3.right;

            Vector3 n0 = Vector3.Cross(_tangents[0], Vector3.Cross(refVec, _tangents[0])).normalized;
            Vector3 b0 = Vector3.Cross(_tangents[0], n0);
            _normals.Add(n0);
            _binormals.Add(b0);

            for (int seg = 0; seg < segmentCount; seg++)
            {
                for (int j = 0; j < Resolution; j++)
                {
                    int idx = seg * Resolution + j;
                    if (idx == 0) continue; // skip first

                    Vector3 prevT = _tangents[idx - 1];
                    Vector3 curT = _tangents[idx];

                    Vector3 axis = Vector3.Cross(prevT, curT);
                    float angle = Mathf.Asin(Mathf.Clamp(axis.magnitude, 0f, 1f));
                    if (axis.sqrMagnitude > 1e-6f)
                        axis.Normalize();
                    else
                        axis = Vector3.zero;

                    Vector3 prevN = _normals[idx - 1];
                    Vector3 prevB = _binormals[idx - 1];

                    Quaternion rot = axis != Vector3.zero ? Quaternion.AngleAxis(angle * Mathf.Rad2Deg, axis) : Quaternion.identity;
                    Vector3 n = rot * prevN;
                    Vector3 b = rot * prevB;

                    // --- Apply interpolated twist for this segment/sample ---
                    float t = j / (float)(Resolution - 1);
                    float twistAngle = EvaluateTwist(seg, t);
                    Quaternion twistRot = Quaternion.AngleAxis(twistAngle, curT);
                    n = twistRot * n;
                    b = twistRot * b;

                    _normals.Add(n);
                    _binormals.Add(b);
                }
            }
        }

        #region Gizmos
        private void OnDrawGizmos() { if (DrawGizmosAlways) DrawGizmoLines(); }
        private void OnDrawGizmosSelected() { if (!DrawGizmosAlways) DrawGizmoLines(); }

        private void DrawGizmoLines()
        {
            Gizmos.color = Color.green;

            for (int i = 0; i < _points.Count - 1; i++)
                Gizmos.DrawLine(transform.TransformPoint(_points[i]), transform.TransformPoint(_points[i + 1]));

            int frameStep = Mathf.Max(1, _points.Count / 32);
            for (int i = 0; i < _points.Count; i += frameStep)
            {
                Vector3 pos = transform.TransformPoint(_points[i]);
                Vector3 tangent = transform.TransformDirection(_tangents[i]);
                Vector3 normal = transform.TransformDirection(_normals[i]);
                Vector3 binormal = transform.TransformDirection(_binormals[i]);

                float size = 0.3f;

                if (ShowTangents) { Gizmos.color = Color.blue; Gizmos.DrawLine(pos, pos + tangent * size); }
                if (ShowNormals) { Gizmos.color = Color.green; Gizmos.DrawLine(pos, pos + normal * size); }
                if (ShowBinormals) { Gizmos.color = Color.red; Gizmos.DrawLine(pos, pos + binormal * size); }
            }
        }
        #endregion

        #region Utilities
        private int WrapIndex(int i, int count)
        {
            if (Closed)
            {
                return (i + count) % count;
            }
            return Mathf.Clamp(i, 0, count - 1);
        }
        public void AddPoint()
        {
            Vector3 last = ControlPoints.Count > 0 ? ControlPoints[ControlPoints.Count - 1] : Vector3.zero;
            ControlPoints.Add(last + Vector3.right);
            ControlPointTwists.Add(0f);
            CalculateCache();
        }

        public void RemoveLastPoint()
        {
            if (ControlPoints.Count > 2)
            {
                ControlPoints.RemoveAt(ControlPoints.Count - 1);
                ControlPointTwists.RemoveAt(ControlPointTwists.Count - 1);
                CalculateCache();
            }
        }

        public void Reverse()
        {
            ControlPoints.Reverse();
            ControlPointTwists.Reverse();
            CalculateCache();
        }

        public void CenterOnOrigin()
        {
            if (ControlPoints.Count == 0) return;
            Vector3 center = Vector3.zero;
            foreach (var p in ControlPoints) center += p;
            center /= ControlPoints.Count;
            for (int i = 0; i < ControlPoints.Count; i++) ControlPoints[i] -= center;
            CalculateCache();
        }
        #endregion
    }

}
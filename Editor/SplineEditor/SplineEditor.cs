using UnityEditor;
using UnityEngine;

namespace Snowdrama.Spline
{
    [CustomEditor(typeof(Spline))]
    public class SplineEditor : Editor
    {
        private Spline spline;
        private void OnEnable()
        {
            spline = (Spline)target;
            spline.CalculateCache();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Spline Tools", EditorStyles.boldLabel);

            if (GUILayout.Button("Add Point")) { spline.AddPoint(); spline.CalculateCache(); }
            if (GUILayout.Button("Remove Last Point")) { spline.RemoveLastPoint(); spline.CalculateCache(); }
            if (GUILayout.Button("Reverse Points")) { spline.Reverse(); spline.CalculateCache(); }
            if (GUILayout.Button("Center On Origin")) { spline.CenterOnOrigin(); spline.CalculateCache(); }

            EditorGUILayout.LabelField("Cache", EditorStyles.boldLabel);
            if (GUILayout.Button("Force Cache Update")) { spline.CalculateCache(); }

            if (GUI.changed) EditorUtility.SetDirty(spline);
        }

        private void OnSceneGUI()
        {
            var points = spline.ControlPoints;
            var twists = spline.ControlPointTwists;

            int pointCount = points.Count;

            for (int i = 0; i < pointCount; i++)
            {
                // --- Position handle ---
                EditorGUI.BeginChangeCheck();
                Vector3 newPos = Handles.PositionHandle(spline.transform.TransformPoint(points[i]), Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(spline, "Move Control Point");

                    if (spline.Mode == Spline.SplineMode._2D)
                    {
                        newPos.z = spline.transform.position.z;
                        points[i] = spline.transform.InverseTransformPoint(newPos);
                    }
                    else
                    {
                        points[i] = spline.transform.InverseTransformPoint(newPos);
                    }
                    spline.CalculateCache();
                }

                // --- Twist handle (3D only) ---
                if (spline.Mode == Spline.SplineMode._3D)
                {
                    Vector3 worldPos = spline.transform.TransformPoint(points[i]);

                    // Tangent at this control point from parallel transport
                    Vector3 tangent;
                    if (i < pointCount - 1 || spline.Closed)
                        tangent = (points[(i + 1) % pointCount] - points[i]).normalized;
                    else
                        tangent = (points[i] - points[i - 1]).normalized;

                    tangent = spline.transform.TransformDirection(tangent).normalized;

                    Handles.color = Color.yellow;
                    EditorGUI.BeginChangeCheck();

                    // Current twist rotation
                    Quaternion currentTwist = Quaternion.AngleAxis(twists[i], tangent);

                    // Draw disc
                    Quaternion newTwist = Handles.Disc(
                        currentTwist,
                        worldPos,
                        tangent,
                        0.6f,
                        false,
                        0f
                    );

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(spline, "Adjust Twist");

                        // Reference vector
                        Vector3 refDir = Vector3.up;

                        // Compute signed angle delta
                        Vector3 before = currentTwist * refDir;
                        Vector3 after = newTwist * refDir;
                        float signed = Vector3.SignedAngle(before, after, tangent);

                        twists[i] += signed;
                        spline.CalculateCache();
                    }

                    Handles.Label(worldPos + Vector3.up * 0.5f, $"Twist: {twists[i]:0.0}°");
                }
            }

            // Optional: draw spline points and frames
            DrawSplineGizmos();
        }

        private void DrawSplineGizmos()
        {
            //List<Vector3> points = spline.GetPoints();
            //int count = points.Count;
            //if (count < 2) return;

            //Handles.color = Color.green;
            //for (int i = 0; i < count - 1; i++)
            //    Handles.DrawLine(spline.transform.TransformPoint(points[i]), spline.transform.TransformPoint(points[i + 1]));

            //if (spline.ShowFrenetFrames || spline.ShowTangents || spline.ShowNormals || spline.ShowBinormals)
            //{
            //    int frameStep = Mathf.Max(1, count / 32); // limit gizmo density
            //    for (int i = 0; i < count; i += frameStep)
            //    {
            //        Vector3 pos = spline.transform.TransformPoint(points[i]);
            //        Vector3 tangent = spline.transform.TransformDirection(spline.GetTangentAtIndex(i));
            //        Vector3 normal = spline.transform.TransformDirection(spline.GetNormalAtIndex(i));
            //        Vector3 binormal = spline.transform.TransformDirection(spline.GetBinormalAtIndex(i));

            //        float size = 0.3f;

            //        if (spline.ShowFrenetFrames)
            //        {
            //            Handles.color = Color.red;
            //            Handles.DrawLine(pos, pos + tangent * size);
            //            Handles.color = Color.green;
            //            Handles.DrawLine(pos, pos + normal * size);
            //            Handles.color = Color.blue;
            //            Handles.DrawLine(pos, pos + binormal * size);
            //        }
            //        else
            //        {
            //            if (spline.ShowTangents)
            //            {
            //                Handles.color = Color.cyan;
            //                Handles.DrawLine(pos, pos + tangent * size);
            //            }
            //            if (spline.ShowNormals)
            //            {
            //                Handles.color = Color.magenta;
            //                Handles.DrawLine(pos, pos + normal * size);
            //            }
            //            if (spline.ShowBinormals)
            //            {
            //                Handles.color = Color.yellow;
            //                Handles.DrawLine(pos, pos + binormal * size);
            //            }
            //        }
            //    }
            //}
        }
    }

}
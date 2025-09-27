using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spline))]
public class SplineEditor : Editor
{
    private Spline spline;

    private void OnEnable()
    {
        spline = (Spline)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Spline Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Point")) spline.AddPoint();
        if (GUILayout.Button("Remove Last Point")) spline.RemoveLastPoint();
        if (GUILayout.Button("Reverse Points")) spline.Reverse();
        if (GUILayout.Button("Center On Origin")) spline.CenterOnOrigin();

        if (GUI.changed) EditorUtility.SetDirty(spline);
    }

    private void OnSceneGUI()
    {
        var points = spline.ControlPoints;
        var twists = spline.ControlPointTwists;

        for (int i = 0; i < points.Count; i++)
        {
            // --- Position handle ---
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.PositionHandle(spline.transform.TransformPoint(points[i]), Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Move Control Point");
                points[i] = spline.transform.InverseTransformPoint(newPos);
            }

            // --- Twist handle (3D only) ---
            if (spline.Mode == Spline.SplineMode._3D)
            {
                Vector3 worldPos = spline.transform.TransformPoint(points[i]);

                // Approximate tangent at this control point
                Vector3 tangent;
                if (i < points.Count - 1 || spline.Closed)
                    tangent = (spline.ControlPoints[(i + 1) % points.Count] - points[i]).normalized;
                else
                    tangent = (points[i] - points[i - 1]).normalized;

                tangent = spline.transform.TransformDirection(tangent).normalized;

                Handles.color = Color.yellow;
                EditorGUI.BeginChangeCheck();

                Quaternion currentTwist = Quaternion.AngleAxis(twists[i], tangent);

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

                    // Choose reference vector
                    Vector3 refDir;
                    if (spline.AdaptiveTwistReference)
                    {
                        // Pick a stable perpendicular to tangent
                        Vector3 arbitrary = Mathf.Abs(Vector3.Dot(tangent, Vector3.up)) > 0.9f
                            ? Vector3.right
                            : Vector3.up;
                        refDir = Vector3.Cross(tangent, arbitrary).normalized;
                    }
                    else
                    {
                        // Use global up
                        refDir = Vector3.up;
                    }

                    Vector3 before = currentTwist * refDir;
                    Vector3 after = newTwist * refDir;

                    float signed = Vector3.SignedAngle(before, after, tangent);
                    twists[i] += signed;
                }

                Handles.Label(worldPos + Vector3.up * 0.5f, $"Twist: {twists[i]:0.0}°");
            }
        }
    }
}

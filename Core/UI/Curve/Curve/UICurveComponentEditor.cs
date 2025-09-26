using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UICurveComponent))]
[CanEditMultipleObjects]
public class UICurveComponentEditor : Editor
{
    private UICurveComponent curveComp;
    private RectTransform rectTransform;

    private void OnEnable()
    {
        curveComp = (UICurveComponent)target;
        rectTransform = curveComp.GetComponent<RectTransform>();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Add Control Point"))
        {
            curveComp.curve.controlPoints.Add(new Vector2(0.5f, 0.5f));
            EditorUtility.SetDirty(curveComp);
        }

        if (GUILayout.Button("Clear Points"))
        {
            curveComp.curve.controlPoints.Clear();
            EditorUtility.SetDirty(curveComp);
        }
    }

    private void OnSceneGUI()
    {
        if (rectTransform == null || curveComp.curve.controlPoints.Count == 0) return;


        //handles will be cyan
        Handles.color = Color.cyan;


        //loop over the control points
        for (int i = 0; i < curveComp.curve.controlPoints.Count; i++)
        {
            //get their screen space offsets
            Vector2 percent = curveComp.curve.controlPoints[i];

            //get that as the world position relative to the rect's offsets
            Vector2 localPos = new Vector2(
                Mathf.LerpUnclamped(rectTransform.rect.xMin, rectTransform.rect.xMax, percent.x),
                Mathf.LerpUnclamped(rectTransform.rect.yMin, rectTransform.rect.yMax, percent.y)
            );

            //get the world point of that local point
            Vector3 worldPos = rectTransform.TransformPoint(localPos);

            //see if we change the handle position
            EditorGUI.BeginChangeCheck();
            Vector3 newWorldPos = Handles.FreeMoveHandle(worldPos, curveComp.handleSize, Vector3.zero, Handles.CircleHandleCap);

            //if we moved the handle
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curveComp, "Update Curve Position");

                //get the new world position and convert it back to local
                Vector2 newLocalPos = rectTransform.InverseTransformPoint(newWorldPos);



                if (curveComp.clampToRect)
                {
                    //inverse lerp it back into screen space
                    Vector2 newPercent = new Vector2(
                        Mathf.InverseLerp(rectTransform.rect.xMin, rectTransform.rect.xMax, newLocalPos.x),
                        Mathf.InverseLerp(rectTransform.rect.yMin, rectTransform.rect.yMax, newLocalPos.y)
                    );

                    if (curveComp.snapping)
                    {
                        newPercent.x = newPercent.x.RoundTo(curveComp.snapAmount);
                        newPercent.y = newPercent.y.RoundTo(curveComp.snapAmount);
                    }

                    //then set the control points back to the percentage of the screen space
                    curveComp.curve.controlPoints[i] = newPercent;
                }
                else
                {
                    //inverse lerp it back into screen space unclamped!
                    Vector2 newPercent = new Vector2(
                        newLocalPos.x.InverseLerpUnclamped(rectTransform.rect.xMin, rectTransform.rect.xMax),
                        newLocalPos.y.InverseLerpUnclamped(rectTransform.rect.yMin, rectTransform.rect.yMax)
                    );
                    if (curveComp.snapping)
                    {
                        newPercent.x = newPercent.x.RoundTo(curveComp.snapAmount);
                        newPercent.y = newPercent.y.RoundTo(curveComp.snapAmount);
                    }
                    //then set the control points back to the percentage of the screen space
                    curveComp.curve.controlPoints[i] = newPercent;
                }
                //mark as dirty to update inspector
                EditorUtility.SetDirty(curveComp);
            }
        }

        // draw preview
        Handles.color = Color.green;
        int resolution = 50;
        //just evaluate some value along the spline
        //resolution might need to go up with longer splines. 
        Vector3 prev = LocalToWorld(curveComp.curve.EvaluateScreen(0));
        for (int i = 1; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 current = LocalToWorld(curveComp.curve.EvaluateScreen(t));
            Handles.DrawLine(prev, current);
            prev = current;
        }
    }

    //takes a screen position and converts it to world since the object moves in world space
    //but we want the handles and final product in screen space
    private Vector3 LocalToWorld(Vector3 screenPos)
    {
        Vector2 percent = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
        Vector2 localPos = new Vector2(
            Mathf.LerpUnclamped(rectTransform.rect.xMin, rectTransform.rect.xMax, percent.x),
            Mathf.LerpUnclamped(rectTransform.rect.yMin, rectTransform.rect.yMax, percent.y)
        );

        return rectTransform.TransformPoint(localPos);
    }
}

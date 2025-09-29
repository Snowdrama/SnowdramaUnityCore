using UnityEngine;

public class UICurveComponent : MonoBehaviour
{
    public bool clampToRect = true;
    public bool snapping = true;
    public float snapAmount = 0.1f;
    public float handleSize = 10.0f;
    private RectTransform rectTransform;
    public UICurve curve = new UICurve()
    {
        controlPoints = new System.Collections.Generic.List<Vector2>()
        {
            new Vector2(0.25f,0.25f),
            new Vector2(0.5f,0.5f),
            new Vector2(0.75f,0.25f),
        }
    };
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        int resolution = 50;
        //just evaluate some value along the spline
        //resolution might need to go up with longer splines. 
        Vector3 prev = LocalToWorld(curve.EvaluateScreen(0));
        for (int i = 1; i <= resolution; i++)
        {
            float gizmoTime = i / (float)resolution;
            Vector3 current = LocalToWorld(curve.EvaluateScreen(gizmoTime));
            Gizmos.DrawLine(prev, current);
            prev = current;
        }
    }
    private void OnValidate()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    //takes a screen position and converts it to world since the object moves in world space
    //but we want the gizmos and final product in screen space
    private Vector3 LocalToWorld(Vector3 screenPos)
    {
        Vector2 percent = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
        Vector2 localPos = new Vector2(
            Mathf.LerpUnclamped(this.rectTransform.rect.xMin, this.rectTransform.rect.xMax, percent.x),
            Mathf.LerpUnclamped(this.rectTransform.rect.yMin, this.rectTransform.rect.yMax, percent.y)
        );
        return this.rectTransform.TransformPoint(localPos);
    }

    public void SetPointPosition(int index, Vector2 point)
    {
        curve.controlPoints[index] = point;
    }
}
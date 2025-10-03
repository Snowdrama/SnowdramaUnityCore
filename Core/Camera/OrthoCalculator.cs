using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class OrthoCalculator : MonoBehaviour
{
    public Camera cam;
    public float verticalUnits;
    public float horizontalUnits;

    private float width;
    private float height;
    private float orthoSize;
    private Vector2 currentScreenSize;
    private float verticalUnitsOld;
    private float horizontalUnitsOld;

    void Update()
    {
        if (currentScreenSize.x != Screen.width || currentScreenSize.y != Screen.height || verticalUnits != verticalUnitsOld || horizontalUnits != horizontalUnitsOld)
        {
            verticalUnitsOld = verticalUnits;
            horizontalUnitsOld = horizontalUnits;
            Debug.LogWarningFormat("Old Resolution: [{0}, {1}]", currentScreenSize.x, currentScreenSize.y);
            Debug.LogWarningFormat("New Resolution: [{0}, {1}]", Screen.width, Screen.height);
            currentScreenSize.x = Screen.width;
            currentScreenSize.y = Screen.height;
            width = CalcWidth();
            height = CalcHeight();
            orthoSize = 5.0f;
            if (width > height)
            {
                orthoSize = width;
            }
            else
            {
                orthoSize = height;
            }

            cam.orthographicSize = orthoSize;
        }
    }

    float CalcWidth()
    {
        return horizontalUnits * Screen.height / Screen.width * 0.5f;
    }
    float CalcHeight()
    {
        return verticalUnits * 0.5f;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(this.transform.position, new Vector3(horizontalUnits, verticalUnits, 0));
    }
}
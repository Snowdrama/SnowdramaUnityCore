using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

[ExecuteAlways]
public class CinemachineOrthographicCalculator : MonoBehaviour
{
    public float verticalUnits;
    public float horizontalUnits;

    private CinemachineCamera cinemachineCamera;
    private float width;
    private float height;
    private float orthoSize;
    private Vector2 currentScreenSize;
    private float verticalUnitsOld;
    private float horizontalUnitsOld;

    private void Update()
    {
        if (cinemachineCamera == null)
        {
            cinemachineCamera = this.GetComponent<CinemachineCamera>();
        }
        if (currentScreenSize.x != Screen.width || currentScreenSize.y != Screen.height || verticalUnits != verticalUnitsOld || horizontalUnits != horizontalUnitsOld)
        {
            verticalUnitsOld = verticalUnits;
            horizontalUnitsOld = horizontalUnits;
            Debug.LogWarningFormat("Old Resolution: [{0}, {1}]", currentScreenSize.x, currentScreenSize.y);
            Debug.LogWarningFormat("New Resolution: [{0}, {1}]", Screen.width, Screen.height);
            currentScreenSize.x = Screen.width;
            currentScreenSize.y = Screen.height;
            width = this.CalcWidth();
            height = this.CalcHeight();
            orthoSize = 5.0f;
            if (width > height)
            {
                orthoSize = width;
            }
            else
            {
                orthoSize = height;
            }

            cinemachineCamera.Lens.ModeOverride = LensSettings.OverrideModes.Orthographic;
            cinemachineCamera.Lens.OrthographicSize = orthoSize;
        }
    }

    private float CalcWidth()
    {
        return horizontalUnits * Screen.height / Screen.width * 0.5f;
    }
    private float CalcHeight()
    {
        return verticalUnits * 0.5f;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(this.transform.position, new Vector3(horizontalUnits, verticalUnits, 0));
    }
}
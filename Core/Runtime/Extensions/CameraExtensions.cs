using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraExtensions
{
    public static Vector2 GetCameraSize2D(float orthographicSize, float aspectRatio)
    {
        return new Vector2((2.0f * orthographicSize) * aspectRatio, 2.0f * orthographicSize);
    }

    /// <summary>
    /// Get the width and height of the camera based on the orthographic size and aspect ratio
    /// 
    /// 
    /// </summary>
    /// <param name="camera">the camera to get the size from</param>
    /// <returns></returns>
    public static Vector2 GetCameraSize2D(this Camera camera)
    {
        if (!camera.orthographic)
        {
            Debug.LogError("The camera is not set to orthographic so it cannot correctly calculate size");
        }
        return new Vector2((2.0f * camera.orthographicSize) * camera.aspect, 2.0f * camera.orthographicSize);
    }
}
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

[ExecuteAlways]
public class CinemachineOrthographicCalculator : MonoBehaviour
{
    //seems Default PPU is 256
    public int pixelsPerUnit = 32;

    //public Vector2 currentResolution = new Vector2(1920, 1080);
    //The resolution that should be fit to the current resolution
    public Vector2 targetResolution = new Vector2(512, 256);

    //since the example target resolution is 256x256
    //PPU fits inside the target resolution 16 times wide, and 8 times tall
    public Vector2 orthographicScale = new Vector2(16, 8);

    //because of this the actual ortho size is 
    public float orthographicSize = 4.0f;

    private int _pixelsPerUnitOld = 32;
    private Vector2 _targetResolutionOld;

    private Vector2 currentScreenSize;

    private CinemachineCamera cinemachineCamera;

    [SerializeField] private float ratio = 1.0f;
    private void Update()
    {
        if (cinemachineCamera == null)
        {
            cinemachineCamera = this.GetComponent<CinemachineCamera>();
            return;
        }

        //was anything changed?
        if (
            currentScreenSize.x != Screen.width ||
            currentScreenSize.y != Screen.height ||
            targetResolution.x != _targetResolutionOld.x ||
            targetResolution.y != _targetResolutionOld.y ||
            pixelsPerUnit != _pixelsPerUnitOld
            )
        {
            //update the old internal unit values
            _targetResolutionOld = targetResolution;
            currentScreenSize = new Vector2(Screen.width, Screen.height);

            //600 / 800 * 0.5f;
            ratio = (float)Screen.height / Screen.width * 0.5f;
            orthographicScale = new Vector2(
                targetResolution.x / pixelsPerUnit * Screen.height / Screen.width * 0.5f,
                targetResolution.y / pixelsPerUnit * 0.5f
            );

            if (orthographicScale.x > orthographicScale.y)
            {
                orthographicSize = orthographicScale.x;
            }
            else
            {
                orthographicSize = orthographicScale.y;
            }

            cinemachineCamera.Lens.ModeOverride = LensSettings.OverrideModes.Orthographic;
            cinemachineCamera.Lens.OrthographicSize = orthographicSize;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(this.transform.position, new Vector3(orthographicScale.x * 1 / ratio, orthographicScale.y * 2, 0));
    }
}
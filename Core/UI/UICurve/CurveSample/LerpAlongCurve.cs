using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class LerpAlongCurve : MonoBehaviour
{
    [SerializeField] private UICurveComponent curveComponent;
    [SerializeField] private RectTransform parentTransform;

    private RectTransform rect;
    private bool forward;

    [SerializeField, Range(0f, 1f)]
    private float time = 0;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {

        //don't play in editor
        if (Application.isPlaying)
        {
            if (forward)
            {
                time += Time.deltaTime;
                if (time >= 1.0f)
                {
                    forward = false;
                    time = 1.0f;
                }
            }
            else
            {
                time -= Time.deltaTime;
                if (time <= 0.0f)
                {
                    forward = true;
                    time = 0.0f;
                }
            }
        }

        if (parentTransform != null)
        {
            this.rect.localPosition = curveComponent.curve.EvaluateRect(time, parentTransform);
        }
        else
        {
            this.rect.position = curveComponent.curve.EvaluateScreen(time);
        }
    }

}

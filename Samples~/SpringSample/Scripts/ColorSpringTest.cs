using Snowdrama.Spring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSpringTest : MonoBehaviour
{
    SpringColor colorSpring;

    public Color colorStart;
    public Color colorEnd;
    public Color activeTarget;
    public SpringConfiguration springConfiguration;

    private bool positionToggle = true;
    private float _timer;
    private Renderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        colorSpring = new SpringColor(springConfiguration, colorStart);
        meshRenderer = this.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        colorSpring.Update(Time.deltaTime);
        if (_timer > 2.0f)
        {
            _timer -= 2.0f;
            positionToggle = !positionToggle;
            if (positionToggle)
            {
                activeTarget = colorStart;
                colorSpring.Target = colorStart;
            }
            else
            {
                activeTarget = colorEnd;
                colorSpring.Target = colorEnd;
            }
        }
        meshRenderer.material.color = colorSpring.Value;
    }
}

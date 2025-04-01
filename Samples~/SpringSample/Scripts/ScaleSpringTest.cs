using Snowdrama.Spring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleSpringTest : MonoBehaviour
{
    public SpringConfiguration springConfiguration;

    public Spring3D spring;
    private float _timer;
    public Vector3 scaleTargetMin;
    public Vector3 scaleTargetMax;
    public bool scaleUp;
    // Start is called before the first frame update
    void Start()
    {
        spring = new Spring3D(springConfiguration, new Vector3(0, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        spring.Update(Time.deltaTime);
        _timer -= Time.deltaTime;
        if(_timer <= 0.0f)
        {
            _timer = 3.0f;
            scaleUp = !scaleUp;
            if (scaleUp)
            {
                spring.Target = scaleTargetMax;
            }
            else
            {
                spring.Target = scaleTargetMin;
            }
        }
        this.transform.localScale = spring.Value;
    }
}

using Snowdrama.Spring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialSpringTest : MonoBehaviour
{
    public SpringConfiguration config;
    public SpringRadial radialSpring;
    public bool run;
    public float targetToSet;
    private float _timer;
    // Start is called before the first frame update
    void Start()
    {
        radialSpring = new SpringRadial(config);
    }

    // Update is called once per frame
    void Update()
    {
        radialSpring.Update(Time.deltaTime);
        _timer += Time.deltaTime;
        if (_timer > 2.0f)
        {
            _timer -= 2.0f;
            radialSpring.Target = Random.Range(0.0f, 360.0f);
        }

        this.transform.rotation = Quaternion.AngleAxis(radialSpring.Value, Vector3.forward);
    }
}

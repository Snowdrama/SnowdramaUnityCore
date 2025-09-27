using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snowdrama.Core;
using Snowdrama.Spring;
using UnityEngine.Splines;
using TMPro;


public class TextParticlePool2D : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float size;
    [SerializeField] private float something;



    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

    }
}

public class TextParticle2D : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float size;
    [SerializeField] private float something;

    [SerializeField] private Spring2D scaleSpring;
    [SerializeField] private SplineContainer animSpline;

    [SerializeField] private Vector2 rangeX;
    [SerializeField] private Vector2 rangeY;

    //animation
    [Header("Animation")]
    [SerializeField] private float animateSpeed = 1.0f;
    [SerializeField] private float animateRotationSpeed = 360.0f;
    private bool animate;
    private float animateValue;
    private float animateRotationValue;

    private float currentDuration;
    private float currentSize;
    private bool playing;

    public Spline testSpline;
    private void Update()
    {
        if (playing)
        {
            Animate();
            if (animateValue >= 1.0f)
            {
                playing = false;
                this.gameObject.SetActive(false);
            }
        }
    }
    public void Play(Vector2 position)
    {
        this.transform.position = position;
        scaleSpring.Value = new Vector2(0, 0);
        scaleSpring.Target = new Vector2(size, size);


        //randomize the particle trajectory
        SetSplineEnd();

        playing = true;
    }
    private void SetSplineEnd()
    {
        //var start = animSpline.Spline[0];
        //var end = animSpline.Spline[1];

        //start.Position = (float3)Vector3.zero;
        //end.Position = (float3)new Vector3(UnityEngine.Random.Range(rangeX.x, rangeX.y), UnityEngine.Random.Range(rangeY.x, rangeY.y), 0);

        //animSpline.Spline[0] = start;
        //animSpline.Spline[1] = end;
    }
    private void Animate()
    {
        //float3 position;
        //float3 tangent;
        //float3 upVector;
        //animateValue += Time.deltaTime * animateSpeed;
        //animateRotationValue += Time.deltaTime * animateRotationSpeed;


        //animSpline.Evaluate(animateValue, out position, out tangent, out upVector);
        //this.transform.position = new Vector3(position.x, position.y, position.z);


        //scaleSpring.Update(Time.deltaTime);
        //this.transform.localScale = scaleSpring.Value;
    }
}
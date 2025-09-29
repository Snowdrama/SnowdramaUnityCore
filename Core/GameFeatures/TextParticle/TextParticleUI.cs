using Snowdrama;
using Snowdrama.Core;
using Snowdrama.Spline;
using Snowdrama.Spring;
using System.Xml.Serialization;
using TMPro;
using UnityEditor;
using UnityEngine;

public class PlayTextParticleColorMessage : AMessage<Vector3, Color> { }
public class PlayTextParticleGradientMessage : AMessage<Vector3, Gradient> { }
[ExecuteAlways]
public class TextParticleUI : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float size;
    [SerializeField] private float something;

    [SerializeField] private Spring2D scaleSpring;
    [SerializeField] private Spline splineComponent;
    [SerializeField] private TMP_Text textComponent;
    //[SerializeField] private  animCurve;

    [SerializeField] private Vector2 rangeX;
    [SerializeField] private Vector2 rangeY;

    //animation
    [Header("Animation")]
    [SerializeField] private bool useSizeCurve = true;
    [SerializeField]
    private AnimationCurve sizeCurve = new AnimationCurve(new Keyframe[]
    {
        new Keyframe(0.0f, 1.0f),
        new Keyframe(0.8f, 1.0f),
        new Keyframe(1.0f, 0.0f)
    });
    [SerializeField] private bool useColorGradient = true;
    [SerializeField] private Gradient colorGradient;
    [SerializeField] private Color textColor = Color.white;


    [Header("Debug")]
    [SerializeField] private GameObject targetObject;
    [SerializeField] private Vector2 DebugSpawnRangeX;
    [SerializeField] private Vector2 DebugSpawnRangeY;
    [SerializeField, Range(0, 1)] private float currentTime;
    [SerializeField, EditorReadOnly] private float speed;
    [SerializeField, EditorReadOnly] private float endTime;
    [SerializeField, EditorReadOnly] private bool playing;

    [SerializeField] private bool runDebug = false;
    [SerializeField] private bool runDebugAnimate = false;
    private void Start()
    {
        if (splineComponent == null) { Debug.LogError("Text Particle UI needs SplineComponent"); return; }
        if (textComponent == null) { Debug.LogError("Text Particle UI needs TextComponent"); return; }

        textComponent.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (splineComponent == null) { Debug.LogError("Text Particle UI needs SplineComponent"); return; }
        if (textComponent == null) { Debug.LogError("Text Particle UI needs TextComponent"); return; }


        if (runDebug)
        {
            runDebug = false;
            SetSplineEnd(new Vector2(
                Random.Range(rangeX.x, rangeX.y),
                Random.Range(rangeY.x, rangeY.y)
                ));
        }

        if (runDebugAnimate)
        {
            runDebugAnimate = false;
            if (targetObject != null)
            {
                Play(targetObject.transform.position, textColor);
            }
            else
            {
                var randomX = Random.Range(DebugSpawnRangeX.x, DebugSpawnRangeX.y);
                var randomY = Random.Range(DebugSpawnRangeY.x, DebugSpawnRangeY.y);
                Play(new Vector3(randomX, randomY, 0), textColor, 1.0f, 0.25f);
            }
        }

        if (playing)
        {
            Animate();
        }
    }

    public void Play(Vector3 worldPosition, Gradient color, float size = 1.0f, float duration = 0.25f)
    {
        //first let's convert from world to screen position
        var screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        this.transform.position = screenPos;

        //then let's store all our values
        colorGradient = color;
        useColorGradient = true;


        playing = true;
        currentTime = 0;
        speed = 1 / duration;


        SetSplineEnd(new Vector2(
            Random.Range(rangeX.x, rangeX.y),
            Random.Range(rangeY.x, rangeY.y)
            ));


        textComponent.gameObject.SetActive(true);
    }

    public void Play(Vector3 worldPosition, Color color, float size = 1.0f, float duration = 0.25f)
    {
        //first let's convert from world to screen position
        var screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        this.transform.position = screenPos;

        //then let's store all our values
        textColor = color;
        useColorGradient = false;


        playing = true;
        currentTime = 0;
        speed = 1 / duration;


        SetSplineEnd(new Vector2(
            Random.Range(rangeX.x, rangeX.y),
            Random.Range(rangeY.x, rangeY.y)
            ));


        textComponent.gameObject.SetActive(true);
    }

    private void SetSplineEnd(Vector2 worldPosition)
    {
        var screenPos = Camera.main.WorldToViewportPoint(worldPosition);
        var x = Random.Range(rangeX.x, rangeX.y);
        var y = Random.Range(rangeY.x, rangeY.y);
        splineComponent.transform.position = this.transform.position;
        splineComponent.SetPointPosition(0, screenPos);
        splineComponent.SetPointPosition(1, screenPos + new Vector3(x * 0.5f, y * 1.5f, 0));
        splineComponent.SetPointPosition(2, screenPos + new Vector3(x, y, 0));
    }

    private void Animate()
    {
        if (playing)
        {
            currentTime += Time.deltaTime * speed;
            currentTime = currentTime.Clamp(0, 1);

            textComponent.transform.localScale = Vector2.Lerp(Vector2.zero, Vector2.one * size, sizeCurve.Evaluate(currentTime));
            if (useSizeCurve)
            {
                textComponent.transform.position = splineComponent.GetPosition(currentTime);
            }
            if (useColorGradient)
            {
                textComponent.color = colorGradient.Evaluate(currentTime);
            }
            else
            {
                textComponent.color = textColor;
            }
            if (currentTime >= 1)
            {
                runDebugAnimate = false;
                playing = false;
                textComponent.gameObject.SetActive(false);
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChangeMixerPropertyMessage : AMessage<string, float, float> { }
public class ResetMixerPropertyMessage : AMessage<string, float> { }

//takes a property and then lerps that property at some speed to another.
public class MessagedMixerPropertyChanger : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    [System.Serializable]
    public class MixerProperty
    {
        public float current;
        public float target;
        public float speed;
        public float defaultValue;
        public Vector2 valueRange;
    }

    [SerializeField] private UnitySerializedDictionary<string, MixerProperty> propertyValues = new UnitySerializedDictionary<string, MixerProperty>();

    [Header("Debug")]
    public string debugKey;
    public float debugValue;
    public float debugSpeed;
    public bool debugRun;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        foreach (var key in propertyValues.Keys)
        {
            var value = propertyValues[key];
            value.current = value.defaultValue;
            value.target = value.defaultValue;
            mixer.SetFloat(key, value.current);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //lerp the properties
        foreach (var key in propertyValues.Keys)
        {
            var value = propertyValues[key];
            var diff = Mathf.Abs(value.current - value.target);
            value.current = Mathf.MoveTowards(value.current, value.target, Time.deltaTime * value.speed);

            mixer.SetFloat(key, value.current);
        }

        if (debugRun)
        {
            debugRun = false;
            OnChangeMixerProperty(debugKey, debugValue, debugSpeed);
        }
    }

    public void OnChangeMixerProperty(string key, float value, float lerpTime)
    {
        if (!propertyValues.ContainsKey(key))
        {
            Debug.LogWarning($"[MessagedMixerPropertyChanger.cs]: Mixer does not have property: {key}");
            return;
        }

        var prop = propertyValues[key];
        prop.target = Mathf.Clamp(value, prop.valueRange.x, prop.valueRange.y);
        prop.speed = Mathf.Abs(prop.target - prop.current) * (1 / lerpTime);

    }
    public void OnResetMixerProperty(string key, float lerpTime)
    {
        if (!propertyValues.ContainsKey(key))
        {
            Debug.LogWarning($"[MessagedMixerPropertyChanger.cs]: Mixer does not have property: {key}");
            return;
        }

        var prop = propertyValues[key];
        prop.target = Mathf.Clamp(prop.defaultValue, prop.valueRange.x, prop.valueRange.y);
        prop.speed = Mathf.Abs(prop.target - prop.current) * (1 / lerpTime);
    }

    private void OnValidate()
    {
        //it can't be 0 because of divide by zero
        debugSpeed = debugSpeed.Clamp(0.1f, float.MaxValue);
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(MessagedMixerPropertyChanger.MixerProperty))]
public class MixerPropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // We have 5 fields (current, target, speed, valueRange, defaultValue)
        return EditorGUIUtility.singleLineHeight * 5 + EditorGUIUtility.standardVerticalSpacing * 4;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Find properties
        SerializedProperty current = property.FindPropertyRelative("current");
        SerializedProperty target = property.FindPropertyRelative("target");
        SerializedProperty speed = property.FindPropertyRelative("speed");
        SerializedProperty defaultValue = property.FindPropertyRelative("defaultValue");
        SerializedProperty valueRange = property.FindPropertyRelative("valueRange");

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        Rect currentRect = new Rect(position.x, position.y, position.width, lineHeight);
        Rect targetRect = new Rect(position.x, currentRect.yMax + spacing, position.width, lineHeight);
        Rect speedRect = new Rect(position.x, targetRect.yMax + spacing, position.width, lineHeight);
        Rect rangeRect = new Rect(position.x, speedRect.yMax + spacing, position.width, lineHeight);
        Rect defaultRect = new Rect(position.x, rangeRect.yMax + spacing, position.width, lineHeight);

        // Draw fields
        EditorGUI.PropertyField(currentRect, current);
        EditorGUI.PropertyField(targetRect, target);

        // Draw speed and clamp it
        EditorGUI.PropertyField(speedRect, speed);
        if (speed.floatValue < 0.1f)
            speed.floatValue = 0.1f;

        // Draw Vector2 field for range
        EditorGUI.PropertyField(rangeRect, valueRange);

        // Use range.x and range.y as min/max for defaultValue slider
        Vector2 range = valueRange.vector2Value;
        defaultValue.floatValue = EditorGUI.Slider(defaultRect, "Default Value", defaultValue.floatValue, range.x, range.y);

        EditorGUI.EndProperty();
    }
}

#endif
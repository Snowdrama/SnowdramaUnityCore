using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Layer))]
public class LayerDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty layerProp = property.FindPropertyRelative("m_layer");
        layerProp.intValue = EditorGUI.LayerField(position, label, layerProp.intValue);
    }
}


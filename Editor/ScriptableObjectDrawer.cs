using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ScriptableObject), true)]
public class ScriptableObjectDrawer : PropertyDrawer
{
    private bool foldout;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float totalHeight = EditorGUIUtility.singleLineHeight;

        if (foldout && property.objectReferenceValue != null)
        {
            SerializedObject serializedObject = new SerializedObject(property.objectReferenceValue);
            SerializedProperty iterator = serializedObject.GetIterator();

            iterator.NextVisible(true); // Skip script
            while (iterator.NextVisible(false))
            {
                totalHeight += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        return totalHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Draw object field
        Rect objectRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.BeginProperty(objectRect, label, property);

        property.objectReferenceValue = EditorGUI.ObjectField(objectRect, label, property.objectReferenceValue, fieldInfo.FieldType, false);

        EditorGUI.EndProperty();

        // Draw foldout and inline inspector if assigned
        if (property.objectReferenceValue != null)
        {
            Rect foldoutRect = new Rect(position.x + 15f, position.y, position.width, EditorGUIUtility.singleLineHeight);
            foldout = EditorGUI.Foldout(foldoutRect, foldout, GUIContent.none, true);

            if (foldout)
            {
                EditorGUI.indentLevel++;
                SerializedObject serializedObject = new SerializedObject(property.objectReferenceValue);
                SerializedProperty iterator = serializedObject.GetIterator();

                float y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                iterator.NextVisible(true); // Skip script field
                while (iterator.NextVisible(false))
                {
                    float height = EditorGUI.GetPropertyHeight(iterator, true);
                    Rect propRect = new Rect(position.x, y, position.width, height);

                    EditorGUI.PropertyField(propRect, iterator, true);
                    y += height + EditorGUIUtility.standardVerticalSpacing;
                }

                serializedObject.ApplyModifiedProperties();
                EditorGUI.indentLevel--;
            }
        }
    }
}

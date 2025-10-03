using UnityEngine;
using UnityEditor;
using System.IO;

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
        // Reserve space for the foldout arrow (even if null)
        Rect foldoutRect = new Rect(position.x, position.y, 0f, EditorGUIUtility.singleLineHeight);
        Rect objectRect = new Rect(position.x + 0f, position.y, position.width - 0f, EditorGUIUtility.singleLineHeight);

        EditorGUI.BeginProperty(position, label, property);

        // Foldout only makes sense if we have an object
        if (property.objectReferenceValue != null)
        {
            foldout = EditorGUI.Foldout(foldoutRect, foldout, GUIContent.none, true);
        }

        // Draw the object field shifted over
        property.objectReferenceValue = EditorGUI.ObjectField(
            objectRect,
            label,
            property.objectReferenceValue,
            fieldInfo.FieldType,
            false
        );

        // Add "Create" button if null
        if (property.objectReferenceValue == null)
        {
            Rect buttonRect = new Rect(objectRect.xMax - 60, objectRect.y, 60, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(buttonRect, "Create"))
            {
                ShowCreateMenu(property);
            }
        }

        EditorGUI.EndProperty();

        // Inline editor if expanded
        if (foldout && property.objectReferenceValue != null)
        {
            EditorGUI.indentLevel++;
            SerializedObject serializedObject = new SerializedObject(property.objectReferenceValue);
            SerializedProperty iterator = serializedObject.GetIterator();

            float y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            iterator.NextVisible(true); // skip script
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

    private void ShowCreateMenu(SerializedProperty property)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("New " + fieldInfo.FieldType.Name), false, () =>
        {
            string path = "Assets";
            if (Selection.activeObject != null)
            {
                string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (Directory.Exists(selectedPath))
                    path = selectedPath;
                else if (!string.IsNullOrEmpty(selectedPath))
                    path = Path.GetDirectoryName(selectedPath);
            }

            string assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path, fieldInfo.FieldType.Name + ".asset"));
            ScriptableObject asset = ScriptableObject.CreateInstance(fieldInfo.FieldType);

            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            property.objectReferenceValue = asset;
            property.serializedObject.ApplyModifiedProperties();
            EditorGUIUtility.PingObject(asset);
        });

        menu.ShowAsContext();
    }
}

using UnityEditor;
using UnityEngine;

namespace Snowdrama.CellularAutomata
{
    [CustomEditor(typeof(CellularAutomata))]
    public class CellularAutomataEditor : Editor
    {
        private SerializedProperty processSettingsObject;
        private int stepSlider = -1;

        private void OnEnable()
        {
            processSettingsObject = serializedObject.FindProperty("processSettingsObject");
        }

        public override void OnInspectorGUI()
        {
            CellularAutomata automata = (CellularAutomata)target;

            serializedObject.Update();
            DrawDefaultInspector();

            if (automata.processSettingsObject != null &&
                automata.processSettingsObject.processes != null &&
                automata.processSettingsObject.processes.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Process Debugging", EditorStyles.boldLabel);

                int maxSteps = automata.processSettingsObject.processes.Count;

                // Slider: -1 means "all processes"
                int newStepSlider = EditorGUILayout.IntSlider(
                    new GUIContent("Preview Step"),
                    stepSlider,
                    -1,
                    maxSteps - 1
                );

                if (newStepSlider != stepSlider)
                {
                    stepSlider = newStepSlider;
                    automata.Generate(stepSlider);
                    EditorUtility.SetDirty(automata);

                }

                GUILayout.Space(10);

                if (GUILayout.Button("Generate"))
                {
                    automata.Generate(stepSlider);

                    // Force scene view + inspector refresh
                    EditorUtility.SetDirty(automata);
                }

                GUILayout.Space(10);

                // Draw texture preview
                Texture2D tex = automata.GetOutputTexture();
                if (tex != null)
                {
                    float aspect = (float)tex.width / tex.height;
                    Rect rect = GUILayoutUtility.GetAspectRect(aspect, GUILayout.ExpandWidth(true));
                    EditorGUI.DrawPreviewTexture(rect, tex, null, ScaleMode.ScaleToFit);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

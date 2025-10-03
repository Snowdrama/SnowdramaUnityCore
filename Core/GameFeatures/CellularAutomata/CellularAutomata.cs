using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Snowdrama.CellularAutomata
{
    [ExecuteAlways]
    public class CellularAutomata : MonoBehaviour
    {
        [Header("Settings Object")]
        [Expandable]
        public CellularAutomataSettings processSettingsObject;

        [Header("RNG")]
        [SerializeField] private int seed;
        private int rngSequenceIndex;

        private bool[,] tileData;
        private Texture2D outputTexture;

        private int processChainMax = 3;

        private void Start()
        {
            processChainMax = processSettingsObject.processes.Count;
        }
        public void SetCurrentProcessChainMax(int max)
        {
            processChainMax = max;
        }

        private void OnValidate()
        {
            Generate(processChainMax);
        }

        public void Generate(int maxSteps = -1)
        {
            processChainMax = maxSteps;
            rngSequenceIndex = 0;

            //init all the processes
            foreach (var process in processSettingsObject.processes)
            {
                if (process == null) continue;
                process.Init();
            }

            // start with a cleared array (all false)
            tileData = new bool[processSettingsObject.width, processSettingsObject.height];

            Debug.Log($"Number of steps Requested: {maxSteps}");
            maxSteps = maxSteps.Clamp(-1, processSettingsObject.processes.Count - 1);
            Debug.Log($"Actually doing: {maxSteps}");
            for (int i = 0; i <= maxSteps; i++)
            {
                Debug.Log($"Processing step: {i}");
                var process = processSettingsObject.processes[i];
                if (process == null) continue;
                tileData = process.Process(tileData, seed, ref rngSequenceIndex);
                rngSequenceIndex++;
            }

            outputTexture = GenerateTexture(tileData);
        }

        public bool[,] GetTileData() => tileData;
        public Texture2D GetOutputTexture() => outputTexture;

        public Texture2D GenerateTexture(bool[,] data)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            Texture2D tex = new Texture2D(width, height);
            tex.filterMode = FilterMode.Point;
            Color32[] colors = new Color32[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colors[x + (y * width)] = data[x, y] ? Color.white : Color.black;
                }
            }
            tex.SetPixels32(colors);
            tex.Apply();
            return tex;
        }
    }
}

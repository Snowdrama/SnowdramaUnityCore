using UnityEngine;

namespace Snowdrama.CellularAutomata
{
    [CreateAssetMenu(menuName = "Snowdrama/CellularAutomata/Process/Fill Noise")]
    public class CellularAutomataProcessFill : CellularAutomataProcess
    {
        [Range(0f, 1f)]
        [Tooltip("Chance for a cell to start alive.")]
        public float threshold = 0.5f;

        public override void Init() { }
        public override bool[,] Process(bool[,] data, int seed, ref int rngSequenceIndex)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            bool[,] newData = new bool[width, height];

            System.Random rng = new System.Random(seed);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    newData[x, y] = rng.NextDouble() < threshold;
                }
            }

            return newData;
        }
    }
}
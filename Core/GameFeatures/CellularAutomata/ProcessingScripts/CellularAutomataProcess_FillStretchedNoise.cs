using UnityEngine;

namespace Snowdrama.CellularAutomata
{
    [CreateAssetMenu(menuName = "Snowdrama/CellularAutomata/Process/FillStretched", fileName = "FillStretched")]
    public class CellularAutomataProcess_FillStretchedNoise : CellularAutomataProcess
    {
        [Range(0f, 1f)]
        [Tooltip("Chance for a cell to start alive.")]
        public float threshold = 0.5f;
        [Range(0f, 1f)]
        public float threshold2 = 1f;

        public float stretchAmount = 10f;

        public override void Init() { }
        public override bool[,] Process(bool[,] data, int seed, ref int rngSequenceIndex)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            bool[,] newData = new bool[width, height];

            System.Random rng = new System.Random(seed);
            rngSequenceIndex++;



            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //gives us some size to stretch on the x axis
                    var stretchSize = rng.NextDouble() * stretchAmount;
                    var shouldPlace = rng.NextDouble() < threshold;
                    var isVertical = rng.NextDouble() < 0.5;
                    var isNegative = rng.NextDouble() < 0.5;

                    if (shouldPlace)
                    {
                        if (isVertical)
                        {
                            for (int i = 0; i < stretchSize; i++)
                            {
                                if (isNegative && newData.IndexInBounds(x, y - i))
                                {
                                    newData[x, y - i] = true;
                                }
                                else if (newData.IndexInBounds(x, y + i))
                                {
                                    newData[x, y + i] = true;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < stretchSize; i++)
                            {
                                if (isNegative && newData.IndexInBounds(x - i, y))
                                {
                                    newData[x - i, y] = true;
                                }
                                else if (newData.IndexInBounds(x + i, y))
                                {
                                    newData[x + i, y] = true;
                                }
                            }
                        }
                    }
                }
            }

            return newData;
        }
    }
}
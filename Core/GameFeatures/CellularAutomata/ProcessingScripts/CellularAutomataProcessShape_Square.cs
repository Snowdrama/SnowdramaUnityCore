using UnityEngine;
using UnityEngine.UIElements;

namespace Snowdrama.CellularAutomata
{
    [CreateAssetMenu(menuName = "Snowdrama/CellularAutomata/Process/Shape/Square")]
    public class CellularAutomataProcessShape_Square : CellularAutomataProcess
    {
        [SerializeField] private bool invert = false;
        [SerializeField] private Vector2 size = Vector2.one;

        public override void Init() { }

        public override bool[,] Process(bool[,] data, int seed, ref int rngSequenceIndex)
        {
            int squareWidth = Mathf.RoundToInt(data.GetLength(0) * size.x);
            int squareHeight = Mathf.RoundToInt(data.GetLength(1) * size.y);
            int squareOffsetX = Mathf.RoundToInt((data.GetLength(0) - squareWidth) * 0.5f);
            int squareOffsetY = Mathf.RoundToInt((data.GetLength(1) - squareHeight) * 0.5f);

            for (int y = squareOffsetY; y < squareHeight; y++)
            {
                for (int x = squareOffsetX; x < squareWidth; x++)
                {
                    if (invert)
                    {
                        data[x, y] = false;
                    }
                    else
                    {
                        data[x, y] = true;
                    }
                }
            }

            return data;
        }
    }
}

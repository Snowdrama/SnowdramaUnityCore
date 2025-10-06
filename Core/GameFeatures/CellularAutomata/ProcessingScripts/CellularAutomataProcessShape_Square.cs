using UnityEngine;
using UnityEngine.UIElements;

namespace Snowdrama.CellularAutomata
{
    [CreateAssetMenu(menuName = "CellularAutomata_Shape/Square")]
    public class CellularAutomataProcessShape_Square : CellularAutomataProcess
    {
        [SerializeField] private bool invert = false;
        [SerializeField] private bool border = false;
        [SerializeField] private Vector2 size = Vector2.one;
        public override void Init() { }

        public override bool[,] Process(bool[,] data, int seed, ref int rngSequenceIndex)
        {
            int squareWidth = Mathf.RoundToInt(data.GetLength(0) * size.x);
            int squareHeight = Mathf.RoundToInt(data.GetLength(1) * size.y);
            int squareOffsetX = Mathf.RoundToInt((data.GetLength(0) - squareWidth) * 0.5f);
            int squareOffsetY = Mathf.RoundToInt((data.GetLength(1) - squareHeight) * 0.5f);

            for (int y = 0; y < data.GetLength(1); y++)
            {
                for (int x = 0; x < data.GetLength(0); x++)
                {

                    if (!border)
                    {
                        //if we're not border and inside the square
                        if (x > squareOffsetX && x < squareOffsetX + squareWidth && y > squareOffsetY && y < squareOffsetY + squareHeight)
                        {
                            if (!invert)
                            {
                                data[x, y] = true;
                            }
                            else
                            {
                                data[x, y] = false;
                            }
                        }
                    }
                    else
                    {
                        //if we're border and not inside the square
                        if (x < squareOffsetX || x > squareOffsetX + squareWidth || y < squareOffsetY || y > squareOffsetY + squareHeight)
                        {
                            if (!invert)
                            {
                                data[x, y] = true;
                            }
                            else
                            {
                                data[x, y] = false;
                            }
                        }
                    }

                }
            }

            return data;
        }
    }
}

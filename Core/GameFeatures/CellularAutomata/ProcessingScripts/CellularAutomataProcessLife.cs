using UnityEngine;
namespace Snowdrama.CellularAutomata
{

    [CreateAssetMenu(menuName = "CellularAutomata_Life")]
    public class CellularAutomataProcessLife : CellularAutomataProcess
    {
        public override void Init() { }
        public override bool[,] Process(bool[,] data, int seed, ref int rngSequenceIndex)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            bool[,] newData = new bool[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int neighbors = NeighborCount8Way(data, x, y);
                    if (data[x, y])
                        newData[x, y] = neighbors == 2 || neighbors == 3;
                    else
                        newData[x, y] = neighbors == 3;
                }
            }

            return newData;
        }

        private int NeighborCount8Way(bool[,] data, int x, int y)
        {
            int count = 0;
            for (int yy = -1; yy <= 1; yy++)
            {
                for (int xx = -1; xx <= 1; xx++)
                {
                    if (xx == 0 && yy == 0) continue;
                    int nx = x + xx, ny = y + yy;
                    if (nx >= 0 && nx < data.GetLength(0) &&
                        ny >= 0 && ny < data.GetLength(1) &&
                        data[nx, ny])
                        count++;
                }
            }
            return count;
        }
    }
}
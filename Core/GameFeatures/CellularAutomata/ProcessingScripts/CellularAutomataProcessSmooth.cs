using UnityEngine;
namespace Snowdrama.CellularAutomata
{
    [CreateAssetMenu(menuName = "CellularAutomata_Smooth")]
    public class CellularAutomataProcessSmooth : CellularAutomataProcess
    {
        public int neighborsForLiving = 4;
        public int neighborsForDead = 5;

        public override void Init() { }
        public override bool[,] Process(bool[,] data, int seed, ref int rngSequenceIndex)
        {
            bool[,] newData = new bool[data.GetLength(0), data.GetLength(1)];

            for (int y = 0; y < data.GetLength(1); y++)
            {
                for (int x = 0; x < data.GetLength(0); x++)
                {
                    int count = NeighborCount8Way(data, x, y);
                    if (data[x, y])
                        newData[x, y] = (count >= neighborsForLiving);
                    else
                        newData[x, y] = (count >= neighborsForDead);
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

                    int nx = x + xx;
                    int ny = y + yy;
                    if (nx >= 0 && nx < data.GetLength(0) &&
                        ny >= 0 && ny < data.GetLength(1) &&
                        data[nx, ny])
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
}
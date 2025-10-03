using UnityEngine;
namespace Snowdrama.CellularAutomata
{
    [CreateAssetMenu(menuName = "Snowdrama/CellularAutomata/Process/Decay")]
    public class CellularAutomataProcessDecay : CellularAutomataProcess
    {
        [Tooltip("Alive cell dies if it has fewer than this many neighbors.")]
        public int neighborThreshold = 2;

        public override void Init() { }
        public override bool[,] Process(bool[,] data, int seed, ref int rngSequenceIndex)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            bool[,] newData = (bool[,])data.Clone();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (data[x, y])
                    {
                        int neighbors = NeighborCount8Way(data, x, y);
                        if (neighbors < neighborThreshold)
                            newData[x, y] = false;
                    }
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

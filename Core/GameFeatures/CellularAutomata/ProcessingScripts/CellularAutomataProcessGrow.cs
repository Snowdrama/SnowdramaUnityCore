using UnityEngine;
namespace Snowdrama
{

    [CreateAssetMenu(menuName = "Snowdrama/CellularAutomata/Process/Grow", fileName = "Grow")]
    public class CellularAutomataProcessGrow : CellularAutomataProcess
    {
        [Tooltip("Cell becomes alive if it has at least this many alive neighbors.")]
        public int neighborThreshold = 4;

        public override void Init() { }
        public override bool[,] Process(bool[,] data, int seed, ref int rngSequenceIndex)
        {
            var width = data.GetLength(0);
            var height = data.GetLength(1);
            var newData = (bool[,])data.Clone();

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (!data[x, y])
                    {
                        var neighbors = this.NeighborCount8Way(data, x, y);
                        if (neighbors >= neighborThreshold)
                            newData[x, y] = true;
                    }
                }
            }

            return newData;
        }

        private int NeighborCount8Way(bool[,] data, int x, int y)
        {
            var count = 0;
            for (var yy = -1; yy <= 1; yy++)
            {
                for (var xx = -1; xx <= 1; xx++)
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
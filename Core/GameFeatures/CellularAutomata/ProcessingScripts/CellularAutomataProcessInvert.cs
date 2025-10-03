using UnityEngine;
namespace Snowdrama.CellularAutomata
{
    [CreateAssetMenu(menuName = "Snowdrama/CellularAutomata/Process/Invert")]
    public class CellularAutomataProcessInvert : CellularAutomataProcess
    {
        public override void Init() { }
        public override bool[,] Process(bool[,] data, int seed, ref int rngSequenceIndex)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);

            bool[,] newData = new bool[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    newData[x, y] = !data[x, y];

            return newData;
        }
    }
}
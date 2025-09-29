using Snowdrama.Core;
using UnityEngine;


namespace Snowdrama.CellularAutomata
{
    [CreateAssetMenu(menuName = "Snowdrama/CellularAutomata/Process/Drunken Walk")]
    public class CellularAutomataProcessDrunkenWalk : CellularAutomataProcess
    {
        private static Vector2Int lastPosition;
        private static bool firstWalkCompleted;

        [Tooltip("Steps the drunken walker will take.")]
        public int minSteps = 500;
        public int maxSteps = 1000;

        [Tooltip("By default the walk always starts from the center, " +
            "however if use use 2 walks and this is true, the drunken walk will continue")]
        public bool useLastWalkEnd = false;

        [Tooltip("By default the walk always starts from the center, " +
            "however this will let you choose the percent 0-1 X and Y where to start")]
        public Vector2 walkStartPercent = new Vector2(0.5f, 0.5f);

        public override void Init()
        {
            lastPosition = new Vector2Int(0, 0);
            firstWalkCompleted = false;
        }
        public override bool[,] Process(bool[,] data, int seed, ref int rngSequenceIndex)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);

            // Start in the center
            int x = Mathf.RoundToInt(width * walkStartPercent.x);
            int y = Mathf.RoundToInt(height * walkStartPercent.y);


            if (useLastWalkEnd && !firstWalkCompleted)
            {
                x = lastPosition.x;
                y = lastPosition.y;
            }

            x = Mathf.Clamp(x, 0, width - 1);
            y = Mathf.Clamp(y, 0, height - 1);

            System.Random rng = new System.Random(seed + rngSequenceIndex);

            var steps = rng.Next(minSteps, maxSteps);

            for (int i = 0; i < steps; i++)
            {
                data[x, y] = true;

                int dir = rng.Next(4);
                switch (dir)
                {
                    case 0: x = Mathf.Clamp(x + 1, 0, width - 1); break;
                    case 1: x = Mathf.Clamp(x - 1, 0, width - 1); break;
                    case 2: y = Mathf.Clamp(y + 1, 0, height - 1); break;
                    case 3: y = Mathf.Clamp(y - 1, 0, height - 1); break;
                }
                x = Mathf.Clamp(x, 0, width);
                y = Mathf.Clamp(y, 0, height);
            }
            lastPosition = new Vector2Int(x, y);
            firstWalkCompleted = true;

            return data;
        }
    }

}
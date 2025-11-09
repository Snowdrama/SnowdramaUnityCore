using UnityEngine;
using System.Collections.Generic;

namespace Snowdrama
{
    [CreateAssetMenu(menuName = "Snowdrama/CellularAutomata/Process/Fill/Maze", fileName = "FillMaze")]
    public class CellularAutomataProcess_Maze : CellularAutomataProcess
    {
        [Header("Maze Generation Settings")]
        [Min(1)]
        [Tooltip("Width of carved hallways in cells.")]
        public int hallWidth = 1;

        [Min(1)]
        [Tooltip("Thickness of walls between halls in cells.")]
        public int wallSize = 1;

        [Range(0f, 1f)]
        [Tooltip("0 favors horizontal paths, 1 favors vertical paths, 0.5 is neutral.")]
        public float verticalBias = 0.5f;

        [Tooltip("Adds solid walls around the maze edges.")]
        public bool addBorderWalls = true;

        public override void Init() { }

        public override bool[,] Process(bool[,] data, int seed, ref int rngSequenceIndex)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            bool[,] newData = new bool[width, height];

            // Fill with walls
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    newData[x, y] = false;

            System.Random rng = new System.Random(seed + rngSequenceIndex);
            rngSequenceIndex++;

            int gridStep = hallWidth + wallSize;
            int gridWidth = Mathf.Max(1, Mathf.FloorToInt((float)width / gridStep));
            int gridHeight = Mathf.Max(1, Mathf.FloorToInt((float)height / gridStep));

            bool[,] visited = new bool[gridWidth, gridHeight];

            // Start from a random grid cell
            Vector2Int start = new Vector2Int(rng.Next(gridWidth), rng.Next(gridHeight));
            Stack<Vector2Int> stack = new Stack<Vector2Int>();
            stack.Push(start);
            visited[start.x, start.y] = true;

            while (stack.Count > 0)
            {
                Vector2Int current = stack.Peek();
                List<Vector2Int> directions = GetBiasedDirections(rng);

                bool carved = false;
                foreach (var dir in directions)
                {
                    Vector2Int next = current + dir;
                    if (next.x >= 0 && next.x < gridWidth && next.y >= 0 && next.y < gridHeight && !visited[next.x, next.y])
                    {
                        CarveCorridor(newData, current, next, gridStep, hallWidth);
                        visited[next.x, next.y] = true;
                        stack.Push(next);
                        carved = true;
                        break;
                    }
                }

                if (!carved)
                    stack.Pop();
            }

            if (addBorderWalls)
                AddBorderWalls(newData);

            return newData;
        }

        private void CarveCorridor(bool[,] map, Vector2Int a, Vector2Int b, int step, int hall)
        {
            int ax = a.x * step + step / 2;
            int ay = a.y * step + step / 2;
            int bx = b.x * step + step / 2;
            int by = b.y * step + step / 2;

            int minX = Mathf.Min(ax, bx) - hall / 2;
            int maxX = Mathf.Max(ax, bx) + hall / 2;
            int minY = Mathf.Min(ay, by) - hall / 2;
            int maxY = Mathf.Max(ay, by) + hall / 2;

            for (int y = minY; y <= maxY && y < map.GetLength(1); y++)
            {
                for (int x = minX; x <= maxX && x < map.GetLength(0); x++)
                {
                    if (x >= 0 && y >= 0)
                        map[x, y] = true;
                }
            }
        }

        private List<Vector2Int> GetBiasedDirections(System.Random rng)
        {
            // Directions: N, S, E, W
            List<Vector2Int> dirs = new List<Vector2Int> {
                new Vector2Int(0, -1),
                new Vector2Int(0, 1),
                new Vector2Int(1, 0),
                new Vector2Int(-1, 0)
            };

            // Shuffle randomly
            for (int i = 0; i < dirs.Count; i++)
            {
                int j = rng.Next(i, dirs.Count);
                (dirs[i], dirs[j]) = (dirs[j], dirs[i]);
            }

            // Apply bias: favor vertical or horizontal directions
            dirs.Sort((a, b) =>
            {
                bool aVert = Mathf.Abs(a.y) > 0;
                bool bVert = Mathf.Abs(b.y) > 0;
                if (aVert == bVert) return 0;
                return rng.NextDouble() < verticalBias ? (aVert ? -1 : 1) : (aVert ? 1 : -1);
            });

            return dirs;
        }

        private void AddBorderWalls(bool[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                map[x, 0] = false;
                map[x, height - 1] = false;
            }

            for (int y = 0; y < height; y++)
            {
                map[0, y] = false;
                map[width - 1, y] = false;
            }
        }
    }
}

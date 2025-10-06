using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.CellularAutomata
{
    [CreateAssetMenu(menuName = "CellularAutomata_Filter/LargestBlob")]
    public class CellularAutomataProcessFilter_LargestBlob : CellularAutomataProcess
    {
        [Header("Flood Fill Settings")]
        [Tooltip("If true, diagonals count as connected. If false, only 4-directional neighbors are considered.")]
        public bool useDiagonalConnections = true;

        public override void Init() { }
        public override bool[,] Process(bool[,] data, int seed, ref int rngSequenceIndex)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);

            bool[,] visited = new bool[width, height];
            List<Vector2Int> largestBlob = new List<Vector2Int>();

            // directions for flood fill
            Vector2Int[] directions4 = new Vector2Int[]
            {
            new Vector2Int(1,0), new Vector2Int(-1,0),
            new Vector2Int(0,1), new Vector2Int(0,-1)
            };

            Vector2Int[] directions8 = new Vector2Int[]
            {
            new Vector2Int(1,0), new Vector2Int(-1,0),
            new Vector2Int(0,1), new Vector2Int(0,-1),
            new Vector2Int(1,1), new Vector2Int(-1,-1),
            new Vector2Int(1,-1), new Vector2Int(-1,1)
            };

            Vector2Int[] dirs = useDiagonalConnections ? directions8 : directions4;

            // flood fill each unvisited blob
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!data[x, y] || visited[x, y]) continue;

                    List<Vector2Int> blob = new List<Vector2Int>();
                    Queue<Vector2Int> frontier = new Queue<Vector2Int>();
                    frontier.Enqueue(new Vector2Int(x, y));
                    visited[x, y] = true;

                    while (frontier.Count > 0)
                    {
                        Vector2Int cell = frontier.Dequeue();
                        blob.Add(cell);

                        foreach (var d in dirs)
                        {
                            int nx = cell.x + d.x;
                            int ny = cell.y + d.y;
                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                if (data[nx, ny] && !visited[nx, ny])
                                {
                                    visited[nx, ny] = true;
                                    frontier.Enqueue(new Vector2Int(nx, ny));
                                }
                            }
                        }
                    }

                    // check largest
                    if (blob.Count > largestBlob.Count)
                    {
                        largestBlob = blob;
                    }
                }
            }

            // clear everything
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    data[x, y] = false;

            // restore only largest blob
            foreach (var cell in largestBlob)
                data[cell.x, cell.y] = true;

            return data;
        }
    }
}
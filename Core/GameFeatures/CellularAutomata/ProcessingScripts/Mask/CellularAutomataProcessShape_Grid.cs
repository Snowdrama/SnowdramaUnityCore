using UnityEngine;
namespace Snowdrama
{
    [CreateAssetMenu(menuName = "Snowdrama/CellularAutomata/Process/Shape/Grid", fileName = "Grid")]
    public class CellularAutomataProcessShape_Grid : CellularAutomataProcess
    {
        [Header("Shape Transform")]
        public Vector2 scale = Vector2.one;
        public float aspectRatio = 1.0f;
        public float rotationDegrees = 0f;

        [Tooltip("Spacing between vertical/horizontal lines.")]
        public int spacing = 8;

        [Tooltip("Thickness of the grid lines.")]
        public int lineThickness = 1;

        public override void Init() { }
        public override bool[,] Process(bool[,] data, int seed, ref int rngSequenceIndex)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);

            Vector2 center = new Vector2(width / 2f, height / 2f);
            float rotation = rotationDegrees * Mathf.Deg2Rad;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2 pos = new Vector2(x, y) - center;

                    // rotate
                    float xr = pos.x * Mathf.Cos(rotation) - pos.y * Mathf.Sin(rotation);
                    float yr = pos.x * Mathf.Sin(rotation) + pos.y * Mathf.Cos(rotation);

                    // apply scale & aspect
                    xr /= scale.x * aspectRatio;
                    yr /= scale.y;

                    bool isGridLine = (Mathf.Abs(xr) % spacing < lineThickness) ||
                                      (Mathf.Abs(yr) % spacing < lineThickness);

                    if (!isGridLine)
                        data[x, y] = false;
                }
            }

            return data;
        }
    }
}
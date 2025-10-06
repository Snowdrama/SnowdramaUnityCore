using UnityEngine;
namespace Snowdrama.CellularAutomata
{
    [CreateAssetMenu(menuName = "Snowdrama/CellularAutomata/Process/Shape/Circle", fileName = "Circle")]
    public class CellularAutomataProcessShape_Circle : CellularAutomataProcess
    {
        [Header("Shape Transform")]
        public Vector2 scale = Vector2.one;
        [Tooltip("Multiply X by this to change overall aspect ratio (non-square cells).")]
        public float aspectRatio = 1.0f;
        [Tooltip("Rotation of the circle in degrees (optional for ellipse orientation).")]
        public float rotationDegrees = 0f;

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

                    // apply scale and aspect
                    xr /= (width / 2f * scale.x * aspectRatio);
                    yr /= (height / 2f * scale.y);

                    if (xr * xr + yr * yr > 1f)
                        data[x, y] = false;
                }
            }

            return data;
        }
    }
}
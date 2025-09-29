using UnityEngine;
namespace Snowdrama.CellularAutomata
{
    [CreateAssetMenu(menuName = "Snowdrama/CellularAutomata/Process/Shape/Sun")]
    public class CellularAutomataProcessShape_Sun : CellularAutomataProcess
    {
        [Range(3, 12)]
        public int points = 5;

        [Range(0.1f, 1f)]
        public float innerRadiusFactor = 0.5f;

        public override void Init() { }
        public override bool[,] Process(bool[,] data, int seed, ref int rngSequenceIndex)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            Vector2 center = new Vector2(width / 2f, height / 2f);
            float outerRadius = Mathf.Min(width, height) / 2f;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2 pos = new Vector2(x, y) - center;
                    float angle = Mathf.Atan2(pos.y, pos.x);
                    if (angle < 0) angle += Mathf.PI * 2;

                    float sector = (angle / (Mathf.PI * 2)) * points;
                    int i = Mathf.FloorToInt(sector);

                    float frac = sector - i;
                    float radius = Mathf.Lerp(outerRadius * innerRadiusFactor, outerRadius, frac < 0.5f ? 0 : 1);

                    if (pos.magnitude > radius)
                        data[x, y] = false;
                }
            }
            return data;
        }
    }
}
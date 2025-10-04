using System.Collections.Generic;
using UnityEngine;
namespace Snowdrama.CellularAutomata
{
    [CreateAssetMenu(menuName = "Snowdrama/CellularAutomata/Process/Shape/Star Polygon")]
    public class CellularAutomataProcessShape_Star : CellularAutomataProcess
    {
        [Header("Star Parameters")]
        [Range(3, 12)]
        public int points = 5;

        [Range(0.1f, 1f)]
        public float innerRadiusFactor = 0.5f;

        [Tooltip("Rotation of the star shape in degrees.")]
        public float rotationDegrees = 0f;
        public bool randomRotation;

        [Header("Shape Transform")]
        [Tooltip("Scale applied to generated shapes. (1,1) = full size")]
        public Vector2 scale = Vector2.one;

        [Tooltip("Multiply X by this to change overall aspect ratio (non-square cells).")]
        public float aspectRatio = 1.0f;
        public override void Init() { }

        public override bool[,] Process(bool[,] data, int seed, ref int rngSequenceIndex)
        {
            System.Random rng = new System.Random(seed + rngSequenceIndex);

            int width = data.GetLength(0);
            int height = data.GetLength(1);

            Vector2 center = new Vector2(width / 2f, height / 2f);

            // adjust radii by scale
            float outerRadius = Mathf.Min(width, height) / 2f * Mathf.Min(scale.x, scale.y);

            float rotationRadians = rotationDegrees * Mathf.Deg2Rad;
            if (randomRotation)
            {
                rotationRadians = rng.Next(0, 360) * Mathf.Deg2Rad;
            }


            // build polygon vertices
            List<Vector2> vertices = new List<Vector2>();
            for (int i = 0; i < points * 2; i++)
            {
                float angle = (Mathf.PI * 2f * i) / (points * 2f) + rotationRadians;
                float radius = (i % 2 == 0) ? outerRadius : outerRadius * innerRadiusFactor;
                // apply aspect ratio to X
                Vector2 point = new Vector2(Mathf.Cos(angle) * radius * aspectRatio, Mathf.Sin(angle) * radius);
                vertices.Add(center + point);
            }

            // for each cell, test if inside polygon
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!PointInPolygon(new Vector2(x, y), vertices))
                    {
                        data[x, y] = false;
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Ray-casting point in polygon test.
        /// </summary>
        private bool PointInPolygon(Vector2 p, List<Vector2> polygon)
        {
            bool inside = false;
            for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
            {
                if (((polygon[i].y > p.y) != (polygon[j].y > p.y)) &&
                    (p.x < (polygon[j].x - polygon[i].x) * (p.y - polygon[i].y) /
                    (polygon[j].y - polygon[i].y) + polygon[i].x))
                {
                    inside = !inside;
                }
            }
            return inside;
        }
    }
}
using UnityEngine;

namespace Snowdrama.CellularAutomata
{
    [CreateAssetMenu(menuName = "Snowdrama/CellularAutomata/Process/Fill/StretchedNoise", fileName = "StretchedNoise")]
    public class CellularAutomataProcess_FillStretchedNoise : CellularAutomataProcess
    {
        [Header("Stretched Noise Settings")]
        [Range(0f, 1f)]
        [Tooltip("Overall chance for any given line to be drawn.")]
        public float density = 0.3f;

        [Min(1)]
        [Tooltip("Thickness of each stripe in cells.")]
        public int lineWidth = 2;

        [Min(1)]
        [Tooltip("Maximum streak length in cells.")]
        public int maxLength = 10;

        [Range(0f, 1f)]
        [Tooltip("Random offset chance for streak direction to vary slightly.")]
        public float wobbleChance = 0.1f;

        [Header("Direction Toggles")]
        [Tooltip("If true, allows horizontal streaks.")]
        public bool allowHorizontal = true;

        [Tooltip("If true, allows vertical streaks.")]
        public bool allowVertical = true;

        [Header("Style Options")]
        [Tooltip("If true, streaks gradually fade out in intensity (more gaps near the end).")]
        public bool fadeOut = false;

        [Tooltip("If true, streaks are dotted or dashed rather than continuous.")]
        public bool dotted = false;

        public override void Init() { }

        public override bool[,] Process(bool[,] data, int seed, ref int rngSequenceIndex)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            bool[,] newData = new bool[width, height];

            System.Random rng = new System.Random(seed + rngSequenceIndex);
            rngSequenceIndex++;

            if (!allowHorizontal && !allowVertical)
                return newData; // Nothing to draw

            // The number of attempted line starts
            int numAttempts = Mathf.CeilToInt(width * height * density / 10f);

            for (int i = 0; i < numAttempts; i++)
            {
                int startX = rng.Next(width);
                int startY = rng.Next(height);

                bool horizontal = false;
                if (allowHorizontal && allowVertical)
                    horizontal = rng.NextDouble() < 0.5;
                else if (allowHorizontal)
                    horizontal = true;

                int length = rng.Next(3, maxLength + 1);
                int thickness = Mathf.Max(1, lineWidth);

                DrawStreak(newData, startX, startY, length, thickness, horizontal, rng);
            }

            return newData;
        }

        private void DrawStreak(bool[,] map, int startX, int startY, int length, int thickness, bool horizontal, System.Random rng)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            for (int i = 0; i < length; i++)
            {
                // Dotted pattern: skip some cells periodically
                if (dotted && (i % 2 == 0) && rng.NextDouble() > 0.6)
                    continue;

                // Fade-out effect: higher index means lower chance to draw
                if (fadeOut)
                {
                    float fade = 1f - (float)i / length;
                    if (rng.NextDouble() > fade)
                        continue;
                }

                int x = startX + (horizontal ? i : 0);
                int y = startY + (horizontal ? 0 : i);

                // Occasionally wobble slightly off path
                if (rng.NextDouble() < wobbleChance)
                {
                    x += (horizontal ? 0 : rng.Next(-1, 2));
                    y += (horizontal ? rng.Next(-1, 2) : 0);
                }

                for (int t = -thickness / 2; t <= thickness / 2; t++)
                {
                    int tx = x + (horizontal ? 0 : t);
                    int ty = y + (horizontal ? t : 0);
                    if (tx >= 0 && tx < width && ty >= 0 && ty < height)
                        map[tx, ty] = true;
                }

                if (horizontal && x >= width - 1) break;
                if (!horizontal && y >= height - 1) break;
            }
        }
    }
}

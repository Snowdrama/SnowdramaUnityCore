using UnityEngine;

namespace Snowdrama.CellularAutomata
{
    [CreateAssetMenu(menuName = "Snowdrama/CellularAutomata/Process/StretchedNoise", fileName = "StretchedNoise")]
    public class CellularAutomataProcess_StretchedNoise : CellularAutomataProcess
    {
        [Header("Stretched Noise Settings")]
        [Range(0f, 1f)]
        [Tooltip("Overall chance for any given line to be drawn.")]
        public float density = 0.3f;

        [Min(1)]
        [Tooltip("Base thickness of each stripe in cells.")]
        public int lineWidth = 2;

        [Min(1)]
        [Tooltip("Maximum streak length in cells.")]
        public int maxLength = 10;

        [Range(0f, 1f)]
        [Tooltip("Chance for each step to wobble slightly from a straight line.")]
        public float wobbleChance = 0.1f;

        [Min(0)]
        [Tooltip("Padding around the edges where no streaks will start or be drawn.")]
        public int borderPadding = 2;

        [Header("Direction Toggles")]
        [Tooltip("If true, allows horizontal streaks.")]
        public bool allowHorizontal = true;

        [Tooltip("If true, allows vertical streaks.")]
        public bool allowVertical = true;

        [Header("Style Options")]
        [Tooltip("If true, streaks taper down in width toward their ends.")]
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
                return newData; // nothing to draw

            // Number of potential starting points scaled by density
            int numAttempts = Mathf.CeilToInt(width * height * density / 10f);

            for (int i = 0; i < numAttempts; i++)
            {
                // Pick a start point within padded area
                int startX = rng.Next(borderPadding, Mathf.Max(borderPadding + 1, width - borderPadding));
                int startY = rng.Next(borderPadding, Mathf.Max(borderPadding + 1, height - borderPadding));

                bool horizontal = false;
                if (allowHorizontal && allowVertical)
                    horizontal = rng.NextDouble() < 0.5;
                else if (allowHorizontal)
                    horizontal = true;

                int length = rng.Next(3, maxLength + 1);
                int baseThickness = Mathf.Max(1, lineWidth);

                DrawStreak(newData, startX, startY, length, baseThickness, horizontal, rng);
            }

            return newData;
        }

        private void DrawStreak(bool[,] map, int startX, int startY, int length, int baseThickness, bool horizontal, System.Random rng)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            for (int i = 0; i < length; i++)
            {
                // Dotted pattern: skip some cells periodically
                if (dotted && (i % 2 == 0) && rng.NextDouble() > 0.6)
                    continue;

                // Compute tapering width if fadeOut is enabled
                int currentThickness = baseThickness;
                if (fadeOut)
                {
                    float t = (float)i / (length - 1);
                    currentThickness = Mathf.Max(1, Mathf.RoundToInt(Mathf.Lerp(baseThickness, 1f, t)));
                }

                int x = startX + (horizontal ? i : 0);
                int y = startY + (horizontal ? 0 : i);

                // Stop if near border to avoid out-of-bounds drawing
                if (x < borderPadding || x >= width - borderPadding ||
                    y < borderPadding || y >= height - borderPadding)
                    break;

                // Occasionally wobble slightly
                if (rng.NextDouble() < wobbleChance)
                {
                    x += (horizontal ? 0 : rng.Next(-1, 2));
                    y += (horizontal ? rng.Next(-1, 2) : 0);
                }

                for (int t = -currentThickness / 2; t <= currentThickness / 2; t++)
                {
                    int tx = x + (horizontal ? 0 : t);
                    int ty = y + (horizontal ? t : 0);

                    if (tx < borderPadding || tx >= width - borderPadding ||
                        ty < borderPadding || ty >= height - borderPadding)
                        continue;

                    map[tx, ty] = true;
                }

                if (horizontal && x >= width - borderPadding - 1) break;
                if (!horizontal && y >= height - borderPadding - 1) break;
            }
        }
    }
}

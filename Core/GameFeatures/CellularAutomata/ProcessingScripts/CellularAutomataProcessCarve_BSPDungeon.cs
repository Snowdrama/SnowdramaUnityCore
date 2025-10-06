using Snowdrama.CellularAutomata;
using System.Collections.Generic;
using UnityEngine;
using Snowdrama.Core;


[CreateAssetMenu(menuName = "Snowdrama/CellularAutomata/Process/Dungeon/BSPDungeon", fileName = "BSPDungeon")]
public class CellularAutomataProcessDungeon_BSP : CellularAutomataProcess
{
    [Header("BSP Settings")]
    public int minLeafSize = 20;
    public int maxLeafSize = 40;

    [Header("Room Settings")]
    public int minRoomSize = 6;
    public int maxRoomSize = 20;
    public int roomPadding = 2;

    [Header("Corridor Settings")]
    [Tooltip("Width of carved corridors (in tiles).")]
    [Min(1)] public int corridorWidth = 1;

    [System.Flags]
    public enum CorridorStyle
    {
        Straight = 1 << 0,
        LShape = 1 << 1,
        ZigZag = 1 << 2,
        Jittered = 1 << 3,
        Arc = 1 << 4,
        Wavy = 1 << 5,
    }

    [Header("Enabled Corridor Styles")]
    public CorridorStyle enabledCorridors = CorridorStyle.Straight
                                          | CorridorStyle.LShape
                                          | CorridorStyle.ZigZag
                                          | CorridorStyle.Jittered
                                          | CorridorStyle.Arc
                                          | CorridorStyle.Wavy;

    private class Leaf
    {
        public RectInt rect;
        public Leaf left;
        public Leaf right;
        public RectInt? room;
        public Leaf(RectInt rect) { this.rect = rect; }
        public bool IsLeaf => left == null && right == null;
    }
    public override void Init()
    {

    }
    public override bool[,] Process(bool[,] data, int seed, ref int rngSequenceIndex)
    {
        int width = data.GetLength(0);
        int height = data.GetLength(1);

        // NOTE: no clearing step! we are additive

        Leaf root = new Leaf(new RectInt(0, 0, width, height));
        List<Leaf> leaves = new List<Leaf> { root };

        // Split leaves
        bool didSplit = true;
        while (didSplit)
        {
            didSplit = false;
            List<Leaf> newLeaves = new List<Leaf>();

            foreach (Leaf leaf in leaves)
            {
                if (leaf.left == null && leaf.right == null)
                {
                    if (leaf.rect.width > maxLeafSize || leaf.rect.height > maxLeafSize || Random.value > 0.5f)
                    {
                        if (SplitLeaf(leaf, seed, ref rngSequenceIndex))
                        {
                            newLeaves.Add(leaf.left);
                            newLeaves.Add(leaf.right);
                            didSplit = true;
                        }
                    }
                }
            }
            leaves.AddRange(newLeaves);
        }

        // Create rooms
        foreach (Leaf leaf in leaves)
        {
            if (leaf.IsLeaf)
            {
                leaf.room = CreateRoomInLeaf(leaf, seed, ref rngSequenceIndex, width, height);
                if (leaf.room.HasValue)
                    CarveRect(data, leaf.room.Value); // only sets true
            }
        }

        // Connect recursively
        ConnectLeaves(root, data, seed, ref rngSequenceIndex);

        return data;
    }

    private bool SplitLeaf(Leaf leaf, int seed, ref int rngSequenceIndex)
    {
        if (leaf.left != null || leaf.right != null) return false;

        bool splitH;
        if (leaf.rect.width > leaf.rect.height && leaf.rect.width >= minLeafSize * 2) splitH = false;
        else if (leaf.rect.height >= minLeafSize * 2) splitH = true;
        else return false;

        if (splitH)
        {
            int maxSplit = leaf.rect.height - minLeafSize;
            if (maxSplit <= minLeafSize) return false;

            int splitY = SafeRandomRange(minLeafSize, maxSplit, ref rngSequenceIndex, seed);
            leaf.left = new Leaf(new RectInt(leaf.rect.x, leaf.rect.y, leaf.rect.width, splitY));
            leaf.right = new Leaf(new RectInt(leaf.rect.x, leaf.rect.y + splitY, leaf.rect.width, leaf.rect.height - splitY));
        }
        else
        {
            int maxSplit = leaf.rect.width - minLeafSize;
            if (maxSplit <= minLeafSize) return false;

            int splitX = SafeRandomRange(minLeafSize, maxSplit, ref rngSequenceIndex, seed);
            leaf.left = new Leaf(new RectInt(leaf.rect.x, leaf.rect.y, splitX, leaf.rect.height));
            leaf.right = new Leaf(new RectInt(leaf.rect.x + splitX, leaf.rect.y, leaf.rect.width - splitX, leaf.rect.height));
        }

        return true;
    }

    private RectInt CreateRoomInLeaf(Leaf leaf, int seed, ref int rngSequenceIndex, int mapWidth, int mapHeight)
    {
        RectInt rect = leaf.rect;
        int minX = rect.xMin + roomPadding;
        int minY = rect.yMin + roomPadding;
        int maxX = rect.xMax - roomPadding;
        int maxY = rect.yMax - roomPadding;

        int availW = Mathf.Max(1, maxX - minX);
        int availH = Mathf.Max(1, maxY - minY);

        int roomW = SafeRandomRange(Mathf.Min(minRoomSize, availW), Mathf.Min(maxRoomSize, availW), ref rngSequenceIndex, seed);
        int roomH = SafeRandomRange(Mathf.Min(minRoomSize, availH), Mathf.Min(maxRoomSize, availH), ref rngSequenceIndex, seed);

        int roomX = minX + SafeRandomRange(0, Mathf.Max(1, availW - roomW), ref rngSequenceIndex, seed);
        int roomY = minY + SafeRandomRange(0, Mathf.Max(1, availH - roomH), ref rngSequenceIndex, seed);

        return new RectInt(
            Mathf.Clamp(roomX, 0, mapWidth - 1),
            Mathf.Clamp(roomY, 0, mapHeight - 1),
            Mathf.Clamp(roomW, 1, mapWidth - roomX),
            Mathf.Clamp(roomH, 1, mapHeight - roomY)
        );
    }

    private Vector2Int? ConnectLeaves(Leaf leaf, bool[,] data, int seed, ref int rngSequenceIndex)
    {
        if (leaf.IsLeaf)
        {
            if (leaf.room.HasValue)
            {
                RectInt r = leaf.room.Value;
                return new Vector2Int(r.x + r.width / 2, r.y + r.height / 2);
            }
            return null;
        }

        Vector2Int? leftCenter = ConnectLeaves(leaf.left, data, seed, ref rngSequenceIndex);
        Vector2Int? rightCenter = ConnectLeaves(leaf.right, data, seed, ref rngSequenceIndex);

        if (leftCenter.HasValue && rightCenter.HasValue)
        {
            CarveHallway(data, leftCenter.Value, rightCenter.Value, seed, ref rngSequenceIndex);
            return leftCenter;
        }

        return leftCenter ?? rightCenter;
    }

    private void CarveRect(bool[,] data, RectInt rect)
    {
        for (int y = rect.yMin; y < rect.yMax; y++)
            for (int x = rect.xMin; x < rect.xMax; x++)
                data[x, y] = true; // only additive
    }

    private void CarveHallway(bool[,] data, Vector2Int a, Vector2Int b, int seed, ref int rngSequenceIndex)
    {
        // Collect enabled styles
        List<CorridorStyle> styles = new List<CorridorStyle>();
        foreach (CorridorStyle s in System.Enum.GetValues(typeof(CorridorStyle)))
        {
            if (enabledCorridors.HasFlag(s)) styles.Add(s);
        }

        if (styles.Count == 0) return;

        int index = SafeRandomRange(0, styles.Count, ref rngSequenceIndex, seed);
        CorridorStyle chosen = styles[index];

        switch (chosen)
        {
            case CorridorStyle.Straight: CarveStraight(data, a, b); break;
            case CorridorStyle.LShape: CarveLShape(data, a, b, rngSequenceIndex % 2 == 0); break;
            case CorridorStyle.ZigZag: CarveZigZag(data, a, b, seed, ref rngSequenceIndex); break;
            case CorridorStyle.Jittered: CarveJittered(data, a, b, seed, ref rngSequenceIndex); break;
            case CorridorStyle.Arc: CarveArc(data, a, b); break;
            case CorridorStyle.Wavy: CarveWavy(data, a, b); break;
        }
    }

    // Corridor carving helpers
    private void CarveStraight(bool[,] data, Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(b.x - a.x), sx = a.x < b.x ? 1 : -1;
        int dy = -Mathf.Abs(b.y - a.y), sy = a.y < b.y ? 1 : -1;
        int err = dx + dy, x = a.x, y = a.y;

        while (true)
        {
            CarveCorridorCell(data, x, y);
            if (x == b.x && y == b.y) break;
            int e2 = 2 * err;
            if (e2 >= dy) { err += dy; x += sx; }
            if (e2 <= dx) { err += dx; y += sy; }
        }
    }

    private void CarveArc(bool[,] data, Vector2Int a, Vector2Int b)
    {
        Vector2 center = (Vector2)(a + b) / 2f;
        float radius = Vector2.Distance(a, b) / 2f;
        int steps = Mathf.Max(8, (int)(radius * Mathf.PI));

        for (int i = 0; i <= steps; i++)
        {
            float angle = (Mathf.PI * i) / steps;
            int x = Mathf.RoundToInt(center.x + radius * Mathf.Cos(angle));
            int y = Mathf.RoundToInt(center.y + radius * Mathf.Sin(angle));
            CarveCorridorCell(data, x, y);
        }
    }

    private void CarveWavy(bool[,] data, Vector2Int a, Vector2Int b)
    {
        Vector2 dir = (b - a);
        float length = dir.magnitude;
        Vector2 step = dir / length;

        for (int i = 0; i <= (int)length; i++)
        {
            Vector2 pos = a + step * i;
            float wave = Mathf.Sin(i * 0.5f) * 3f;
            int x = Mathf.RoundToInt(pos.x + wave);
            int y = Mathf.RoundToInt(pos.y);
            CarveCorridorCell(data, x, y);
        }
    }

    private void CarveLShape(bool[,] data, Vector2Int a, Vector2Int b, bool horizontalFirst)
    {
        int x = a.x, y = a.y;
        if (horizontalFirst)
        {
            while (x != b.x) { CarveCorridorCell(data, x, y); x += (b.x > x) ? 1 : -1; }
            while (y != b.y) { CarveCorridorCell(data, x, y); y += (b.y > y) ? 1 : -1; }
        }
        else
        {
            while (y != b.y) { CarveCorridorCell(data, x, y); y += (b.y > y) ? 1 : -1; }
            while (x != b.x) { CarveCorridorCell(data, x, y); x += (b.x > x) ? 1 : -1; }
        }
        CarveCorridorCell(data, b.x, b.y);
    }

    private void CarveZigZag(bool[,] data, Vector2Int a, Vector2Int b, int seed, ref int rngSequenceIndex)
    {
        int midX = SafeRandomRange(Mathf.Min(a.x, b.x), Mathf.Max(a.x, b.x), ref rngSequenceIndex, seed);
        int midY = SafeRandomRange(Mathf.Min(a.y, b.y), Mathf.Max(a.y, b.y), ref rngSequenceIndex, seed);
        Vector2Int mid = new Vector2Int(midX, midY);

        CarveLShape(data, a, mid, rngSequenceIndex % 2 == 0);
        CarveLShape(data, mid, b, rngSequenceIndex % 2 != 0);
    }

    private void CarveJittered(bool[,] data, Vector2Int a, Vector2Int b, int seed, ref int rngSequenceIndex)
    {
        int x = a.x, y = a.y;
        while (x != b.x || y != b.y)
        {
            CarveCorridorCell(data, x, y);
            int dx = (x < b.x) ? 1 : (x > b.x ? -1 : 0);
            int dy = (y < b.y) ? 1 : (y > b.y ? -1 : 0);

            if (SafeRandomRange(0, 100, ref rngSequenceIndex, seed) < 30)
            {
                dx = SafeRandomRange(-1, 2, ref rngSequenceIndex, seed);
                dy = SafeRandomRange(-1, 2, ref rngSequenceIndex, seed);
            }
            x = Mathf.Clamp(x + dx, 0, data.GetLength(0) - 1);
            y = Mathf.Clamp(y + dy, 0, data.GetLength(1) - 1);
        }
        CarveCorridorCell(data, b.x, b.y);
    }

    private void CarveCorridorCell(bool[,] data, int cx, int cy)
    {
        int width = data.GetLength(0);
        int height = data.GetLength(1);
        int half = corridorWidth / 2;

        for (int dy = -half; dy <= half; dy++)
            for (int dx = -half; dx <= half; dx++)
            {
                int nx = cx + dx, ny = cy + dy;
                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                    data[nx, ny] = true; // only additive
            }
    }

    private int SafeRandomRange(int min, int max, ref int rngSequenceIndex, int seed)
    {
        if (max <= min) return min;
        int val = Noise.Squirrel3Range(min, max, rngSequenceIndex, (uint)seed);
        rngSequenceIndex++;
        return val;
    }
}

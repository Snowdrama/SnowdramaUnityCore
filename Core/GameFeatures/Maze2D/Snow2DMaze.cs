using UnityEngine;

[ExecuteAlways]
public class Snow2DMaze : MonoBehaviour
{
    [SerializeField] private int debugScale = 100;
    [SerializeField] private float debugLineWidth = 3.0f;
    [SerializeField] private bool useArrows = false;
    private SnowFillCell2D[,] debugMaze = new SnowFillCell2D[16, 16];
    [SerializeField] private Vector2Int debugMapSize = new Vector2Int(16, 16);
    [SerializeField] private Vector2Int debugRoomSizeMin = new Vector2Int(2, 2);
    [SerializeField] private Vector2Int debugRoomSizeMax = new Vector2Int(5, 5);
    [SerializeField] private Vector2Int debugRoomGapSizeMin = new Vector2Int(1, 1);
    [SerializeField] private Vector2Int debugRoomGapSizeMax = new Vector2Int(2, 2);
    [SerializeField] private Vector2Int debugStartPoint = new Vector2Int(8, 8);
    [SerializeField] private bool generateDebugMaze;
    private System.Diagnostics.Stopwatch benchmarkStopwatch = new System.Diagnostics.Stopwatch();

    [SerializeField] private bool debugShowDeadEnds;
    [SerializeField] private bool drawRoomsOnTop;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {

    }

    public void Update()
    {
        if (generateDebugMaze)
        {
            Debug.Log("Generazing Maze");
            generateDebugMaze = false;
            benchmarkStopwatch.Reset();
            benchmarkStopwatch.Start();

            debugMaze = new SnowFillCell2D[debugMapSize.x, debugMapSize.y];
            SnowMazeGenerator.GenerateMaze(ref debugMaze, debugMapSize, debugStartPoint);

            benchmarkStopwatch.Stop();
            Debug.Log($"Generating took {benchmarkStopwatch.Elapsed}");
        }
        _Draw();
    }

    public void _Draw()
    {
        //DrawCircle(cursorPosition * debugScale, 10, Color.Red, true);
        if (useArrows)
        {
            DrawArrowPaths(Color.grey, Color.black);
        }
        if (!drawRoomsOnTop)
        {
            DrawRooms(Color.blue);
        }
        DrawHalls(Color.yellow);
        if (drawRoomsOnTop)
        {
            DrawRooms(Color.red);
        }
    }

    private void DrawArrowPaths(Color color, Color deadEndColor)
    {
        for (int y = 0; y < debugMaze.GetLength(1); y++)
        {
            for (int x = 0; x < debugMaze.GetLength(0); x++)
            {
                var mazeCell = debugMaze[x, y];
                var from = mazeCell.position * debugScale;
                var to = mazeCell.position * debugScale + mazeCell.direction * debugScale;

                if (mazeCell.removeCell)
                {
                    if (debugShowDeadEnds)
                    {
                        DrawArrow(from, to, deadEndColor, debugLineWidth);
                    }
                }
                else if (mazeCell.isConnected)
                {
                    DrawArrow(from, to, color, debugLineWidth);
                }
            }
        }
    }

    public void DrawRooms(Color color)
    {
        for (int y = 0; y < debugMaze.GetLength(1); y++)
        {
            for (int x = 0; x < debugMaze.GetLength(0); x++)
            {
                var mazeCell = debugMaze[x, y];

                if (!mazeCell.removeCell && mazeCell.isIsPartOfRoom)
                {
                    var cellSize = new Vector2(debugScale, debugScale);
                    var halfSize = cellSize / 2.0f;
                    DrawRect(new Rect((mazeCell.position * debugScale) - halfSize, cellSize), color);
                }
            }
        }
    }
    public void DrawHalls(Color color)
    {
        for (int y = 0; y < debugMaze.GetLength(1); y++)
        {
            for (int x = 0; x < debugMaze.GetLength(0); x++)
            {
                var mazeCell = debugMaze[x, y];

                if (!mazeCell.removeCell && mazeCell.isConnected)
                {
                    var cellSize = new Vector2(debugScale, debugScale);
                    var halfSize = cellSize / 2.0f;
                    DrawRect(new Rect((mazeCell.position * debugScale) - halfSize, cellSize), color);
                }
            }
        }
    }
    private void DrawArrow(Vector2 from, Vector2 to, Color color, float width = 10.0f)
    {
        var direction = (from - to).normalized * 100;


        //var arrowLeft = to + (Quaternion.AngleAxis(160, Vector3.up) * direction.normalized * 10);
        //var arrowRight = to + (direction.normalized * 10).Rotated(Mathf.Deg2Rad * 160);


        DrawLine(from, to, color, width);
        //DrawLine(to, arrowLeft, color, width);
        //DrawLine(to, arrowRight, color, width);
    }

    public void DrawLine(Vector2 from, Vector2 to, Color color, float width)
    {
        DebugDraw.DebugArrow(from - new Vector2(debugScale, debugScale) * 0.5f, to - from, color);
    }

    public void DrawRect(Rect rect, Color color)
    {
        Gizmos.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x, -rect.y));
        DebugDraw.DebugBounds(new Bounds((Vector3)rect.position, (Vector3)rect.size), color);
        //DebugDrawing.DebugArrow(rect.position, rect.position + new Vector2(rect.size.x, 0), color, 10.0f);
        //DebugDrawing.DebugArrow(rect.position, rect.position + new Vector2(0, rect.size.y), color, 10.0f);
        //DebugDrawing.DebugArrow(rect.position + new Vector2(rect.size.x, 0), rect.position + new Vector2(rect.size.x, rect.size.y), color, 10.0f);
        //DebugDrawing.DebugArrow(rect.position + new Vector2(0, rect.size.y), rect.position + new Vector2(rect.size.x, rect.size.y), color, 10.0f);
    }

}

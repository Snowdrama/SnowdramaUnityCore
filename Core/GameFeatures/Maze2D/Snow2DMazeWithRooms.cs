using UnityEngine;

[ExecuteAlways]
public partial class Snow2DMazeWithRooms : MonoBehaviour
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
            GenerateRooms(ref debugMaze, debugRoomSizeMin, debugRoomSizeMax, debugRoomGapSizeMin, debugRoomGapSizeMax);


            for (int i = 0; i < debugMapSize.x * debugMapSize.y; i++)
            {
                RemoveDeadEndRoutine(ref debugMaze);
            }

            benchmarkStopwatch.Stop();
            Debug.Log($"Generating took {benchmarkStopwatch.Elapsed}");
        }
        _Draw();
    }


    public static void GenerateMazeWithRooms(ref SnowFillCell2D[,] maze, Vector2Int mapSize, Vector2Int centerPoint, Vector2Int roomSizeSettingMin, Vector2Int roomSizeSettingMax, Vector2Int roomGapSizeSettingMin, Vector2Int roomGapSizeSettingMax)
    {
        SnowMazeGenerator.GenerateMaze(ref maze, mapSize, centerPoint);
        GenerateRooms(ref maze, roomSizeSettingMin, roomSizeSettingMax, roomGapSizeSettingMin, roomGapSizeSettingMax);
        for (int i = 0; i < mapSize.x * mapSize.y; i++)
        {
            RemoveDeadEndRoutine(ref maze);
        }
        CheckRoomConnections(ref maze, mapSize);
    }
    public static void GenerateRooms(ref SnowFillCell2D[,] map, Vector2Int roomSizeSettingMin, Vector2Int roomSizeSettingMax, Vector2Int roomGapSizeSettingMin, Vector2Int roomGapSizeSettingMax)
    {
        System.Random rand = new System.Random();
        int stepCount = 0;
        int roomIndex = 0;
        while (stepCount < 1000)
        {
            Vector2Int roomPosition = new Vector2Int(
                rand.Next(0, map.GetLength(0)),
                rand.Next(0, map.GetLength(1))
                );
            Vector2Int roomSize = new Vector2Int(
                rand.Next(roomSizeSettingMin.x, roomSizeSettingMax.x),
                rand.Next(roomSizeSettingMin.y, roomSizeSettingMax.y)
                );
            Vector2Int roomGap = new Vector2Int(
                rand.Next(roomGapSizeSettingMin.x, roomGapSizeSettingMax.x),
                rand.Next(roomGapSizeSettingMin.y, roomGapSizeSettingMax.y)
                );
            if (IsRoomInBound(map, roomPosition, roomSize))
            {
                if (!OverlappingRoom(map, roomPosition, roomSize))
                {
                    Debug.Log($"Creating room {roomIndex} at: {roomPosition} sized: {roomSize}");
                    SetRoom(ref map, roomPosition, roomSize, roomGap, roomIndex);
                    SetRoomConnections(ref map, roomPosition, roomSize);
                    roomIndex++;

                }
            }
            stepCount++;
        }

    }
    private static void SetRoom(ref SnowFillCell2D[,] map, Vector2Int roomPosition, Vector2Int roomSize, Vector2Int gapSetting, int roomId)
    {
        for (int y = -gapSetting.y; y < roomSize.y + gapSetting.y; y++)
        {
            for (int x = -gapSetting.x; x < roomSize.x + gapSetting.x; x++)
            {
                var pos = roomPosition + new Vector2Int(x, y);
                if (IsPointInBounds(map, pos))
                {
                    map[pos.x, pos.y].isRoomGap = true;
                }
            }
        }

        for (int y = 0; y < roomSize.y; y++)
        {
            for (int x = 0; x < roomSize.x; x++)
            {
                var pos = roomPosition + new Vector2Int(x, y);
                if (IsPointInBounds(map, pos))
                {
                    map[pos.x, pos.y].isIsPartOfRoom = true;
                    map[pos.x, pos.y].roomId = roomId;
                }
            }
        }

    }

    public static void SetRoomConnections(ref SnowFillCell2D[,] map, Vector2Int roomPosition, Vector2Int roomSize)
    {
        //mark the segments that are part of the room as being room connections
        for (int y = 0; y < roomSize.y; y++)
        {
            for (int x = 0; x < roomSize.x; x++)
            {
                var pos = roomPosition + new Vector2Int(x, y);
                var left = pos + Vector2Int.left;
                var right = pos + Vector2Int.right;
                var up = pos + Vector2Int.up;
                var down = pos + Vector2Int.down;

                if (IsPointInBounds(map, left) && map[left.x, left.y].isIsPartOfRoom)
                {
                    map[pos.x, pos.y].connectedDirections[Vector2Int.left] = ConnectedDirectionType.Room;
                }

                if (IsPointInBounds(map, right) && map[right.x, right.y].isIsPartOfRoom)
                {
                    map[pos.x, pos.y].connectedDirections[Vector2Int.right] = ConnectedDirectionType.Room;
                }

                if (IsPointInBounds(map, up) && map[up.x, up.y].isIsPartOfRoom)
                {
                    map[pos.x, pos.y].connectedDirections[Vector2Int.up] = ConnectedDirectionType.Room;
                }

                if (IsPointInBounds(map, down) && map[down.x, down.y].isIsPartOfRoom)
                {
                    map[pos.x, pos.y].connectedDirections[Vector2Int.down] = ConnectedDirectionType.Room;
                }
            }
        }
    }
    public static void CheckRoomConnections(ref SnowFillCell2D[,] map, Vector2Int mapSize)
    {

        //mark the segments that are part of the room as being room connections
        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                var pos = new Vector2Int(x, y);
                var left = pos + Vector2Int.left;
                var right = pos + Vector2Int.right;
                var up = pos + Vector2Int.up;
                var down = pos + Vector2Int.down;

                if (IsPointInBounds(map, left) && IsPointRemoved(map, left))
                {
                    map[pos.x, pos.y].connectedDirections.Remove(Vector2Int.left);
                }

                if (IsPointInBounds(map, right) && IsPointRemoved(map, right))
                {
                    map[pos.x, pos.y].connectedDirections.Remove(Vector2Int.right);
                }

                if (IsPointInBounds(map, up) && IsPointRemoved(map, up))
                {
                    map[pos.x, pos.y].connectedDirections.Remove(Vector2Int.up);
                }

                if (IsPointInBounds(map, down) && IsPointRemoved(map, down))
                {
                    map[pos.x, pos.y].connectedDirections.Remove(Vector2Int.down);
                }
            }
        }
    }
    private static bool OverlappingRoom(SnowFillCell2D[,] map, Vector2Int roomPosition, Vector2Int roomSize)
    {
        for (int y = 0; y < roomSize.y; y++)
        {
            for (int x = 0; x < roomSize.x; x++)
            {
                var offset = new Vector2Int(x, y);
                var pos = roomPosition + offset;
                if (map[pos.x, pos.y].isIsPartOfRoom || map[pos.x, pos.y].isRoomGap)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static bool IsRoomInBound(SnowFillCell2D[,] map, Vector2Int roomPosition, Vector2Int roomSize)
    {
        if (
            roomPosition.x < 0 ||
            roomPosition.y < 0 ||
            roomPosition.x + roomSize.x > map.GetLength(0) ||
            roomPosition.y + roomSize.y > map.GetLength(1)
        )
        {
            return false;
        }
        return true;
    }

    private static bool IsPointRemoved(SnowFillCell2D[,] map, Vector2Int point)
    {
        if (map[point.x, point.y].removeCell)
        {
            return true;
        }
        return false;
    }
    private static bool IsPointInBounds(SnowFillCell2D[,] map, Vector2Int point)
    {
        if (point.x < 0 || point.y < 0 || point.x >= map.GetLength(0) || point.y >= map.GetLength(1))
        {
            return false;
        }
        return true;
    }

    private static void RemoveDeadEndRoutine(ref SnowFillCell2D[,] map)
    {
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                var currentPosition = new Vector2Int(x, y);

                if (!map[currentPosition.x, currentPosition.y].isIsPartOfRoom)
                {
                    var down = new Vector2Int(0, 1);
                    var up = new Vector2Int(0, -1);
                    var right = new Vector2Int(1, 0);
                    var left = new Vector2Int(-1, 0);

                    var downCell = currentPosition + down;
                    var upCell = currentPosition + up;
                    var leftCell = currentPosition + left;
                    var rightCell = currentPosition + right;

                    var upConnects = CheckConnection(map, upCell, down);
                    var downConnects = CheckConnection(map, downCell, up);
                    var leftConnects = CheckConnection(map, leftCell, right);
                    var rightConnects = CheckConnection(map, rightCell, left);
                    if (!upConnects && !downConnects && !leftConnects && !rightConnects)
                    {
                        //nothing points to us!
                        map[currentPosition.x, currentPosition.y].removeCell = true;
                    }
                }
            }
        }
    }


    private static bool CheckConnection(SnowFillCell2D[,] map, Vector2Int testCell, Vector2Int testDirection)
    {
        if (
            //make sure it's in bounds
            IsPointInBounds(map, testCell) &&
            //if the cell isn't removed
            !map[testCell.x, testCell.y].removeCell &&
            //if it's pointing towards us
            map[testCell.x, testCell.y].direction == testDirection
        )
        {
            return true;
        }
        return false;
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
        DrawHalls(new Color(1.0f, 0.0f, 0.8f));
        if (drawRoomsOnTop)
        {
            DrawRooms(new Color(1.0f, 0.6f, 0.0f));
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
        DebugDraw.DebugBounds(new Bounds((Vector3)rect.position, (Vector3)rect.size), color);
        //DebugDraw.DebugArrow(rect.position, rect.position + new Vector2(rect.size.x, 0), color, 10.0f);
        //DebugDraw.DebugArrow(rect.position, rect.position + new Vector2(0, rect.size.y), color, 10.0f);
        //DebugDraw.DebugArrow(rect.position + new Vector2(rect.size.x, 0), rect.position + new Vector2(rect.size.x, rect.size.y), color, 10.0f);
        //DebugDraw.DebugArrow(rect.position + new Vector2(0, rect.size.y), rect.position + new Vector2(rect.size.x, rect.size.y), color, 10.0f);
    }
}

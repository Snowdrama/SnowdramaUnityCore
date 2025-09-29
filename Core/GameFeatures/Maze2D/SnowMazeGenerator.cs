using UnityEngine;
using System.Collections.Generic;

public class SnowMazeGenerator
{
    private static Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
    };

    public static readonly Vector2Int DOWN = new Vector2Int(0, -1);
    public static readonly Vector2Int UP = new Vector2Int(0, 1);
    public static readonly Vector2Int RIGHT = new Vector2Int(1, 0);
    public static readonly Vector2Int LEFT = new Vector2Int(-1, 0);

    public static void GenerateMaze(ref SnowFillCell2D[,] maze, Vector2Int size, Vector2Int startPoint)
    {
        maze = new SnowFillCell2D[size.x, size.y];

        var filledCount = 0;
        var cellCount = size.x * size.y;

        CreateMaze(ref maze, size, startPoint);
        FloodFillRoutine(ref maze);

        var stepCount = 0;
        var targetStepCount = 1000;

        while (stepCount < targetStepCount && filledCount < cellCount)
        {
            RandomizeDisconnectedCells(ref maze);
            FloodFillRoutine(ref maze);
            filledCount = CountFilled(maze);
            if (filledCount >= cellCount)
            {
                break;
            }
            stepCount++;
        }

        //finally hook up all directions

        for (int y = 0; y < maze.GetLength(1); y++)
        {
            for (int x = 0; x < maze.GetLength(0); x++)
            {
                Vector2Int currentPosition = new Vector2Int(x, y);
                bool connecedUp = CheckValidPathInDirection(maze, currentPosition, UP);
                bool connecedDown = CheckValidPathInDirection(maze, currentPosition, DOWN);
                bool connecedLeft = CheckValidPathInDirection(maze, currentPosition, LEFT);
                bool connecedRight = CheckValidPathInDirection(maze, currentPosition, RIGHT);

                if (connecedUp)
                {
                    maze[currentPosition.x, currentPosition.y].connectedDirections.Add(UP, ConnectedDirectionType.Hall);
                }
                if (connecedDown)
                {
                    maze[currentPosition.x, currentPosition.y].connectedDirections.Add(DOWN, ConnectedDirectionType.Hall);
                }
                if (connecedLeft)
                {
                    maze[currentPosition.x, currentPosition.y].connectedDirections.Add(LEFT, ConnectedDirectionType.Hall);
                }
                if (connecedRight)
                {
                    maze[currentPosition.x, currentPosition.y].connectedDirections.Add(RIGHT, ConnectedDirectionType.Hall);
                }
            }
        }


        Debug.Log($"Finshed in {stepCount} steps");
    }

    private static void CreateMaze(ref SnowFillCell2D[,] map, Vector2Int size, Vector2Int start)
    {
        map = new SnowFillCell2D[size.x, size.y];
        Debug.Log($"Starting Maze of mapSize {new Vector2(map.GetLength(0), map.GetLength(1))} ");
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                map[x, y].connectedDirections = new ConnectedDirectionDictionary();
                map[x, y].extraProperties_int = new Dictionary<string, int>();
                map[x, y].extraProperties_float = new Dictionary<string, float>();
                map[x, y].extraProperties_bool = new Dictionary<string, bool>();
                map[x, y].extraProperties_string = new Dictionary<string, string>();

                map[x, y].position = new Vector2Int(x, y);
                map[x, y].direction = directions.GetRandom();
                if (map[x, y].position == start)
                {
                    map[x, y].isConnected = true;
                }
                else
                {
                    map[x, y].isConnected = false;
                }
            }
        }
    }

    private static void RandomizeDisconnectedCells(ref SnowFillCell2D[,] map)
    {
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                if (map[x, y].isConnected == false)
                {
                    map[x, y].direction = directions.GetRandom();
                }
            }
        }
    }

    private static void FloodFillRoutine(ref SnowFillCell2D[,] map)
    {
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                //we flood fill from each cell, if the cell isn't connected
                //we escape early anyway
                FloodFill(map, new Vector2Int(x, y), 0);
            }
        }
    }


    private static void FloodFill(SnowFillCell2D[,] map, Vector2Int currentPosition, int depth)
    {
        //prevent going super deep
        //TODO: This really should be like mapSize.x * mapSize.y?
        if (depth > 100)
        {
            return;
        }

        //don't check neighbors if we're not connected, we'll check when we get to a connected node
        if (!map[currentPosition.x, currentPosition.y].isConnected)
        {
            return;
        }

        //check each direction and if it points to me, then we should mark as connected
        var down = new Vector2Int(0, 1);
        var up = new Vector2Int(0, -1);
        var right = new Vector2Int(1, 0);
        var left = new Vector2Int(-1, 0);

        var downCell = currentPosition + down;
        var upCell = currentPosition + up;
        var leftCell = currentPosition + left;
        var rightCell = currentPosition + right;

        //check if the nodes near me are pointing to me
        CheckConnection(map, upCell, down, depth);
        CheckConnection(map, downCell, up, depth);
        CheckConnection(map, leftCell, right, depth);
        CheckConnection(map, rightCell, left, depth);
    }

    private static void CheckConnection(SnowFillCell2D[,] map, Vector2Int testCell, Vector2Int testDirection, int depth)
    {
        if (IsInBounds(map, testCell) && !map[testCell.x, testCell.y].isConnected && map[testCell.x, testCell.y].direction == testDirection)
        {
            map[testCell.x, testCell.y].isConnected = true;
            FloodFill(map, testCell, depth++);
        }
    }

    private static bool CheckValidPathInDirection(SnowFillCell2D[,] map, Vector2Int testCell, Vector2Int testDirection)
    {
        //are we both in bounds?
        if (IsInBounds(map, testCell) && IsInBounds(map, testCell + testDirection))
        {
            //do we point to them?
            if (map[testCell.x, testCell.y].direction == testDirection)
            {
                return true;
            }
            //or do they point to us
            if (map[testCell.x + testDirection.x, testCell.y + testDirection.y].direction == -testDirection)
            {
                return true;
            }
        }

        return false;
    }

    private static int CountFilled(SnowFillCell2D[,] map)
    {
        int count = 0;
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                if (map[x, y].isConnected)
                {
                    count++;
                }
            }
        }
        return count;
    }

    private static bool IsInBounds(SnowFillCell2D[,] map, Vector2Int point)
    {
        if (point.x < 0 || point.y < 0 || point.x >= map.GetLength(0) || point.y >= map.GetLength(1))
        {
            return false;
        }
        return true;
    }

}

[System.Serializable]
public struct SnowFillCell2D
{
    public bool removeCell;

    public Vector2Int position;
    public bool isConnected;
    public Vector2Int direction;


    public bool isIsPartOfRoom;
    public bool isRoomGap;
    public int roomId;

    public ConnectedDirectionDictionary connectedDirections;
    //public List<Vector2Int> connectedDirections;

    public Dictionary<string, int> extraProperties_int;
    public Dictionary<string, float> extraProperties_float;
    public Dictionary<string, bool> extraProperties_bool;
    public Dictionary<string, string> extraProperties_string;
}

[System.Serializable]
public class ConnectedDirectionDictionary : UnityDictionary<Vector2Int, ConnectedDirectionType> { }

[System.Serializable]
public enum ConnectedDirectionType
{
    Hall,
    Room,
}
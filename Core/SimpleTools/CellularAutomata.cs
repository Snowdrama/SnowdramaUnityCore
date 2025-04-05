using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Snowdrama.Core
{
    public enum CellularAutomataProcessType
    {
        Grow,
        Decay,
        Smooth,
        Contiguous,
        Sharpen,
        Shape_Circle,
        Shape_Cross,
        Invert,
        Life,
        Edge,
        MinSize,
        DrunkenWalk_Rad1,
        DrunkenWalk_Rad2,
        DrunkenWalk_Rad3,
    }
    [ExecuteAlways]
    public class CellularAutomata : MonoBehaviour
    {
        [Header("Settings Object")]
        [Expandable]
        public CellularAutomataSettings processSettingsObject;

        [Button(nameof(LoadSettings))]
        public bool loadSettings;
        [Button(nameof(SaveSettings))]
        public bool saveSettings;

        [Header("RNG")]
        [SerializeField] private int seed;
        private System.Random random;
        private int rngSequenceIndex;
        [Header("Size")]
        [SerializeField] private int width = 128;
        [SerializeField] private int height = 128;
        [Header("Threshold")]
        [SerializeField, Range(0,1)] private float threshold = 0.5f;
        [Tooltip("When dealing with the walls of space does it " +
            "count as walls when factoring in neighbors" +
            "(try smoothing with this on and off)")]
        [SerializeField] private bool gridWallsCountAsSolid = true;
        [SerializeField] private List<CellularAutomataProcessType> processes;


        private bool[,] tileData;
        private Texture2D outputTexture;

        [Button(nameof(Generate))]
        public bool generate;

        public void LoadSettings()
        {
            if (processSettingsObject != null)
            {
                width = processSettingsObject.width;
                height = processSettingsObject.height;
                gridWallsCountAsSolid = processSettingsObject.gridWallsCountAsSolid;
                threshold = processSettingsObject.threshold;
                processes = new List<CellularAutomataProcessType>(processSettingsObject.processes);
            }
        }
        public void SaveSettings()
        {
            if (processSettingsObject != null)
            {
                 processSettingsObject.width = width;
                 processSettingsObject.height = height;
                 processSettingsObject.gridWallsCountAsSolid = gridWallsCountAsSolid;
                 processSettingsObject.threshold = threshold;
                 processSettingsObject.processes = new List<CellularAutomataProcessType>(processes); ;
            }
        }
        public void Update()
        {
            if (processSettingsObject != null)
            {
                if (width != processSettingsObject.width ||
                    height != processSettingsObject.height ||
                    gridWallsCountAsSolid != processSettingsObject.gridWallsCountAsSolid ||
                    threshold != processSettingsObject.threshold ||
                    processes != processSettingsObject.processes)
                {

                    Generate();
                }
            }
        }
        private void OnValidate()
        {
            Generate();
        }
        public void Generate()
        {
            rngSequenceIndex = 0;
            tileData = GenerateRandomNoise(width, height, threshold);

            foreach (var process in processes)
            {
                switch (process)
                {
                    case CellularAutomataProcessType.Smooth:
                        tileData = Smooth(tileData, 4, 5);
                        break;
                    case CellularAutomataProcessType.Sharpen:
                        tileData = Sharpen(tileData);
                        break;
                    case CellularAutomataProcessType.Contiguous:
                        tileData = Contiguous(tileData);
                        break;
                    case CellularAutomataProcessType.Shape_Circle:
                        tileData = ShapeCircle(tileData);
                        break;
                    case CellularAutomataProcessType.Shape_Cross:
                        tileData = ShapeCross(tileData, width / 3, height / 3);
                        break;
                    case CellularAutomataProcessType.Grow:
                        tileData = Grow(tileData);
                        break;
                    case CellularAutomataProcessType.Decay:
                        tileData = Decay(tileData);
                        break;
                    case CellularAutomataProcessType.Invert:
                        tileData = Invert(tileData);
                        break;
                    case CellularAutomataProcessType.Life:
                        tileData = Life(tileData);
                        break;
                    case CellularAutomataProcessType.Edge:
                        tileData = Edge(tileData);
                        break;
                    case CellularAutomataProcessType.DrunkenWalk_Rad1:
                        tileData = DrunkenWalkCore(tileData, 1);
                        break;
                    case CellularAutomataProcessType.DrunkenWalk_Rad2:
                        tileData = DrunkenWalkCore(tileData, 2);
                        break;
                    case CellularAutomataProcessType.DrunkenWalk_Rad3:
                        tileData = DrunkenWalkCore(tileData, 3);
                        break;
                }
            }
            outputTexture = GenerateTexture(tileData);
        }

        bool[,] Invert(bool[,] data)
        {
            for (int y = 0; y < data.GetLength(1); y++)
            {
                for (int x = 0; x < data.GetLength(0); x++)
                {
                    data[x, y] = !data[x, y];
                }
            }
            return data;
        }

        bool[,] DrunkenWalkCore(bool[,] data, int carveRadius)
        {
            int randomX = Noise.Squirrel3Range(0, width, rngSequenceIndex, (uint)seed);
            rngSequenceIndex++;
            int randomY = Noise.Squirrel3Range(0, height, rngSequenceIndex, (uint)seed);
            rngSequenceIndex++;
            int walkLength = Noise.Squirrel3Range(100, 1000, rngSequenceIndex, (uint)seed);
            rngSequenceIndex++;

            int x = randomX;
            int y = randomY;
            int direction = 0;

            for (int i = 0; i < walkLength; i++)
            {
                uint rotateDirection = Noise.Squirrel3(rngSequenceIndex, (uint)seed);
                rngSequenceIndex++;
                //pick a random direction
                switch (rotateDirection % 2)
                {
                    case 0: 
                        direction += 1;
                        direction %= 4;
                        break;
                    case 1: //E
                        direction += 3;
                        direction %= 4;
                        break;
                }

                //try to move in that direction 
                int nextX;
                int nextY;
                do
                {
                    nextX = x;
                    nextY = y;
                    switch (direction)
                    {
                        case 0:
                            nextY++;
                            break;
                        case 1:
                            nextX++;
                            break;
                        case 2:
                            nextY--;
                            break;
                        case 3:
                            nextX--;
                            break;
                    }

                    //if the new location is off grid, we need to turn and try again
                    if (OffGrid(data, nextX, nextY))
                    {
                        //turn right
                        direction++;
                        direction %= 4;
                    }
                } while (OffGrid(data, nextX, nextY)); 
                
                x = nextX;
                y = nextY;

                for (int carveY = -carveRadius; carveY < carveRadius; carveY++)
                {
                    for (int carveX = -carveRadius; carveX < carveRadius; carveX++)
                    {
                        if (carveX + carveY < carveRadius)
                        {
                            int carveTestX = x + carveX;
                            int carveTestY = y + carveY;

                            if (!OffGrid(data, carveTestX, carveTestY))
                            {
                                data[carveTestX, carveTestY] = false;
                            }
                        }
                    }
                }

            }
            return data;
        }


        bool[,] Contiguous(bool[,] data)
        {
            int[,] sections = new int[data.GetLength(0), data.GetLength(1)];
            int currentIndex = 0;
            for (int y = 0; y < data.GetLength(1); y++)
            {
                for (int x = 0; x < data.GetLength(0); x++)
                {
                    sections = FloodFill(data, sections, x, y, currentIndex);
                    currentIndex++;
                }
            }

            //if there's 0 sections found, or only 1 it's already contigous
            if (currentIndex == 0 || currentIndex == 1) return data;

            int[] sectionCount = new int[currentIndex + 1];
            for (int y = 0; y < data.GetLength(1); y++)
            {
                for (int x = 0; x < data.GetLength(0); x++)
                {
                    var sectionIndex = sections[x, y];
                    if (sectionIndex >= 0)
                    {
                        sectionCount[sectionIndex]++;
                    }
                }
            }
            //if there's no sections then... I guess it's already contiguous
            if (sectionCount.Length == 0) return data;

            int largestSectionIndex = 1;

            //we ignore section 0 since that's empty space
            for (int i = 1; i < sectionCount.Length; i++)
            {
                if (sectionCount[largestSectionIndex] < sectionCount[i])
                {
                    largestSectionIndex = i;
                }
            }

            //NOW we finally modify the data

            for (int y = 0; y < data.GetLength(1); y++)
            {
                for (int x = 0; x < data.GetLength(0); x++)
                {
                    if (sections[x, y] == largestSectionIndex)
                    {
                        data[x, y] = data[x, y];
                    }
                    else
                    {
                        data[x, y] = false;
                    }
                }
            }

            return data;
        }

        int[,] FloodFill(bool[,] data, int[,] sections, int x, int y, int index)
        {
            Queue<Vector2Int> stack = new Queue<Vector2Int>();

            if (!OffGrid(data, x, y) && data[x, y] && sections[x, y] == 0)
            {
                stack.Enqueue(new Vector2Int(x, y));
            }

            int hardExit = 0;
            while (stack.Count > 0 && hardExit <= data.GetLength(0) * data.GetLength(1))
            {
                hardExit++;
                var pos = stack.Dequeue();

                sections[pos.x, pos.y] = index;

                var north = pos + new Vector2Int(0, 1);
                var south = pos + new Vector2Int(0, -1);
                var east = pos + new Vector2Int(1, 0);
                var west = pos + new Vector2Int(-1, 0);

                if (!stack.Contains(north) && !OffGrid(data, north.x, north.y) && data[north.x, north.y] && sections[north.x, north.y] == 0)
                {
                    stack.Enqueue(north);
                }
                if (!stack.Contains(south) && !OffGrid(data, south.x, south.y) && data[south.x, south.y] && sections[south.x, south.y] == 0)
                {
                    stack.Enqueue(south);
                }
                if (!stack.Contains(east) && !OffGrid(data, east.x, east.y) && data[east.x, east.y] && sections[east.x, east.y] == 0)
                {
                    stack.Enqueue(east);
                }
                if (!stack.Contains(west) && !OffGrid(data, west.x, west.y) && data[west.x, west.y] && sections[west.x, west.y] == 0)
                {
                    stack.Enqueue(west);
                }
            }
            return sections;
        }

        void DebugSections(int[,] sections)
        {
            //have we vistied this cell yet?
            string debug = "";
            for (int debugY = 0; debugY < sections.GetLength(1); debugY++)
            {
                for (int debugX = 0; debugX < sections.GetLength(0); debugX++)
                {
                    debug += $"{sections[debugX, debugY]}";
                }
                debug += "\n";
            }

            Debug.Log(debug);
        }

        bool[,] Edge(bool[,] data)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            for (int y = 0; y < data.GetLength(1); y++)
            {
                for (int x = 0; x < data.GetLength(0); x++)
                {
                    if (x < 5 || y < 5 || x > width - 5 || y > height - 5)
                    {
                        data[x, y] = true;
                    }
                    else
                    {
                        data[x, y] = data[x, y];
                    }
                }
            }
            return data;
        }


        bool[,] GenerateRandomNoise(int width, int height, float threshold)
        {
            bool[,] newNoise = new bool[width, height];

            for (int y = 0; y < newNoise.GetLength(1); y++)
            {
                for (int x = 0; x < newNoise.GetLength(0); x++)
                {
                    float value = Noise.NormalizedSquirrel3(rngSequenceIndex, (uint)seed);
                    rngSequenceIndex++;
                    if (value > threshold)
                    {
                        newNoise[x, y] = true;
                    }
                    else
                    {
                        newNoise[x, y] = false;
                    }
                }
            }

            
            return newNoise;
        }

        bool[,] Life(bool[,] data)
        {
            bool[,] newData = CopyValues(data);

            for (int y = 0; y < newData.GetLength(1); y++)
            {
                for (int x = 0; x < newData.GetLength(0); x++)
                {
                    int count = NeighborCount8Way(data, x, y);
                    if(data[x, y])
                    {
                        //if it's alive
                        if (count < 2)
                        {
                            //Any live cell with fewer than two live neighbours dies, as if by underpopulation.
                            newData[x, y] = false;
                        }
                        else if (count == 2 || count == 3)
                        {
                            //Any live cell with two or three live neighbours lives on to the next generation.
                            newData[x, y] = true;
                        }
                        else if(count > 3)
                        {
                            //Any live cell with more than three live neighbours dies, as if by overpopulation.
                            newData[x, y] = false;
                        }
                    }
                    else 
                    {
                        //space is empty
                        //Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
                        if (count == 3)
                        {
                            newData[x, y] = true;
                        }
                    }
                }
            }
            return newData;
        }
        bool[,] Smooth(bool[,] data, int neighborsForLivingCells, int neighborsForDeadCells)
        {
            bool[,] newData = CopyValues(data);
            //for every tile in the data
            for (int y = 0; y < newData.GetLength(1); y++)
            {
                for (int x = 0; x < newData.GetLength(0); x++)
                {
                    //get how many neighbors
                    int count = NeighborCount8Way(data, x, y);
                    if (newData[x, y])
                    {
                        //if the tile is currently true
                        //set the tile only if the neighbor count is 4 or greater
                        if (count >= neighborsForLivingCells)
                        {
                            newData[x, y] = true;
                        }
                        else
                        {
                            newData[x, y] = false;
                        }
                    }
                    else
                    {
                        //if the current tile is not already true
                        //make it true if the Value is 5 or greater
                        if (count >= neighborsForDeadCells)
                        {
                            newData[x, y] = true;
                        }
                        else
                        {
                            newData[x, y] = false;
                        }

                    }
                }
            }
            return newData;
        }

        bool[,] Sharpen(bool[,] data)
        {
            for (int y = 0; y < data.GetLength(1); y++)
            {
                for (int x = 0; x < data.GetLength(0); x++)
                {
                    int count = NeighborCount4Way(data, x, y);
                    if (data[x, y])
                    {
                        //if this is a space and count is > 3
                        if (count >= 3)
                        {
                            data[x, y] = false;
                        }
                    }
                    else
                    {
                        //if this is not a space and count is greater than 4
                        if (count >= 4)
                        {
                            data[x, y] = true;
                        }
                    }
                }
            }
            return data;
        }

        bool[,] Decay(bool[,] data)
        {
            bool[,] newData = CopyValues(data);
            for (int y = 0; y < newData.GetLength(1); y++)
            {
                for (int x = 0; x < newData.GetLength(0); x++)
                {
                    //we do this first for RNG consistency
                    float value = Noise.NormalizedSquirrel3(rngSequenceIndex, (uint)seed);
                    rngSequenceIndex++;

                    //if it's not a space return we only decay spaces that are full
                    if (!newData[x, y]) continue;

                    //count the number of neighbors
                    int count = NeighborCount8Way(data, x, y);
                    if (count < 6)
                    {
                        if (value < 0.5f)
                        {
                            newData[x, y] = false ;
                        }
                    }
                }
            }
            return newData;
        }

        bool[,] Grow(bool[,] data)
        {
            bool[,] newData = CopyValues(data);
            for (int y = 0; y < newData.GetLength(1); y++)
            {
                for (int x = 0; x < newData.GetLength(0); x++)
                {
                    //we do this first for RNG consistency
                    float value = Noise.NormalizedSquirrel3(rngSequenceIndex, (uint)seed);
                    rngSequenceIndex++;

                    //if it's already full then we don't need to check this
                    if (newData[x, y]) continue;

                    //count the number of neighbors
                    int count = NeighborCount8Way(data, x, y);

                    if (count > 1) {
                        if(value < 0.5f)
                        {
                            newData[x, y] = true;
                        }
                    }
                }
            }
            return newData;
        }

        bool[,] ShapeCircle(bool[,] data)
        {
            //for every tile in the data
            int halfWidth = data.GetLength(0) / 2;
            int halfHeight = data.GetLength(1) / 2;
            for (int y = 0; y < data.GetLength(1); y++)
            {
                for (int x = 0; x < data.GetLength(0); x++)
                {
                    //check if it's inside the circle
                    if ((Mathf.Pow(x - halfWidth, 2) + Mathf.Pow(y - halfHeight, 2)) > halfWidth * halfWidth)
                    {
                        data[x, y] = false;
                    }
                }
            }
            return data;
        }
        bool[,] ShapeCross(bool[,] data, int sizeX, int sizeY)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            //for every tile in the data
            for (int y = 0; y < data.GetLength(1); y++)
            {
                for (int x = 0; x < data.GetLength(0); x++)
                {
                    if(x > sizeX && x < width - sizeX)
                    {
                        data[x, y] = data[x, y];
                    }
                    else if(y > sizeY && y < height - sizeY)
                    {
                        data[x, y] = data[x, y];
                    }
                    else
                    {
                        data[x, y] = false;                        
                    }
                }
            }
            return data;
        }
        bool[,] CopyValues(bool[,] data)
        {
            bool[,] newData = new bool[data.GetLength(0), data.GetLength(1)];
            //for every tile in the data
            for (int y = 0; y < newData.GetLength(1); y++)
            {
                for (int x = 0; x < newData.GetLength(0); x++)
                {
                    newData[x, y] = data[x, y];
                }
            }
            return newData;
        }

        private int NeighborCount8Way(bool[,] data, int x, int y)
        {
            int count = 0;
            for (int yOff = -1; yOff <= 1; yOff++)
            {
                for (int xOff = -1; xOff <= 1; xOff++)
                {
                    if (yOff == 0 && xOff == 0) continue;

                    //bounds check

                    if (IsWall(data, x + xOff, y + yOff, gridWallsCountAsSolid))
                    {
                        count += 1;
                    }
                }
            }
            return count;
        }
        private int NeighborCount4Way(bool[,] data, int x, int y)
        {
            int count = 0;
            if (IsWall(data, x - 1, y, gridWallsCountAsSolid)) count += 1;
            if (IsWall(data, x + 1, y, gridWallsCountAsSolid)) count += 1;
            if (IsWall(data, x, y - 1, gridWallsCountAsSolid)) count += 1;
            if (IsWall(data, x, y + 1, gridWallsCountAsSolid)) count += 1;
            return count;
        }


        private bool IsWall(bool[,] data, int x, int y, bool offGrid = false)
        {
            //if the Value is off the grid, return the flag
            //if the flag is true then we count pixels off the grid
            //as being a solid block
            if (x < 0) return offGrid;
            if (x >= data.GetLength(0)) return offGrid;
            if (y < 0) return offGrid; ;
            if (y >= data.GetLength(1)) return offGrid;

            return data[x, y];
        }

        private bool OffGrid(bool[,] data, int x, int y)
        {
            if (x < 0) return true;
            if (x >= data.GetLength(0)) return true;
            if (y < 0) return true; ;
            if (y >= data.GetLength(1)) return true;

            return false;
        }
        public bool[,] GetTileData()
        {
            return tileData;
        }
        public Texture2D GetOutputTexture()
        {
            return outputTexture;
        }


        public Texture2D GenerateTexture(bool[,] data)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            Texture2D tex = new Texture2D(width, height);
            tex.filterMode = FilterMode.Point;
            Color32[] colors = new Color32[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colors[x + (y * width)] = data[x, y] ? Color.white : Color.black;
                }
            }
            tex.SetPixels32(colors);
            tex.Apply();
            return tex;
        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CellularAutomata))]
    public class CellularAutomataEditor : Editor
    {
        static Texture2D blackTexture;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (blackTexture == null)
            {
                blackTexture = new Texture2D(2, 2);
                blackTexture.SetPixels32(new Color32[] { new Color32(0, 0, 0, 255), new Color32(0, 0, 0, 255), new Color32(0, 0, 0, 255), new Color32(0, 0, 0, 255) });
                blackTexture.Apply();
            }
            CellularAutomata cellularAutomata = target as CellularAutomata;

            base.OnInspectorGUI();

            GUILayout.BeginHorizontal();
            if (cellularAutomata.GetTileData() != null)
            {
                int width = cellularAutomata.GetTileData().GetLength(0);
                int height = cellularAutomata.GetTileData().GetLength(1);
                var rect = GUILayoutUtility.GetAspectRect((float)width / height);

                GUI.DrawTexture(rect, cellularAutomata.GetOutputTexture());
            }
            GUILayout.EndHorizontal();
        }
    }
#endif
}
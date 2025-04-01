using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snowdrama.UI
{
    [System.Serializable]
    public struct GridCell
    {
        public Color color;
        public int cellId;
        public int width;
        public int height;
        //unity uses for anchors
        public Vector2Int bottomLeftCell;
        public Vector2Int topRightCell;

        public Vector2Int calculatedBottomLeft;
        public Vector2Int calculatedTopRight;
    }
    [ExecuteInEditMode]
    public class UIImageGridLayout : MonoBehaviour
    {

        public Sprite palette;
        public Sprite styleSprite;

        private Dictionary<Color, int> paletteList = new Dictionary<Color, int>();

        [Header("Cell Padding")]
        [SerializeField] public float topPadding = 0.0f;
        [SerializeField] public float botPadding = 0.0f;
        [SerializeField] public float leftPadding = 0.0f;
        [SerializeField] public float rightPadding = 0.0f;

        [SerializeField, EditorReadOnly] private int width;
        [SerializeField, EditorReadOnly] private int height;
        [SerializeField, EditorReadOnly] private float percentWidthCell;
        [SerializeField, EditorReadOnly] private float percentHeightCell;
        [SerializeField, EditorReadOnly] private Dictionary<int, GridCell> gridCells = new Dictionary<int, GridCell>();
        [SerializeField, EditorReadOnly] private List<RectTransform> children = new List<RectTransform>();

        [Header("Debug")]
        public bool forceUpdate = false;

        [SerializeField] private List<Color> debugPaletteKeys = new List<Color>();
        [SerializeField] private List<int> debugPaletteValues = new List<int>();

        [SerializeField] private List<int> debugKeys = new List<int>();
        [SerializeField] private List<GridCell> debugCells = new List<GridCell>();

#if UNITY_EDITOR
        // Start is called before the first frame update
        void Start()
        {
            forceUpdate = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (forceUpdate)
            {
                UpdateGrid();
            }
        }

        private void UpdateGrid()
        {

            forceUpdate = false;
            int paletteIndex = -1;
            if (palette == null || styleSprite == null)
            {
                return;
            }
            paletteList.Clear();
            for (int y = 0; y < palette.texture.height; y++)
            {
                for (int x = 0; x < palette.texture.width; x++)
                {
                    var color = palette.texture.GetPixel(x, y);
                    if (!paletteList.ContainsKey(color) && color.a >= 1.0f)
                    {
                        paletteList.Add(color, paletteIndex);
                        paletteIndex++;
                    }
                }
            }
            debugPaletteKeys = paletteList.Keys.ToList();
            debugPaletteValues = paletteList.Values.ToList();

            //we assume a grid is always square so take the first row/column
            percentHeightCell = 1.0f / styleSprite.texture.height;
            percentWidthCell = 1.0f / styleSprite.texture.width;

            gridCells.Clear();
            for (int y = 0; y < styleSprite.texture.height; y++)
            {
                for (int x = 0; x < styleSprite.texture.width; x++)
                {
                    int cellID = -1;
                    var colorKey = styleSprite.texture.GetPixel(x, y);
                    if (paletteList.ContainsKey(colorKey))
                    {
                        cellID = paletteList[colorKey];
                    }
                    if (cellID >= 0)
                    {
                        if (gridCells.ContainsKey(cellID))
                        {
                            //a grid already exists for this
                            GridCell cell = gridCells[cellID];
                            cell.color = colorKey;
                            if (x < cell.bottomLeftCell.x)
                            {
                                cell.bottomLeftCell.x = x;
                            }
                            if (y < cell.bottomLeftCell.y)
                            {
                                cell.bottomLeftCell.x = y;
                            }

                            if (x + 1 > cell.topRightCell.x)
                            {
                                cell.topRightCell.x = x + 1;
                            }
                            if (y + 1 > cell.topRightCell.y)
                            {
                                cell.topRightCell.y = y + 1;
                            }
                            cell.width = cell.topRightCell.x - cell.bottomLeftCell.x;
                            cell.height = cell.topRightCell.y - cell.bottomLeftCell.y;
                            gridCells[cellID] = cell;
                        }
                        else
                        {
                            GridCell tempCell = new GridCell();
                            tempCell.color = colorKey;
                            tempCell.cellId = cellID;
                            tempCell.bottomLeftCell = new Vector2Int(x, y);
                            tempCell.topRightCell = new Vector2Int(x + 1, y + 1);
                            gridCells.Add(cellID, tempCell);
                        }
                    }
                }
            }
            debugKeys = new List<int>(gridCells.Keys);
            debugCells = new List<GridCell>(gridCells.Values);

            for (int i = 0; i < gridCells.Keys.Count; i++)
            {
                Debug.Log($"Chekcing for Key {i} in gridCells: {!gridCells.ContainsKey(i)}");
                if (!gridCells.ContainsKey(i))
                {
                    Debug.LogError($"gridCells doesn't contain {i}!");
                    Debug.Log("Skipped Cells", this.gameObject);
                    break;
                }
            }

            children.Clear();
            foreach (Transform child in transform)
            {
                children.Add(child.GetComponent<RectTransform>());
            }

            for (int i = 0; i < children.Count; i++)
            {
                if (gridCells.ContainsKey(i))
                {
                    GridCell cell = gridCells[i];
                    gridCells[i] = cell;
                    children[i].anchorMin = new Vector2(
                        cell.bottomLeftCell.x * percentWidthCell,
                        cell.bottomLeftCell.y * percentHeightCell
                        );
                    children[i].anchorMax = new Vector2(
                        cell.topRightCell.x * percentWidthCell,
                        cell.topRightCell.y * percentHeightCell
                        );
                    children[i].offsetMin = new Vector2(leftPadding, botPadding);
                    children[i].offsetMax = new Vector2(-rightPadding, -topPadding);
                }
            }
        }
#endif
    }
}
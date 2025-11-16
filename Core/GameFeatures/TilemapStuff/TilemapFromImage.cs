using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



[System.Serializable]
public struct IndexedTile
{
    public int index;
    public TileBase tileBase;
}

[ExecuteInEditMode]
public class TilemapFromImage : MonoBehaviour
{
    [SerializeField] private Texture2D palette;
    [SerializeField] private Texture2D texture;

    [SerializeField] private TilemapRegions regions;

    [SerializeField] private bool run;

    private Dictionary<Color, int> colorRegionIds = new Dictionary<Color, int>();
    private Dictionary<int, List<Vector2Int>> colorPositions = new Dictionary<int, List<Vector2Int>>();

    [SerializeField] private List<IndexedTile> tilesList = new List<IndexedTile>();
    private Dictionary<int, TileBase> tiles = new Dictionary<int, TileBase>();

    [Header("Debug")]
    [SerializeField, EditorReadOnly] private List<Color> colors = new List<Color>();
    private void Update()
    {
        if (run)
        {
            tiles.Clear();
            foreach (var tile in tilesList)
            {
                if (!tiles.ContainsKey(tile.index))
                {
                    tiles.Add(tile.index, tile.tileBase);
                }
                else
                {
                    Debug.LogError($"Has Duplicate Key: {tile.index} Skipping");
                }
            }


            run = false;

            int index = 0;

            for (int y = palette.height; y > 0; y--)
            {
                for (int x = 0; x < palette.width; x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    Color color = palette.GetPixel(x, y);
                    if (!colorRegionIds.ContainsKey(color))
                    {
                        colorRegionIds.Add(color, index);
                        index++;
                    }
                }
            }

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    var pixelColor = texture.GetPixel(x, y);

                    if (colorRegionIds.ContainsKey(pixelColor))
                    {
                        int colorIndex = colorRegionIds[pixelColor];
                        if (!colorPositions.ContainsKey(colorIndex))
                        {
                            colorPositions.Add(colorIndex, new List<Vector2Int>());
                        }
                        colorPositions[colorIndex].Add(new Vector2Int(x, y));
                    }
                }
            }

            foreach (var colorIndex in colorRegionIds.Values)
            {
                if (colorPositions.ContainsKey((int)colorIndex))
                {
                    regions.SetRegionTilePositions(colorIndex, colorPositions[colorIndex]);
                    if (tiles.ContainsKey(colorIndex))
                    {
                        regions.SetRegionTile(colorIndex, tiles[colorIndex]);
                    }
                    else
                    {
                        Debug.LogError($"Tried using tile index {colorIndex} but no tiles assigned");
                        regions.SetRegionTile(colorIndex, null);
                    }
                }
            }

            colors.Clear();
            foreach (var item in colorRegionIds)
            {
                colors.Add(item.Key);
            }
        }
    }
}


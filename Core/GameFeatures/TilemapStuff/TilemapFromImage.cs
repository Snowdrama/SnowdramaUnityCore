using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class TilemapFromImage : MonoBehaviour
{
    [SerializeField] private Texture2D palette;
    [SerializeField] private Texture2D texture;

    [SerializeField] private TilemapRegions regions;

    [SerializeField] private bool run;

    private Dictionary<Color, int> colorRegionIds = new Dictionary<Color, int>();
    private Dictionary<int, List<Vector2Int>> colorPositions = new Dictionary<int, List<Vector2Int>>();

    [SerializeField] private List<TileBase> tiles = new List<TileBase>();

    [Header("Debug")]
    [SerializeField, EditorReadOnly] private List<Color> colors = new List<Color>();
    private void Update()
    {
        if (run)
        {

            run = false;

            int index = 0;
            for (int y = 0; y < palette.height; y++)
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
                    if (colorIndex >= 0 && colorIndex < tiles.Count)
                    {
                        if (tiles[colorIndex] != null)
                        {
                            regions.SetRegionTile(colorIndex, tiles[colorIndex]);
                        }
                        else
                        {

                            regions.SetRegionTile(colorIndex, null);
                        }
                    }
                }
            }

            colors.Clear();
            foreach (var item in colorRegionIds.Keys)
            {
                colors.Add(item);
            }
        }
    }
}


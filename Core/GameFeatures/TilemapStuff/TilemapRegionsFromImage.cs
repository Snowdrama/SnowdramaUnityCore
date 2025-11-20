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
public class TilemapRegionsFromImage : MonoBehaviour
{
    [SerializeField] private Texture2D palette;
    [SerializeField] private Texture2D texture;

    [SerializeField] private TilemapRegions regions;

    [SerializeField] private bool debugRun;
    [SerializeField] private bool clear;

    private Dictionary<Color, int> colorRegionIds = new Dictionary<Color, int>();
    private Dictionary<int, List<Vector2Int>> colorPositions = new Dictionary<int, List<Vector2Int>>();

    [Header("Debug")]
    [SerializeField, EditorReadOnly] private List<Color> colors = new List<Color>();

    private void Awake()
    {
        regions.ClearTiles();
        GetTilesFromImage();
    }

    private void Update()
    {
        if (clear)
        {
            clear = false;
            regions.ClearTiles();
        }
        if (debugRun)
        {
            debugRun = false;

            regions.ClearTiles();
            GetTilesFromImage();

            colors.Clear();
            foreach (var item in colorRegionIds)
            {
                colors.Add(item.Key);
            }
        }
    }

    private void GetTilesFromImage()
    {
        int index = 0;

        //this converts the palette
        //into a map of Color -> RegionId
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

        //then we take the actual texture and
        //use the colors to map the color to the
        //region by ID. 
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                var pixelColor = texture.GetPixel(x, y);

                if (colorRegionIds.ContainsKey(pixelColor))
                {
                    int regionId = colorRegionIds[pixelColor];
                    regions.AddTilePositionToRegion(regionId, new Vector2Int(x, y));
                }
            }
        }
    }

}


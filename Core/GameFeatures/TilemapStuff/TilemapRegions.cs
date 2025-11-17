using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapRegions : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Dictionary<int, List<Vector2Int>> tileRegions = new Dictionary<int, List<Vector2Int>>();
    private Dictionary<Vector2Int, int> regionByPosition = new Dictionary<Vector2Int, int>();
    [SerializeField] private bool drawRegionIdZero; //TODO: We'll see

    public void ClearTiles()
    {
        foreach (var tileRegion in tileRegions)
        {
            foreach (var tile in tileRegion.Value)
            {
                tilemap.SetTile((Vector3Int)tile, null);
            }
        }
        tileRegions.Clear();
        UpdateDebug();
    }

    public void SetRegionTilePositions(int regionId, List<Vector2Int> tilePositions)
    {
        if (!tileRegions.ContainsKey(regionId))
        {
            tileRegions.Add(regionId, new List<Vector2Int>());
        }

        foreach (var tile in tilePositions)
        {
            tileRegions[regionId].Add(tile);
            regionByPosition.Add(tile, regionId);
        }

        UpdateDebug();
    }

    public void SetRegionTile(int regionID, TileBase tileToSet)
    {
        if (tileRegions.ContainsKey(regionID))
        {
            foreach (var pos in tileRegions[regionID])
            {
                tilemap.SetTile((Vector3Int)pos, tileToSet);
            }
        }
        UpdateDebug();
    }

    [Header("Debug")]
    [SerializeField] private List<int> debugRegions = new List<int>();
    public void UpdateDebug()
    {
        debugRegions.Clear();
        debugRegions = tileRegions.Keys.ToList();
    }


    public int GetTile(Vector2 position)
    {
        Vector2 localPos = position - (Vector2)this.transform.position;
        Vector2Int tilePos = new Vector2Int(Mathf.FloorToInt(localPos.x), Mathf.FloorToInt(localPos.y));

        return regionByPosition[tilePos];
    }
}

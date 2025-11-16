using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapRegions : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Dictionary<int, List<Vector2Int>> tileRegions = new Dictionary<int, List<Vector2Int>>();
    [SerializeField] private bool drawRegionIdZero; //TODO: We'll see

    [Header("Debug")]
    [SerializeField] private List<int> regions = new List<int>();
    public void SetRegionTilePositions(int regionId, List<Vector2Int> tilePositions)
    {
        if (!tileRegions.ContainsKey(regionId))
        {
            tileRegions.Add(regionId, tilePositions);
        }
        else
        {
            tileRegions[regionId] = tilePositions;
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

    public void UpdateDebug()
    {
        regions.Clear();
        regions = tileRegions.Keys.ToList();
    }
}

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

/// <summary>
/// This class maps an integer to a region of tiles
/// 
/// This lets you at runtime send it a TileBase to set all tiles
/// That match the region
/// </summary>
public class TilemapRegions : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Dictionary<int, List<Vector2Int>> regionPositionList = new Dictionary<int, List<Vector2Int>>();
    private Dictionary<Vector2Int, int> regionByPosition = new Dictionary<Vector2Int, int>();
    [SerializeField] private bool drawRegionIdZero; //TODO: We'll see
    [SerializeField] private TileBase regionIdZeroTile;
    public void ClearTiles()
    {
        //first set all existing tiles to null
        foreach (var tileRegion in regionPositionList)
        {
            foreach (var tile in tileRegion.Value)
            {
                tilemap.SetTile((Vector3Int)tile, null);
            }
        }

        //then clear all the lists.
        regionByPosition.Clear();
        regionPositionList.Clear();

        UpdateDebug();
    }

    public void AddTilePositionToRegion(int regionId, List<Vector2Int> tilePositions)
    {
        if (!regionPositionList.ContainsKey(regionId))
        {
            regionPositionList.Add(regionId, new List<Vector2Int>());
        }

        foreach (var tile in tilePositions)
        {
            regionPositionList[regionId].Add(tile);
            regionByPosition.Add(tile, regionId);
        }

        UpdateDebug();
    }

    public void AddTilePositionToRegion(int regionId, Vector2Int position)
    {
        if (!regionPositionList.ContainsKey(regionId))
        {
            regionPositionList.Add(regionId, new List<Vector2Int>());
        }
        regionPositionList[regionId].Add(position);
        regionByPosition.Add(position, regionId);

        UpdateDebug();
    }

    public void RemoveTilePositionFromRegion(Vector2Int position)
    {
        int regionId = regionByPosition[position];
        regionByPosition.Remove(position);
        regionPositionList[regionId].Remove(position);
    }

    public void SetAllRegionPositionsToTile(int regionID, TileBase tileToSet)
    {
        if (regionPositionList.ContainsKey(regionID))
        {
            foreach (var pos in regionPositionList[regionID])
            {
                tilemap.SetTile((Vector3Int)pos, tileToSet);
            }
        }
        UpdateDebug();
    }


    public Vector2Int GetTilePositionFromWorldPosition(Vector2 position)
    {
        return new Vector2Int(
            Mathf.FloorToInt(position.x - this.transform.position.x),
            Mathf.FloorToInt(position.y - this.transform.position.y)
        );
    }

    public int GetRegionFromPosition(Vector2 position)
    {
        var tilePos = GetTilePositionFromWorldPosition(position);
        return GetRegionFromPosition(tilePos);
    }

    public int GetRegionFromPosition(Vector2Int position)
    {
        if (regionByPosition.ContainsKey(position))
        {
            return regionByPosition[position];
        }
        return -1;
    }

    public List<Vector2Int> GetTilePositions(int regionId)
    {
        if (regionPositionList.ContainsKey(regionId))
        {
            return regionPositionList[regionId];
        }

        //Maybe this should return an empty list? 
        return null;
    }

    [Header("Debug")]
    [SerializeField] private List<int> debugRegions = new List<int>();
    public void UpdateDebug()
    {
        debugRegions.Clear();
        debugRegions = regionPositionList.Keys.ToList();
    }
}

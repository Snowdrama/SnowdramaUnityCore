using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class SetRegionTiles : MonoBehaviour
{
    [SerializeField] private TilemapRegions regions;

    [SerializeField] private List<IndexedTile> tilesList = new List<IndexedTile>();
    private Dictionary<int, TileBase> tiles = new Dictionary<int, TileBase>();

    [Header("Debug")]
    [SerializeField] private bool debugRun;
    private void Awake()
    {
    }

    private void Start()
    {
        ConvertListToDictionary();
        SetTilesByRegion();
    }
    private void Update()
    {
        if (debugRun)
        {
            debugRun = false;
            ConvertListToDictionary();
            SetTilesByRegion();
        }
    }

    private void ConvertListToDictionary()
    {
        tiles.Clear();
        foreach (var tile in tilesList)
        {
            if (!tiles.ContainsKey(tile.index))
            {
                tiles.Add(tile.index, tile.tileBase);
            }
        }
    }

    private void SetTilesByRegion()
    {
        foreach (var tile in tilesList)
        {
            regions.SetAllRegionPositionsToTile(tile.index, tile.tileBase);
        }
    }
}

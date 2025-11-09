using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Snowdrama
{
    [ExecuteAlways]
    public class TilemapFromCelularAutomata : MonoBehaviour
    {
        [SerializeField] private CellularAutomata cellularAutomata;
        [SerializeField] private bool invertCells;
        [SerializeField] private bool run;

        [SerializeField] private TilemapRegions regions;

        [SerializeField] private TileBase tileBlack;
        [SerializeField] private TileBase tileWhite;
        private Dictionary<int, List<Vector2Int>> cellPositions = new Dictionary<int, List<Vector2Int>>();

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {
            if (run)
            {
                run = false;
                var cells = cellularAutomata.GetTileData();

                cellPositions.Clear();
                //since there's only black and white:
                cellPositions.Add(0, new List<Vector2Int>());
                cellPositions.Add(1, new List<Vector2Int>());

                for (int y = 0; y < cells.GetLength(1); y++)
                {
                    for (int x = 0; x < cells.GetLength(0); x++)
                    {
                        if (cells[x, y])
                        {
                            cellPositions[1].Add(new Vector2Int(x, y));
                        }
                        else
                        {
                            cellPositions[0].Add(new Vector2Int(x, y));
                        }
                    }
                }
                foreach (var item in cellPositions)
                {
                    regions.SetRegionTilePositions(item.Key, item.Value);
                }

                //since there's only enabled or disabled
                if (tileBlack != null)
                {
                    regions.SetRegionTile(1, tileBlack);
                }
                else
                {
                    regions.SetRegionTile(1, null);
                }

                if (tileWhite != null)
                {
                    regions.SetRegionTile(0, tileWhite);
                }
                else
                {
                    regions.SetRegionTile(0, null);
                }
            }
        }
    }
}
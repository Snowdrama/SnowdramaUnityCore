using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.Core
{
    [CreateAssetMenu(menuName = "Snowdrama/CellularAutomataSettings")]
    public class CellularAutomataSettings : ScriptableObject
    {
        public int width = 128;
        public int height = 128;
        public bool gridWallsCountAsSolid = true;
        [Header("Threshold")]
        [Range(0, 1)] public float threshold = 0.5f;
        public List<CellularAutomataProcessType> processes = new List<CellularAutomataProcessType>();
    }
}

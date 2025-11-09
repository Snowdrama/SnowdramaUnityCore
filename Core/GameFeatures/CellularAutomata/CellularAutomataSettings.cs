using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Snowdrama/CellularAutomataSettings")]
    public class CellularAutomataSettings : ScriptableObject
    {
        public int width = 128;
        public int height = 128;

        [Header("Process Chain")]
        public List<CellularAutomataProcess> processes = new List<CellularAutomataProcess>();
    }

}

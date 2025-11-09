using UnityEngine;
namespace Snowdrama
{
    public abstract class CellularAutomataProcess : ScriptableObject
    {
        public abstract void Init();
        /// <summary>
        /// Processes the cell data and returns modified data.
        /// </summary>
        public abstract bool[,] Process(bool[,] data, int seed, ref int rngSequenceIndex);
    }
}
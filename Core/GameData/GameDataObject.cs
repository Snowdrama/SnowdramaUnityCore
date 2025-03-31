using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.Core.GameData
{
    [CreateAssetMenu(menuName = "Snowdrama/GameData/GameDataObject")]
    public class GameDataObject : ScriptableObject
    {
        public GameData gameData;
        private void OnEnable()
        {
            if (gameData == null)
            {
                gameData = new GameData();
            }
            gameData.ClearAllData();
        }
        private void OnDisable()
        {
            if (gameData == null)
            {
                gameData = new GameData();
            }
            gameData.ClearAllData();
        }
    }
}
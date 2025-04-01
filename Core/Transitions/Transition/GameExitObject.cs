using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "GameExitObject", menuName = "Snowdrama/Transitions/GameExitObject")]
public class GameExitObject : ScriptableObject{
    
        /// <summary>
        /// Shortcut to quit game
        /// </summary>
        public void ExitGame()
        {
#if !UNITY_EDITOR && !UNITY_WEBGL
            Application.Quit();
#else
            Debug.LogError("Tried to quit in a reference that can't 'Quit' like the editor or WebGL");
#endif
        }
}
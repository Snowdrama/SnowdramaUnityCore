using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Reference this object to get access to Exit game from something like a button or UnityAction
/// </summary>
[System.Serializable]
[CreateAssetMenu(fileName = "GameExitObject", menuName = "Snowdrama/Transitions/GameExitObject")]
public class GameExitObject{
    
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
using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Reference this object to get access to Exit game from something like a button or UnityAction
/// </summary>
[System.Serializable]
[CreateAssetMenu(fileName = "GameExitObject", menuName = "Snowdrama/Transitions/GameExitObject")]
public class GameExitObject : ScriptableObject
{
    /// <summary>
    /// Shortcut to quit game
    /// </summary>
    [SerializeField] private bool _useConfirmation = false;
    [SerializeField] private string _confirmationMessage = "Are you sure you want to quit the game?";
    [SerializeField] private string _confirmationMessage_Yes = "Yes";
    [SerializeField] private string _confirmationMessage_No = "No";
    [SerializeField] private float _confirmationDisableTime_Yes = 1.0f;
    [SerializeField] private float _confirmationDisableTime_No = 0.0f;
    public bool UseConfirmation { get { return _useConfirmation; } }
    public string ConfirmationMessage { get { return _confirmationMessage; } }
    public string ConfirmationMessage_Yes { get { return _confirmationMessage_Yes; } }
    public string ConfirmationMessage_No { get { return _confirmationMessage_No; } }
    public void ExitGame()
    {
#if !UNITY_EDITOR && !UNITY_WEBGL
        if (_useConfirmation)
        {
            Debug.Log($"Started Transition with Confirmation");
            Messages.GetOnce<OpenConfirmationModalMessage>().Dispatch(
                _confirmationMessage,
                new ModalButtonData()
                {
                    text = _confirmationMessage_Yes,
                    disableTime = _confirmationDisableTime_Yes,
                    pressCallback = () =>
                    {
                        Application.Quit();
                    }
                },
                new ModalButtonData()
                {
                    text = this.ConfirmationMessage_No,
                    disableTime = _confirmationDisableTime_No,
                    pressCallback = null
                });
        }
        else
        {
            Application.Quit();
        }
#else
        Debug.LogError("Tried to quit in a reference that can't 'Quit' like the editor or WebGL");
#endif
    }
}
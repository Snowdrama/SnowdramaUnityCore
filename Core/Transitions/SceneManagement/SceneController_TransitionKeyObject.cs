using Snowdrama;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneTransitionKey", menuName = "Snowdrama/Transitions/SceneTransitionKey")]
public class SceneController_TransitionKeyObject : ScriptableObject
{
    [SerializeField] private string _sceneName;
    public string SceneName => _sceneName;

    [SerializeField] private bool _useConfirmation = false;
    [SerializeField] private string _confirmationMessage = "Are you sure you want to go to the scene?";
    [SerializeField] private string _confirmationMessage_Yes = "Yes";
    [SerializeField] private string _confirmationMessage_No = "No";
    [SerializeField] private float _confirmationDisableTime_Yes = 0.0f;
    [SerializeField] private float _confirmationDisableTime_No = 0.0f;
    public bool UseConfirmation => _useConfirmation;
    public string ConfirmationMessage => _confirmationMessage;
    public string ConfirmationMessage_Yes => _confirmationMessage_Yes;
    public string ConfirmationMessage_No => _confirmationMessage_No;

    private Action onModalConfirm;
    private Action onModalCancel;
    public void GoToScene(Action onModalConfirm = null, Action onModalCancel = null)
    {
        this.onModalConfirm = onModalConfirm;
        this.onModalCancel = onModalCancel;
        this.GoToScene();
    }
    public void GoToScene()
    {
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
                        onModalConfirm?.Invoke();
                        SceneController.GoToScene(_sceneName);
                        onModalCancel = null;
                        onModalConfirm = null;
                    }
                },
                new ModalButtonData()
                {
                    text = this.ConfirmationMessage_No,
                    disableTime = _confirmationDisableTime_No,
                    pressCallback = () =>
                    {
                        //Nothing happened
                        onModalCancel?.Invoke();
                        onModalCancel = null;
                        onModalConfirm = null;
                    }
                });
        }
        else
        {
            SceneController.GoToScene(_sceneName);
        }
    }

    /// <summary>
    /// Helper for starting a new game witha modal
    /// </summary>
    public void StartNewGame()
    {
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
                        SaveManager.NewGame(false);
                        onModalConfirm?.Invoke();
                        SceneController.GoToScene(_sceneName);
                        onModalCancel = null;
                        onModalConfirm = null;
                    }
                },
                new ModalButtonData()
                {
                    text = this.ConfirmationMessage_No,
                    disableTime = _confirmationDisableTime_No,
                    pressCallback = () =>
                    {
                        //Nothing happened
                        onModalCancel?.Invoke();
                        onModalCancel = null;
                        onModalConfirm = null;
                    }
                });
        }
        else
        {
            SceneController.GoToScene(_sceneName);
        }
    }
}

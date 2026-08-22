using Snowdrama.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private string _pauseRouteName = "Pause";
    [SerializeField] private UIRouter _pauseRouter;
    [SerializeField] private List<InputActionReference> _pauseActions;
    [SerializeField] private List<InputActionReference> _cancelActions;
    [Header("Background")]
    [SerializeField] private CanvasGroup _backgroundCanvasGroup;
    [SerializeField] private float _showHideTime = 0.25f;

    [Header("Debug")]
    [SerializeField, EditorReadOnly] private bool _paused;
    [SerializeField, EditorReadOnly] private float _currentAlpha = 0.0f;
    [SerializeField, EditorReadOnly] private float _targetAlpha = 0.0f;
    [SerializeField, EditorReadOnly] private float _currentAlphaVelocity = 0.0f;
    private void Start()
    {
        this.PauseOpen = false;
        this.UpdateState();
    }

    private bool _pauseOpen;
    private bool PauseOpen
    {
        get { return _pauseOpen; }
        set
        {
            if (_pauseOpen != value)
            {
                _pauseOpen = value;
                this.UpdateState();
            }
        }
    }
    private void UpdateState()
    {
        if (_pauseOpen)
        {
            _paused = true;
            _targetAlpha = 1f;
            PauseManager.RequestPause("PauseController");
            CursorManager.CursorSourceVisible("PauseController");
            _backgroundCanvasGroup.interactable = true;
            _backgroundCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            _paused = false;
            _targetAlpha = 0f;
            PauseManager.RequestUnpause("PauseController");
            CursorManager.CursorSourceHidden("PauseController");
            _backgroundCanvasGroup.interactable = false;
            _backgroundCanvasGroup.blocksRaycasts = false;
        }
    }


    private void OnEnable()
    {
        foreach (var pauseAction in _pauseActions)
        {
            pauseAction.action.Enable();
            pauseAction.action.started += this.OnPause;
        }
        foreach (var cancelAction in _cancelActions)
        {
            cancelAction.action.Enable();
            cancelAction.action.started += this.OnCancel;
        }
    }

    private void OnDisable()
    {
        foreach (var pauseAction in _pauseActions)
        {
            pauseAction.action.Disable();
            pauseAction.action.started -= this.OnPause;
        }
        foreach (var cancelAction in _cancelActions)
        {
            cancelAction.action.Disable();
            cancelAction.action.started -= this.OnCancel;
        }
        //if we're disabling the pause menu, then we're probably deleting
        //ensure we're no longer requesting pause
        PauseManager.RequestUnpause("PauseController");

        //and also clear all the routes
        _pauseRouter.CloseAll();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Pausing...");
            if (!_paused)
            {
                Debug.Log("Pausing!");
                _pauseRouter.OpenRoute(_pauseRouteName);
                this.PauseOpen = true;
            }
            else
            {
                Debug.Log("Unpausing");
                _pauseRouter.CloseAll();
                this.PauseOpen = false;
            }
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_paused)
            {
                _pauseRouter.Back();
                if (_pauseRouter.GetOpenRouteCount() == 0)
                {
                    this.PauseOpen = false;
                }
            }
        }
    }
    private void Update()
    {
        _currentAlpha = Mathf.SmoothDamp(_currentAlpha, _targetAlpha, ref _currentAlphaVelocity, _showHideTime, Mathf.Infinity, Time.unscaledDeltaTime);
        _backgroundCanvasGroup.alpha = _currentAlpha;

        //ensure if we closed all routes then we unpause
        if (_pauseRouter.GetOpenRouteCount() == 0)
        {
            this.PauseOpen = false;
        }
    }
}

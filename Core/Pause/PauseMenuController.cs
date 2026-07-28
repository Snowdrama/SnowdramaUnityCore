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
    private bool paused;
    [Header("Background")]
    [SerializeField] private Image _pauseMenuBackground;
    [SerializeField] private Color _targetBackgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.25f);
    private Color _currentBackgroundColor;
    [SerializeField] private float showHideTime = 0.25f;

    private void Start()
    {
        _currentBackgroundColor = _pauseMenuBackground.color;
    }

    public void OnPause()
    {
        this.StartCoroutine(this.FadeIn());
        PauseManager.RequestPause("PauseController");
        CursorManager.CursorSourceVisible("PauseController");
        paused = true;
    }

    public void OnUnpause()
    {
        this.StartCoroutine(this.FadeOut());
        PauseManager.RequestUnpause("PauseController");
        CursorManager.CursorSourceHidden("PauseController");
        paused = false;
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
            if (!paused)
            {
                _pauseRouter.OpenRoute(_pauseRouteName);
                this.OnPause();
            }
            else
            {
                _pauseRouter.CloseAll();
                this.OnUnpause();
            }
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (paused)
            {
                _pauseRouter.Back();
                if (_pauseRouter.GetOpenRouteCount() == 0)
                {
                    this.OnUnpause();
                }
            }
        }
    }


    private IEnumerator FadeIn()
    {
        var currentAlpha = 0.0f;
        var targetAlpha = _targetBackgroundColor.a;
        var currentAlphaVelocity = 0.0f;
        while (!Mathf.Approximately(currentAlpha, targetAlpha))
        {
            currentAlpha = Mathf.SmoothDamp(currentAlpha, targetAlpha, ref currentAlphaVelocity, showHideTime, Mathf.Infinity, Time.unscaledDeltaTime);
            _currentBackgroundColor.a = currentAlpha;

            //break execution
            yield return null;
        }
    }
    private IEnumerator FadeOut()
    {
        var currentAlpha = 0.0f;
        var targetAlpha = 0.0f;
        var currentAlphaVelocity = 0.0f;
        while (!Mathf.Approximately(currentAlpha, targetAlpha))
        {
            currentAlpha = Mathf.SmoothDamp(currentAlpha, targetAlpha, ref currentAlphaVelocity, showHideTime, Mathf.Infinity, Time.unscaledDeltaTime);
            _currentBackgroundColor.a = currentAlpha;

            //break execution
            yield return null;
        }
    }
}

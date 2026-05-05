using Snowdrama.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private string pauseRouteName = "Pause";
    [SerializeField] private UIRouter pauseRouter;
    [SerializeField] private List<InputActionReference> pauseActions;
    [SerializeField] private List<InputActionReference> cancelActions;
    private bool paused;
    private void Update()
    {
        if (pauseRouter.GetOpenRouteCount() == 0)
        {
            PauseManager.RequestUnpause("PauseController");
            paused = false;
        }

        if (pauseRouter.GetOpenRouteCount() >= 1)
        {
            PauseManager.RequestPause("PauseController");
            paused = true;
        }
    }
    private void OnEnable()
    {
        foreach (var pauseAction in pauseActions)
        {
            pauseAction.action.Enable();
            pauseAction.action.started += this.OnPause;
        }
        foreach (var cancelAction in cancelActions)
        {
            cancelAction.action.Enable();
            cancelAction.action.started += this.OnCancel;
        }
    }

    private void OnDisable()
    {
        foreach (var pauseAction in pauseActions)
        {
            pauseAction.action.Disable();
            pauseAction.action.started -= this.OnPause;
        }
        foreach (var cancelAction in cancelActions)
        {
            cancelAction.action.Disable();
            cancelAction.action.started -= this.OnCancel;
        }
        //if we're disabling the pause menu, then we're probably deleting
        //ensure we're no longer requesting pause
        PauseManager.RequestUnpause("PauseController");

        //and also clear all the routes
        pauseRouter.CloseAll();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!paused)
            {
                pauseRouter.OpenRoute(pauseRouteName);
            }
            else
            {
                pauseRouter.CloseAll();
            }
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (paused)
            {
                pauseRouter.Back();
            }
        }
    }
}

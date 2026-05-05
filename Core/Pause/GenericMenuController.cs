using Snowdrama.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GenericMenuController : MonoBehaviour
{
    [SerializeField] private string firstRouteName = "Main";
    [SerializeField] private UIRouter router;
    [SerializeField] private List<InputActionReference> openRouteAction;
    [SerializeField] private List<InputActionReference> closeRouteAction;
    private bool menuOpen;
    private void OnEnable()
    {
        foreach (var pauseAction in openRouteAction)
        {
            pauseAction.action.Enable();
            pauseAction.action.started += this.OnPause;
        }
        foreach (var cancelAction in closeRouteAction)
        {
            cancelAction.action.Enable();
            cancelAction.action.started += this.OnCancel;
        }
    }

    private void OnDisable()
    {
        foreach (var pauseAction in openRouteAction)
        {
            pauseAction.action.Disable();
            pauseAction.action.started -= this.OnPause;
        }
        foreach (var cancelAction in closeRouteAction)
        {
            cancelAction.action.Disable();
            cancelAction.action.started -= this.OnCancel;
        }

        //Clear all the routes if we're disabling the menu
        router.CloseAll();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!menuOpen)
            {
                router.OpenRoute(firstRouteName);
                menuOpen = true;
            }
            else
            {
                router.CloseAll();
                menuOpen = false;
            }
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (menuOpen)
            {
                router.Back();
                if (router.GetOpenRouteCount() == 0)
                {
                    menuOpen = false;
                }
            }
        }
    }
}

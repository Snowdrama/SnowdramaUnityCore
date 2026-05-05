using Snowdrama.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string mainRouteName = "Main";
    [SerializeField] private UIRouter mainMenuRouter;
    [SerializeField] private List<InputActionReference> cancelActions;
    private void Update()
    {
        if (mainMenuRouter.GetOpenRouteCount() == 0)
        {
            //force the menu open if it closed somehow
            mainMenuRouter.OpenRoute(mainRouteName);
        }
    }
    private void OnEnable()
    {
        foreach (var cancelAction in cancelActions)
        {
            cancelAction.action.Enable();
            cancelAction.action.started += this.OnCancel;
        }
    }

    private void OnDisable()
    {
        foreach (var cancelAction in cancelActions)
        {
            cancelAction.action.Disable();
            cancelAction.action.started -= this.OnCancel;
        }
        //and also clear all the routes
        mainMenuRouter.CloseAll();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //the man menu can't go back past route 1
            //if it's at 1 open don't allow it to go back
            if (mainMenuRouter.GetOpenRouteCount() > 1)
            {
                mainMenuRouter.Back();
            }
        }
    }
}

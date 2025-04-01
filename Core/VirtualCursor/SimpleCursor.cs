using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

//you need the low level InputSystem stuff
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class SimpleCursor : MonoBehaviour
{
    public RectTransform cursorGraphic;
    public Vector2 currentPosition;

    MouseState nextMouseState;

    Mouse virtualMouse;
    void Start()
    {
        //create a new fake mouse
        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if (!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }
        //set the default mouse state to 0,0
        nextMouseState = new MouseState
        {
            position = new Vector2(0,0),
            delta = new Vector2(0, 0),
        };
        //update the motion after the update
        InputSystem.onAfterUpdate += UpdateMotion;
    }
    private void OnDisable()
    {
        if (virtualMouse != null && virtualMouse.added)
        {
            InputSystem.RemoveDevice(virtualMouse);
        }
        InputSystem.onAfterUpdate -= UpdateMotion;
    }
    void UpdateMotion()
    {
        //get the gamepad input
        var gamepad = Gamepad.current;
        var gamepadStickDelta = gamepad.leftStick.value;

        //get the currnet position of the fake mouse
        currentPosition = virtualMouse.position.ReadValue();
        //clamp to screen
        currentPosition = new Vector2(Mathf.Clamp(currentPosition.x, 0, Screen.width), Mathf.Clamp(currentPosition.y, 0, Screen.height));
        
        //use the gamepad input, to change the position and delta of the next mouse state
        nextMouseState.position = currentPosition + gamepadStickDelta;
        nextMouseState.delta = gamepadStickDelta;

        if (gamepad.aButton.isPressed)
        {
            nextMouseState.WithButton(MouseButton.Left, true);
        }
        else
        {
            nextMouseState.WithButton(MouseButton.Left, false);
        }
        //change the virtual mouse by the next mouseState
        InputState.Change(virtualMouse, nextMouseState);

        cursorGraphic.anchoredPosition = currentPosition;
    }
}
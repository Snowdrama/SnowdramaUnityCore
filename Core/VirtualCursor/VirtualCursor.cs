using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.Rendering;

public class VirtualCursor : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;

    [SerializeField]
    private RectTransform cursorTransform;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private RectTransform canvasTransform;

    [SerializeField]
    private float cursorSpeedHover = 250.0f;
    [SerializeField]
    private float cursorSpeedNormal = 500.0f;
    private float cursorSpeed = 500.0f;
    [SerializeField]
    AnimationCurve cursorSpeedCurve = new AnimationCurve(new Keyframe[]
    {
        new Keyframe(0, 0, 1, 1),
        new Keyframe(1, 1, 1, 1),
    });

    [SerializeField]
    private float padding = 0.0f;

    [SerializeField]
    VirtualCursorStickEnum virtualCursorStick;

    [SerializeField]
    bool useHardwareCursor;

    private bool previousMouseState;

    private Mouse virtualMouse;
    private Mouse currentMouse;

    private Camera mainCamera;

    private string currentControlScheme = keyboardAndMouseScheme;

    private const string gamepadScheme = "Gamepad";
    private const string keyboardAndMouseScheme = "KBM";

    OnUIHoverStartMessage onUIHoverStartSignal;
    OnUIHoverEndMessage onUIHoverEndSignal;
    int hovered = 0;
    public void OnUIHoverStart()
    {
        hovered++;
    }
    private void OnUIHoverEnd()
    {
        hovered--;
    }
    private void OnEnable()
    {
        mainCamera = Camera.main;
        currentMouse = Mouse.current;

        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if (!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }

        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        if (cursorTransform != null)
        {
            var position = cursorTransform.anchoredPosition;
            InputState.Change(virtualMouse.position, position);
        }

        InputSystem.onAfterUpdate += UpdateMotion;
        playerInput.onControlsChanged += OnControlsChanged;

        onUIHoverStartSignal = Messages.Get<OnUIHoverStartMessage>();
        onUIHoverEndSignal = Messages.Get<OnUIHoverEndMessage>();
        onUIHoverStartSignal.AddListener(OnUIHoverStart);
        onUIHoverEndSignal.AddListener(OnUIHoverEnd);
    }

    private void OnDisable()
    {
        if (virtualMouse != null && virtualMouse.added)
        {
            InputSystem.RemoveDevice(virtualMouse);
        }
        InputSystem.onAfterUpdate -= UpdateMotion;
        playerInput.onControlsChanged -= OnControlsChanged;

        onUIHoverStartSignal.RemoveListener(OnUIHoverStart);
        onUIHoverEndSignal.RemoveListener(OnUIHoverEnd);
        Messages.Return<OnUIHoverStartMessage>();
        Messages.Return<OnUIHoverEndMessage>();
    }

    private void UpdateMotion()
    {
        if (virtualMouse == null || Gamepad.current == null)
        {
            return;
        }

        if (currentControlScheme == keyboardAndMouseScheme)
        {
            AnchorCursor(currentMouse.position.ReadValue());
        }
        else
        {
            if (hovered > 0)
            {
                cursorSpeed = cursorSpeedHover;
            }
            else
            {
                cursorSpeed = cursorSpeedNormal;
            }

            Vector2 deltaValue = Vector2.zero;
            if (virtualCursorStick == VirtualCursorStickEnum.Right)
            {
                var inputDirection = Gamepad.current.rightStick.ReadValue();
                deltaValue = Gamepad.current.rightStick.ReadValue() * inputDirection.sqrMagnitude * cursorSpeed * Time.deltaTime;
            }
            else if (virtualCursorStick == VirtualCursorStickEnum.Left)
            {
                var inputDirection = Gamepad.current.leftStick.ReadValue();
                deltaValue = Gamepad.current.leftStick.ReadValue() * inputDirection.sqrMagnitude * cursorSpeed * Time.deltaTime;
            }

            var currentPosition = virtualMouse.position.ReadValue();
            var newPosition = currentPosition + deltaValue;

            newPosition.x = Mathf.Clamp(newPosition.x, padding, Screen.width - padding);
            newPosition.y = Mathf.Clamp(newPosition.y, padding, Screen.height - padding);

            InputState.Change(virtualMouse.position, newPosition, InputUpdateType.Default);
            InputState.Change(virtualMouse.delta, deltaValue, InputUpdateType.Default);

            bool aButtonIsPressed = Gamepad.current.buttonSouth.IsPressed();
            if (previousMouseState != aButtonIsPressed)
            {
                virtualMouse.CopyState<MouseState>(out var mouseState);

                mouseState.WithButton(MouseButton.Left, aButtonIsPressed);
                InputState.Change(virtualMouse, mouseState);
                previousMouseState = aButtonIsPressed;
            }

            AnchorCursor(newPosition);
        }
    }

    private void AnchorCursor(Vector2 position)
    {
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, position, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera, out anchoredPosition);
        
        if (RectTransformUtility.RectangleContainsScreenPoint(canvasTransform, position))
        {
            if (useHardwareCursor && currentControlScheme == keyboardAndMouseScheme)
            {
                UnityEngine.Cursor.visible = true;
            }
            else
            {
                UnityEngine.Cursor.visible = false;
            }
        }
        else
        {
            //mouse is outside the bounds!
            UnityEngine.Cursor.visible = true;
        }

        cursorTransform.anchoredPosition = anchoredPosition;
    }

    private void OnControlsChanged(PlayerInput input)
    {
        if (!cursorTransform)
            return;

        if(virtualMouse == null || currentMouse == null)
        {
            return;
        }
        if (playerInput.currentControlScheme == keyboardAndMouseScheme && currentControlScheme != keyboardAndMouseScheme)
        {
            Debug.Log("Using Physical Cursor");
            currentControlScheme = keyboardAndMouseScheme;

            if (useHardwareCursor)
            {
                cursorTransform.gameObject.SetActive(false);
                UnityEngine.Cursor.visible = true;
            }

            //Make sure the virtualMouseExists before we try polling it.
            if (virtualMouse.added)
            {
                currentMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
            }
        }
        else if (playerInput.currentControlScheme == gamepadScheme && currentControlScheme != gamepadScheme)
        {
            Debug.Log("Using Virtual Cursor");
            currentControlScheme = gamepadScheme;

            //enable the graphic for the virtual cursor
            cursorTransform.gameObject.SetActive(true);
            UnityEngine.Cursor.visible = false;

            //Make sure the virtualMouseExists before we try polling it.
            if (virtualMouse.added)
            {
                InputState.Change(virtualMouse.position, currentMouse.position.ReadValue());
            }
            AnchorCursor(currentMouse.position.ReadValue());
        }
    }


}

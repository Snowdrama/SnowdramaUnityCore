using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;
public enum ControlSchemeType
{
    None,
    KBM,
    Gamepad,
    Touch,
}

public class ControlSchemeChangedMessage : AMessage<ControlSchemeType> { }

public class ControlSchemeManager : MonoBehaviour
{
    public static ControlSchemeType SchemeType = ControlSchemeType.None;
    public static ControlSchemeChangedMessage changeMesage;
    public static void RequestSchemeType(ControlSchemeType type)
    {
        if (SchemeType != type)
        {
            if (changeMesage == null)
            {
                changeMesage = Messages.Get<ControlSchemeChangedMessage>();
            }
            Debug.Log($"Scheme type {SchemeType} changing to {type}");
            SchemeType = type;
            changeMesage.Dispatch(type);
        }
    }
    [SerializeField] private InputActionReference[] mouseInputs;
    [SerializeField] private InputActionReference[] gamepadInputs;
    [SerializeField] private InputActionReference[] touchInputs;

    private void OnEnable()
    {
        foreach (var input in mouseInputs)
        {
            input.action.performed += this.OnMousePerformed;
            input.action.Enable();
        }
        foreach (var input in gamepadInputs)
        {
            input.action.performed += this.OnGamepadPerformed;
            input.action.Enable();
        }
        foreach (var input in touchInputs)
        {
            input.action.performed += this.OnTouchPerformed;
            input.action.Enable();
        }

    }
    private void OnDisable()
    {
        foreach (var input in mouseInputs)
        {
            input.action.Disable();
            input.action.performed -= this.OnMousePerformed;
        }
        foreach (var input in gamepadInputs)
        {
            input.action.Disable();
            input.action.performed -= this.OnGamepadPerformed;
        }
        foreach (var input in touchInputs)
        {
            input.action.Disable();
            input.action.performed -= this.OnTouchPerformed;
        }
    }

    public void OnMousePerformed(InputAction.CallbackContext context)
    {
        RequestSchemeType(ControlSchemeType.KBM);
    }
    public void OnGamepadPerformed(InputAction.CallbackContext context)
    {
        RequestSchemeType(ControlSchemeType.Gamepad);
    }
    public void OnTouchPerformed(InputAction.CallbackContext context)
    {
        RequestSchemeType(ControlSchemeType.Touch);
    }
}

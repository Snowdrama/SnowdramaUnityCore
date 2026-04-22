using System.Data;
using UnityEngine;
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
}

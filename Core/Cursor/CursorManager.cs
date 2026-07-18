using System.Collections.Generic;
using System.Linq;
using UnityEngine;



/// <summary>
/// This assumes that most of the time for a game, you want the cursor to be locked
/// 
/// Typically this is because we use mouse to aim like in third person or first person games,
/// or are using a gamepad in which case the cursor should be hidden at the least.
/// </summary>
public class CursorManager : MonoBehaviour
{
    private static CursorManager instance;
    [Header("Default Mode")]
    [SerializeField] private bool defaultVisible = false;
    [SerializeField] private CursorLockMode defaultMode = CursorLockMode.Locked;

    [Header("Gamepad Mode")]
    [SerializeField] private bool gamepadVisible = false;
    [SerializeField] private CursorLockMode gamepadMode = CursorLockMode.Locked;
    [Header("KBM Mode")]
    [SerializeField] private bool kbmVisible = false;
    [SerializeField] private bool kbmVisible_sourcesVisible = false;
    [SerializeField] private CursorLockMode kbmMode = CursorLockMode.Locked;
    [SerializeField] private CursorLockMode kbmMode_sourcesVisible = CursorLockMode.Confined;
    [Header("Touch Mode")]
    [SerializeField] private CursorLockMode touchMode = CursorLockMode.Locked;

    private static Dictionary<string, GameObject> visibleSourcesRequestingCursor = new Dictionary<string, GameObject>();

    private static ControlSchemeChangedMessage changeMesage;
    private void Start()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        Cursor.lockState = defaultMode;
        Cursor.visible = false;
        if (changeMesage == null)
        {
            changeMesage = Messages.Get<ControlSchemeChangedMessage>();
            changeMesage.AddListener(this.UpdateCursorState);
        }
    }

    private void Update()
    {
        //occasionally look for null gameobjects
        var sourceCount = visibleSourcesRequestingCursor.Count;

        visibleSourcesRequestingCursor = visibleSourcesRequestingCursor
            .Where(x => x.Value == null)
            .ToDictionary(t => t.Key, t => t.Value);
        var newCount = visibleSourcesRequestingCursor.Count;

        //there's a change in count, we need to see if we
        //need to update the cursor state
        if (sourceCount != newCount)
        {
            this.UpdateCursorState(ControlSchemeManager.CurrentSchemeType);
        }
    }


    private void UpdateCursorState(ControlSchemeType newType)
    {
        switch (newType)
        {
            case ControlSchemeType.None:
                Cursor.lockState = defaultMode;
                break;
            case ControlSchemeType.KBM:
                Cursor.lockState = kbmMode;
                break;
            case ControlSchemeType.Gamepad:
                Cursor.lockState = gamepadMode;
                break;
            case ControlSchemeType.Touch:
                Cursor.lockState = touchMode;
                break;
            default:
                break;
        }
    }

    public static void RequestSourceVisible(string name, GameObject go)
    {
        if (!visibleSourcesRequestingCursor.ContainsKey(name))
        {
            visibleSourcesRequestingCursor.Add(name, go);
            instance.UpdateCursorState(ControlSchemeManager.CurrentSchemeType);
        }
    }
    public static void UnRequestSourceVisible(string name, GameObject go)
    {
        if (visibleSourcesRequestingCursor.ContainsKey(name))
        {
            visibleSourcesRequestingCursor.Remove(name);
            instance.UpdateCursorState(ControlSchemeManager.CurrentSchemeType);
        }
    }
}

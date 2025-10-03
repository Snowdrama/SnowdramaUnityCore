using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class TrackedNameElementExample : MonoBehaviour, ITrackedObject
{
    [SerializeField] private TMP_Text nameText;

    [Header("Debug")]
    [SerializeField, EditorReadOnly] private string ObjectName;
    [SerializeField, EditorReadOnly] private string DisplayName;
    [SerializeField, EditorReadOnly] private Vector3 screenPosition;

    public void TrackObject(string trackedName, Vector3 pos, string[] args)
    {
        screenPosition = Camera.main.WorldToScreenPoint(pos);
        ObjectName = trackedName;
        if (args.Length > 0)
        {
            DisplayName = args[0];
        }

        if (screenPosition.x >= 0 && screenPosition.x <= Screen.width && screenPosition.y >= 0 && screenPosition.y <= Screen.height && screenPosition.z > 0)
        {
            nameText.gameObject.SetActive(true);
            nameText.transform.position = screenPosition;
            nameText.text = DisplayName;
        }
        else
        {
            nameText.gameObject.SetActive(false);
        }
    }

    public void StopTrackObject(string trackedName)
    {
        nameText.gameObject.SetActive(false);
    }
}
using UnityEngine;

public class TrackedNameExample : MonoBehaviour
{
    private string TrackedObjectPoolName = "TrackNameObjectPool";
    private MessageHub messageHub;
    private TrackUIObjectMessage trackUIObject;
    private StopTrackUIObjectMessage stopTrackUIObject;

    [SerializeField] private string ObjectName;
    [SerializeField] private string DisplayName;
    private void OnEnable()
    {
        messageHub = Messages.GetHub(TrackedObjectPoolName);
        trackUIObject = messageHub.Get<TrackUIObjectMessage>();
        stopTrackUIObject = messageHub.Get<StopTrackUIObjectMessage>();
    }

    private void OnDisable()
    {
        stopTrackUIObject?.Dispatch(ObjectName);

        messageHub.Return<TrackUIObjectMessage>();
        messageHub.Return<StopTrackUIObjectMessage>();
        Messages.ReturnHub(TrackedObjectPoolName);
        trackUIObject = null;
        stopTrackUIObject = null;
        messageHub = null;
    }

    private string[] args = new string[1];

    private void Update()
    {
        args[0] = this.DisplayName;
        trackUIObject?.Dispatch(ObjectName, this.transform.position, args);
    }
}

using UnityEngine;

public class TrackedHealthBarExample : MonoBehaviour
{
    private string TrackedObjectPoolName = "TrackHealthBarObjectPool";
    private MessageHub messageHub;
    private TrackUIObjectMessage trackUIObject;
    private StopTrackUIObjectMessage stopTrackUIObject;
    private void OnEnable()
    {
        messageHub = Messages.GetHub(TrackedObjectPoolName);
        trackUIObject = messageHub.Get<TrackUIObjectMessage>();
        stopTrackUIObject = messageHub.Get<StopTrackUIObjectMessage>();
    }

    private void OnDisable()
    {
        messageHub.Return<TrackUIObjectMessage>();
        messageHub.Return<StopTrackUIObjectMessage>();
        Messages.ReturnHub(TrackedObjectPoolName);
        trackUIObject = null;
        stopTrackUIObject = null;
        messageHub = null;
    }
}

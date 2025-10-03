using System.Collections.Generic;
using UnityEngine;


public class TrackUIObjectMessage : AMessage<string, Vector3, string[]> { }
public class StopTrackUIObjectMessage : AMessage<string> { }

public interface ITrackedObject
{
    void TrackObject(string trackedName, Vector3 pos, string[] args);
    void StopTrackObject(string trackedName);
}
public class TrackUIObjectPool : MonoBehaviour
{
    [SerializeField] private string TrackedObjectPoolName = "TrackHealthBarObjectPool";

    private Dictionary<string, ITrackedObject> trackedObjects = new Dictionary<string, ITrackedObject>();
    private Stack<ITrackedObject> trackingObjectPool = new Stack<ITrackedObject>();
    private MessageHub messageHub;
    private TrackUIObjectMessage trackUIObject;
    private StopTrackUIObjectMessage stopTrackUIObject;

    private void OnEnable()
    {
        messageHub = Messages.GetHub(TrackedObjectPoolName);
        trackUIObject = messageHub.Get<TrackUIObjectMessage>();
        stopTrackUIObject = messageHub.Get<StopTrackUIObjectMessage>();

        trackUIObject.AddListener(OnTrackObject);
        stopTrackUIObject.AddListener(StopTrackObject);


        trackedObjects.Clear();
        trackingObjectPool.Clear();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            var child = this.transform.GetChild(i);
            var comp = child.GetComponent<ITrackedObject>();
            if (comp != null)
            {
                trackingObjectPool.Push(comp);
            }
        }
    }

    private void OnDisable()
    {
        trackUIObject.RemoveListener(OnTrackObject);
        stopTrackUIObject.RemoveListener(StopTrackObject);
        messageHub.Return<TrackUIObjectMessage>();
        messageHub.Return<StopTrackUIObjectMessage>();
        Messages.ReturnHub(TrackedObjectPoolName);
        trackUIObject = null;
        stopTrackUIObject = null;
        messageHub = null;
    }

    public void OnTrackObject(string trackedName, Vector3 pos, string[] args)
    {
        if (trackedObjects.ContainsKey(trackedName))
        {
            trackedObjects[trackedName].TrackObject(trackedName, pos, args);
            return;
        }

        Debug.Log($"Creating new tracked object {trackedName}");
        var trackedObject = trackingObjectPool.Pop();
        trackedObject.TrackObject(trackedName, pos, args);
        trackedObjects.Add(trackedName, trackedObject);
    }

    public void StopTrackObject(string trackedName)
    {
        if (trackedObjects.ContainsKey(trackedName))
        {
            var trackedObj = trackedObjects[trackedName];
            trackedObj.StopTrackObject(trackedName);
            trackedObjects.Remove(trackedName);
            trackingObjectPool.Push(trackedObj);
        }
    }
}

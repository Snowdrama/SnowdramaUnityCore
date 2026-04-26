using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private static PauseManager instance;
    public static bool IsPaused => requestedPauseList.Count > 0;

    private static List<string> requestedPauseList = new List<string>();

#if UNITY_EDITOR
    [SerializeField] private List<string> requestedPauseListDebug = new List<string>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
    }

    public void Update()
    {
        requestedPauseListDebug = requestedPauseList;
    }
#endif

    public static void RequestPause(string requester)
    {
        if (!requestedPauseList.Contains(requester))
        {
            requestedPauseList.Add(requester);
        }

        if (requestedPauseList.Count > 0)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }

    public static void RequestUnpause(string requester)
    {
        if (requestedPauseList.Contains(requester))
        {
            requestedPauseList.Remove(requester);
        }

        if (requestedPauseList.Count > 0)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }

}

using Snowdrama.Timer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerTest : MonoBehaviour
{
    public TimerComponent timerComponent;
    // Start is called before the first frame update
    void Start()
    {
        timerComponent.OnComplete += Blah;
    }

    public void Blah()
    {
        Debug.Log("Timer Complete called!");
    }
}

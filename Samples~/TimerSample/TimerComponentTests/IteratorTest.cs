using Snowdrama.Timer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IteratorTest : MonoBehaviour
{
    public IteratorComponent iteratorComponent;
    // Start is called before the first frame update
    void Start()
    {
        iteratorComponent.OnIterate += Blah;
    }

    public void Blah()
    {
        Debug.Log("Iterator OnIterate called!");
    }
}

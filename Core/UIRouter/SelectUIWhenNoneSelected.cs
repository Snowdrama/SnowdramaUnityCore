using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectUIWhenNoneSelected : MonoBehaviour
{
    void Update()
    {
        if(EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null){
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }
    }
}
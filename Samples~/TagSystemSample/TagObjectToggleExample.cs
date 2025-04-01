using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagObjectToggleExample : MonoBehaviour
{
    public Camera cam;
    // Start is called before the first frame update

    public LayerMask objectMask;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = Input.mousePosition;
            var camRay = cam.ScreenPointToRay(mousePos);
            RaycastHit hitInfo;
            if (Physics.Raycast(camRay, out hitInfo, 1000, objectMask))
            {
                if (hitInfo.collider != null)
                {
                    var tagObject = hitInfo.collider.gameObject.GetComponent<TagObjectExample>();
                    if (tagObject != null)
                    {
                        tagObject.Toggle();
                    }
                }
            }
        }
        
    }
}

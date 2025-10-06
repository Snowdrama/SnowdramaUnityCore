using TMPro;
using UnityEngine;


[ExecuteAlways]
public class TrackToScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text screenElement;

    [SerializeField] private Vector3 offset = new Vector3(0, 1, 0);
    private void Update()
    {
        if (screenElement == null) { return; }
        var screenPos = Camera.main.WorldToScreenPoint(this.transform.position + offset);

        if (screenPos.x >= 0 && screenPos.x <= Screen.width && screenPos.y >= 0 && screenPos.y <= Screen.height && screenPos.z > 0)
        {
            // Your object is in the range of the camera, you can apply your behaviour
            screenElement.gameObject.SetActive(true);
            screenElement.transform.position = screenPos;
        }
        else
        {
            screenElement.gameObject.SetActive(false);
        }
    }
}

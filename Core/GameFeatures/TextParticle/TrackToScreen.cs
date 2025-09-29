using TMPro;
using UnityEngine;


[ExecuteAlways]
public class TrackToScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text screenElement;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (screenElement == null) { return; }
        var screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
        screenElement.transform.position = screenPos;
    }
}

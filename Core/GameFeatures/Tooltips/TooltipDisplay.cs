using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TooltipDisplay : MonoBehaviour
{
    private TooltipData data;
    private RectTransform rectTransform;

    [SerializeField] private TMP_Text tooltipText;

    private void Start()
    {
        rectTransform = this.GetComponent<RectTransform>();
    }

    private void Update()
    {
        data.timeRemaining -= Time.deltaTime;
        if (data.timeRemaining < 0)
        {
            //hide the tooltip after some time
            Debug.Log($"Removing: {data.id}");
            this.gameObject.SetActive(false);
            onRemove?.Invoke();

            Destroy(this.gameObject);
        }
    }

    public void SetTooltipData(TooltipData data)
    {
        Debug.Log($"Setting Data for: {data.id}");
        this.data = data;
        tooltipText.text = data.tooltipText;

        rectTransform = this.GetComponent<RectTransform>();

        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;


        rectTransform.position = data.position;
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, data.size.x);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, data.size.y);
    }

    private Action onRemove;
    public void AddRemoveCallback(Action callback)
    {
        onRemove = callback;
    }
}

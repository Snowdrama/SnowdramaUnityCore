using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class TooltipManager : MonoBehaviour
{
    public Dictionary<string, TooltipDisplay> tooltips = new Dictionary<string, TooltipDisplay>();

    [SerializeField] private TooltipDisplay TooltipPrefab;

    private ShowTooltipMessage ShowTooltipMessage;
    private void OnEnable()
    {
        ShowTooltipMessage = Messages.Get<ShowTooltipMessage>();
        ShowTooltipMessage.AddListener(this.ShowTooltip);
    }

    private void OnDisable()
    {
        ShowTooltipMessage.RemoveListener(this.ShowTooltip);
        Messages.Return<ShowTooltipMessage>();
        ShowTooltipMessage = null;
    }

    public void ShowTooltip(TooltipData data)
    {
        //create the tooltip object
        if (!tooltips.ContainsKey(data.id))
        {
            Debug.Log($"Creating new Tooltip: {data.id}");
            var go = Instantiate(TooltipPrefab);
            go.transform.SetParent(this.transform, false);
            tooltips.Add(data.id, go);
            //let the thing call back and tell us to remove it
            go.AddRemoveCallback(() =>
            {
                tooltips.Remove(data.id);
                Debug.Log($"Removing: {data.id}");
            });
        }

        //this also updates a tooltip if it changed
        var tooltip = tooltips[data.id];
        tooltip.SetTooltipData(data);
    }
}

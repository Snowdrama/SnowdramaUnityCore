using Snowdrama.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// A basic tool for making tabbed containers
/// 
/// layout should be handled by layout tools
/// </summary>
public class TabbedMenu : MonoBehaviour
{
    [SerializeField] private Transform tabButtonContainer;
    [SerializeField] private Transform tabContainer;

    [Header("Debug")]
    [SerializeField, EditorReadOnly] private List<TabbedMenuButton> tabButtons = new List<TabbedMenuButton>();
    [SerializeField, EditorReadOnly] private List<GameObject> tabs = new List<GameObject>();

    private void Start()
    {
        //get the tab buttons
        for (var i = 0; i < tabButtonContainer.childCount; i++)
        {
            var child = tabButtonContainer.GetChild(i);
            var childButton = child.GetComponent<TabbedMenuButton>();
            tabButtons.AddIfDoesntExist(childButton);
        }

        //get the tab contents themselves
        for (var i = 0; i < tabContainer.childCount; i++)
        {
            var child = tabContainer.GetChild(i);
            tabs.AddIfDoesntExist(child.gameObject);
        }

        //add the on click for the buttons
        for (var i = 0; i < tabButtons.Count; i++)
        {
            var tab = tabButtons[i];
            tab.SetTabButtonInfo(i, this.OpenTab);
        }
    }

    public void OpenTab(int index)
    {
        foreach (var tab in tabs)
        {
            tab.SetActive(false);
        }
        tabs[index].SetActive(true);
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

public class TabbedMenuButton : MonoBehaviour
{
    private int index;
    private Action<int> onClick;
    private Button button;
    private void Start()
    {
        button = this.GetComponent<Button>();
        button.onClick.AddListener(this.Pressed);
    }
    public void SetTabButtonInfo(int index, Action<int> callback)
    {
        this.index = index;
        onClick += callback;
    }
    private void Pressed()
    {
        onClick?.Invoke(index);
    }
}
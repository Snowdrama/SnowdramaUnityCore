using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SelectFirstSelectable : MonoBehaviour
{
    [Tooltip("If true, selection happens on Start. Otherwise call Select() manually.")]
    [SerializeField] private bool selectOnStart = true;
    private GameObject firstFoundSelectable;
    private void Start()
    {
        if (selectOnStart)
        {
            this.Select();
        }
    }
    private void Update()
    {
        if (firstFoundSelectable != null && EventSystem.current.currentSelectedGameObject == null)
        {
            this.Select();
        }
    }
    public void Select()
    {
        //use the cached version
        if (firstFoundSelectable != null)
        {
            EventSystem.current.SetSelectedGameObject(firstFoundSelectable);
            return;
        }

        // Get all Selectables in children (including inactive if needed)
        var selectables = this.GetComponentsInChildren<Selectable>(true);

        if (selectables.Length > 0)
        {
            // Pick the first active & interactable one
            foreach (var selectable in selectables)
            {
                if (selectable.IsActive() && selectable.IsInteractable())
                {
                    //cache this as the first selectable
                    firstFoundSelectable = selectables[0].gameObject;
                    EventSystem.current.SetSelectedGameObject(firstFoundSelectable);
                    return;
                }
            }

            // Fallback: just select the first one even if not interactable
            firstFoundSelectable = selectables[0].gameObject;
            EventSystem.current.SetSelectedGameObject(firstFoundSelectable);
        }
        else
        {
            Debug.LogWarning("No Selectable found in children of " + this.gameObject.name);
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace Snowdrama.UI
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class UIRoute : MonoBehaviour
    {
        [SerializeField] private UIRouter _router;
        [SerializeField] private string _routeSegment;
        [SerializeField] private GameObject _mainContent;
        [SerializeField] private bool _startEnabled = false;

        [SerializeField] private Selectable _objectToSelectOnOpen;

        private Selectable _lastSelected;

        private void Start()
        {
            _router.RegisterRoute(_routeSegment, this);

            if (_startEnabled)
            {
                _router.OpenRoute(_routeSegment);
                _mainContent?.SetActive(true);
            }
            else
            {
                _mainContent?.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            _router.UnregisterRoute(_routeSegment);
        }

        public void OpenRoute()
        {
            _mainContent?.SetActive(true);

            // Try restoring last selected if still valid
            if (_lastSelected != null && _lastSelected.gameObject.activeInHierarchy && _lastSelected.IsInteractable())
            {
                _lastSelected.Select();
            }
            else if (_objectToSelectOnOpen != null)
            {
                _objectToSelectOnOpen.Select();
            }
        }

        public void CloseRoute()
        {
            // Save currently selected UI element before closing
            var current = EventSystem.current?.currentSelectedGameObject;

            if (current != null)
            {
                var selectable = current.GetComponent<Selectable>();
                if (selectable != null)
                {
                    _lastSelected = selectable;
                }
            }

            _mainContent?.SetActive(false);
        }

        /// <summary>
        /// Call this when all menus are closed to reset selection memory.
        /// </summary>
        public void AllClosed()
        {
            _lastSelected = null;
        }
    }
}
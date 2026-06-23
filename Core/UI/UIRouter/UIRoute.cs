using UnityEngine;
using UnityEngine.UI;

namespace Snowdrama.UI
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [RequireComponent(typeof(CanvasGroup))]
    public class UIRoute : MonoBehaviour
    {
        [SerializeField] private UIRouter _router;
        [SerializeField] private string _routeSegment;
        //[SerializeField] private GameObject[] _toggleWhenActive;
        [SerializeField] private bool _startEnabled = false;

        [SerializeField] private Selectable _objectToSelectOnOpen;

        private Selectable _lastSelected;

        private CanvasGroup canvasGroup;
        [SerializeField] private float showHideTime = 0.25f;

        private bool _routeActive;
        private bool RouteActive
        {
            get { return _routeActive; }
            set
            {
                _routeActive = value;
                if (_routeActive)
                {
                    targetAlpha = 1f;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
                else
                {
                    targetAlpha = 0f;
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }
            }
        }
        private float currentAlpha;
        private float targetAlpha;
        private float currentAlphaVelocity;

        private void Start()
        {
            _router.RegisterRoute(_routeSegment, this);

            if (_startEnabled)
            {
                _router.OpenRoute(_routeSegment);
                this.RouteActive = true;
            }
            else
            {
                targetAlpha = 0.0f;
                this.RouteActive = false;
            }
            canvasGroup = this.GetComponent<CanvasGroup>();
        }

        private void OnDestroy()
        {
            _router.UnregisterRoute(_routeSegment);
        }

        public void OpenRoute()
        {
            this.RouteActive = true;

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

            this.RouteActive = false;
        }

        /// <summary>
        /// Call this when all menus are closed to reset selection memory.
        /// </summary>
        public void AllClosed()
        {
            _lastSelected = null;
        }

        private void Update()
        {
            currentAlpha = Mathf.SmoothDamp(currentAlpha, targetAlpha, ref currentAlphaVelocity, showHideTime);
            canvasGroup.alpha = currentAlpha;
        }
    }
}
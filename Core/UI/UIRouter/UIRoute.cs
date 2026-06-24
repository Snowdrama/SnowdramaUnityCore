using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Snowdrama.UI
{

    [RequireComponent(typeof(CanvasGroup))]
    public class UIRoute : MonoBehaviour
    {
        [SerializeField] private UIRouter _router;
        [SerializeField] private string _routeSegment;
        [SerializeField] private GameObject[] _toggleWhenActive;
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
        private bool _elementsActive;
        private bool ElementsActive
        {
            get { return _elementsActive; }
            set
            {
                if (_elementsActive != value)
                {
                    _elementsActive = value;
                    if (_elementsActive)
                    {
                        foreach (var item in _toggleWhenActive)
                        {
                            item.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        foreach (var item in _toggleWhenActive)
                        {
                            item.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
        [Header("Debug")]
        [SerializeField] private float currentAlpha;
        [SerializeField] private float targetAlpha;
        [SerializeField] private float currentAlphaVelocity;

        private void Start()
        {
            _router.RegisterRoute(_routeSegment, this);
            canvasGroup = this.GetComponent<CanvasGroup>();

            if (_startEnabled)
            {
                _router.OpenRoute(_routeSegment);
                this.RouteActive = true;
            }
            else
            {
                this.RouteActive = false;
            }
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
            currentAlpha = Mathf.SmoothDamp(currentAlpha, targetAlpha, ref currentAlphaVelocity, showHideTime, Mathf.Infinity, Time.unscaledDeltaTime);
            canvasGroup.alpha = currentAlpha;

            if (Mathf.Approximately(currentAlpha, Mathf.Epsilon))
            {
                this.ElementsActive = false;
            }
            else
            {
                this.ElementsActive = true;
            }
            currentAlpha = currentAlpha.Clamp(0, 1);
        }
    }
}
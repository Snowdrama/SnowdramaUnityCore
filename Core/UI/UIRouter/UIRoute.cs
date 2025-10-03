using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Snowdrama.UI
{
    public class UIRoute : MonoBehaviour
    {
        [SerializeField] private UIRouter _router;
        [SerializeField] private string routeSegment;
        [SerializeField] private GameObject mainContent;
        [SerializeField] private bool startEnabled = false;

        [SerializeField] private Selectable objectToSelectOnOpen;
        private void Start()
        {
            _router.RegisterRoute(routeSegment, this);
            if (startEnabled)
            {
                _router.OpenRoute(routeSegment);
                mainContent.SetActive(true);
            }
            else
            {
                mainContent.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            _router.UnregisterRoute(routeSegment);
        }

        public void OpenRoute()
        {
            mainContent.SetActive(true);

            if (objectToSelectOnOpen)
            {
                objectToSelectOnOpen.Select();
            }
        }

        public void CloseRoute()
        {
            mainContent.SetActive(false);
        }
    }
}
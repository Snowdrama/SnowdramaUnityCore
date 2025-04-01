using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Snowdrama.UI
{
    public class UIRoute : MonoBehaviour
    {
        [SerializeField] private UIRoutingSystem _routingSystem;
        [SerializeField] private string routeSegment;
        [SerializeField] private GameObject mainContent;
        [SerializeField] private bool startEnabled = false;


        [SerializeField] private Selectable objectToSelectOnOpen;
        private void Start()
        {
            _routingSystem.GetRouter().RegisterRoute(routeSegment, this);
            if (startEnabled)
            {
                _routingSystem.GetRouter().OpenRoute(routeSegment);
                mainContent.SetActive(true);
            }
            else
            {
                mainContent.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            _routingSystem.GetRouter().UnregisterRoute(routeSegment);
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
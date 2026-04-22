using System;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.UI
{
    [CreateAssetMenu(fileName = "UIRouter", menuName = "Snowdrama/UI Router")]
    public class UIRouter : ScriptableObject
    {
        private Dictionary<string, UIRoute> routes = new Dictionary<string, UIRoute>();
        private StackList<string> routesOpened = new StackList<string>();
        private int openRouteCount = 0;

        [Header("Debug[Don't Edit!]")]
        [SerializeField] private List<string> registeredRoutes = new List<string>();
        [SerializeField] private List<string> currentStack = new List<string>();

        private Action OnAllRoutesClosed;
        private Action<string> OnOpenRoute;


        public StackList<string> GetRoutesOpened()
        {
            return routesOpened;
        }
        public int GetOpenRouteCount()
        {
            return openRouteCount;
        }

        private void OnEnable()
        {
            routes.Clear();
            routesOpened.Clear();
            registeredRoutes.Clear();
            currentStack.Clear();
        }

        private void OnDisable()
        {
            routes.Clear();
            routesOpened.Clear();
            registeredRoutes.Clear();
            currentStack.Clear();
        }
        public bool IsRouteOpen(string routeSegment)
        {
            if (routesOpened.Count == 0)
            {
                return false;
            }

            if (routesOpened.Peek() == routeSegment.ToLower())
            {
                return true;
            }

            return false;
        }

        public bool IsRouteInRouteStack(string routeSegment)
        {
            if (routesOpened.Count == 0)
            {
                return false;
            }

            if (routesOpened.Contains(routeSegment.ToLower()))
            {
                return true;
            }
            return false;
        }

        public void RegisterRoute(string routeSegment, UIRoute reference)
        {
            if (routes.ContainsKey(routeSegment.ToLower()))
            {
                routes[routeSegment.ToLower()] = reference;
            }
            else
            {
                routes.Add(routeSegment.ToLower(), reference);
                registeredRoutes.Add(routeSegment.ToLower());
            }
        }

        public void UnregisterRoute(string routeSegment)
        {
            if (routes.ContainsKey(routeSegment.ToLower()))
            {
                routes.Remove(routeSegment.ToLower());
                registeredRoutes.Remove(routeSegment.ToLower());
                //remove it if we're destroying the element
                //to ensure we don't try and open it
                if (currentStack.Contains(routeSegment.ToLower()))
                {
                    currentStack.Remove(routeSegment.ToLower());
                }
                if (routesOpened.Contains(routeSegment.ToLower()))
                {
                    routesOpened.Remove(routeSegment.ToLower());
                }
            }
        }

        public void OpenRoute(string routeSegment)
        {
            if (routes.ContainsKey(routeSegment.ToLower()))
            {
                if (routesOpened.Count > 0)
                {
                    var rs = routesOpened.Peek();
                    routes[rs].CloseRoute();
                }
                routesOpened.Push(routeSegment.ToLower());
                routes[routeSegment.ToLower()].OpenRoute();
                openRouteCount++;
            }
            else
            {
                Debug.LogError("No Route with segment: " + routeSegment.ToLower());
            }
            this.UpdateDebug();
        }

        public void OpenRouteExclusive(string routeSegment)
        {
            if (routes.ContainsKey(routeSegment.ToLower()))
            {
                this.CloseAll();
                routesOpened.Push(routeSegment.ToLower());
                routes[routeSegment.ToLower()].OpenRoute();
                openRouteCount = 1;
            }
            else
            {
                Debug.LogError("No Route with segment: " + routeSegment.ToLower());
            }
            this.UpdateDebug();
        }

        public void CloseAll()
        {
            if (routesOpened.Count > 0)
            {
                var rs = routesOpened.Peek();
                routes[rs].CloseRoute();
            }
            OnAllRoutesClosed?.Invoke();
            foreach (var route in routes.Values)
            {
                route.AllClosed();
            }

            openRouteCount = 0;
            routesOpened.Clear();
            this.UpdateDebug();
        }

        public void Back()
        {
            if (routesOpened.Count > 0)
            {
                var rs = routesOpened.Pop();
                if (routes.ContainsKey(rs))
                {
                    routes[rs].CloseRoute();
                    openRouteCount--;
                }
                if (routesOpened.Count > 0)
                {
                    var peekRoute = routesOpened.Peek();
                    if (routes.ContainsKey(peekRoute))
                    {
                        routes[peekRoute].OpenRoute();
                    }
                }
            }
            if (routesOpened.Count <= 0)
            {
                OnAllRoutesClosed?.Invoke();
                foreach (var route in routes.Values)
                {
                    route.AllClosed();
                }
                openRouteCount = 0;
            }

            this.UpdateDebug();
        }


        public void UpdateDebug()
        {
            currentStack = new List<string>(routesOpened);
        }




        //Shortcut to use to exit the game
        public void ExitGame()
        {
            Messages.GetOnce<OpenConfirmationModalMessage>().Dispatch(
                "Are you sure you want to Exit To The Desktop?",
                new ModalButtonData()
                {
                    text = "Yes",
                    disableTime = 0.5f,
                    pressCallback = () =>
                    {
                        Application.Quit();
                    }
                },
                new ModalButtonData()
                {
                    text = "No",
                    disableTime = 0.0f,
                    pressCallback = null
                }
            );
        }
    }
}
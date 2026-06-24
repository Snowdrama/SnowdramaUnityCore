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
        [SerializeField] private List<string> registeredRoutesDebug = new List<string>();
        [SerializeField] private List<string> currentStackDebug = new List<string>();

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
            openRouteCount = routesOpened.Count;
            registeredRoutesDebug.Clear();
            currentStackDebug.Clear();
        }

        private void OnDisable()
        {
            routes.Clear();
            routesOpened.Clear();
            openRouteCount = routesOpened.Count;
            registeredRoutesDebug.Clear();
            currentStackDebug.Clear();
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
                registeredRoutesDebug.Add(routeSegment.ToLower());
            }
            this.UpdateDebug();
        }

        public void UnregisterRoute(string routeSegment)
        {
            if (routes.ContainsKey(routeSegment.ToLower()))
            {
                routes.Remove(routeSegment.ToLower());
                registeredRoutesDebug.Remove(routeSegment.ToLower());
                if (routesOpened.Contains(routeSegment.ToLower()))
                {
                    routesOpened.Remove(routeSegment.ToLower());
                    openRouteCount = routesOpened.Count;
                }
            }
            this.UpdateDebug();
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
                openRouteCount = routesOpened.Count;
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
                openRouteCount = routesOpened.Count;
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

            routesOpened.Clear();
            openRouteCount = routesOpened.Count;
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
            }

            openRouteCount = routesOpened.Count;
            this.UpdateDebug();
        }


        public void UpdateDebug()
        {
            currentStackDebug = new List<string>(routesOpened);
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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.UI
{
    [CreateAssetMenu(fileName = "UIRouter", menuName = "Snowdrama/UI Router")]
    public class UIRouter : ScriptableObject
    {
        Dictionary<string, UIRoute> routes = new Dictionary<string, UIRoute>();
        public Stack<string> routesOpened = new Stack<string>();

        [Header("Debug[Don't Edit!]")]
        public List<string> registeredRoutes = new List<string>();
        public List<string> currentStack = new List<string>();

        public Action OnAllRoutesClosed;
        public Action<string> OnOpenRoute;

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
            if(routesOpened.Count == 0)
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
            }
            else
            {
                Debug.LogError("No Route with segment: " + routeSegment.ToLower());
            }
            UpdateDebug();
        }

        public void OpenRouteExclusive(string routeSegment)
        {
            if (routes.ContainsKey(routeSegment.ToLower()))
            {
                CloseAll();
                routesOpened.Push(routeSegment.ToLower());
                routes[routeSegment.ToLower()].OpenRoute();
            }
            else
            {
                Debug.LogError("No Route with segment: " + routeSegment.ToLower());
            }
            UpdateDebug();
        }

        public void CloseAll()
        {
            if (routesOpened.Count > 0)
            {
                var rs = routesOpened.Peek();
                routes[rs].CloseRoute();
            }
            OnAllRoutesClosed?.Invoke();
            routesOpened.Clear();
            UpdateDebug();
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
            }

            UpdateDebug();
        }


        public void UpdateDebug()
        {
            currentStack = new List<string>(routesOpened);
        }
    }
}
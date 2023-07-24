using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.InputSystem;


public class PlayerInputBinderSettings
{
    public bool UseCachedReflectionProperties { get; set; } = true;
}

public static class PlayerInputBinder
{
    private struct NestedData
    {
        public Type[] NestedTypes { get; set; }
        public Type[] Interfaces { get; set; }
    }

    private struct InstanceData
    {
        public MethodInfo[] Methods { get; set; }
        public Type[] Interfaces { get; set; }
    }

    private static Dictionary<Type, InstanceData> InstanceDataCache = new Dictionary<Type, InstanceData>();
    private static Dictionary<Type, NestedData> NestedDataCache = new Dictionary<Type, NestedData>();

    private static Dictionary<Type, MethodInfo[]> InterfaceMethodsCache = new Dictionary<Type, MethodInfo[]>();
    private static Dictionary<InputAction, EventInfo[]> InputActionEventsCache = new Dictionary<InputAction, EventInfo[]>();

    private static Dictionary<InputAction, Dictionary<object, List<Delegate>>> InputEventDelegateMappings = new Dictionary<InputAction, Dictionary<object, List<Delegate>>>();

    public static void BindPlayerInputToClass<T>(PlayerInput playerInput, Type generatedInputClassType, T instanceToBindTo, PlayerInputBinderSettings Settings = default)
        where T : class
    {
        if (playerInput == null)
        {
            throw new ArgumentNullException(nameof(playerInput));
        }

        if (generatedInputClassType == null)
        {
            throw new ArgumentNullException(nameof(generatedInputClassType));
        }

        if (instanceToBindTo == null)
        {
            throw new ArgumentNullException(nameof(instanceToBindTo));
        }

        if(Settings == null)
        {
            Settings = new PlayerInputBinderSettings();
        }

        Type instanceType = instanceToBindTo.GetType();
        (Type[] nestedTypes, Type[] nestedInterfaces, MethodInfo[] instanceMethods, Type[] instanceInterfaces) = GetReflectionProperties(instanceType, generatedInputClassType, Settings);

        foreach (var ni in nestedInterfaces)
        {
            if (!instanceInterfaces.Any(x => x.Name == ni.Name))
                continue;

            var actionMapName = ni.Name.Substring(1).Replace("Actions", string.Empty);
            var interfaceMethods = GetInterfaceMethods(ni, Settings);

            foreach (var interfaceMethod in interfaceMethods)
            {
                var actionName = interfaceMethod.Name.Substring(2);
                var inputAction = GetInputAction(playerInput, actionMapName, actionName);
                var methodToBindTo = instanceMethods.First(x => x.Name == interfaceMethod.Name);

                var currentEvents = GetInputActionEvents(inputAction, Settings);
                foreach (var ev in currentEvents)
                {
                    Delegate handler = Delegate.CreateDelegate(ev.EventHandlerType, instanceToBindTo, methodToBindTo, true);
                    AddEventHandler(inputAction, instanceToBindTo, ev, handler);
                }
            }
        }
    }

    public static void UnbindPlayerInputToClass<T>(PlayerInput playerInput, Type generatedInputClassType, T instanceToBindTo, PlayerInputBinderSettings Settings = default)
        where T : class
    {
        if (playerInput == null)
        {
            throw new ArgumentNullException(nameof(playerInput));
        }

        if (generatedInputClassType == null)
        {
            throw new ArgumentNullException(nameof(generatedInputClassType));
        }

        if (instanceToBindTo == null)
        {
            throw new ArgumentNullException(nameof(instanceToBindTo));
        }

        if (Settings == null)
        {
            Settings = new PlayerInputBinderSettings();
        }

        Type instanceType = instanceToBindTo.GetType();
        (Type[] nestedTypes, Type[] nestedInterfaces, MethodInfo[] instanceMethods, Type[] instanceInterfaces) = GetReflectionProperties(instanceType, generatedInputClassType, Settings);

        foreach (var ni in nestedInterfaces)
        {
            if (!instanceInterfaces.Any(x => x.Name == ni.Name))
                continue;

            var actionMapName = ni.Name.Substring(1).Replace("Actions", string.Empty);
            var interfaceMethods = GetInterfaceMethods(ni, Settings);

            foreach (var interfaceMethod in interfaceMethods)
            {
                var actionName = interfaceMethod.Name.Substring(2);
                var inputAction = GetInputAction(playerInput, actionMapName, actionName);

                ClearEventDelegates(instanceToBindTo, inputAction, Settings);
            }
        }
    }

    private static InputAction GetInputAction(PlayerInput playerInput, string actionMapName, string actionName)
    {
        return playerInput.actions[$"{actionMapName}/{actionName}"];
    }

    private static (Type[] nestedTypes, Type[] nestedInterfaces, MethodInfo[] instanceMethods, Type[] instanceInterfaces) GetReflectionProperties(Type instanceType, Type generatedType, PlayerInputBinderSettings Settings = default)
    {
        Type[] nestedTypes;
        Type[] nestedInterfaces;

        MethodInfo[] instanceMethods;
        Type[] instanceInterfaces;

        if (Settings.UseCachedReflectionProperties)
        {
            if (!InstanceDataCache.ContainsKey(instanceType))
            {
                instanceMethods = instanceType.GetMethods();
                instanceInterfaces = instanceType.GetInterfaces();

                InstanceDataCache.Add(instanceType, new InstanceData { Methods = instanceMethods, Interfaces = instanceInterfaces });
            }
            else
            {
                var instanceData = InstanceDataCache[instanceType];

                instanceMethods = instanceData.Methods;
                instanceInterfaces = instanceData.Interfaces;
            }

            if (!NestedDataCache.ContainsKey(generatedType))
            {
                nestedTypes = generatedType.GetNestedTypes();
                nestedInterfaces = nestedTypes.Where(x => x.IsInterface).ToArray();

                NestedDataCache.Add(generatedType, new NestedData { NestedTypes = nestedTypes, Interfaces = nestedInterfaces });
            }
            else
            {
                var nestedData = NestedDataCache[generatedType];

                nestedTypes = nestedData.NestedTypes;
                nestedInterfaces = nestedData.Interfaces;
            }
        }
        else
        {
            nestedTypes = generatedType.GetNestedTypes();
            nestedInterfaces = nestedTypes.Where(x => x.IsInterface).ToArray();
            instanceMethods = instanceType.GetMethods();
            instanceInterfaces = instanceType.GetInterfaces();
        }

        return (nestedTypes: nestedTypes, nestedInterfaces: nestedInterfaces, instanceMethods: instanceMethods, instanceInterfaces: instanceInterfaces);
    }

    private static MethodInfo[] GetInterfaceMethods(Type interfaceType, PlayerInputBinderSettings Settings = default)
    {
        if(Settings.UseCachedReflectionProperties)
        {
            if(!InterfaceMethodsCache.ContainsKey(interfaceType))
            {
                InterfaceMethodsCache.Add(interfaceType, interfaceType.GetMethods());
            }
            return InterfaceMethodsCache[interfaceType];
        }
        else
        {
            return interfaceType.GetMethods();
        }
    }

    private static EventInfo[] GetInputActionEvents(InputAction action, PlayerInputBinderSettings Settings = default)
    {
        if(Settings.UseCachedReflectionProperties)
        {
            if (!InputActionEventsCache.ContainsKey(action))
            {
                InputActionEventsCache.Add(action, action.GetType().GetEvents());
            }
            return InputActionEventsCache[action];
        }
        else
        {
            return action.GetType().GetEvents();
        }
    }

    private static void AddEventHandler<T>(InputAction action, T instance, EventInfo ev, Delegate handler)
    {
        if (!InputEventDelegateMappings.ContainsKey(action))
        {
            InputEventDelegateMappings.Add(action, new Dictionary<object, List<Delegate>>());
        }

        var instanceList = InputEventDelegateMappings[action];

        if (!instanceList.ContainsKey(instance))
        {
            instanceList[instance] = new List<Delegate>();
        }

        ev.AddEventHandler(action, handler);

        instanceList[instance].Add(handler);
    }

    private static void ClearEventDelegates<T>(T instanceToBindTo, InputAction inputAction, PlayerInputBinderSettings Settings) where T : class
    {
        if (InputEventDelegateMappings.ContainsKey(inputAction) && InputEventDelegateMappings[inputAction].ContainsKey(instanceToBindTo))
        {
            var instances = InputEventDelegateMappings[inputAction];
            var handlers = instances[instanceToBindTo];

            var currentEvents = GetInputActionEvents(inputAction, Settings);

            foreach (var ev in currentEvents)
            {
                foreach (var handler in handlers)
                {
                    ev.RemoveEventHandler(inputAction, handler);
                }
            }

            handlers.Clear();
        }
    }
}

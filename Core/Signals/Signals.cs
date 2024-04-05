// ========================================================================================
// Signals - A typesafe, lightweight messaging lib for Unity.
// ========================================================================================
// 2017-2019, Yanko Oliveira  / http://yankooliveira.com / http://twitter.com/yankooliveira
// Special thanks to Max Knoblich for code review and Aswhin Sudhir for the anonymous 
// function asserts suggestion.
//
// 2023, John "Snowdrama" Close
// Used in our Ludum Dare game, and made some changes to allow more flexible useage. 
// ========================================================================================
// Inspired by StrangeIOC, minus the clutter.
// Based on http://wiki.unity3d.com/index.php/CSharpMessenger_Extended
// Converted to use strongly typed parameters and prevent use of strings as ids.
//
// Supports up to 3 parameters. More than that, and you should probably use a VO.
//
// Usage:
//    1) Define your class, eg:
//          ScoreSignal : ASignal<int> {}
//    2) Add listeners on portions that should react, eg on Awake():
//          Signals.Get<ScoreSignal>().AddListener(OnScore);
//    3) Dispatch, eg:
//          Signals.Get<ScoreSignal>().Dispatch(userScore);
//    4) Don'time forget to remove the listeners upon destruction! Eg on OnDestroy():
//          Signals.Get<ScoreSignal>().RemoveListener(OnScore);
//    5) If you don'time want to use global Signals, you can have your very own SignalHub
//       instance in your class
//
// ========================================================================================
// Added by Snowdrama 10/24/2023:
//
// SignalHubs can now be created similar to how signals work, it uses name strings for the hubs name
// For example using something like a unique entity Id, or something like "PlayerOne"
//      1) Get a reference to your hub:
//          playerHub = Signals.GetHub("PlayerOne");
//      2) Signal refrences should be gotten through the unique hub instead of the globalHub:
//          scoreSignal = playerHub.Get<ScoreSignal>();
//
// SignalHubs now can have signal usages returned to them so we can know if the signal is
// no longer being used, and if the hub also has no users.
// 
// Short example of using and returning signals and hubs.
// 
// public class ScoreSignal : ASignal<int> { }
// public class GameOverSignal : ASignal { }
// public class Player : MonoBehaviour
// {
//     SignalHub playerHub;
//     ScoreSignal scoreSignal;
//     void OnEnable()
//     {
//         playerHub = Signals.GetHub("PlayerOne");
//         scoreSignal = playerHub.Get<ScoreSignal>();
//         //Note this is a global signal
//         gameOverSignal = Signals.Get<GameOverSignal>(); 
//     }
// 
//     void OnDisable()
//     {
//         //remember to return the score signal to the hub!
//         playerHub.Return<ScoreSignal>();
// 
//         //also return the hub referene!
//         Signals.ReturnHub("PlayerOne");
//
//         //global signals are just returned to the globalHub
//         Signals.Return<GameOverSignal>();
//     }
// }

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Base interface for Signals
/// </summary>
public interface ISignal
{
    void AddUser();
    void RemoveUser();
    int GetUserCount();
}

/// <summary>
/// Signals main facade class for global, game-wide signals
/// </summary>
public static class Signals
{
    private static Dictionary<string, SignalHub> signalHubs = new Dictionary<string, SignalHub>();
    private static readonly SignalHub globalHub = new SignalHub();

    public static SType Get<SType>() where SType : ISignal, new()
    {
        return globalHub.Get<SType>();
    }

    public static void Return<SType>() where SType : ISignal, new()
    {
        globalHub.Return<SType>();
    }

    //This creates a named SignalHub which is useful for when
    //Several components share a signal type
    //for example a local multiplayer game where each
    //player has a health bar. They can all share a "HealthChanged" signal
    //but each use their own hub!
    public static SignalHub GetHub(string hubName)
    {
        if (signalHubs.ContainsKey(hubName))
        {
            signalHubs[hubName].AddUser();
            return signalHubs[hubName];
        }
        var newHub = new SignalHub();
        newHub.AddUser();
        signalHubs.Add(hubName, newHub);
        return newHub;
    }

    //returns a reference of the hub 
    public static void ReturnHub(string hubName)
    {
        //you can only return a use of a hub that exists
        if (signalHubs.ContainsKey(hubName))
        {
            signalHubs[hubName].RemoveUser();
            if (signalHubs[hubName].UserCount == 0)
            {
                //if the hub has no users, then we can safely remove it!
                signalHubs.Remove(hubName);
            }
        }
    }

    //This uses the UserCount value of a signal hub
    //to remove unused signals. This has to be called
    //manually and 
    public static void CleanUpSignalHubs()
    {
        //remove any hubs with 0 signals
        //
        signalHubs = signalHubs.Where(x => x.Value.GetSignalCount() == 0).ToDictionary(x => x.Key, x => x.Value);
    }
}

/// <summary>
/// A hub for Signals you can implement in your classes
/// </summary>
public class SignalHub
{
    public int UserCount { get; private set; }
    private Dictionary<Type, ISignal> signals = new Dictionary<Type, ISignal>();

    /// <summary>
    /// Getter for a ammoSignal of a given type
    /// </summary>
    /// <typeparam name="SType">Type of ammoSignal</typeparam>
    /// <returns>The proper ammoSignal binding</returns>
    public SType Get<SType>() where SType : ISignal, new()
    {
        Type signalType = typeof(SType);
        ISignal signal;

        if (signals.TryGetValue(signalType, out signal))
        {
            signal.AddUser();
            return (SType)signal;
        }
        var newSignal = (SType)Bind(signalType);
        newSignal.AddUser();
        return newSignal;
    }

    /// <summary>
    /// Returns a useage of a signal, decreasing the users of that signal
    /// allowing us to know when the signal is no longer in use
    /// </summary>
    /// <typeparam name="SType"></typeparam>
    public void Return<SType>() where SType : ISignal, new()
    {
        Type signalType = typeof(SType);
        ISignal signal;

        //obviously we can't return a signal that doesn't exist
        if (signals.TryGetValue(signalType, out signal))
        {
            //decrease the user count
            signal.RemoveUser();

            //if there's no users, remove the whole signal
            //so we can dispose of the hub if there's no
            //signals in the hub
            if(signal.GetUserCount() == 0)
            {
                signals.Remove(signalType);
            }
        }
    }

    private ISignal Bind(Type signalType)
    {
        ISignal signal;
        if (signals.TryGetValue(signalType, out signal))
        {
            UnityEngine.Debug.LogError(string.Format("Signal already registered for type {0}", signalType.ToString()));
            return signal;
        }

        signal = (ISignal)Activator.CreateInstance(signalType);
        signals.Add(signalType, signal);
        return signal;
    }

    private ISignal Bind<T>() where T : ISignal, new()
    {
        return Bind(typeof(T));
    }

    public int GetSignalCount()
    {
        return signals.Count;
    }

    /// <summary>
    /// Called when a user is added to the signal
    /// </summary>
    public void AddUser()
    {
        ++UserCount;
    }

    /// <summary>
    /// Get the nuber of signal users
    /// </summary>
    /// <returns>The number of signal users</returns>
    public int GetUserCount()
    {
        return UserCount;
    }

    /// <summary>
    /// Called when a user is removed from a signal
    /// </summary>
    public void RemoveUser()
    {
        --UserCount;
    }
}

/// <summary>
/// Abstract class for Signals, provides hash by type functionality
/// </summary>
public abstract class ABaseSignal : ISignal
{
    public int UserCount {  get; private set; }

    /// <summary>
    /// Called when a user is added to the signal
    /// </summary>
    public void AddUser()
    {
        ++UserCount;
    }

    /// <summary>
    /// Get the nuber of signal users
    /// </summary>
    /// <returns>The number of signal users</returns>
    public int GetUserCount()
    {
        return UserCount;
    }

    /// <summary>
    /// Called when a user is removed from a signal
    /// </summary>
    public void RemoveUser()
    {
        --UserCount;
    }
}

/// <summary>
/// Strongly typed messages with no parameters
/// </summary>
public abstract class ASignal : ABaseSignal
{
    private Action callback;

    /// <summary>
    /// Adds a listener to this Signal
    /// </summary>
    /// <param name="handler">Method to be called when ammoSignal is fired</param>
    public void AddListener(Action handler)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Assert(handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false).Length == 0,
            "Adding anonymous delegates as Signal callbacks is not supported (you wouldn'time be able to unregister them later).");
#endif
        callback += handler;
    }

    /// <summary>
    /// Removes a listener from this Signal
    /// </summary>
    /// <param name="handler">Method to be unregistered from ammoSignal</param>
    public void RemoveListener(Action handler)
    {
        callback -= handler;
    }

    /// <summary>
    /// Dispatch this ammoSignal
    /// </summary>
    public void Dispatch()
    {
        if (callback != null)
        {
            callback();
        }
    }
}

/// <summary>
/// Strongly typed messages with 1 parameter
/// </summary>
/// <typeparam name="T">Parameter type</typeparam>
public abstract class ASignal<T> : ABaseSignal
{
    private Action<T> callback;

    /// <summary>
    /// Adds a listener to this Signal
    /// </summary>
    /// <param name="handler">Method to be called when ammoSignal is fired</param>
    public void AddListener(Action<T> handler)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Assert(handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false).Length == 0,
            "Adding anonymous delegates as Signal callbacks is not supported (you wouldn'time be able to unregister them later).");
#endif
        callback += handler;
    }

    /// <summary>
    /// Removes a listener from this Signal
    /// </summary>
    /// <param name="handler">Method to be unregistered from ammoSignal</param>
    public void RemoveListener(Action<T> handler)
    {
        callback -= handler;
    }

    /// <summary>
    /// Dispatch this ammoSignal with 1 parameter
    /// </summary>
    public void Dispatch(T arg1)
    {
        if (callback != null)
        {
            callback(arg1);
        }
    }
}

/// <summary>
/// Strongly typed messages with 2 parameters
/// </summary>
/// <typeparam name="T">First parameter type</typeparam>
/// <typeparam name="U">Second parameter type</typeparam>
public abstract class ASignal<T, U> : ABaseSignal
{
    private Action<T, U> callback;

    /// <summary>
    /// Adds a listener to this Signal
    /// </summary>
    /// <param name="handler">Method to be called when ammoSignal is fired</param>
    public void AddListener(Action<T, U> handler)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Assert(handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false).Length == 0,
            "Adding anonymous delegates as Signal callbacks is not supported (you wouldn'time be able to unregister them later).");
#endif
        callback += handler;
    }

    /// <summary>
    /// Removes a listener from this Signal
    /// </summary>
    /// <param name="handler">Method to be unregistered from ammoSignal</param>
    public void RemoveListener(Action<T, U> handler)
    {
        callback -= handler;
    }

    /// <summary>
    /// Dispatch this ammoSignal
    /// </summary>
    public void Dispatch(T arg1, U arg2)
    {
        if (callback != null)
        {
            callback(arg1, arg2);
        }
    }
}

/// <summary>
/// Strongly typed messages with 3 parameter
/// </summary>
/// <typeparam name="T">First parameter type</typeparam>
/// <typeparam name="U">Second parameter type</typeparam>
/// <typeparam name="V">Third parameter type</typeparam>
public abstract class ASignal<T, U, V> : ABaseSignal
{
    private Action<T, U, V> callback;

    /// <summary>
    /// Adds a listener to this Signal
    /// </summary>
    /// <param name="handler">Method to be called when ammoSignal is fired</param>
    public void AddListener(Action<T, U, V> handler)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Assert(handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false).Length == 0,
            "Adding anonymous delegates as Signal callbacks is not supported (you wouldn'time be able to unregister them later).");
#endif
        callback += handler;
    }

    /// <summary>
    /// Removes a listener from this Signal
    /// </summary>
    /// <param name="handler">Method to be unregistered from ammoSignal</param>
    public void RemoveListener(Action<T, U, V> handler)
    {
        callback -= handler;
    }

    /// <summary>
    /// Dispatch this ammoSignal
    /// </summary>
    public void Dispatch(T arg1, U arg2, V arg3)
    {
        if (callback != null)
        {
            callback(arg1, arg2, arg3);
        }
    }
}
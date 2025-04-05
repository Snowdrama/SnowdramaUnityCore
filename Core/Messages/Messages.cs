/* ========================================================================================
 * Messages(Previously Signals) - A typesafe, lightweight messaging lib for Unity.
 * ========================================================================================
 * 2017-2019, Yanko Oliveira  / http://yankooliveira.com / http://twitter.com/yankooliveira
 * Special thanks to Max Knoblich for code review and Aswhin Sudhir for the anonymous 
 * function asserts suggestion.
 *
 * 2023, John "Snowdrama" Close
 * Used in our Ludum Dare game, and made some changes to allow more flexible useage. 
 * 
 * Changed name to Messages as this as I use this in to Godot as well 
 * and godot has something called messages but it's different.
 * ========================================================================================
 * Inspired by StrangeIOC, minus the clutter.
 * Based on http://wiki.unity3d.com/index.php/CSharpMessenger_Extended
 * Converted to use strongly typed parameters and prevent use of strings as ids.
 *
 * Supports up to 3 parameters. More than that, and you should probably use a VO.
 *
 * Usage:
 *    1) Define your class, eg:
 *          ScoreMessage : AMessage<int> {}
 *    2) Add listeners on portions that should react, eg on Awake():
 *          Messages.Get<ScoreMessage>().AddListener(OnScore);
 *    3) Dispatch, eg:
 *          Messages.Get<ScoreMessage>().Dispatch(userScore);
 *    4) Don'time forget to remove the listeners upon destruction! Eg on OnDestroy():
 *          Messages.Get<ScoreMessage>().RemoveListener(OnScore);
 *    5) If you don'time want to use global Messages, you can have your very own MessageHub
 *       instance in your class
 *
 * ========================================================================================
 * Added by Snowdrama 10/24/2023:
 *
 * MessageHubs can now be created similar to how messages work, it uses name strings for the hubs name
 * For example using something like a unique entity Id, or something like "PlayerOne"
 *      1) Get a reference to your hub:
 *          playerHub = Messages.GetHub("PlayerOne");
 *      2) Message refrences should be gotten through the unique hub instead of the globalHub:
 *          scoreMessage = playerHub.Get<ScoreMessage>();
 *
 * MessageHubs now can have message usages returned to them so we can know if the message is
 * no longer being used, and if the hub also has no users.
 * 
 * Short example of using and returning messages and hubs.
 * 
 * public class ScoreMessage : AMessage<int> { }
 * public class GameOverMessage : AMessage { }
 * public class Player : MonoBehaviour
 * {
 *     MessageHub playerHub;
 *     ScoreMessage scoreMessage;
 *     void OnEnable()
 *     {
 *         playerHub = Messages.GetHub("PlayerOne");
 *         scoreMessage = playerHub.Get<ScoreMessage>();
 *         //Note this is a global message
 *         gameOverMessage = Messages.Get<GameOverMessage>(); 
 *     }
 * 
 *     void OnDisable()
 *     {
 *         //remember to return the score message to the hub!
 *         playerHub.Return<ScoreMessage>();
 * 
 *         //also return the hub referene!
 *         Messages.ReturnHub("PlayerOne");
 *
 *         //global messages are just returned to the globalHub
 *         Messages.Return<GameOverMessage>();
 *     }
 * }
 * 
 * This could be converted into using objects however I've found this to be messy
 * in cases where you procedurally generate the hub's parent object. 
 * 
 * however the solution for normal messages does prevent the use of string keys which 
 * does mean more typesafety and prevents typos which is ideal
 * 
 * Maybe at some point we consider this, or some kind of variant that works with a
 * type AND a string. 
 * 
 * public class Player1Hub : AMessageHub {}
 * 
 * Messages.GetHub<Player1Hub>().Get<ScoreMessage>();
 * 
 * vs.
 * 
 * Messages.GetHub("Player1Hub").Get<ScoreMessage>();
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Base interface for Messages
/// </summary>
public interface IMessage
{
    void AddUser();
    void RemoveUser();
    int GetUserCount();
}

/// <summary>
/// Messages main facade class for global, game-wide messages
/// </summary>
public static class Messages
{
    private static Dictionary<string, MessageHub> messageHubs = new Dictionary<string, MessageHub>();
    private static readonly MessageHub globalHub = new MessageHub();

    public static SType Get<SType>() where SType : IMessage, new()
    {
        return globalHub.Get<SType>();
    }

    public static void Return<SType>() where SType : IMessage, new()
    {
        globalHub.Return<SType>();
    }

    //This creates a named MessageHub which is useful for when
    //Several components share a message type
    //for example a local multiplayer game where each
    //player has a health bar. They can all share a "HealthChanged" message
    //but each use their own hub!
    public static MessageHub GetHub(string hubName)
    {
        if (messageHubs.ContainsKey(hubName))
        {
            messageHubs[hubName].AddUser();
            return messageHubs[hubName];
        }
        var newHub = new MessageHub();
        newHub.AddUser();
        messageHubs.Add(hubName, newHub);
        return newHub;
    }

    //returns a reference of the hub 
    public static void ReturnHub(string hubName)
    {
        //you can only return a use of a hub that exists
        if (messageHubs.ContainsKey(hubName))
        {
            messageHubs[hubName].RemoveUser();
            if (messageHubs[hubName].UserCount == 0)
            {
                //if the hub has no users, then we can safely remove it!
                messageHubs.Remove(hubName);
            }
        }
    }

    //This uses the UserCount Value of a message hub
    //to remove unused messages. This has to be called
    //manually and 
    public static void CleanUpMessageHubs()
    {
        //remove any hubs with 0 messages
        //
        messageHubs = messageHubs.Where(x => x.Value.GetMessageCount() == 0).ToDictionary(x => x.Key, x => x.Value);
    }
}

/// <summary>
/// A hub for Messages you can implement in your classes
/// </summary>
public class MessageHub
{
    public int UserCount { get; private set; }
    private Dictionary<Type, IMessage> messages = new Dictionary<Type, IMessage>();

    /// <summary>
    /// Getter for a ammoMessage of a given type
    /// </summary>
    /// <typeparam name="SType">Type of ammoMessage</typeparam>
    /// <returns>The proper ammoMessage binding</returns>
    public SType Get<SType>() where SType : IMessage, new()
    {
        Type messageType = typeof(SType);
        IMessage message;

        if (messages.TryGetValue(messageType, out message))
        {
            message.AddUser();
            return (SType)message;
        }
        var newMessage = (SType)Bind(messageType);
        newMessage.AddUser();
        return newMessage;
    }

    /// <summary>
    /// Returns a useage of a message, decreasing the users of that message
    /// allowing us to know when the message is no longer in use
    /// </summary>
    /// <typeparam name="SType"></typeparam>
    public void Return<SType>() where SType : IMessage, new()
    {
        Type messageType = typeof(SType);
        IMessage message;

        //obviously we can't return a message that doesn't exist
        if (messages.TryGetValue(messageType, out message))
        {
            //decrease the user count
            message.RemoveUser();

            //if there's no users, remove the whole message
            //so we can dispose of the hub if there's no
            //messages in the hub
            if(message.GetUserCount() == 0)
            {
                messages.Remove(messageType);
            }
        }
    }

    private IMessage Bind(Type messageType)
    {
        IMessage message;
        if (messages.TryGetValue(messageType, out message))
        {
            UnityEngine.Debug.LogError(string.Format("Message already registered for type {0}", messageType.ToString()));
            return message;
        }

        message = (IMessage)Activator.CreateInstance(messageType);
        messages.Add(messageType, message);
        return message;
    }

    private IMessage Bind<T>() where T : IMessage, new()
    {
        return Bind(typeof(T));
    }

    public int GetMessageCount()
    {
        return messages.Count;
    }

    /// <summary>
    /// Called when a user is added to the message
    /// </summary>
    public void AddUser()
    {
        ++UserCount;
    }

    /// <summary>
    /// Get the nuber of message users
    /// </summary>
    /// <returns>The number of message users</returns>
    public int GetUserCount()
    {
        return UserCount;
    }

    /// <summary>
    /// Called when a user is removed from a message
    /// </summary>
    public void RemoveUser()
    {
        --UserCount;
    }
}

/// <summary>
/// Abstract class for Messages, provides hash by type functionality
/// </summary>
public abstract class ABaseMessage : IMessage
{
    public int UserCount {  get; private set; }

    /// <summary>
    /// Called when a user is added to the message
    /// </summary>
    public void AddUser()
    {
        ++UserCount;
    }

    /// <summary>
    /// Get the nuber of message users
    /// </summary>
    /// <returns>The number of message users</returns>
    public int GetUserCount()
    {
        return UserCount;
    }

    /// <summary>
    /// Called when a user is removed from a message
    /// </summary>
    public void RemoveUser()
    {
        --UserCount;
    }
}

/// <summary>
/// Strongly typed messages with no parameters
/// </summary>
public abstract class AMessage : ABaseMessage
{
    private Action callback;

    /// <summary>
    /// Adds a listener to this Message
    /// </summary>
    /// <param name="handler">Method to be called when ammoMessage is fired</param>
    public void AddListener(Action handler)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Assert(handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false).Length == 0,
            "Adding anonymous delegates as Message callbacks is not supported (you wouldn'time be able to unregister them later).");
#endif
        callback += handler;
    }

    /// <summary>
    /// Removes a listener from this Message
    /// </summary>
    /// <param name="handler">Method to be unregistered from ammoMessage</param>
    public void RemoveListener(Action handler)
    {
        callback -= handler;
    }

    /// <summary>
    /// Dispatch this ammoMessage
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
public abstract class AMessage<T> : ABaseMessage
{
    private Action<T> callback;

    /// <summary>
    /// Adds a listener to this Message
    /// </summary>
    /// <param name="handler">Method to be called when ammoMessage is fired</param>
    public void AddListener(Action<T> handler)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Assert(handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false).Length == 0,
            "Adding anonymous delegates as Message callbacks is not supported (you wouldn'time be able to unregister them later).");
#endif
        callback += handler;
    }

    /// <summary>
    /// Removes a listener from this Message
    /// </summary>
    /// <param name="handler">Method to be unregistered from ammoMessage</param>
    public void RemoveListener(Action<T> handler)
    {
        callback -= handler;
    }

    /// <summary>
    /// Dispatch this ammoMessage with 1 parameter
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
public abstract class AMessage<T, U> : ABaseMessage
{
    private Action<T, U> callback;

    /// <summary>
    /// Adds a listener to this Message
    /// </summary>
    /// <param name="handler">Method to be called when ammoMessage is fired</param>
    public void AddListener(Action<T, U> handler)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Assert(handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false).Length == 0,
            "Adding anonymous delegates as Message callbacks is not supported (you wouldn'time be able to unregister them later).");
#endif
        callback += handler;
    }

    /// <summary>
    /// Removes a listener from this Message
    /// </summary>
    /// <param name="handler">Method to be unregistered from ammoMessage</param>
    public void RemoveListener(Action<T, U> handler)
    {
        callback -= handler;
    }

    /// <summary>
    /// Dispatch this ammoMessage
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
public abstract class AMessage<T, U, V> : ABaseMessage
{
    private Action<T, U, V> callback;

    /// <summary>
    /// Adds a listener to this Message
    /// </summary>
    /// <param name="handler">Method to be called when ammoMessage is fired</param>
    public void AddListener(Action<T, U, V> handler)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Assert(handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false).Length == 0,
            "Adding anonymous delegates as Message callbacks is not supported (you wouldn'time be able to unregister them later).");
#endif
        callback += handler;
    }

    /// <summary>
    /// Removes a listener from this Message
    /// </summary>
    /// <param name="handler">Method to be unregistered from ammoMessage</param>
    public void RemoveListener(Action<T, U, V> handler)
    {
        callback -= handler;
    }

    /// <summary>
    /// Dispatch this ammoMessage
    /// </summary>
    public void Dispatch(T arg1, U arg2, V arg3)
    {
        if (callback != null)
        {
            callback(arg1, arg2, arg3);
        }
    }
}

/// <summary>
/// Strongly typed messages with 3 parameter
/// </summary>
/// <typeparam name="T">First parameter type</typeparam>
/// <typeparam name="U">Second parameter type</typeparam>
/// <typeparam name="V">Third parameter type</typeparam>
public abstract class AMessage<T, U, V, W> : ABaseMessage
{
    private Action<T, U, V, W> callback;

    /// <summary>
    /// Adds a listener to this Message
    /// </summary>
    /// <param name="handler">Method to be called when ammoMessage is fired</param>
    public void AddListener(Action<T, U, V, W> handler)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Assert(handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false).Length == 0,
            "Adding anonymous delegates as Message callbacks is not supported (you wouldn'time be able to unregister them later).");
#endif
        callback += handler;
    }

    /// <summary>
    /// Removes a listener from this Message
    /// </summary>
    /// <param name="handler">Method to be unregistered from ammoMessage</param>
    public void RemoveListener(Action<T, U, V, W> handler)
    {
        callback -= handler;
    }

    /// <summary>
    /// Dispatch this ammoMessage
    /// </summary>
    public void Dispatch(T arg1, U arg2, V arg3, W arg4)
    {
        if (callback != null)
        {
            callback(arg1, arg2, arg3, arg4);
        }
    }
}
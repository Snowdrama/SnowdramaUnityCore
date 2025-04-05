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

public interface IMessage
{
    void AddUser();
    void RemoveUser();
    int GetUserCount();
}

public class Messages
{
    private static Dictionary<string, MessageHub> messageHubs = new Dictionary<string, MessageHub>();
    private static readonly MessageHub globalHub = new MessageHub();

    /// <summary>
    /// Gets a message object of the type passed and adds a user count
    /// 
    /// If you use it imediately and don't store a reference use Messages.GetNoCount
    /// Ex: Messages.GetNoCount<SType>().Dispatch(); <- No reference is stored.
    /// 
    /// This gets it from a global hub, if you need to differentiate it
    /// consider using GetHub("SomeIdentifier").Get<SType>() instead
    /// 
    /// Make sure to return the reference so memory can be freed if no references exist using:
    /// Messages.Return<SType>();
    /// </summary>
    /// <typeparam name="SType"></typeparam>
    public static SType Get<SType>() where SType : IMessage, new()
    {
        return globalHub.Get<SType>();
    }
    /// <summary>
    /// Gets a message object of the type passed DOES NOT add a reference count
    /// 
    /// If you are holding onto a reference use Messages.Get to ensure the memory it isn't freed
    /// Ex: Messages.Get<SType>(); <- Reference is counted.
    /// 
    /// This gets it from a global hub, if you need to differentiate it
    /// consider using GetHub("SomeIdentifier").Get<SType>() instead
    /// </summary>
    /// <typeparam name="SType"></typeparam>
    /// <returns></returns>
    public static SType GetNoCount<SType>() where SType : IMessage, new()
    {
        return globalHub.Get<SType>();
    }
    /// <summary>
    /// Returns a copy of the message, by returning it we can 
    /// manage the memory of used messages, if there's no,
    /// users actively holding a reference. 
    /// </summary>
    /// <typeparam name="SType"></typeparam>
    public static void Return<SType>() where SType : IMessage, new()
    {
        globalHub.Return<SType>();
    }
    /// <summary>
    /// get's a message without changing user count
    /// used for intentionally only getting it to dispatch once
    /// </summary>
    /// <typeparam name="SType">The Type of the message</typeparam>
    /// <returns>The message object</returns>
    public static SType GetOnce<SType>() where SType : IMessage, new()
    {
        return globalHub.GetOnce<SType>();
    }
    public static MessageHub GetHub(string hubName)
    {
        if (messageHubs.ContainsKey(hubName))
        {
            return messageHubs[hubName];
        }
        var newHub = new MessageHub();
        messageHubs.Add(hubName, newHub);
        return newHub;
    }
    public static void ReturnHub(string hubName)
    {
        if (messageHubs.ContainsKey(hubName))
        {
            messageHubs[hubName].RemoveUser();
            if (messageHubs[hubName].UserCount == 0)
            {
                messageHubs.Remove(hubName);
            }
        }
    }
}

public class MessageHub
{
    public int UserCount { get; private set; }
    private Dictionary<Type, IMessage> messages = new Dictionary<Type, IMessage>();

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

    public SType GetNoCount<SType>() where SType : IMessage, new()
    {
        Type messageType = typeof(SType);
        IMessage message;

        if (messages.TryGetValue(messageType, out message))
        {
            return (SType)message;
        }
        var newMessage = (SType)Bind(messageType);
        return newMessage;
    }

    public void Return<SType>() where SType : IMessage, new()
    {
        Type messageType = typeof(SType);
        IMessage message;

        if (messages.TryGetValue(messageType, out message))
        {
            message.RemoveUser();
            if (message.GetUserCount() == 0)
            {
                messages.Remove(messageType);
            }
        }
    }
    /// <summary>
    /// get's a message without changing user count
    /// used for intentionally only getting it to dispatch once
    /// </summary>
    /// <typeparam name="SType">The Type of the message</typeparam>
    /// <returns>The message object</returns>
    public SType GetOnce<SType>() where SType : IMessage, new()
    {
        Type messageType = typeof(SType);
        IMessage message;

        if (messages.TryGetValue(messageType, out message))
        {
            return (SType)message;
        }
        var newMessage = (SType)Bind(messageType);
        return newMessage;
    }

    public IMessage Bind(Type messageType)
    {
        IMessage message;

        if (messages.TryGetValue(messageType, out message))
        {
            return message;
        }

        message = (IMessage)Activator.CreateInstance(messageType);
        messages.Add(messageType, message);
        return message;
    }
    public void AddUser()
    {
        ++UserCount;
    }
    public int GetUserCount()
    {
        return UserCount;
    }
    public void RemoveUser()
    {
        --UserCount;
    }
}
public abstract class ABaseMessage : IMessage
{
    public int UserCount { get; private set; }
    public void AddUser()
    {
        ++UserCount;
    }
    public int GetUserCount()
    {
        return UserCount;
    }
    public void RemoveUser()
    {
        --UserCount;
    }
}

public abstract class AMessage : ABaseMessage
{
    private Action callback;

    public void AddListener(Action handler)
    {
        callback += handler;
    }

    public void RemoveListener(Action handler)
    {
        callback -= handler;
    }

    public void Dispatch()
    {
        callback?.Invoke();
    }
}
public abstract class AMessage<T> : ABaseMessage
{
    private Action<T> callback;

    public void AddListener(Action<T> handler)
    {
        callback += handler;
    }

    public void RemoveListener(Action<T> handler)
    {
        callback -= handler;
    }

    public void Dispatch(T arg1)
    {
        callback?.Invoke(arg1);
    }
}
public abstract class AMessage<T, U> : ABaseMessage
{
    private Action<T, U> callback;

    public void AddListener(Action<T, U> handler)
    {
        callback += handler;
    }

    public void RemoveListener(Action<T, U> handler)
    {
        callback -= handler;
    }

    public void Dispatch(T arg1, U arg2)
    {
        callback?.Invoke(arg1, arg2);
    }
}
public abstract class AMessage<T, U, V> : ABaseMessage
{
    private Action<T, U, V> callback;

    public void AddListener(Action<T, U, V> handler)
    {
        callback += handler;
    }

    public void RemoveListener(Action<T, U, V> handler)
    {
        callback -= handler;
    }

    public void Dispatch(T arg1, U arg2, V arg3)
    {
        callback?.Invoke(arg1, arg2, arg3);
    }
}

public abstract class AMessage<T, U, V, W> : ABaseMessage
{
    private Action<T, U, V, W> callback;

    public void AddListener(Action<T, U, V, W> handler)
    {
        callback += handler;
    }

    public void RemoveListener(Action<T, U, V, W> handler)
    {
        callback -= handler;
    }

    public void Dispatch(T arg1, U arg2, V arg3, W arg4)
    {
        callback?.Invoke(arg1, arg2, arg3, arg4);
    }
}
public abstract class AMessage<T1, T2, T3, T4, T5> : ABaseMessage
{
    private Action<T1, T2, T3, T4, T5> callback;

    public void AddListener(Action<T1, T2, T3, T4, T5> handler)
    {
        callback += handler;
    }

    public void RemoveListener(Action<T1, T2, T3, T4, T5> handler)
    {
        callback -= handler;
    }

    public void Dispatch(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        callback?.Invoke(arg1, arg2, arg3, arg4, arg5);
    }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

#region Options

[System.Serializable]
public class Options2
{
    private static Dictionary<Type, IOptionsContainer> data = new Dictionary<Type, IOptionsContainer>();

    public static void Set<T>(string key, T value)
    {
        //make sure we have a container
        if (!data.TryGetValue(typeof(T), out var container))
        {
            container = new OptionsContainer<T>();
            data.Add(typeof(T), container);
        }

        //now that we know this is set, we call set key, value
        ((OptionsContainer<T>)container).Set(key, value);
        Save();
    }
    public static T Get<T>(string key, T defaultValue)
    {
        //If we don't have a container for this type
        //then we definitely don't have a value for this key
        if (!data.TryGetValue(typeof(T), out var container))
        {
            return defaultValue;
        }

        //now that we know this is set, we call set key, value
        return ((OptionsContainer<T>)container).Get(key, defaultValue);
    }

    public static void SetCallback<T>(string key, Action<T> callback)
    {
        //make sure we have a container
        if (!data.TryGetValue(typeof(T), out var container))
        {
            container = new OptionsContainer<T>();
            data.Add(typeof(T), container);
        }

        //now that we know this is set, we call set key, value
        ((OptionsContainer<T>)container).SetCallback(key, callback);
    }

    public static void UnsetCallback<T>(string key, Action<T> callback)
    {
        //make sure we have a container
        if (!data.TryGetValue(typeof(T), out var container))
        {
            container = new OptionsContainer<T>();
            data.Add(typeof(T), container);
        }

        //now that we know this is set, we call set key, value
        ((OptionsContainer<T>)container).UnsetCallback(key, callback);
    }
    public static void Save()
    {
        var json = new JObject();
        foreach (var (type, container) in data)
        {
            //TODO: make sure this produces like "Vector2" and not the fully qualified name??
            var typeName = type.ToString();
            if (typeName == null)
            {
                continue;
            }

            //get the dictionary of KVP as a JToken
            json[typeName] = container.GetValuesToken();
        }
        //Format the string
        var jsonString = json.ToString(Formatting.Indented);

        //TODO: Actually write stuff
        //File.WriteAllText($"{Application.persistentDataPath}/Options.json", jsonString);
    }

    public static void Load(string jsonString)
    {
        var root = JObject.Parse(jsonString);
        var loadedData = new Dictionary<Type, IOptionsContainer>();

        foreach (var property in root.Properties())
        {
            var type = Type.GetType(property.Name, throwOnError: false);
            if (type == null)
            {
                continue;
            }

            var containerType = typeof(OptionsContainer<>).MakeGenericType(type);
            var container = (IOptionsContainer)property.Value.ToObject(containerType);

            if (container != null)
            {
                container.LoadValues(property.Value);
                loadedData[type] = container;
            }
        }

        data = loadedData;
    }
    public static void Clear()
    {
        data.Clear();
    }
}

#endregion

#region Interfaces
public interface IOptionsValue { }
public interface IOptionsContainer
{
    JToken GetValuesToken();
    void LoadValues(JToken token);
}
#endregion

#region Classes
[System.Serializable]
public class OptionsValue<T> : IOptionsValue
{
    public string Key { get; private set; }
    public T Value { get; private set; }
    [NonSerialized] public Action<T> OnChanged;
    public OptionsValue(string key, T value, Action<T> callback = null)
    {
        this.Key = key;
        this.Value = value;
        if (callback != null)
        {
            OnChanged += callback;
        }
    }
    public void Set(T value) { this.Value = value; }
    public T Get() { return this.Value; }
    public void SetCallback(Action<T> callback) { OnChanged += callback; }
    public void UnsetCallback(Action<T> callback) { OnChanged -= callback; }
}

[System.Serializable]
public class OptionsContainer<T> : IOptionsContainer
{
    public Dictionary<string, OptionsValue<T>> values = new Dictionary<string, OptionsValue<T>>();
    //[NonSerialized] public Dictionary<string, Action<T>> callbacks = new Dictionary<string, Action<T>>();

    /// <summary>
    /// Sets the value of the option by the key
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Set(string key, T value)
    {

        if (values.ContainsKey(key))
        {
            values[key].Set(value);
        }
        else
        {
            values.Add(key, new OptionsValue<T>(key, value));
        }

        if (values.ContainsKey(key))
        {
            values[key]?.OnChanged?.Invoke(value);
        }
    }

    /// <summary>
    /// Gets the value of the option by the key
    /// </summary>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public T Get(string key, T defaultValue)
    {
        if (!values.ContainsKey(key)) { return defaultValue; }
        return values[key].Value;
    }

    /// <summary>
    /// Sets a callback that is called when the option with the key is changed
    /// </summary>
    /// <param name="key">The key to lisen for changes</param>
    /// <param name="callback">The callback to execute</param>
    public void SetCallback(string key, Action<T> callback)
    {
        if (values.ContainsKey(key))
        {
            values[key].SetCallback(callback);
        }
    }


    /// <summary>
    /// Unsets the callback
    /// </summary>
    /// <param name="key">The option key</param>
    /// <param name="callback">The callback function to unset</param>
    public void UnsetCallback(string key, Action<T> callback)
    {
        if (values.ContainsKey(key))
        {
            values[key].UnsetCallback(callback);
        }
    }


    /// <summary>
    /// Creates a JToken from the Dictionary<string, T>()
    /// </summary>
    /// <returns>A JToken from the Dictionary<string, T>()</returns>
    public JToken GetValuesToken()
    {
        return JToken.FromObject(values);
    }

    /// <summary>
    /// Loads the token into the object a Dictionary<string, T>()
    /// 
    /// Or creates a new one if null
    /// </summary>
    /// <param name="token">The token containing the Dictionary</param>
    public void LoadValues(JToken token)
    {
        values = token.ToObject<Dictionary<string, OptionsValue<T>>>() ?? new Dictionary<string, OptionsValue<T>>();
    }
}
#endregion

using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

[System.Serializable]
public class OptionsContainer<T> : IOptionsContainer
{
    public Dictionary<string, T> values = new Dictionary<string, T>();
    [NonSerialized] public Dictionary<string, Action<T>> callbacks = new Dictionary<string, Action<T>>();

    public void Set(string key, T value)
    {
        if (callbacks.ContainsKey(key))
        {
            callbacks[key]?.Invoke(value);
        }

        if (values.ContainsKey(key))
        {
            values[key] = value;
        }
        else
        {
            values.Add(key, value);
        }
    }
    public T Get(string key, T defaultValue)
    {
        if (!values.ContainsKey(key)) { return defaultValue; }
        return values[key];
    }
    public void SetCallback(string key, Action<T> callback)
    {

        if (!callbacks.ContainsKey(key))
        {
            callbacks.Add(key, callback);
        }
        else
        {
            callbacks[key] += callback;
        }
    }
    public void UnsetCallback(string key, Action<T> callback)
    {
        if (callbacks.ContainsKey(key))
        {
            callbacks[key] -= callback;
        }
    }
    public JToken GetValuesToken() => JToken.FromObject(values);

    public void LoadValues(JToken token)
    {
        values = token.ToObject<Dictionary<string, T>>()
                 ?? new Dictionary<string, T>();
    }
}
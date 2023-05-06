using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Snowdrama.Signals
{

    public class SignalConstants
    {
        public static readonly bool DISPLAY_LOGS = false;
    }
    /// <summary>
    /// Example use case
    /// A player picks up a coin, the player should NOT know about the game state BUT
    /// the player CAN know there's a signal to call when a coin is picked up. 
    /// 
    /// This is picked up by a Score Manager that's listening for the coin pickup
    /// The score manager can update it's score variable to reflect the score
    /// 
    /// The score manager than fires a "Score Changed" event that the UI is listening
    /// for so that it can update the score text!
    /// </summary>

    [CreateAssetMenu(fileName = "Signal", menuName = "Snowdrama/DataTypes/Signals/Signal", order = 0)]
    public class Signal : ScriptableObject
    {
        private Action action;

        public void RegisterListener(Action listener)
        {
            if (SignalConstants.DISPLAY_LOGS)
            {
                Debug.Log($"Registering new listener to {this.name}", this);
            }
            action += listener;
        }

        public void RemoveListener(Action listener)
        {
            if (SignalConstants.DISPLAY_LOGS)
            {
                Debug.Log($"Removing listener from {this.name}", this);
            }
            action -= listener;
        }

        public void Invoke()
        {
            if (SignalConstants.DISPLAY_LOGS)
            {
                Debug.Log($"Invoking Signal {this.name}", this);
            }
            action?.Invoke();
        }
    }


    public class Signal<T> : ScriptableObject
    {
        private Action<T> action;

        public void RegisterListener(Action<T> listener)
        {
            if (SignalConstants.DISPLAY_LOGS)
            {
                Debug.Log($"Registering new listener to {this.name}", this);
            }
            action += listener;
        }

        public void RemoveListener(Action<T> listener)
        {
            if (SignalConstants.DISPLAY_LOGS)
            {
                Debug.Log($"Removing listener from {this.name}", this);
            }
            action -= listener;
        }

        public void Invoke(T value)
        {
            if (SignalConstants.DISPLAY_LOGS)
            {
                Debug.Log($"Invoking Signal {this.name}, with value {value}", this);
            }
            action?.Invoke(value);
        }
    }


    [CreateAssetMenu(fileName = "Signal Int", menuName = "Snowdrama/DataTypes/Signals/Signal Int", order = 1)]
    public class Signal_Int : Signal<int> { }
    [CreateAssetMenu(fileName = "Signal Long", menuName = "Snowdrama/DataTypes/Signals/Signal Long", order = 2)]
    public class Signal_Long : Signal<long> { }
    [CreateAssetMenu(fileName = "Signal Float", menuName = "Snowdrama/DataTypes/Signals/Signal Float", order = 3)]
    public class Signal_Float : Signal<float> { }
    [CreateAssetMenu(fileName = "Signal Double", menuName = "Snowdrama/DataTypes/Signals/Signal Double", order = 4)]
    public class Signal_Double : Signal<double> { }
    [CreateAssetMenu(fileName = "Signal Bool", menuName = "Snowdrama/DataTypes/Signals/Signal Bool", order = 5)]
    public class Signal_Bool : Signal<bool> { }
    [CreateAssetMenu(fileName = "Signal String", menuName = "Snowdrama/DataTypes/Signals/Signal String", order = 6)]
    public class Signal_String : Signal<string> { }
    [CreateAssetMenu(fileName = "Signal Vector2", menuName = "Snowdrama/DataTypes/Signals/Signal Vector2", order = 7)]
    public class Signal_Vector2 : Signal<Vector2> { }
    [CreateAssetMenu(fileName = "Signal Vector2Int", menuName = "Snowdrama/DataTypes/Signals/Signal Vector2Int", order = 8)]
    public class Signal_Vector2Int : Signal<Vector2Int> { }
    [CreateAssetMenu(fileName = "Signal Vector3", menuName = "Snowdrama/DataTypes/Signals/Signal Vector3", order = 9)]
    public class Signal_Vector3 : Signal<Vector3> { }
    [CreateAssetMenu(fileName = "Signal Vector3Int", menuName = "Snowdrama/DataTypes/Signals/Signal Vector3Int", order = 10)]
    public class Signal_Vector3Int : Signal<Vector3Int> { }
    [CreateAssetMenu(fileName = "Signal Vector4", menuName = "Snowdrama/DataTypes/Signals/Signal Vector4", order = 11)]
    public class Signal_Vector4 : Signal<Vector4> { }
    [CreateAssetMenu(fileName = "Signal Color", menuName = "Snowdrama/DataTypes/Signals/Signal Color", order = 12)]
    public class Signal_Color : Signal<Color> { }
}



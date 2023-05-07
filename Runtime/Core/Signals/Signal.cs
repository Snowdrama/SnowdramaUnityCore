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
}



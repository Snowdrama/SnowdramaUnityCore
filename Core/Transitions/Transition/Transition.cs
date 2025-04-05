using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Snowdrama.Transition
{
    public abstract class Transition : MonoBehaviour
    {
        [Tooltip("The Key that allows you to ")]
        [SerializeField] public string transitionName;
        public abstract void UpdateTransition(float transitionValue, bool hiding);
        public virtual void OnTransitionStarted() { }
        public virtual void OnHideStarted() {}
        public virtual void OnHideCompleted() {}
        public virtual void OnScenesLoaded() {}
        public virtual void OnShowStarted() {}
        public virtual void OnShowCompleted() {}
        public virtual void OnTransitionComplete() {}

        public virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(transitionName))
            {
                Debug.Log($"Updating transition name to: {transitionName}");
                transitionName = this.gameObject.name;
            }
        }
    }
}
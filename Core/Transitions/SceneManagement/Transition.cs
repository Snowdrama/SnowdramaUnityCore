using UnityEngine;

namespace Snowdrama.Transition
{
    public abstract class Transition : MonoBehaviour
    {
        [Tooltip("The Key that allows you to filter transitions when choosing to transition")]
        public string transitionName
        {
            get { return gameObject.name; }
        }
        public abstract void UpdateTransition(float transitionValue, bool hiding);
        public virtual void OnTransitionStarted() { }
        public virtual void OnHideStarted() { }
        public virtual void OnHideCompleted() { }
        public virtual void OnScenesLoaded() { }
        public virtual void OnShowStarted() { }
        public virtual void OnShowCompleted() { }
        public virtual void OnTransitionComplete() { }

        public virtual void OnValidate() { }
    }
}
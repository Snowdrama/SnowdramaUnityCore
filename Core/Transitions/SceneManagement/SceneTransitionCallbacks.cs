using System;

namespace Snowdrama.Transition
{
    public struct SceneTransitionCallbacks
    {
        public Action onTransitionStarted;
        public Action onHideStarted;
        public Action onHideCompleted;
        public Action onScenesLoaded;
        public Action onShowStarted;
        public Action onShowCompleted;
        public Action onTransitionCompltete;
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Snowdrama.Transition
{
    public class TransitionDriver : MonoBehaviour
    {
        [SerializeField]
        private Transition currentTransition;
        [SerializeField]
        private enum TransitionState
        {
            None,
            Start,
            HidingScene,
            SceneHidden,
            WaitingForLoad,
            RevealingScene,
            End,
        }
        [SerializeField]
        private TransitionState transitionState;

        private void Start()
        {
            TransitionController.startTransition += StartTransition;
            TransitionController.sceneLoaded += SceneLoaded;
        }

        private void Update()
        {

            switch (transitionState)
            {
                case TransitionState.None:
                    break;
                case TransitionState.Start:
                    break;
                case TransitionState.HidingScene:
                    currentTransition.HideScene(SceneHidden);
                    break;
                case TransitionState.SceneHidden:
                    transitionState = TransitionState.WaitingForLoad;
                    break;
                case TransitionState.WaitingForLoad:
                    break;
                case TransitionState.RevealingScene:
                    currentTransition.ShowScene(SceneShown);
                    break;
                case TransitionState.End:
                    transitionState = TransitionState.None;
                    break;
            }
        }

        public void SceneHidden()
        {
            transitionState = TransitionState.SceneHidden;
            TransitionController.SceneHidden();
        }
        public void SceneShown()
        {
            transitionState = TransitionState.End;
            TransitionController.TransitionComplete();
        }

        public void StartTransition()
        {
            transitionState = TransitionState.HidingScene;
        }


        public void SceneLoaded()
        {
            transitionState = TransitionState.RevealingScene;
        }
    }
}
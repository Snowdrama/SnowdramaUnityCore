using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Snowdrama.Transition
{
    public class SetAllowedTransitionsMessage : AMessage<List<string>> { }
    public class StartHideTransitionMessage : AMessage<Action, float> { }
    public class StartShowTransitionMessage : AMessage<Action, float> { }
    [ExecuteAlways]
    public class TransitionDriver : MonoBehaviour
    {
        [SerializeField] private string MessageHubName = "TransitionDriver";
        private MessageHub TransitionMessageHub;

        private StartHideTransitionMessage StartHideTransitionMessage;
        private StartShowTransitionMessage StartShowTransitionMessage;
        private SetAllowedTransitionsMessage SetAllowedTransitionsMessage;

        [SerializeField]
        private GameObject transitionCanvas;

        [SerializeField]
        private bool pauseTimeDuringTransition = false;

        [SerializeField]
        private Dictionary<string, Transition> transitions = new Dictionary<string, Transition>();
        [SerializeField]
        private Transition currentTransition;

        [SerializeField] private TransitionState state;

        [Header("Debug")]
        [SerializeField, Range(0, 1)] private float transitionValue;
        [SerializeField, EditorReadOnly] private float transitionSpeed;
        [SerializeField, EditorReadOnly] private List<string> debugTransitionNameKeys = new List<string>();
        [SerializeField, EditorReadOnly] private List<string> currentAllowedTransitions;

        private static SceneTransitionCallbacks transitionCallbacks;
        private void OnEnable()
        {
            TransitionMessageHub = Messages.GetHub(MessageHubName);
            StartHideTransitionMessage = TransitionMessageHub.Get<StartHideTransitionMessage>();
            StartShowTransitionMessage = TransitionMessageHub.Get<StartShowTransitionMessage>();
            SetAllowedTransitionsMessage = TransitionMessageHub.Get<SetAllowedTransitionsMessage>();

            StartHideTransitionMessage.AddListener(StartHideScreen);
            StartShowTransitionMessage.AddListener(StartShowScreen);
            SetAllowedTransitionsMessage.AddListener(SetAllowedTransitions);
        }

        private void OnDisable()
        {
            StartHideTransitionMessage.RemoveListener(StartHideScreen);
            StartShowTransitionMessage.RemoveListener(StartShowScreen);
            SetAllowedTransitionsMessage.RemoveListener(SetAllowedTransitions);
            TransitionMessageHub.Return<StartHideTransitionMessage>();
            TransitionMessageHub.Return<StartShowTransitionMessage>();
            TransitionMessageHub.Return<SetAllowedTransitionsMessage>();
            Messages.ReturnHub(MessageHubName);
        }

        private void Start()
        {
            FindTransitions();
            transitionCanvas?.SetActive(false);
            currentTransition?.gameObject?.SetActive(false);
        }
        private void OnValidate()
        {
            FindTransitions();
        }

        private void FindTransitions()
        {
            if (transitionCanvas == null)
            {
                //attmpt to get the canvas if we didn't set it explicitly
                transitionCanvas = this.transform.GetChild(0).gameObject;
            }

            transitions.Clear();
            debugTransitionNameKeys.Clear();
            for (int i = 0; i < transitionCanvas.transform.childCount; i++)
            {
                var child = transitionCanvas.transform.GetChild(i);
                var transitionElement = child.GetComponent<Transition>();
                if (transitionElement != null)
                {
                    //set them all to false
                    transitionElement.gameObject.SetActive(false);
                    transitions.Add(transitionElement.transitionName, transitionElement);
                    debugTransitionNameKeys.Add(transitionElement.transitionName);
                }
            }

            if (transitions.Count == 0)
            {
                Debug.LogWarning($"Found 0 transitions, you need at least 1 transition that's a child of the Transition Driver");
            }

            ValidateTransition();
        }

        private void ValidateTransition()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            if (transitions.Keys.Count == 0)
            {
                Debug.LogError($"Transition list has 0 transitions, one transition is required", this.gameObject);
                return;
            }
            if (currentTransition == null)
            {
                currentTransition = transitions[transitions.Keys.First()];
            }
        }


        private void ChooseTransition(List<string> allowedTransitions)
        {
            if (allowedTransitions.Count == 0)
            {
                //if we don't allow any choose at random
                currentTransition = transitions.GetRandom().Value;
            }
            else
            {
                //otherwise get a random one from the list
                var listOfAllowedTransitions = transitions.Where(x => allowedTransitions.Contains(x.Key)).ToList();
                currentTransition = listOfAllowedTransitions.GetRandom().Value;
            }
        }

        public void SetAllowedTransitions(List<string> allowedTransitions)
        {
            currentAllowedTransitions = allowedTransitions;
        }

        public async void StartHideScreen(Action onSceneHidden, float hideTime)
        {
            ChooseTransition(currentAllowedTransitions);

            await HideScene(hideTime); //wait until the scene is hidden
            onSceneHidden?.Invoke(); //call the allback
        }
        private async Awaitable HideScene(float hideTime)
        {
            transitionCanvas?.SetActive(true);
            currentTransition?.gameObject?.SetActive(true);
            transitionValue = 0.0f;
            while (transitionValue < 1.0f)
            {
                state = TransitionState.HidingScene;
                transitionValue += Time.deltaTime * hideTime.CreateSpeedFromTime();
                currentTransition?.UpdateTransition(transitionValue, true);
                await Awaitable.NextFrameAsync();
            }
        }

        public async void StartShowScreen(Action onSceneShown, float showTime)
        {
            await ShowScene(showTime);
            onSceneShown?.Invoke();
        }

        private async Awaitable ShowScene(float showTime)
        {
            transitionValue = 1.0f;
            while (transitionValue > 0.0f)
            {
                state = TransitionState.ShowingScene;
                transitionValue -= Time.deltaTime * showTime.CreateSpeedFromTime();
                currentTransition?.UpdateTransition(transitionValue, false);
                await Awaitable.NextFrameAsync();
            }
            transitionCanvas?.SetActive(false);
            currentTransition?.gameObject?.SetActive(false);
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                //This is debug plz just force visible
                if (transitionValue > 0)
                {
                    transitionCanvas?.SetActive(true);
                    currentTransition?.gameObject?.SetActive(true);
                }
                else
                {
                    transitionCanvas?.SetActive(false);
                    currentTransition?.gameObject?.SetActive(false);
                }
                currentTransition?.UpdateTransition(transitionValue, (state == TransitionState.HidingScene) ? true : false);
            }
        }
    }
}
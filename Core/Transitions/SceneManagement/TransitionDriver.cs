using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Snowdrama.Transition
{
    public class StartHideTransitionMessage : AMessage<float, float, List<string>, Action, Action> { }
    public class StartShowTransitionMessage : AMessage<float, Action> { }
    [ExecuteAlways]
    public class TransitionDriver : MonoBehaviour
    {
        [SerializeField] private string MessageHubName = "TransitionDriver";
        private MessageHub TransitionMessageHub;

        private StartHideTransitionMessage StartHideTransitionMessage;
        private StartShowTransitionMessage StartShowTransitionMessage;

        [SerializeField] private GameObject transitionCanvas;
        [SerializeField] private Transition currentTransition;


        [SerializeField] private bool pauseTimeDuringTransition = false;

        private Dictionary<string, Transition> transitions = new Dictionary<string, Transition>();

        [SerializeField] private TransitionState state;

        [Header("Debug")]
        [SerializeField, Range(0, 1)] private float debugTransitionValue;
        [SerializeField, EditorReadOnly] private float transitionSpeed;
        [SerializeField, EditorReadOnly] private List<string> debugTransitionNameKeys = new List<string>();
        //[SerializeField, EditorReadOnly] private List<string> currentAllowedTransitions;

        private static SceneTransitionCallbacks transitionCallbacks;
        private void OnEnable()
        {
            TransitionMessageHub = Messages.GetHub(MessageHubName);
            StartHideTransitionMessage = TransitionMessageHub.Get<StartHideTransitionMessage>();
            StartShowTransitionMessage = TransitionMessageHub.Get<StartShowTransitionMessage>();

            StartHideTransitionMessage.AddListener(HideScene);
            StartShowTransitionMessage.AddListener(ShowScreen);
        }

        private void OnDisable()
        {
            StartHideTransitionMessage.RemoveListener(HideScene);
            StartShowTransitionMessage.RemoveListener(ShowScreen);
            TransitionMessageHub.Return<StartHideTransitionMessage>();
            TransitionMessageHub.Return<StartShowTransitionMessage>();
            Messages.ReturnHub(MessageHubName);
        }

        private void Start()
        {
            FindTransitions();
            //transitionCanvas?.SetActive(false);
            //currentTransition?.gameObject?.SetActive(false);
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


        private Transition ChooseTransition(List<string> allowedTransitions)
        {
            if (allowedTransitions.Count == 0)
            {
                //if we don't allow any choose at random
                return transitions.GetRandom().Value;
            }
            else
            {
                //otherwise get a random one from the list
                var listOfAllowedTransitions = transitions.Where(x => allowedTransitions.Contains(x.Key)).ToList();
                return listOfAllowedTransitions.GetRandom().Value;
            }
        }

        private async void HideScene(
            float hideTime,
            float fakeLoadTime,
            List<string> allowedTransitions,
            Action sceneHiddenCallback,
            Action fakeLoadComplete
        )
        {
            currentTransition = ChooseTransition(allowedTransitions);

            //activate the transition and canvas
            transitionCanvas?.SetActive(true);
            currentTransition?.gameObject.SetActive(true);

            float currentHideTime = hideTime;
            float speed = hideTime.CreateSpeedFromTime();
            float transitionValue = 0.0f;

            while (transitionValue < 1.0f)
            {
                transitionValue += Time.deltaTime * speed;
                currentTransition?.UpdateTransition(transitionValue, true);
                await Awaitable.NextFrameAsync();
            }

            sceneHiddenCallback?.Invoke();

            while (fakeLoadTime > 0.0f)
            {
                fakeLoadTime -= Time.deltaTime;
                await Awaitable.NextFrameAsync();
            }

            fakeLoadComplete?.Invoke();
        }

        public async void ShowScreen(
            float showTime,
            Action sceneShownCallback
        )
        {
            float currentShowTime = showTime;
            float speed = showTime.CreateSpeedFromTime();
            float transitionValue = 1.0f;

            transitionCanvas?.SetActive(true);
            currentTransition?.gameObject.SetActive(true);

            while (transitionValue > 0.0f)
            {
                state = TransitionState.ShowingScene;
                transitionValue -= Time.deltaTime * showTime.CreateSpeedFromTime();
                currentTransition?.UpdateTransition(transitionValue, false);
                await Awaitable.NextFrameAsync();
            }
            transitionCanvas?.SetActive(false);
            currentTransition?.gameObject?.SetActive(false);

            sceneShownCallback?.Invoke();
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                //currentTransition?.gameObject?.SetActive(true);
            }
            else if (!Application.isPlaying)
            {
                //This is debug plz just force visible
                if (debugTransitionValue > 0)
                {
                    transitionCanvas?.SetActive(true);
                    currentTransition?.gameObject?.SetActive(true);
                }
                else
                {
                    transitionCanvas?.SetActive(false);
                    currentTransition?.gameObject?.SetActive(false);
                }
                currentTransition?.UpdateTransition(debugTransitionValue, (state == TransitionState.HidingScene) ? true : false);
            }
        }
    }
}
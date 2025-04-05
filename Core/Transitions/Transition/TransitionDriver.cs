using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Snowdrama.Transition
{
    [ExecuteAlways]
    public class TransitionDriver : MonoBehaviour
    {
        [SerializeField]
        private GameObject transitionCanvas;

        [SerializeField]
        private bool pauseTimeDuringTransition = false;

        [SerializeField]
        private Dictionary<string, Transition> transitions = new Dictionary<string, Transition>();
        [SerializeField]
        private Transition currentTransition;

        private bool hiding;

        [Header("Debug")]
        [SerializeField, Range(0, 1)]
        public float debugTransitionValue;
        public List<string> debugTransitionNameKeys = new List<string>();
        private void OnEnable()
        {
            SceneController.transitionCallbacks.onTransitionStarted += OnTransitionStarted;
            SceneController.transitionCallbacks.onHideStarted += OnHideStarted;
            SceneController.transitionCallbacks.onHideCompleted += OnHideCompleted;
            SceneController.transitionCallbacks.onScenesLoaded += OnScenesLoaded;
            SceneController.transitionCallbacks.onShowStarted += OnShowStarted;
            SceneController.transitionCallbacks.onTransitionCompltete += OnTransitionComplete;
            SceneController.transitionCallbacks.onShowCompleted += OnShowCompleted;
        }

        private void OnDisable()
        {
            SceneController.transitionCallbacks.onTransitionStarted -= OnTransitionStarted;
            SceneController.transitionCallbacks.onHideStarted -= OnHideStarted;
            SceneController.transitionCallbacks.onHideCompleted -= OnHideCompleted;
            SceneController.transitionCallbacks.onScenesLoaded -= OnScenesLoaded;
            SceneController.transitionCallbacks.onShowStarted -= OnShowStarted;
            SceneController.transitionCallbacks.onTransitionCompltete -= OnTransitionComplete;
            SceneController.transitionCallbacks.onShowCompleted -= OnShowCompleted;
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

        public void FindTransitions()
        {
            if (transitionCanvas == null)
            {
                transitionCanvas = this.transform.GetChild(0).gameObject;
            }

            transitions.Clear();
            debugTransitionNameKeys.Clear();

            Debug.Log($"Checking for child transitions");
            for (int i = 0; i < transitionCanvas.transform.childCount; i++)
            {
                var child = transitionCanvas.transform.GetChild(i);
                Debug.Log($"Checking {child.name}");

                var transitionElement = child.GetComponent<Transition>();
                if (transitionElement)
                {
                    transitions.Add(transitionElement.transitionName, transitionElement);
                    debugTransitionNameKeys.Add(transitionElement.transitionName);
                }
            }

            ValidateTransition();
        }

        public void OnTransitionStarted()
        {
            if (pauseTimeDuringTransition)
            {
                Time.timeScale = 0;
            }

            ValidateTransition();
            ChooseTransition(SceneController.allowedTransitions);
            currentTransition?.gameObject?.SetActive(true);

            transitionCanvas?.SetActive(true);
            currentTransition?.OnTransitionStarted();
        }
        public void OnHideStarted()
        {
            currentTransition?.OnHideStarted();
            hiding = true;
        }
        public void OnHideCompleted()
        {
            currentTransition?.OnHideCompleted();
            hiding = false;
        }
        public void OnScenesLoaded()
        {
            currentTransition?.OnScenesLoaded();
        }
        public void OnShowStarted()
        {
            if (currentTransition == null)
            {
                currentTransition = transitions[transitions.Keys.First()];
            }
            ChooseTransition(SceneController.allowedTransitions);
            currentTransition?.OnShowStarted();
        }
        public void OnShowCompleted()
        {
            currentTransition?.OnShowCompleted();
        }
        public void OnTransitionComplete()
        {
            if (pauseTimeDuringTransition)
            {
                Time.timeScale = 1;
            }
            transitionCanvas?.SetActive(false);
            currentTransition?.OnTransitionComplete();
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                debugTransitionValue = SceneController.transitionValue;
                currentTransition?.UpdateTransition(SceneController.transitionValue, hiding);
            }
            else
            {
                currentTransition?.UpdateTransition(debugTransitionValue, hiding);
            }
        }


        public void ValidateTransition()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            if(transitions.Keys.Count == 0)
            {
                Debug.LogError($"Transition list has 0 transitions, one transition is required", this.gameObject);
                return;
            }
            if (currentTransition == null)
            {
                currentTransition = transitions[transitions.Keys.First()];
            }
        }


        public void ChooseTransition(List<string> allowedTransitions)
        {

            if (allowedTransitions.Count == 0)
            {
                currentTransition = transitions.GetRandom().Value;
            }
            else
            {
                var listOfAllowedTransitions = transitions.Where(x => allowedTransitions.Contains(x.Key)).ToList();
                currentTransition = listOfAllowedTransitions.GetRandom().Value;
            }
        }

        //public void RandomizeTransition(bool randomize)
        //{
        //    if (randomize && transitionList.Count > 0)
        //    {
        //        transitionIndex = Random.Range(0, transitionList.Count);
        //        currentTransition?.gameObject?.SetActive(false);
        //        currentTransition = transitionList[transitionIndex];
        //        currentTransition?.gameObject?.SetActive(true);
        //    }
        //}
    }
}
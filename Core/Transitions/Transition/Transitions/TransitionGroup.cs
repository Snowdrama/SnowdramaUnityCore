using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Snowdrama.Transition
{


    /// <summary>
    /// TransitionGroup manages several transitions at once
    /// Allowing you to reveal several transition segments in sequence
    /// </summary>
   
    public class TransitionGroup : Transition
    {
        [System.Serializable]
        public class TransitionGroupElement
        {
            public Transition subTransition;
            [Header("Hide")]
            [Range(0.0f, 1.0f)]
            public float hideMinTime;
            [Range(0.0f, 1.0f)]
            public float hideMaxTime;

            [Header("Show")]
            [Range(0.0f, 1.0f)]
            public float showMinTime;
            [Range(0.0f, 1.0f)]
            public float showMaxTime;


            [Header("Time")]
            [Range(0.0f, 1.0f)]
            public float currentTime;
        }
        public List<TransitionGroupElement> transitionElements;

        public override void OnTransitionStarted()
        {

            foreach (var e in transitionElements)
            {
                e.subTransition.OnTransitionStarted();
            }
        }
        public override void OnHideStarted()
        {
            foreach (var e in transitionElements)
            {
                e.subTransition.OnHideStarted();
            }
        }
        public override void OnHideCompleted()
        {
            foreach (var e in transitionElements)
            {
                e.subTransition.OnHideCompleted();
            }
        }
        public override void OnScenesLoaded()
        {
            foreach (var e in transitionElements)
            {
                e.subTransition.OnScenesLoaded();
            }
        }
        public override void OnShowStarted()
        {
            foreach (var e in transitionElements)
            {
                e.subTransition.OnShowStarted();
            }
        }
        public override void OnShowCompleted()
        {
            foreach (var e in transitionElements)
            {
                e.subTransition.OnShowCompleted();
            }
        }
        public override void OnTransitionComplete()
        {
            foreach (var e in transitionElements)
            {
                e.subTransition.OnTransitionComplete();
            }
        }
        public override void UpdateTransition(float transitionValue, bool hiding)
        {
            foreach (var e in transitionElements)
            {
                if (hiding)
                {
                    e.currentTime = Mathf.InverseLerp(e.hideMinTime, e.hideMaxTime, transitionValue);
                }
                else
                {
                    e.currentTime = Mathf.InverseLerp(e.showMinTime, e.showMaxTime, transitionValue);
                }

                e.subTransition.UpdateTransition(e.currentTime, hiding);
            }
        }

        public override void OnValidate()
        {
            base.OnValidate();
            foreach (var e in transitionElements)
            {
                if(e.hideMinTime > e.hideMaxTime)
                {
                    e.hideMaxTime = e.hideMinTime;
                }
            }
        }
    }

}
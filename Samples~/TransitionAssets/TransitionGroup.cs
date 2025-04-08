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
            [SerializeField] Transition _subTransition;
            public Transition SubTransition => _subTransition;
            [Header("Hide")]
            [Range(0.0f, 1.0f)]
            [SerializeField] float _hideMinTime;
            public float HideMinTime
            {
                get
                {
                    return _hideMinTime;
                }
                set
                {
                    _hideMinTime = value;
                    if (_hideMinTime > _hideMaxTime)
                    {
                        _hideMaxTime = _hideMinTime;
                    }
                }
            }
            [Range(0.0f, 1.0f)]
            [SerializeField] float _hideMaxTime;
            public float HideMaxTime { 
                get 
                { 
                    return _hideMaxTime;
                } 
                set 
                { 
                    _hideMaxTime = value;
                    if(_hideMaxTime < _hideMinTime)
                    {
                        _hideMinTime = _hideMaxTime;
                    }
                }
            }

            [Header("Show")]
            [Range(0.0f, 1.0f)]
            [SerializeField] float _showMinTime;
            public float ShowMinTime => _showMinTime;
            [Range(0.0f, 1.0f)]
            [SerializeField] float _showMaxTime;
            public float ShowMaxTime => _showMaxTime;


            [Header("Time")]
            [Range(0.0f, 1.0f)]
            [SerializeField] float _currentTime;
            public float CurrentTime
            {
                get
                {
                    return _currentTime;
                }
                set
                {
                    _currentTime = value;
                }
            }
        }
        [SerializeField] List<TransitionGroupElement> transitionElements;

        public override void OnTransitionStarted()
        {

            foreach (var e in transitionElements)
            {
                e.SubTransition.OnTransitionStarted();
            }
        }
        public override void OnHideStarted()
        {
            foreach (var e in transitionElements)
            {
                e.SubTransition.OnHideStarted();
            }
        }
        public override void OnHideCompleted()
        {
            foreach (var e in transitionElements)
            {
                e.SubTransition.OnHideCompleted();
            }
        }
        public override void OnScenesLoaded()
        {
            foreach (var e in transitionElements)
            {
                e.SubTransition.OnScenesLoaded();
            }
        }
        public override void OnShowStarted()
        {
            foreach (var e in transitionElements)
            {
                e.SubTransition.OnShowStarted();
            }
        }
        public override void OnShowCompleted()
        {
            foreach (var e in transitionElements)
            {
                e.SubTransition.OnShowCompleted();
            }
        }
        public override void OnTransitionComplete()
        {
            foreach (var e in transitionElements)
            {
                e.SubTransition.OnTransitionComplete();
            }
        }
        public override void UpdateTransition(float transitionValue, bool hiding)
        {
            foreach (var e in transitionElements)
            {
                if (hiding)
                {
                    e.CurrentTime = Mathf.InverseLerp(e.HideMinTime, e.HideMaxTime, transitionValue);
                }
                else
                {
                    e.CurrentTime = Mathf.InverseLerp(e.ShowMinTime, e.ShowMaxTime, transitionValue);
                }

                e.SubTransition.UpdateTransition(e.CurrentTime, hiding);
            }
        }
    }

}
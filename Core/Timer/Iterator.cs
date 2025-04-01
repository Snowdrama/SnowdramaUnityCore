using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.Timer
{
    /// <summary>
    /// A tool for calling a function every specific amount of time automatically
    /// 
    /// Example: new Itterator(1.0f) will call OnItterate every second forever
    /// 
    /// Example: new Itterator(1.0f, 10.0f) will call OnItterate 
    /// every second until 10 seconds is reached
    /// then it will call OnDurationCompltete and Stop
    /// </summary>
    public class Iterator
    {
        /// <summary>
        /// Called whenever the itteration time is greater than the set time.
        /// </summary>
        public event Action OnIterate; //called each timer iteration
        /// <summary>
        /// Called only when the _durationTarget of the itterator is complete
        /// </summary>
        public event Action OnDurationComplete;
        /// <summary>
        /// Called when the itterator is started
        /// </summary>
        public event Action OnStart;
        /// <summary>
        /// Called whenever Stop is called and when the _durationTarget is reached.
        /// </summary>
        public event Action OnStop;
        /// <summary>
        /// Called when the timer is paused
        /// </summary>
        public event Action OnPause;
        /// <summary>
        /// Called when the timer is Resumed from pause
        /// </summary>
        public event Action OnResume;

        [Header("Itterate")]
        [SerializeField] private float _iterateTarget;
        [SerializeField] private float _currentItterationTime;
        public float CurrentTime { get { return _currentItterationTime; } }

        [Header("Duration")]
        [SerializeField, Tooltip("How long to iterate for before stopping, less than 0 = forever")] private float _durationTarget; //how long to iterate for before stopping, less than 0 = forever
        [SerializeField] private float _currentTime;
        public float CurrentDuration { get { return _currentTime; } }

       
        [SerializeField] private bool _active;
        public bool Active { get { return _active; } }
        [SerializeField] private bool _paused;
        public bool Paused { get { return _paused; } }
        [SerializeField, Tooltip("Should the itterator initialize started")] private bool autoStart;

        public Iterator(float iterateTime, float duration = -1, bool autoStart = true)
        {
            _currentItterationTime = 0;
            this._iterateTarget = iterateTime;
            this._durationTarget = duration;
            this.autoStart = autoStart;
            if (this.autoStart)
            {
                _active = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iterateTime"></param>
        public void SetNewItterateTime(float iterateTime)
        {
            this._iterateTarget = iterateTime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltaTime"></param>
        public void UpdateTime(float deltaTime)
        {
            if (_active && !_paused)
            {
                _currentItterationTime += deltaTime;
                _currentTime += deltaTime;

                if (_currentItterationTime >= _iterateTarget)
                {
                    OnIterate?.Invoke();
                    _currentItterationTime -= _iterateTarget;
                }
                if (_durationTarget >= 0)
                {
                    if (_currentTime > _durationTarget)
                    {
                        OnDurationComplete?.Invoke();
                        Stop();
                    }
                }
            }
        }
        //starts or resumes the timer depending on if Stop or Pause was used
        public void Start()
        {
            if (!_active)
            {
                _active = true;
                _paused = false;
                OnStart?.Invoke();
                return;
            }

            //if we were active but paused, Start will also unpause
            if (_paused)
            {
                Resume();
            }
        }
        //stops the timer, and resets the current time to 0
        public void Stop()
        {
            if (_active)
            {
                _active = false;
                _paused = false;
                OnStop?.Invoke();
                _currentItterationTime = 0;
                _currentTime = 0;
            }
        }

        /// <summary>
        /// stops the timer and allows resuming from the current time
        /// </summary>
        public void Pause()
        {
            _paused = true;
            OnPause?.Invoke();
        }

        /// <summary>
        /// unpauses the timer if pause was called
        /// </summary>
        public void Resume()
        {
            _paused = false;
            OnResume?.Invoke();
        }

        /// <summary>
        /// Gets the percentage from 0 to 1 as a percentage of current time to the itterators's _durationTarget
        /// </summary>
        /// <returns>A float from 0 to 1</returns>
        public float GetPercentageComplete()
        {
            return _currentTime / _durationTarget;
        }

        public float GetTime()
        {
            return _currentTime;
        }

        public float GetTimeRemaining()
        {
            return _currentTime - _durationTarget;
        }

        /// <summary>
        /// start the timer over from 0, allow passing in a new time.
        /// </summary>
        /// <param name="newTime"></param>
        public void RestartTimer(float newTime = -1)
        {
            _active = true;
            _currentTime = 0;
            SetNewTime(newTime);
        }

        public void SetNewTime(float newTime = -1)
        {
            if (newTime > 0)
            {
                _durationTarget = newTime;
            }
            else
            {
                throw new Exception("New Time must be greater than 0");
            }
        }
    }
}
using System;
using UnityEngine;

namespace Snowdrama.Timer
{
    [System.Serializable]
    public class Timer
    {
        /// <summary>
        /// Called when the timer reaches max
        /// </summary>
        public event Action OnComplete;

        /// <summary>
        /// Called when the timer is started over from 0
        /// </summary>
        public event Action OnRestart;

        /// <summary>
        /// called when the timer is started after stopping or on first start
        /// </summary>
        public event Action OnStart;

        /// <summary>
        /// Called when the timer is paused
        /// </summary>
        public event Action OnPause;
        /// <summary>
        /// Called when the timer is Stopped
        /// </summary>
        public event Action OnStop;

        /// <summary>
        /// Called when the timer is Resumed from pause
        /// </summary>
        public event Action OnResume;

        [SerializeField] private float _currentTime;
        [SerializeField] private float _durationTarget;
        [SerializeField] public bool Active { get; private set; }
        [SerializeField] private bool autoRestart;
        private bool paused;
        public Timer(float time, bool autoStart = false, bool autoRestart = false)
        {
            if (time <= 0)
            {
                time = 0; //prevent negative
                Debug.LogError("Time for timer needs to be greater than 0!");
            }
            _durationTarget = time;
            _currentTime = _durationTarget;
            Active = autoStart;
            this.autoRestart = autoRestart;
        }

        /// <summary>
        /// Updates the timer based on time that has passed since last updated. Will automatically invoke
        /// Action OnComplete if the _currentTime elapses the timer _durationTarget. If AutoRestart is set OnRestart
        /// will be called and the timer will automacially reset the timer to 0 and continue.
        ///  
        /// Note: if the delta time ends up longer than 2 times the _durationTarget and auto restart is enabled OnRestart
        /// will be called each update until the _currentTime becomes less than the timer _durationTarget. 
        /// </summary>
        /// <param name="deltaTime">the delta of time passed since the last timer update</param>
        public void UpdateTime(float deltaTime)
        {
            if (Active)
            {
                _currentTime -= deltaTime;
                if (_currentTime <= 0)
                {
                    _currentTime += _durationTarget;
                    Active = false;
                    if (autoRestart)
                    {
                        OnComplete?.Invoke();
                        RestartTimer();
                    }
                    else
                    {
                        _currentTime = _durationTarget;
                        OnComplete?.Invoke();
                    }
                }
            }
        }
        /// <summary>
        /// Starts or resumes the timer depending on if Stop or Pause was used
        /// </summary>
        public void Start()
        {
            if (!Active)
            {
                OnStart?.Invoke();
                //the timer was paused so resume
                if (paused)
                {
                    paused = false;
                    OnResume?.Invoke();
                }
                Active = true;
            }
        }
        /// <summary>
        /// Stops the timer, and resets the current time to 0
        /// </summary>
        public void Stop()
        {
            if (Active)
            {
                Debug.Log("Stopping!");
                Active = false;
                _currentTime = _durationTarget;
                OnStop?.Invoke();
            }
        }

        /// <summary>
        /// Stops the timer and allows resuming from the current time
        /// </summary>
        public void Pause()
        {
            Active = false;
            paused = true;
            OnPause?.Invoke();
        }

        /// <summary>
        /// returns the current time in seconds
        /// </summary>
        /// <returns>A float representing the time elapsed</returns>
        public float GetTime()
        {
            return _currentTime;
        }

        /// <summary>
        /// returns the percentage from 0 to 1 as a percentage of current time to the timer's _durationTarget
        /// </summary>
        /// <returns>A Float from 0 to 1 representing the total time</returns>
        public float GetPercentageComplete()
        {
            return _currentTime / _durationTarget;
        }

        /// <summary>
        /// returns the amount of time remaining until the timer ends
        /// </summary>
        /// <returns>A Float representing time remaining</returns>
        public float GetTimeRemaining()
        {
            return _durationTarget - _currentTime;
        }

        /// <summary>
        /// start the timer over from 0, allow passing in a new time.
        /// </summary>
        /// <param name="newTime"></param>
        public void RestartTimer(float newTime = -1)
        {
            Active = true;
            OnRestart?.Invoke();
            SetNewTime(newTime);
            _currentTime = _durationTarget;
        }

        public void SetNewTime(float newTime)
        {
            //if new time is less than 0 like -1 we just use the previous time
            if (newTime > 0)
            {
                _durationTarget = newTime;
            }
        }
    }
}
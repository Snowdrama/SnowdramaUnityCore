using System;
using UnityEngine;

namespace Snowdrama.Timer
{
    [System.Serializable]
    public class TimerComponent : MonoBehaviour
    {
        public float time;
        public bool autoStart;
        public bool autoRestart;
        Timer timer;

        public Action OnStart;
        public Action OnRestart;
        public Action OnComplete;
        public Action OnPause;
        public Action OnStop;
        public Action OnResume;

        private void Start()
        {
            timer = new Timer(time, autoStart, autoRestart);
            timer.OnStart += () => { OnStart?.Invoke(); };
            timer.OnRestart += () => { OnRestart?.Invoke(); };
            timer.OnComplete += () => { OnComplete?.Invoke(); };
            timer.OnPause += () => { OnPause?.Invoke(); };
            timer.OnStop += () => { OnStop?.Invoke(); };
            timer.OnResume += () => { OnResume?.Invoke(); };
        }

        public void Update ()
        {
            timer.UpdateTime(Time.deltaTime);
        }        
        
        /// <summary>
        /// Starts or resumes the timer depending on if Stop or Pause was used
        /// </summary>
        public void StartTimer()
        {
            timer.Start();
        }

        /// <summary>
        /// Stops the timer, and resets the current time to 0
        /// </summary>
        public void StopTimer()
        {
            timer.Stop();
        }

        /// <summary>
        /// Stops the timer and allows resuming from the current time
        /// </summary>
        public void Pause()
        {
            timer.Pause();
        }

        /// <summary>
        /// returns the current time in seconds
        /// </summary>
        /// <returns>A float representing the time elapsed</returns>
        public float GetTime()
        {
            return timer.GetTime();
        }

        /// <summary>
        /// returns the percentage from 0 to 1 as a percentage of current time to the timer's _durationTarget
        /// </summary>
        /// <returns>A Float from 0 to 1 representing the total time</returns>
        public float GetPercentageComplete()
        {
            return timer.GetPercentageComplete();
        }

        /// <summary>
        /// returns the amount of time remaining until the timer ends
        /// </summary>
        /// <returns>A Float representing time remaining</returns>
        public float GetTimeRemaining()
        {
            return timer.GetTimeRemaining();
        }

        /// <summary>
        /// start the timer over, allow passing in a new time.
        /// </summary>
        /// <param name="newTime"></param>
        public void RestartTimer(float newTime = -1)
        {
            timer.RestartTimer(newTime);
        }

        public void SetNewTime(float newTime)
        {
            timer.SetNewTime(newTime);
        }
    }
}
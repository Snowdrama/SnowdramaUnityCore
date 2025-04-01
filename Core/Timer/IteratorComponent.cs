using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.Timer
{
    public class IteratorComponent : MonoBehaviour
    {
        private Iterator iterator;

        public float iterateTime = 1;
        public float duration = -1;
        public bool autoStart;

        public Action OnStart;
        public Action OnIterate;
        public Action OnDurationComplete;
        public Action OnPause;
        public Action OnStop;
        public Action OnResume;
        // Start is called before the first frame update
        void Start()
        {
            iterator = new Iterator(iterateTime, duration, autoStart);
            iterator.OnIterate += () => { OnIterate?.Invoke(); };
            iterator.OnDurationComplete += () => { OnDurationComplete?.Invoke(); };

            iterator.OnStart += () => { OnStart?.Invoke(); };
            iterator.OnStop += () => { OnStop?.Invoke(); };

            iterator.OnPause += () => { OnPause?.Invoke(); };
            iterator.OnResume += () => { OnResume?.Invoke(); };
        }

        // Update is called once per frame
        void Update()
        {
            iterator.UpdateTime(Time.deltaTime);
        }

        /// <summary>
        /// Starts or resumes the iterator depending on if Stop or Pause was used
        /// </summary>
        public void StartTimer()
        {
            iterator.Start();
        }

        /// <summary>
        /// Stops the iterator, and resets the current time to 0
        /// </summary>
        public void StopTimer()
        {
            iterator.Stop();
        }

        /// <summary>
        /// Stops the iterator and allows resuming from the current time
        /// </summary>
        public void Pause()
        {
            iterator.Pause();
        }

        /// <summary>
        /// returns the current time in seconds
        /// </summary>
        /// <returns>A float representing the time elapsed</returns>
        public float GetTime()
        {
            return iterator.GetTime();
        }

        /// <summary>
        /// returns the percentage from 0 to 1 as a percentage of current time to the iterator's _durationTarget
        /// </summary>
        /// <returns>A Float from 0 to 1 representing the total time</returns>
        public float GetPercentageComplete()
        {
            return iterator.GetPercentageComplete();
        }

        /// <summary>
        /// returns the amount of time remaining until the iterator ends
        /// </summary>
        /// <returns>A Float representing time remaining</returns>
        public float GetTimeRemaining()
        {
            return iterator.GetTimeRemaining();
        }

        /// <summary>
        /// start the iterator over, allow passing in a new time.
        /// </summary>
        /// <param name="newTime"></param>
        public void RestartTimer(float newTime = -1)
        {
            iterator.RestartTimer(newTime);
            
        }

        public void SetNewTime(float newTime)
        {
            iterator.SetNewTime(newTime);
        }
    }

}
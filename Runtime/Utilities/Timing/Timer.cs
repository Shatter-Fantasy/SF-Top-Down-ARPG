using System;
using UnityEngine;

namespace SF
{
    [Serializable]
    public class Timer
    {
        public float Duration = 3;
        public float RemainingTime;
        [NonSerialized] public float ElapsedTimer = 0;
        /// <summary>
        /// An Action that is called at the end when the Timer hits 0.
        /// </summary>
        public Action OnTimerCompleted;

        [NonSerialized] public AwaitableCompletionSource<bool> CompletionSource = new AwaitableCompletionSource<bool>();
        [NonSerialized] public bool TimerStopped;
        
        public Timer(Action onTimerCompleted = null)
        {
            if (onTimerCompleted == null)
                return;

            OnTimerCompleted += onTimerCompleted;
        }
        public Timer(float duration, Action onTimerCompleted = null) : this(onTimerCompleted)
        {
            Duration = duration;
            RemainingTime = duration;
        }
        /// <summary>
        /// Resets the <see cref="Timer"/> <see cref="RemainingTime"/> and <see cref="TimerStopped"/> values,
        /// but doesn't make it start again.
        /// </summary>
        public void ResetTimer()
        {
            TimerStopped = false;
            RemainingTime = Duration;
        }
        
        /// <summary>
        /// Resets the timer values and than starts the Timer again.
        /// Also returns true when the timer finishes.
        /// </summary>
        public async Awaitable<bool> RestartTimer()
        {
            ResetTimer();
            await StartTimerAsync();
            return true;
        }
        
        /// <summary>
        /// Starts an async timer that when completed raises the 
        /// <see cref="OnTimerCompleted"/> event.
        /// </summary>
        /// <returns>
        /// True when the timer is finished or false while the timer is currently counting down.
        /// Note the <see cref="OnTimerCompleted"/> will be run before the true value is returned.
        /// If you try to use the returned value during an Action invoked by the <see cref="OnTimerCompleted"/>
        /// it might not have the value you expect. 
        /// </returns>
        public async Awaitable<bool> StartTimerAsync()
        {
            ResetTimer();
            await UpdateTimerAsync();
            if (!TimerStopped)
            {
                CompletionSource.TrySetResult(true);
                OnTimerCompleted?.Invoke();
            }
            return true;
        }

        public void StopTimer()
        {
            TimerStopped = true;
        }

        /// <summary>
        /// Tells the timer to start updating and counting down asynchronously and invokes the <see cref="OnTimerCompleted"/> event when the timer reaches zero.
        /// </summary>
        /// <returns></returns>
        public async Awaitable UpdateTimerAsync()
        {
            while (RemainingTime > 0)
            {
                if(TimerStopped)
                    break;
                
                RemainingTime -= Time.deltaTime;
                ElapsedTimer += Time.deltaTime;
                if (RemainingTime > 0)
                    await Awaitable.EndOfFrameAsync();
            }
            
            RemainingTime = 0;
            ElapsedTimer = 0;
        }
    }
}
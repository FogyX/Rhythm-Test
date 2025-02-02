// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using System;
using UnityEngine;

namespace LDG.Core.Timers
{
    public struct ElapsedEventArgs
    {
        public static ElapsedEventArgs None = default;
        public int Iteration;
        public float Time;
    }

    /// <summary>
    /// Timer class that keeps time with Unity's time.
    /// </summary>
    public class Timer
    {
        public float Time
        { 
            get
            {
                return timer;
            }
        }

        public bool TimeChanged
        {
            get
            {
                return (prevTimer != timer);
            }
        }

        /// <summary>
        /// Returns the number of times the Elapsed event handler has been called.
        /// </summary>
        public int Iteration { get; protected set; }

        /// <summary>
        /// User hanlder that is called on interval up to the number of specified repetitions.
        /// </summary>
        public event Action<object, ElapsedEventArgs> Elapsed;

        /// <summary>
        /// Cause the timer to update or not
        /// </summary>
        public bool Enabled = false;

        /// <summary>
        /// The number of times the Elapsed event handler should be called
        /// </summary>
        public int Repetitions = 1;

        /// <summary>
        /// Cause the timer to auto reset
        /// </summary>
        public bool AutoReset = false;

        /// <summary>
        /// The amount of time in between Elapsed event handler calls
        /// </summary>
        public float Interval = 1.0f;

        private float timer;
        private float prevTimer;

        public Timer()
        {
            timer = 0;
            prevTimer = 0;
            Iteration = 0;
        }

        public void Start()
        {
            Start(false);
        }

        /// <summary>
        /// Starts the timer with the option to trigger the Elapsed event handler 
        /// </summary>
        /// <param name="triggerElapsed"></param>
        public void Start(bool triggerElapsed)
        {
            Start(triggerElapsed, 0.0f);
        }

        /// <summary>
        /// Starts the timer with the option to trigger the Elapsed event handler 
        /// </summary>
        /// <param name="triggerElapsed"></param>
        public void Start(bool triggerElapsed, float timer)
        {
            this.timer = timer;
            prevTimer = timer;

            Iteration = 0;

            Enabled = true;

            if (Elapsed != null)
            {
                if (triggerElapsed)
                {
                    ElapsedEventArgs e = new ElapsedEventArgs
                    {
                        Iteration = 0,
                        Time = timer
                    };

                    Elapsed.Invoke(this, e);
                }
            }
        }

        public void Stop()
        {
            Enabled = false;
        }

        public void Reset()
        {
            timer = 0.0f;
            prevTimer = timer;
        }

        /// <summary>
        /// Terminates before the timer ends forcing the Elapsed event to be called in the next Update.
        /// </summary>
        public void TerminateEarly()
        {
            timer = Interval;
        }

        /// <summary>
        /// Count down by specified delta time
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            prevTimer = timer;

            if (!Enabled || Iteration >= Repetitions) return;

            while (timer >= Interval)
            {
                Iteration++;

                if (Elapsed != null)
                {
                    ElapsedEventArgs e = new ElapsedEventArgs
                    {
                        Iteration = (AutoReset) ? Iteration % Repetitions : Iteration,
                        Time = (float)(Iteration - 1) * Interval + timer
                    };

                    Elapsed.Invoke(this, e);
                }

                if (Iteration < Repetitions)
                {
                    timer -= Interval;
                }
                else
                {
                    if (AutoReset)
                    {
                        timer -= Interval;
                        Iteration = 0;
                        break;
                    }
                    else
                    {
                        Enabled = false;
                        break;
                    }
                }
            }

            timer += deltaTime;
        }

        /// <summary>
        /// Count down using Unity's Time.deltaTime
        /// </summary>
        public void Update()
        {
            Update(UnityEngine.Time.deltaTime);
        }
    }
}
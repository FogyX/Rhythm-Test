// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using System;

namespace LDG.Core
{
    using LDG.Core.Timers;
    using UnityEngine;

    /// <summary>
    /// A Particle can be emitted and will return to its pool after a certain amount of time, or when forced.
    /// </summary>
    public class MonoBehaviourParticle : MonoBehaviourEx
    {
        public float TimeAlive
        {
            get
            {
                return timer.Time;
            }
        }

        public event Action<MonoBehaviourParticle> OnEmit;
        public event Action<MonoBehaviourParticle> OnExpire;

        private IParticleCollection owner = null;

        private Timer timer = new Timer();

        protected virtual void Awake()
        {
            timer.Elapsed += TimeElapsed;
        }

        protected virtual void Update()
        {
            timer.Update();
        }

        private void TimeElapsed(object source, ElapsedEventArgs e)
        {
            if (OnExpire != null)
            {
                OnExpire.Invoke(this);
            }

            if(owner != null)
			{
                owner.ReturnParticle(this);
            }
        }

        public void SetOwner(IParticleCollection particleCollection)
        {
            owner = particleCollection;
        }

        public void Emit(float lifetime)
        {
            timer.Interval = lifetime;
            timer.Start();

            if(OnEmit != null)
            {
                OnEmit.Invoke(this);
            }
        }

        public void ReturnParticle()
        {
            timer.TerminateEarly();
            timer.Update();
            timer.Enabled = false;
        }

        public virtual void Initialize(object parameters)
        {
            
        }

        public virtual void Finalize(object parameters)
        {

        }

        public virtual void Refresh()
        {

        }
    }
}
// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG.SoundReactor
{
    public class ParticleEmitterDriver : PropertyDriver
    {
        private ParticleSystem ps = null;

        private void Start()
        {
            if(!(ps = GetComponent<ParticleSystem>()))
            {
                Debug.LogWarning("ParticleEmitterDriver can't find a ParticleSystem.", this);
                componentMissing = true;
            }
        }

        protected override void DoLevel()
        {
            if (ps)
            {
                ps.Emit(Mathf.RoundToInt(LevelScalar()));
            }
        }
    }
}

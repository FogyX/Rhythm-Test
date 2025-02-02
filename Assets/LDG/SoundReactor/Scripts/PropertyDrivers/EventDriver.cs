// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine.Events;

namespace LDG.SoundReactor
{
    public class EventDriver : PropertyDriver
    {
        [System.Serializable]
        public class OnLevel : UnityEvent<PropertyDriver> { }

        public OnLevel onLevel;

        protected override void DoLevel()
        {
            if (onLevel != null)
            {
                onLevel.Invoke(this);
            }
        }
    }
}

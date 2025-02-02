// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG.SoundReactor
{
    [DisallowMultipleComponent]
    public class RotateDriver : PropertyDriver
    {
        public Quaternion localRotation;

        void Start()
        {
            localRotation = transform.localRotation;
        }

        protected override void DoLevel()
        {
            Quaternion rotation;

            Vector3 level = LevelVector();

            rotation = Quaternion.Euler(level.x, level.y, level.z);

            transform.localRotation = localRotation * rotation;
        }
    }
}

// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG.SoundReactor
{
    [DisallowMultipleComponent]
    public class PositionDriver : PropertyDriver
    {
        Vector3 localPosition;

        private void Start()
        {
            localPosition = transform.localPosition;
        }

        protected override void DoLevel()
        {
            transform.localPosition = localPosition + LevelVector();
        }
    }
}

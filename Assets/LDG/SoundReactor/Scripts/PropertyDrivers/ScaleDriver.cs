// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG.SoundReactor
{
    [DisallowMultipleComponent]
    public class ScaleDriver : PropertyDriver
    {
        Vector3 localScale;

        void Start()
        {
            localScale = transform.localScale;
        }

        protected override void DoLevel()
        {
            transform.localScale = localScale + LevelVector();
        }
    }
}

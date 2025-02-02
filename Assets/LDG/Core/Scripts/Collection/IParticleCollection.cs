// Sound Reactor
// Copyright (c) 2021, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG.Core
{
    public interface IParticleCollection
    {
        MonoBehaviourParticle GetParticle(bool enableGameObject);
        void ReturnParticle(MonoBehaviourParticle particle);
        void DeactivateAll();
    }
}